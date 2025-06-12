using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SGA.BlazorApp.Client.Auth;
using SGA.BlazorClient.Services;
using System;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Configuración del HttpClient para comunicarse con SGA.Api
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7126")
});

// Configuración de Autenticación
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<JwtTokenHandler>();

// Configurar HttpClient para incluir el token automáticamente
builder.Services.AddHttpClient("AuthorizedClient", client => 
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7126");
})
.AddHttpMessageHandler<AuthorizationMessageHandler>();
builder.Services.AddScoped<AuthorizationMessageHandler>();

// Registrar los servicios HTTP
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<IPromotionRequestService, PromotionRequestService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IAcademicDegreeService, AcademicDegreeService>();
builder.Services.AddScoped<IUserTypeService, UserTypeService>();
builder.Services.AddScoped<IRequirementService, RequirementService>();

await builder.Build().RunAsync();
