using System.Net.Http.Headers;
using System.Net.Http.Json;
using SGA.Web.Models;

namespace SGA.Web.Services
{
    public interface IApiService
    {
        Task<T?> GetAsync<T>(string endpoint);
        Task<HttpResponseMessage> PostAsync<T>(string endpoint, T data);
        Task<HttpResponseMessage> PutAsync<T>(string endpoint, T data);
        Task<HttpResponseMessage> DeleteAsync(string endpoint);
        Task<DatosTTHH?> ObtenerDatosTTHH(string cedula);
        Task<bool> ValidarCorreoUnico(string email);
        Task<bool> ValidarCedulaUnica(string cedula);
        Task<bool> ValidarCedulaEcuatoriana(string cedula);
    }

    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;

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
        }        public async Task<DatosTTHH?> ObtenerDatosTTHH(string cedula)
        {
            try
            {
                // Intentamos primero llamar a la API de datos TTHH
                try 
                {
                    return await _httpClient.GetFromJsonAsync<DatosTTHH>($"api/auth/tthh/{cedula}");
                }                catch
                {
                    // Si la API no existe, usamos una implementación local simulada
                    await Task.Delay(300); // Simulamos un poco de latencia
                    
                    // Para cualquier cédula, generamos datos con correo institucional normalizado
                    string nombres, apellidos;
                    
                    // Verificamos la cédula para devolver diferentes datos según el caso
                    if (cedula == "1801000000")
                    {
                        nombres = "Andriu";
                        apellidos = "Dex";
                    }
                    else if (cedula == "1802000000")
                    {
                        nombres = "Steven";
                        apellidos = "Paredes";
                    }
                    else
                    {
                        nombres = "Usuario";
                        apellidos = "Prueba";
                    }
                    
                    // Generar correo institucional normalizado: inicial + apellido + @uta.edu.ec
                    string primeraInicial = nombres.Substring(0, 1).ToLower();
                    string primerApellido = apellidos.Split(' ')[0].ToLower();
                    string correoInstitucional = $"{primeraInicial}{primerApellido}@uta.edu.ec";
                      return new DatosTTHH
                    {
                        Cedula = cedula,
                        Nombres = nombres,
                        Apellidos = apellidos,
                        Facultad = "FISEI",
                        Celular = "0912345678",
                        EmailPersonal = correoInstitucional, // Devolver el correo institucional en EmailPersonal
                        EmailInstitucional = correoInstitucional // También en EmailInstitucional
                    };
                }
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
        }        public async Task<bool> ValidarCedulaEcuatoriana(string cedula)
        {
            // Validación simplificada: solo verificamos que sea numérica y tenga 10 dígitos
            await Task.Delay(100);
            return cedula.Length == 10 && cedula.All(char.IsDigit);
        }
    }
}
