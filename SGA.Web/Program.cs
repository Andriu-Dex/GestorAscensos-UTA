using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SGA.Web;
using Blazored.LocalStorage;
using Blazored.Toast;
using SGA.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Authorization;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Add MudBlazor services
builder.Services.AddMudServices();

// Add Authorization services
builder.Services.AddAuthorizationCore();

// Add Blazored services
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredToast();

// Add custom services
builder.Services.AddScoped<SGA.Web.Services.ILocalStorageService, LocalStorageService>();
builder.Services.AddScoped<IApiUrlService, ApiUrlService>();
builder.Services.AddScoped<EcuadorDateService>(); // Servicio para manejo de fechas en zona horaria Ecuador
builder.Services.AddScoped<AuthorizationMessageHandler>();

// Configure HttpClient with authorization handler
builder.Services.AddHttpClient<HttpClient>("SGA.Api", client =>
{
    client.BaseAddress = new Uri("https://localhost:7030/");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddHttpMessageHandler<AuthorizationMessageHandler>();

// Register HttpClient as scoped
builder.Services.AddScoped<HttpClient>(provider =>
{
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    return httpClientFactory.CreateClient("SGA.Api");
});

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IApiService, ApiService>();
builder.Services.AddScoped<DocumentosService>();
builder.Services.AddScoped<DocumentVisualizationService>();
builder.Services.AddScoped<NotificacionesService>();
builder.Services.AddScoped<TitulosAcademicosService>();
builder.Services.AddScoped<ArchivosImportadosService>();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();

await builder.Build().RunAsync();