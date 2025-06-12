using Microsoft.EntityFrameworkCore;
using SGA.Application.Services;
using SGA.Domain.Interfaces;
using SGA.Infrastructure.Data;
using SGA.Infrastructure.Repositories;

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

// Register services
builder.Services.AddScoped<IPromotionService>(provider => 
    new PromotionService(
        provider.GetRequiredService<ITeacherRepository>(),
        provider.GetRequiredService<IPromotionRequestRepository>(),
        provider.GetRequiredService<IDocumentRepository>()
    ));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazorClient");

app.MapControllers();

app.Run();
