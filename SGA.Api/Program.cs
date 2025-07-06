using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using SGA.Application;
using SGA.Infrastructure;
using SGA.Api.Configuration;
using SGA.Api.Middleware;
using SGA.Api.Hubs;
using System.Text;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Configurar logging para suprimir logs no críticos
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Warning);

// Cargar archivo .env si existe
var envFile = Path.Combine(Directory.GetCurrentDirectory(), "..", ".env");
if (File.Exists(envFile))
{
    Env.Load(envFile);
    Console.WriteLine("✅ Archivo .env cargado correctamente");
}
else
{
    Console.WriteLine("ℹ️  Archivo .env no encontrado, usando variables de entorno del sistema");
}

// Cargar variables de entorno
builder.Configuration.AddEnvironmentVariables("SGA_");

// Validar configuración requerida al inicio
try 
{
    ConfigurationHelper.ValidateRequiredConfiguration(builder.Configuration);
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"❌ Error de configuración: {ex.Message}");
    Console.WriteLine("💡 Consulta ENVIRONMENT_SETUP.md para configurar las variables de entorno");
    throw;
}

// Configurar servicios
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // Mantener nombres de propiedades tal como están
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString;
    });
builder.Services.AddEndpointsApiExplorer();

// Configurar Swagger con autenticación JWT
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sistema de Gestión de Ascensos Docentes API",
        Version = "v1",
        Description = "API para la gestión de ascensos docentes de la Universidad Técnica de Ambato"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configurar CORS con orígenes desde configuración
var corsOrigins = builder.Configuration.GetSection("CORS:AllowedOrigins").Get<string[]>() 
                 ?? Environment.GetEnvironmentVariable("SGA_CORS_ORIGINS")?.Split(',')
                 ?? new[] { "https://localhost:7149", "http://localhost:5039", "http://localhost:7149" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp", policy =>
    {
        policy.WithOrigins(corsOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
              .SetIsOriginAllowed(origin => true); // Permitir redirects en preflight requests
    });
});

// Configurar SignalR para notificaciones en tiempo real
builder.Services.AddSignalR();

// Configurar autenticación JWT con variables de entorno
var jwtSettings = builder.Configuration.GetSection("JWT");
var secretKey = Environment.GetEnvironmentVariable("SGA_JWT_SECRET_KEY") 
               ?? jwtSettings["SecretKey"] 
               ?? throw new InvalidOperationException("JWT SecretKey not configured. Set SGA_JWT_SECRET_KEY environment variable.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"] ?? "SGA.Api",
        ValidAudience = jwtSettings["Audience"] ?? "SGA.Client",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
    
    // Configurar SignalR para usar el token desde query string
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            
            // Si la request es para nuestro hub
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && 
                (path.StartsWithSegments("/notificacionesHub")))
            {
                // Leer el token desde query string
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

// Registrar capas de la aplicación
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Corregir la inyección del Hub para NotificacionTiempoRealService
// Registramos explícitamente el IHubContext<Hub> para que use el IHubContext<NotificacionesHub>
builder.Services.AddScoped<IHubContext<Hub>>(provider => 
    provider.GetRequiredService<IHubContext<SGA.Api.Hubs.NotificacionesHub>>());

var app = builder.Build();

// Configurar pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SGA API v1");
        options.RoutePrefix = "swagger";
    });
}

// Configurar HTTPS redirection para producción y desarrollo
app.UseCors("AllowBlazorApp");
app.UseHttpsRedirection();

// Middleware personalizado para autenticación por query string
app.UseMiddleware<QueryStringAuthenticationMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Configurar hub de SignalR
app.MapHub<SGA.Api.Hubs.NotificacionesHub>("/notificacionesHub");

// Inicializar base de datos
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<SGA.Infrastructure.Data.ApplicationDbContext>();
        // Usar migraciones en lugar de EnsureCreated para manejar cambios de esquema
        context.Database.Migrate();
        
        // También crear las bases de datos externas (estas pueden usar EnsureCreated ya que son externas)
        var tthhContext = scope.ServiceProvider.GetRequiredService<SGA.Infrastructure.Data.External.TTHHDbContext>();
        tthhContext.Database.EnsureCreated();
        
        var dacContext = scope.ServiceProvider.GetRequiredService<SGA.Infrastructure.Data.External.DACDbContext>();
        dacContext.Database.EnsureCreated();
        
        var diticContext = scope.ServiceProvider.GetRequiredService<SGA.Infrastructure.Data.External.DITICDbContext>();
        diticContext.Database.EnsureCreated();
        
        var dirInvContext = scope.ServiceProvider.GetRequiredService<SGA.Infrastructure.Data.External.DIRINVDbContext>();
        dirInvContext.Database.EnsureCreated();
        
        Console.WriteLine("Bases de datos inicializadas correctamente usando migraciones");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al inicializar base de datos: {ex.Message}");
    }
}

// Endpoint de salud simple
app.MapGet("/api/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }));

app.Run();
