using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using SGA.Web.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SGA.Web.Pages.Components;

public partial class EvidenciasInvestigacionComponent : ComponentBase
{
    [Parameter] public bool ShowComponent { get; set; } = true;
    [Parameter] public EventCallback OnEvidenciasChanged { get; set; }
    [Parameter] public List<EvidenciaInvestigacionViewModel>? solicitudesEvidencias { get; set; }
    [Parameter] public bool isLoadingEvidenciasParam { get; set; }
    [Parameter] public EventCallback<Guid> OnEliminarEvidencia { get; set; }
    [Parameter] public EventCallback<Guid> OnReenviarEvidencia { get; set; }
    [Parameter] public EventCallback OnActualizarEvidencias { get; set; }

    private List<EvidenciaInvestigacionViewModel>? evidencias;
    private List<EvidenciaInvestigacionViewModel>? evidenciasFiltradas;
    private bool isLoadingEvidencias = false;
    private string selectedEstado = "";
    private string selectedTipo = "";
    
    // Modal states
    private bool showEvidenciasModal = false;
    private EvidenciaInvestigacionViewModel? evidenciaToEdit = null;
    private EvidenciaInvestigacionViewModel? evidenciaToReplaceFile = null;
    private EvidenciaInvestigacionViewModel? evidenciaRechazada = null;
    
    // File replacement
    private IBrowserFile? newFile = null;
    private string? replaceFileError = null;
    private bool isReplacingFile = false;

    protected override async Task OnInitializedAsync()
    {
        if (solicitudesEvidencias != null)
        {
            // Usar las evidencias pasadas como parámetro
            evidencias = solicitudesEvidencias;
            isLoadingEvidencias = isLoadingEvidenciasParam;
            AplicarFiltros();
        }
        else
        {
            // Cargar evidencias internamente si no se pasan como parámetro
            await CargarEvidencias();
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        if (solicitudesEvidencias != null)
        {
            // Actualizar evidencias cuando cambien los parámetros
            evidencias = solicitudesEvidencias;
            isLoadingEvidencias = isLoadingEvidenciasParam;
            AplicarFiltros();
            StateHasChanged();
        }
        
        await base.OnParametersSetAsync();
    }

    protected override void OnParametersSet()
    {
        if (solicitudesEvidencias != null)
        {
            evidencias = solicitudesEvidencias;
            isLoadingEvidencias = isLoadingEvidenciasParam;
            AplicarFiltros();
            StateHasChanged();
        }
    }

    private async Task CargarEvidencias()
    {
        if (!ShowComponent) return;

        try
        {
            isLoadingEvidencias = true;
            StateHasChanged();

            var token = await LocalStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrEmpty(token))
            {
                ToastService.ShowError("Sesión expirada. Por favor, inicie sesión nuevamente.");
                return;
            }

            Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var response = await Http.GetAsync("api/evidencias-investigacion/mis-evidencias");
            
            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var responseData = JsonSerializer.Deserialize<ResponseEvidenciasInvestigacionDto>(
                    jsonContent, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                
                if (responseData?.Exitoso == true)
                {
                    evidencias = responseData.Evidencias;
                    AplicarFiltros();
                }
                else
                {
                    evidencias = new List<EvidenciaInvestigacionViewModel>();
                    evidenciasFiltradas = new List<EvidenciaInvestigacionViewModel>();
                    ToastService.ShowInfo(responseData?.Mensaje ?? "No se encontraron evidencias.");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                ToastService.ShowError("Sesión expirada. Por favor, inicie sesión nuevamente.");
                evidencias = new List<EvidenciaInvestigacionViewModel>();
                evidenciasFiltradas = new List<EvidenciaInvestigacionViewModel>();
            }
            else
            {
                ToastService.ShowError("Error al cargar las evidencias de investigación.");
                evidencias = new List<EvidenciaInvestigacionViewModel>();
                evidenciasFiltradas = new List<EvidenciaInvestigacionViewModel>();
            }
        }
        catch (Exception ex)
        {
            ToastService.ShowError($"Error inesperado: {ex.Message}");
            evidencias = new List<EvidenciaInvestigacionViewModel>();
            evidenciasFiltradas = new List<EvidenciaInvestigacionViewModel>();
        }
        finally
        {
            isLoadingEvidencias = false;
            StateHasChanged();
        }
    }

    private void AplicarFiltros()
    {
        if (evidencias == null)
        {
            evidenciasFiltradas = new List<EvidenciaInvestigacionViewModel>();
            return;
        }

        evidenciasFiltradas = evidencias.Where(e =>
            (string.IsNullOrEmpty(selectedEstado) || e.Estado == selectedEstado) &&
            (string.IsNullOrEmpty(selectedTipo) || e.TipoEvidencia == selectedTipo)
        ).ToList();
    }

    private void OnEstadoFilterChanged(ChangeEventArgs e)
    {
        selectedEstado = e.Value?.ToString() ?? "";
        AplicarFiltros();
        StateHasChanged();
    }

    private void OnTipoFilterChanged(ChangeEventArgs e)
    {
        selectedTipo = e.Value?.ToString() ?? "";
        AplicarFiltros();
        StateHasChanged();
    }

    private void ClearFilters()
    {
        selectedEstado = "";
        selectedTipo = "";
        AplicarFiltros();
        StateHasChanged();
    }

    // Modal management
    private void ShowEvidenciasModal()
    {
        showEvidenciasModal = true;
        StateHasChanged();
    }

    private void CloseEvidenciasModal()
    {
        showEvidenciasModal = false;
        StateHasChanged();
    }

    private async Task OnEvidenciasCreated()
    {
        showEvidenciasModal = false;
        await CargarEvidencias();
        await OnEvidenciasChanged.InvokeAsync();
        ToastService.ShowSuccess("Evidencias de investigación agregadas exitosamente.");
    }

    // Edit functionality
    private void EditarEvidencia(EvidenciaInvestigacionViewModel evidencia)
    {
        evidenciaToEdit = new EvidenciaInvestigacionViewModel
        {
            Id = evidencia.Id,
            TipoEvidencia = evidencia.TipoEvidencia,
            TituloProyecto = evidencia.TituloProyecto,
            InstitucionFinanciadora = evidencia.InstitucionFinanciadora,
            RolInvestigador = evidencia.RolInvestigador,
            FechaInicio = evidencia.FechaInicio,
            FechaFin = evidencia.FechaFin,
            MesesDuracion = evidencia.MesesDuracion,
            CodigoProyecto = evidencia.CodigoProyecto,
            AreaTematica = evidencia.AreaTematica,
            Descripcion = evidencia.Descripcion
        };
        StateHasChanged();
    }

    private void CancelEdit()
    {
        evidenciaToEdit = null;
        StateHasChanged();
    }

    private async Task SaveEditedEvidencia()
    {
        if (evidenciaToEdit == null) return;

        try
        {
            var token = await LocalStorage.GetItemAsync<string>("authToken");
            Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var editData = new
            {
                evidenciaToEdit.TipoEvidencia,
                evidenciaToEdit.TituloProyecto,
                evidenciaToEdit.InstitucionFinanciadora,
                evidenciaToEdit.RolInvestigador,
                evidenciaToEdit.FechaInicio,
                evidenciaToEdit.FechaFin,
                evidenciaToEdit.MesesDuracion,
                evidenciaToEdit.CodigoProyecto,
                evidenciaToEdit.AreaTematica,
                evidenciaToEdit.Descripcion
            };

            var json = JsonSerializer.Serialize(editData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await Http.PutAsync($"api/evidencias-investigacion/editar-metadatos/{evidenciaToEdit.Id}", content);

            if (response.IsSuccessStatusCode)
            {
                evidenciaToEdit = null;
                await CargarEvidencias();
                await OnEvidenciasChanged.InvokeAsync();
                ToastService.ShowSuccess("Evidencia actualizada exitosamente.");
            }
            else
            {
                ToastService.ShowError("Error al actualizar la evidencia.");
            }
        }
        catch (Exception ex)
        {
            ToastService.ShowError($"Error: {ex.Message}");
        }
    }

    // File replacement functionality
    private void ReemplazarArchivoEvidencia(EvidenciaInvestigacionViewModel evidencia)
    {
        evidenciaToReplaceFile = evidencia;
        newFile = null;
        replaceFileError = null;
        StateHasChanged();
    }

    private void CancelReplaceFile()
    {
        evidenciaToReplaceFile = null;
        newFile = null;
        replaceFileError = null;
        StateHasChanged();
    }

    private void OnReplaceFileSelected(InputFileChangeEventArgs e)
    {
        newFile = e.File;
        replaceFileError = null;

        if (newFile != null)
        {
            if (newFile.ContentType != "application/pdf")
            {
                replaceFileError = "Solo se permiten archivos PDF.";
                newFile = null;
            }
            else if (newFile.Size > 10 * 1024 * 1024) // 10 MB
            {
                replaceFileError = "El archivo no puede ser mayor a 10 MB.";
                newFile = null;
            }
        }

        StateHasChanged();
    }

    private async Task SaveReplacedFile()
    {
        if (evidenciaToReplaceFile == null || newFile == null) return;

        try
        {
            isReplacingFile = true;
            StateHasChanged();

            var token = await LocalStorage.GetItemAsync<string>("authToken");
            Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var content = new MultipartFormDataContent();
            using var fileContent = new StreamContent(newFile.OpenReadStream(10 * 1024 * 1024));
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(newFile.ContentType);
            content.Add(fileContent, "Archivo", newFile.Name);

            var response = await Http.PutAsync($"api/evidencias-investigacion/reemplazar-archivo/{evidenciaToReplaceFile.Id}", content);

            if (response.IsSuccessStatusCode)
            {
                evidenciaToReplaceFile = null;
                newFile = null;
                await CargarEvidencias();
                await OnEvidenciasChanged.InvokeAsync();
                ToastService.ShowSuccess("Archivo reemplazado exitosamente.");
            }
            else
            {
                ToastService.ShowError("Error al reemplazar el archivo.");
            }
        }
        catch (Exception ex)
        {
            ToastService.ShowError($"Error: {ex.Message}");
        }
        finally
        {
            isReplacingFile = false;
            StateHasChanged();
        }
    }

    // Other actions
    private async Task VisualizarEvidencia(Guid evidenciaId)
    {
        try
        {
            var token = await LocalStorage.GetItemAsync<string>("authToken");
            Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await Http.GetAsync($"api/evidencias-investigacion/visualizar-archivo/{evidenciaId}");

            if (response.IsSuccessStatusCode)
            {
                var fileBytes = await response.Content.ReadAsByteArrayAsync();
                var base64 = Convert.ToBase64String(fileBytes);
                
                // Obtener el título de la evidencia para mostrar en el modal
                var evidencia = evidencias?.FirstOrDefault(e => e.Id == evidenciaId);
                var title = evidencia?.TituloProyecto ?? "Evidencia de Investigación";
                
                // Mostrar PDF usando el método en download.js
                await JSRuntime.InvokeVoidAsync("showPdfInModal", base64, title);
            }
            else
            {
                ToastService.ShowError("Error al visualizar el archivo. Código: " + (int)response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            ToastService.ShowError($"Error al visualizar: {ex.Message}");
        }
    }

    private async Task DescargarEvidencia(Guid evidenciaId)
    {
        try
        {
            var token = await LocalStorage.GetItemAsync<string>("authToken");
            Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await Http.GetAsync($"api/evidencias-investigacion/visualizar-archivo/{evidenciaId}");

            if (response.IsSuccessStatusCode)
            {
                var fileBytes = await response.Content.ReadAsByteArrayAsync();
                var evidencia = evidencias?.FirstOrDefault(e => e.Id == evidenciaId);
                var fileName = evidencia?.ArchivoNombre ?? "evidencia.pdf";
                
                await JSRuntime.InvokeVoidAsync("downloadFile", fileName, Convert.ToBase64String(fileBytes), "application/pdf");
            }
            else
            {
                ToastService.ShowError("Error al descargar el archivo. Código: " + (int)response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            ToastService.ShowError($"Error al descargar: {ex.Message}");
        }
    }

    private void VerMotivoRechazoEvidencia(EvidenciaInvestigacionViewModel evidencia)
    {
        evidenciaRechazada = evidencia;
        StateHasChanged();
    }

    private void CerrarMotivoRechazo()
    {
        evidenciaRechazada = null;
        StateHasChanged();
    }

    private async Task ActualizarEvidencias()
    {
        await OnActualizarEvidencias.InvokeAsync();
    }

    private async Task EliminarEvidencia(Guid evidenciaId)
    {
        await OnEliminarEvidencia.InvokeAsync(evidenciaId);
    }

    private async Task ReenviarEvidencia(Guid evidenciaId)
    {
        await OnReenviarEvidencia.InvokeAsync(evidenciaId);
    }

    // Permission checks
    private bool PuedeVisualizarEvidencia(string estado) => !string.IsNullOrEmpty(estado);
    private bool PuedeEditarEvidencia(string estado) => estado == "Pendiente" || estado == "Rechazada";
    private bool PuedeReemplazarArchivoEvidencia(string estado) => estado == "Pendiente" || estado == "Rechazada";
    private bool PuedeReenviarEvidencia(string estado) => estado == "Rechazada";
    private bool PuedeEliminarEvidencia(string estado) => estado == "Pendiente" || estado == "Rechazada";
}
