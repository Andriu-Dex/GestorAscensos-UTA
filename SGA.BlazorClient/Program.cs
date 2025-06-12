using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SGA.BlazorClient.Services;
using System;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Configuración del HttpClient para comunicarse con SGA.Api
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7119")
});

// Registrar los servicios HTTP
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<IPromotionRequestService, PromotionRequestService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IAcademicDegreeService, AcademicDegreeService>();
builder.Services.AddScoped<IUserTypeService, UserTypeService>();
builder.Services.AddScoped<IRequirementService, RequirementService>();

await builder.Build().RunAsync();
