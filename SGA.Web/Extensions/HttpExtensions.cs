using System.Net.Http.Json;
using System.Text.Json;

namespace SGA.Web.Extensions
{
    public static class HttpExtensions
    {
        /// <summary>
        /// Deserializa una respuesta HTTP a un objeto T con manejo de errores mejorado.
        /// </summary>
        public static async Task<T> DeserializeJsonSafeAsync<T>(this HttpResponseMessage response, JsonSerializerOptions? options = null)
        {
            try
            {
                var jsonOptions = options ?? new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
                };
                var content = await response.Content.ReadAsStringAsync();
                
                if (string.IsNullOrWhiteSpace(content))
                    throw new InvalidOperationException("La respuesta del servidor está vacía.");

                return JsonSerializer.Deserialize<T>(content, jsonOptions) 
                    ?? throw new InvalidOperationException("Error al deserializar la respuesta del servidor.");
            }
            catch (JsonException ex)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Error al deserializar JSON: {ex.Message}. Contenido: {content.Substring(0, Math.Min(content.Length, 200))}", ex);
            }
        }

        /// <summary>
        /// Extrae el mensaje de error de una respuesta HTTP para mostrar al usuario.
        /// </summary>
        public static async Task<string> GetErrorMessageAsync(this HttpResponseMessage response)
        {
            try
            {
                var content = await response.Content.ReadAsStringAsync();
                
                if (string.IsNullOrWhiteSpace(content))
                    return $"Error HTTP: {(int)response.StatusCode} {response.ReasonPhrase}";

                try
                {
                    var errorObj = JsonSerializer.Deserialize<JsonElement>(content);
                    if (errorObj.TryGetProperty("message", out var messageElement))
                        return messageElement.GetString() ?? "Error desconocido";
                    
                    if (errorObj.TryGetProperty("title", out var titleElement))
                        return titleElement.GetString() ?? "Error desconocido";
                    
                    return content.Length > 100 ? content.Substring(0, 100) + "..." : content;
                }
                catch
                {
                    return content.Length > 100 ? content.Substring(0, 100) + "..." : content;
                }
            }
            catch
            {
                return $"Error HTTP: {(int)response.StatusCode} {response.ReasonPhrase}";
            }
        }
    }
}
