using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SGA.Application.Services;
using SGA.Domain.Interfaces;
using SGA.Infrastructure.Data;
using SGA.Infrastructure.Repositories;
using SGA.Infrastructure.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add controllers
builder.Services.AddControllers();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient",
        builder => builder
            .WithOrigins("https://localhost:7130", "http://localhost:5130")
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Configure JWT Authentication
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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// Configure database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories
builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
builder.Services.AddScoped<IPromotionRequestRepository, PromotionRequestRepository>();
builder.Services.AddScoped<IUserTypeRepository, UserTypeRepository>();
builder.Services.AddScoped<IAcademicDegreeRepository, AcademicDegreeRepository>();
builder.Services.AddScoped<IRequirementRepository, RequirementRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IDocumentObservationRepository, DocumentObservationRepository>();
builder.Services.AddScoped<IPromotionObservationRepository, PromotionObservationRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Register services
builder.Services.AddScoped<IPromotionService>(provider => 
    new PromotionService(
        provider.GetRequiredService<ITeacherRepository>(),
        provider.GetRequiredService<IPromotionRequestRepository>(),
        provider.GetRequiredService<IDocumentRepository>()
    ));

// Register authentication services
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
}

// Apply database migrations - Temporarily disabled for JWT testing
/*
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("Applying database migrations");
        await SGA.Infrastructure.Data.DatabaseMigrationManager.MigrateAsync(services, logger);
        logger.LogInformation("Database migrations applied successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while applying database migrations");
    }
}
*/

app.UseHttpsRedirection();
app.UseCors("AllowBlazorClient");

// Add authentication middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
