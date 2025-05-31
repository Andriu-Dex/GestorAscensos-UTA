using System.Net.Http.Json;

namespace SGA.Web.Services
{
    public interface IApiService
    {
        Task<T?> GetAsync<T>(string endpoint);
        Task<HttpResponseMessage> PostAsync<T>(string endpoint, T data);
        Task<HttpResponseMessage> PutAsync<T>(string endpoint, T data);
        Task<HttpResponseMessage> DeleteAsync(string endpoint);
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
        }
    }
}
