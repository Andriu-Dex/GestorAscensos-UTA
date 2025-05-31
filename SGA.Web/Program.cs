using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SGA.Web;
using System.Text.Json;
using SGA.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Obtener la URL de la API desde appsettings.json
var http = new HttpClient();
using var response = await http.GetAsync($"{builder.HostEnvironment.BaseAddress}appsettings.json");
using var stream = await response.Content.ReadAsStreamAsync();
var config = await JsonSerializer.DeserializeAsync<JsonElement>(stream);
var apiBaseUrl = config.GetProperty("API").GetProperty("BaseUrl").GetString();

// Configurar HttpClient con la URL de la API
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl ?? "https://localhost:7030") });

// Registrar el servicio de API
builder.Services.AddScoped<IApiService, ApiService>();

await builder.Build().RunAsync();
