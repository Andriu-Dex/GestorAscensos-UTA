using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using SGA.Web.Models;

namespace SGA.Web.Services
{
    public interface IApiService
    {
        Task<T?> GetAsync<T>(string endpoint);
        Task<HttpResponseMessage> PostAsync<T>(string endpoint, T data);
        Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data);
        Task<byte[]?> PostBytesAsync<T>(string endpoint, T data);
        Task<HttpResponseMessage> PutAsync<T>(string endpoint, T data);
        Task<HttpResponseMessage> DeleteAsync(string endpoint);
        Task<DatosTTHH?> ObtenerDatosTTHH(string cedula);
        Task<bool> ValidarCorreoUnico(string email);
        Task<bool> ValidarCedulaUnica(string cedula);
        
        // Nuevos métodos para contenido no-JSON
        Task<string?> GetHtmlAsync(string endpoint);
        Task<string?> PostForHtmlAsync<T>(string endpoint, T data);
        Task<byte[]?> GetBytesAsync(string endpoint);
    }

    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<T>(endpoint);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en solicitud GET a {endpoint}: {ex.Message}");
                throw;
            }
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string endpoint, T data)
        {
            try
            {
                return await _httpClient.PostAsJsonAsync(endpoint, data);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en solicitud POST a {endpoint}: {ex.Message}");
                throw;
            }
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(endpoint, data);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<TResponse>();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en solicitud POST a {endpoint}: {ex.Message}");
                throw;
            }
        }

        public async Task<byte[]?> PostBytesAsync<T>(string endpoint, T data)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(endpoint, data);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsByteArrayAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en solicitud POST bytes a {endpoint}: {ex.Message}");
                throw;
            }
        }

        public async Task<HttpResponseMessage> PutAsync<T>(string endpoint, T data)
        {
            try
            {
                return await _httpClient.PutAsJsonAsync(endpoint, data);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en solicitud PUT a {endpoint}: {ex.Message}");
                throw;
            }
        }

        public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
        {
            try
            {
                return await _httpClient.DeleteAsync(endpoint);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en solicitud DELETE a {endpoint}: {ex.Message}");
                throw;
            }
        }

        public async Task<DatosTTHH?> ObtenerDatosTTHH(string cedula)
        {
            try
            {
                // Llamar al endpoint correcto de validación de cédula
                var request = new { Cedula = cedula };
                var response = await _httpClient.PostAsJsonAsync("api/auth/validate-cedula", request);
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
                    
                    if (jsonResponse.TryGetProperty("valid", out var validProp) && validProp.GetBoolean())
                    {
                        if (jsonResponse.TryGetProperty("empleado", out var empleadoProp))
                        {
                            return new DatosTTHH
                            {
                                Cedula = empleadoProp.GetProperty("cedula").GetString() ?? "",
                                Nombres = empleadoProp.GetProperty("nombres").GetString() ?? "",
                                Apellidos = empleadoProp.GetProperty("apellidos").GetString() ?? "",
                                EmailInstitucional = empleadoProp.GetProperty("correoInstitucional").GetString() ?? "",
                                Celular = empleadoProp.GetProperty("celular").GetString() ?? "",
                                Facultad = empleadoProp.GetProperty("facultad").GetString() ?? "",
                                EmailPersonal = empleadoProp.GetProperty("email").GetString() ?? ""
                            };
                        }
                    }
                }
                else
                {
                    Console.Error.WriteLine($"Error al validar cédula {cedula}: {response.StatusCode} - {response.ReasonPhrase}");
                }
                
                return null; // Si no se encuentra o hay error
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al obtener datos de TTHH para cédula {cedula}: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> ValidarCorreoUnico(string email)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/auth/verificar-email/{email}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al validar correo {email}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ValidarCedulaUnica(string cedula)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/auth/verificar-cedula/{cedula}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al validar cédula {cedula}: {ex.Message}");
                return false;
            }
        }

        public async Task<string?> GetHtmlAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en solicitud GET HTML a {endpoint}: {ex.Message}");
                throw;
            }
        }

        public async Task<string?> PostForHtmlAsync<T>(string endpoint, T data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync(endpoint, content);
                response.EnsureSuccessStatusCode();
                
                var responseContent = await response.Content.ReadAsStringAsync();
                
                // Deserializar el JSON para obtener el HTML
                var htmlContent = JsonSerializer.Deserialize<string>(responseContent, _jsonOptions);
                return htmlContent;
            }
            catch (JsonException ex)
            {
                Console.Error.WriteLine($"Error deserializando respuesta JSON de {endpoint}: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en solicitud POST HTML a {endpoint}: {ex.Message}");
                throw;
            }
        }

        public async Task<byte[]?> GetBytesAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsByteArrayAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en solicitud GET bytes a {endpoint}: {ex.Message}");
                throw;
            }
        }
    }
}
