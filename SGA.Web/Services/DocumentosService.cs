using SGA.Web.Models;
using Blazored.Toast.Services;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace SGA.Web.Services
{
    public class DocumentosService
    {
        private readonly HttpClient _http;
        private readonly ILocalStorageService _localStorage;
        private readonly IToastService _toastService;
        private readonly IJSRuntime _jsRuntime;

        public DocumentosService(HttpClient http, ILocalStorageService localStorage, 
            IToastService toastService, IJSRuntime jsRuntime)
        {
            _http = http;
            _localStorage = localStorage;
            _toastService = toastService;
            _jsRuntime = jsRuntime;
        }

        private async Task SetAuthorizationHeader()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            _http.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        // Obras Académicas
        public async Task<List<ObraAcademicaDetalleDto>?> LoadObrasAcademicasAsync()
        {
            try
            {
                await SetAuthorizationHeader();
                var response = await _http.GetFromJsonAsync<ResponseObrasAcademicasDto>("api/obraacademicas/mis-obras");
                return response?.Exitoso == true ? response.Obras : null;
            }
            catch (Exception ex)
            {
                _toastService.ShowError($"Error al cargar obras académicas: {ex.Message}");
                return null;
            }
        }

        public async Task<List<ObraAcademicaDetalleDto>?> LoadTodasSolicitudesObrasAsync()
        {
            try
            {
                await SetAuthorizationHeader();
                var response = await _http.GetFromJsonAsync<ResponseObrasAcademicasDto>("api/obraacademicas/mis-solicitudes");
                return response?.Exitoso == true ? response.Obras : null;
            }
            catch (Exception ex)
            {
                _toastService.ShowError($"Error al cargar solicitudes: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> EnviarSolicitudObrasAsync(SolicitudObrasAcademicasDto solicitud)
        {
            try
            {
                await SetAuthorizationHeader();
                var response = await _http.PostAsJsonAsync("api/obraacademicas/solicitar-nuevas", solicitud);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ResponseObrasAcademicasDto>();
                    if (result?.Exitoso == true)
                    {
                        _toastService.ShowSuccess(result.Mensaje);
                        return true;
                    }
                    else
                    {
                        _toastService.ShowError(result?.Mensaje ?? "Error desconocido al enviar solicitud");
                        return false;
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _toastService.ShowError($"Error al enviar solicitud: {response.StatusCode} - {errorContent}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _toastService.ShowError($"Error al enviar solicitud: {ex.Message}");
                return false;
            }
        }

        // Certificados de Capacitación
        public async Task<List<CertificadoCapacitacionDetalleDto>?> LoadTodasSolicitudesCertificadosAsync()
        {
            try
            {
                await SetAuthorizationHeader();
                var response = await _http.GetAsync("api/certificados-capacitacion/mis-solicitudes");
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ResponseCertificadosCapacitacionDto>();
                    return result?.Exitoso == true ? result.Certificados : null;
                }
                return null;
            }
            catch (Exception ex)
            {
                _toastService.ShowError($"Error al cargar certificados: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> GuardarCertificadosAsync(SolicitarCertificadosCapacitacionDto solicitudDto)
        {
            try
            {
                await SetAuthorizationHeader();
                var response = await _http.PostAsJsonAsync("api/certificados-capacitacion/solicitar", solicitudDto);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ResponseGenericoDto>();
                    if (result?.Exitoso == true)
                    {
                        _toastService.ShowSuccess(result.Mensaje);
                        return true;
                    }
                    else
                    {
                        _toastService.ShowError(result?.Mensaje ?? "Error al guardar certificados");
                        return false;
                    }
                }
                else
                {
                    _toastService.ShowError("Error al guardar certificados");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _toastService.ShowError($"Error al guardar certificados: {ex.Message}");
                return false;
            }
        }

        // Documentos
        public async Task<List<DocumentoDto>?> LoadDocumentosAsync()
        {
            try
            {
                await SetAuthorizationHeader();
                return await _http.GetFromJsonAsync<List<DocumentoDto>>("api/documento/mis-documentos");
            }
            catch (Exception ex)
            {
                _toastService.ShowError($"Error al cargar documentos: {ex.Message}");
                return null;
            }
        }

        public async Task<List<TipoDocumentoDto>?> LoadTiposDocumentoAsync()
        {
            try
            {
                await SetAuthorizationHeader();
                return await _http.GetFromJsonAsync<List<TipoDocumentoDto>>("api/tipodocumento");
            }
            catch (Exception ex)
            {
                _toastService.ShowError($"Error al cargar tipos de documento: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DeleteDocumentAsync(int id)
        {
            if (!await _jsRuntime.InvokeAsync<bool>("confirm", "¿Está seguro que desea eliminar este documento?"))
                return false;
                
            try
            {
                await SetAuthorizationHeader();
                var response = await _http.DeleteAsync($"api/documento/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    _toastService.ShowSuccess("Documento eliminado con éxito");
                    return true;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _toastService.ShowError($"Error al eliminar documento: {error}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _toastService.ShowError($"Error al eliminar documento: {ex.Message}");
                return false;
            }
        }

        public async Task ViewDocumentAsync(DocumentoDto documento)
        {
            try
            {
                var token = await _localStorage.GetItemAsync<string>("authToken");
                var url = $"api/documento/{documento.Id}/view?token={token}";
                await _jsRuntime.InvokeVoidAsync("open", url, "_blank");
            }
            catch (Exception ex)
            {
                _toastService.ShowError($"Error al preparar visualización: {ex.Message}");
            }
        }

        public async Task DownloadDocumentAsync(DocumentoDto documento)
        {
            try
            {
                var token = await _localStorage.GetItemAsync<string>("authToken");
                _http.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                
                var response = await _http.GetAsync($"api/documento/{documento.Id}/download");
                
                if (response.IsSuccessStatusCode)
                {
                    var fileBytes = await response.Content.ReadAsByteArrayAsync();
                    var fileName = response.Content.Headers.ContentDisposition?.FileName?.Trim('"') ?? 
                                  documento.Nombre ?? $"documento-{documento.Id}.pdf";
                    
                    // Usar downloadFileFromStream que maneja mejor los bytes
                    await _jsRuntime.InvokeVoidAsync("downloadFileFromStream", fileName, fileBytes);
                    _toastService.ShowSuccess("Documento descargado correctamente");
                }
                else
                {
                    _toastService.ShowError("No se pudo descargar el documento");
                }
            }
            catch (Exception ex)
            {
                _toastService.ShowError($"Error al descargar documento: {ex.Message}");
            }
        }
    }
}
