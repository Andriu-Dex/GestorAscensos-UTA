using Microsoft.JSInterop;
using Blazored.Toast.Services;
using Blazored.LocalStorage;
using SGA.Web.Models;

namespace SGA.Web.Services;

public class DocumentVisualizationService
{
    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorage;
    private readonly IToastService _toastService;
    private readonly IJSRuntime _jsRuntime;

    public DocumentVisualizationService(
        HttpClient http, 
        ILocalStorageService localStorage, 
        IToastService toastService, 
        IJSRuntime jsRuntime)
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

    public async Task<DocumentViewResult> VisualizarObraAcademica(Guid solicitudId)
    {
        try
        {
            await SetAuthorizationHeader();
            var response = await _http.GetAsync($"api/obraacademicas/visualizar-archivo/{solicitudId}");
            
            if (response.IsSuccessStatusCode)
            {
                var fileBytes = await response.Content.ReadAsByteArrayAsync();
                var fileName = response.Content.Headers.ContentDisposition?.FileName?.Trim('"') ?? "documento.pdf";
                
                var base64 = Convert.ToBase64String(fileBytes);
                var pdfUrl = $"data:application/pdf;base64,{base64}";
                
                return new DocumentViewResult 
                { 
                    Success = true, 
                    PdfUrl = pdfUrl, 
                    FileName = fileName 
                };
            }
            else
            {
                var errorMessage = "No se pudo cargar el documento";
                _toastService.ShowError(errorMessage);
                return new DocumentViewResult 
                { 
                    Success = false, 
                    ErrorMessage = errorMessage 
                };
            }
        }
        catch (Exception ex)
        {
            var errorMessage = $"Error al cargar el documento: {ex.Message}";
            _toastService.ShowError(errorMessage);
            return new DocumentViewResult 
            { 
                Success = false, 
                ErrorMessage = errorMessage 
            };
        }
    }

    public async Task<bool> DescargarObraAcademica(Guid solicitudId)
    {
        try
        {
            await SetAuthorizationHeader();
            var response = await _http.GetAsync($"api/obraacademicas/descargar-archivo/{solicitudId}");
            
            if (response.IsSuccessStatusCode)
            {
                var fileBytes = await response.Content.ReadAsByteArrayAsync();
                var fileName = response.Content.Headers.ContentDisposition?.FileName?.Trim('"') ?? $"obra-{solicitudId}.pdf";
                
                // Usar directamente los bytes sin conversión a base64
                await _jsRuntime.InvokeVoidAsync("downloadFileFromStream", fileName, fileBytes);
                
                _toastService.ShowSuccess("Archivo descargado correctamente");
                return true;
            }
            else
            {
                _toastService.ShowError("No se pudo descargar el archivo");
                return false;
            }
        }
        catch (Exception ex)
        {
            _toastService.ShowError($"Error al descargar: {ex.Message}");
            return false;
        }
    }

    public async Task<DocumentViewResult> VisualizarCertificado(Guid certificadoId)
    {
        try
        {
            await SetAuthorizationHeader();
            var response = await _http.GetAsync($"api/certificados-capacitacion/visualizar-archivo/{certificadoId}");
            
            if (response.IsSuccessStatusCode)
            {
                var fileBytes = await response.Content.ReadAsByteArrayAsync();
                var fileName = response.Content.Headers.ContentDisposition?.FileName?.Trim('"') ?? "certificado.pdf";
                
                var base64 = Convert.ToBase64String(fileBytes);
                var pdfUrl = $"data:application/pdf;base64,{base64}";
                
                return new DocumentViewResult 
                { 
                    Success = true, 
                    PdfUrl = pdfUrl, 
                    FileName = fileName 
                };
            }
            else
            {
                var errorMessage = "No se pudo cargar el certificado";
                _toastService.ShowError(errorMessage);
                return new DocumentViewResult 
                { 
                    Success = false, 
                    ErrorMessage = errorMessage 
                };
            }
        }
        catch (Exception ex)
        {
            var errorMessage = $"Error al cargar el certificado: {ex.Message}";
            _toastService.ShowError(errorMessage);
            return new DocumentViewResult 
            { 
                Success = false, 
                ErrorMessage = errorMessage 
            };
        }
    }

    public async Task<bool> DescargarCertificado(Guid certificadoId)
    {
        try
        {
            await SetAuthorizationHeader();
            var response = await _http.GetAsync($"api/certificados-capacitacion/descargar-archivo/{certificadoId}");
            
            if (response.IsSuccessStatusCode)
            {
                var fileBytes = await response.Content.ReadAsByteArrayAsync();
                var fileName = response.Content.Headers.ContentDisposition?.FileName?.Trim('"') ?? $"certificado-{certificadoId}.pdf";
                
                // Usar directamente los bytes sin conversión a base64
                await _jsRuntime.InvokeVoidAsync("downloadFileFromStream", fileName, fileBytes);
                
                _toastService.ShowSuccess("Certificado descargado correctamente");
                return true;
            }
            else
            {
                _toastService.ShowError("No se pudo descargar el certificado");
                return false;
            }
        }
        catch (Exception ex)
        {
            _toastService.ShowError($"Error al descargar: {ex.Message}");
            return false;
        }
    }

    public async Task<DocumentViewResult> VisualizarDocumentoSolicitud(Guid documentoId)
    {
        try
        {
            await SetAuthorizationHeader();
            var response = await _http.GetAsync($"api/documento/{documentoId}/view");
            
            if (response.IsSuccessStatusCode)
            {
                var fileBytes = await response.Content.ReadAsByteArrayAsync();
                var fileName = response.Content.Headers.ContentDisposition?.FileName?.Trim('"') ?? "documento.pdf";
                
                var base64 = Convert.ToBase64String(fileBytes);
                var pdfUrl = $"data:application/pdf;base64,{base64}";
                
                return new DocumentViewResult 
                { 
                    Success = true, 
                    PdfUrl = pdfUrl, 
                    FileName = fileName 
                };
            }
            else
            {
                var errorMessage = "No se pudo cargar el documento";
                _toastService.ShowError(errorMessage);
                return new DocumentViewResult 
                { 
                    Success = false, 
                    ErrorMessage = errorMessage 
                };
            }
        }
        catch (Exception ex)
        {
            var errorMessage = $"Error al cargar el documento: {ex.Message}";
            _toastService.ShowError(errorMessage);
            return new DocumentViewResult 
            { 
                Success = false, 
                ErrorMessage = errorMessage 
            };
        }
    }

    public async Task<bool> DescargarDocumentoSolicitud(Guid documentoId)
    {
        try
        {
            await SetAuthorizationHeader();
            var response = await _http.GetAsync($"api/documento/{documentoId}/download");
            
            if (response.IsSuccessStatusCode)
            {
                var fileBytes = await response.Content.ReadAsByteArrayAsync();
                var fileName = response.Content.Headers.ContentDisposition?.FileName?.Trim('"') ?? $"documento-{documentoId}.pdf";
                
                await _jsRuntime.InvokeVoidAsync("downloadFileFromStream", fileName, fileBytes);
                
                _toastService.ShowSuccess("Documento descargado correctamente");
                return true;
            }
            else
            {
                _toastService.ShowError("No se pudo descargar el documento");
                return false;
            }
        }
        catch (Exception ex)
        {
            _toastService.ShowError($"Error al descargar documento: {ex.Message}");
            return false;
        }
    }

    public async Task<DocumentViewResult> VisualizarReporteSolicitud(Guid solicitudId)
    {
        try
        {
            await SetAuthorizationHeader();
            var response = await _http.GetAsync($"api/reportes/solicitud/{solicitudId}");
            
            if (response.IsSuccessStatusCode)
            {
                var fileBytes = await response.Content.ReadAsByteArrayAsync();
                var fileName = $"reporte-solicitud-{solicitudId.ToString().Substring(0, 8)}.pdf";
                
                var base64 = Convert.ToBase64String(fileBytes);
                var pdfUrl = $"data:application/pdf;base64,{base64}";
                
                return new DocumentViewResult 
                { 
                    Success = true, 
                    PdfUrl = pdfUrl, 
                    FileName = fileName 
                };
            }
            else
            {
                var errorMessage = "No se pudo generar el reporte";
                _toastService.ShowError(errorMessage);
                return new DocumentViewResult 
                { 
                    Success = false, 
                    ErrorMessage = errorMessage 
                };
            }
        }
        catch (Exception ex)
        {
            var errorMessage = $"Error al generar el reporte: {ex.Message}";
            _toastService.ShowError(errorMessage);
            return new DocumentViewResult 
            { 
                Success = false, 
                ErrorMessage = errorMessage 
            };
        }
    }

    public async Task<bool> DescargarReporteSolicitud(Guid solicitudId)
    {
        try
        {
            await SetAuthorizationHeader();
            var response = await _http.GetAsync($"api/reportes/solicitud/{solicitudId}");
            
            if (response.IsSuccessStatusCode)
            {
                var fileBytes = await response.Content.ReadAsByteArrayAsync();
                var fileName = $"reporte-solicitud-{solicitudId.ToString().Substring(0, 8)}.pdf";
                
                await _jsRuntime.InvokeVoidAsync("downloadFileFromStream", fileName, fileBytes);
                
                _toastService.ShowSuccess("Reporte descargado correctamente");
                return true;
            }
            else
            {
                _toastService.ShowError("No se pudo descargar el reporte");
                return false;
            }
        }
        catch (Exception ex)
        {
            _toastService.ShowError($"Error al descargar reporte: {ex.Message}");
            return false;
        }
    }
}
