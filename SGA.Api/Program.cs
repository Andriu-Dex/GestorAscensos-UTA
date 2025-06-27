using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SGA.Application;
using SGA.Infrastructure;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configurar servicios
builder.Services.AddControllers();
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

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp", policy =>
    {
        policy.WithOrigins(
                "https://localhost:7149", "http://localhost:5039", // SGA.Web (puertos correctos)
                "https://localhost:7030", "http://localhost:5115"   // API local para testing
              )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Configurar autenticación JWT
var jwtSettings = builder.Configuration.GetSection("JWT");
var secretKey = jwtSettings["SecretKey"] ?? "DefaultSecretKeyForDevelopment123456789012345678901234567890";

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
});

// Registrar capas de la aplicación
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

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

// Comentado temporalmente para evitar problemas de CORS con mixed HTTP/HTTPS
// app.UseHttpsRedirection();
app.UseCors("AllowBlazorApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Inicializar base de datos
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<SGA.Infrastructure.Data.ApplicationDbContext>();
        context.Database.EnsureCreated();
        
        // También crear las bases de datos externas
        var tthhContext = scope.ServiceProvider.GetRequiredService<SGA.Infrastructure.Data.External.TTHHDbContext>();
        tthhContext.Database.EnsureCreated();
        
        var dacContext = scope.ServiceProvider.GetRequiredService<SGA.Infrastructure.Data.External.DACDbContext>();
        dacContext.Database.EnsureCreated();
        
        var diticContext = scope.ServiceProvider.GetRequiredService<SGA.Infrastructure.Data.External.DITICDbContext>();
        diticContext.Database.EnsureCreated();
        
        var dirInvContext = scope.ServiceProvider.GetRequiredService<SGA.Infrastructure.Data.External.DIRINVDbContext>();
        dirInvContext.Database.EnsureCreated();
        
        Console.WriteLine("Bases de datos inicializadas correctamente");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al inicializar base de datos: {ex.Message}");
    }
}

// Endpoint de salud simple
app.MapGet("/api/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }));

app.Run();
