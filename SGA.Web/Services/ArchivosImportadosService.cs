using Blazored.LocalStorage;
using Blazored.Toast.Services;
using SGA.Web.Models;
using System.Net.Http.Json;

namespace SGA.Web.Services
{
    public class ArchivosImportadosService
    {
        private readonly HttpClient _http;
        private readonly ILocalStorageService _localStorage;
        private readonly IToastService _toastService;

        public ArchivosImportadosService(HttpClient http, ILocalStorageService localStorage, IToastService toastService)
        {
            _http = http;
            _localStorage = localStorage;
            _toastService = toastService;
        }

        private async Task SetAuthorizationHeader()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            _http.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<List<ArchivoImportado>?> GetArchivosImportadosAsync()
        {
            try
            {
                await SetAuthorizationHeader();
                
                // Obtener datos del API
                var response = await _http.GetFromJsonAsync<List<ArchivoImportadoApiDto>>("api/archivos-importados");
                
                if (response == null)
                    return new List<ArchivoImportado>();
                
                // Convertir de DTO del API a modelo del frontend
                return response.Select(dto => new ArchivoImportado
                {
                    Id = dto.Id,
                    NombreArchivo = dto.NombreArchivo,
                    TipoDocumento = dto.TipoDocumento,
                    TamanoArchivo = dto.TamanoArchivo,
                    FechaImportacion = dto.FechaImportacion,
                    Estado = dto.Estado,
                    FechaEnvioValidacion = dto.FechaEnvioValidacion,
                    SolicitudId = dto.SolicitudId
                }).ToList();
            }
            catch (Exception ex)
            {
                _toastService.ShowError($"Error al cargar archivos importados: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> EnviarArchivoParaValidacionAsync(Guid archivoId, FormularioValidacion formulario)
        {
            try
            {
                await SetAuthorizationHeader();
                
                var request = new EnviarValidacionRequestDto
                {
                    TipoDocumento = formulario.TipoObra, // Simplificado por ahora
                    Comentarios = formulario.ComentariosAdicionales,
                    CamposFormulario = new Dictionary<string, object>
                    {
                        { "titulo", formulario.Titulo },
                        { "descripcion", formulario.Descripcion }
                    }
                };
                
                var response = await _http.PostAsJsonAsync($"api/archivos-importados/{archivoId}/enviar-validacion", request);
                
                if (response.IsSuccessStatusCode)
                {
                    _toastService.ShowSuccess("Archivo enviado para validación exitosamente");
                    return true;
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                _toastService.ShowError($"Error al enviar archivo para validación: {errorContent}");
                return false;
            }
            catch (Exception ex)
            {
                _toastService.ShowError($"Error al enviar archivo para validación: {ex.Message}");
                return false;
            }
        }
    }

    // DTO para la respuesta del API
    public class ArchivoImportadoApiDto
    {
        public Guid Id { get; set; }
        public string NombreArchivo { get; set; } = string.Empty;
        public string TipoDocumento { get; set; } = string.Empty;
        public long TamanoArchivo { get; set; }
        public DateTime FechaImportacion { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime? FechaEnvioValidacion { get; set; }
        public Guid? SolicitudId { get; set; }
    }

    public class EnviarValidacionRequestDto
    {
        public Dictionary<string, object> CamposFormulario { get; set; } = new();
        public string TipoDocumento { get; set; } = string.Empty;
        public string? Comentarios { get; set; }
    }
}
