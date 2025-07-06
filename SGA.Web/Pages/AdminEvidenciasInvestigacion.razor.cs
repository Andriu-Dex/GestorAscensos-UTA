using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Blazored.Toast.Services;
using Blazored.LocalStorage;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using SGA.Web.Models;

namespace SGA.Web.Pages
{
    public partial class AdminEvidenciasInvestigacion : ComponentBase
    {
        [Inject] private HttpClient Http { get; set; } = null!;
        [Inject] private ILocalStorageService LocalStorage { get; set; } = null!;
        [Inject] private IToastService ToastService { get; set; } = null!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = null!;

        // Variables de estado
        private List<EvidenciaInvestigacionViewModel>? evidencias;
        private List<EvidenciaInvestigacionViewModel>? evidenciasFiltradas;
        private bool isLoading = true;
        private bool isLoadingPdf = false;
        private bool isGuardandoRevision = false;

        // Filtros
        private string filtroEstado = "";
        private string filtroTipo = "";
        private string busquedaTitulo = "";
        private string busquedaDocente = "";

        // Contadores
        private int cantidadPendientes = 0;
        private int cantidadAprobadas = 0;
        private int cantidadRechazadas = 0;

        // Modal de revisión
        private EvidenciaInvestigacionViewModel? evidenciaEnRevision;
        private bool? decisionAprobada;
        private string comentariosRevision = "";
        private string pdfDataUrl = "";

        // Modal de motivo de rechazo
        private EvidenciaInvestigacionViewModel? evidenciaRechazada;
        private bool showModalRechazo = false;

        // Modal de visualización de PDF
        private bool showVisualizarModal = false;
        private EvidenciaInvestigacionViewModel? evidenciaParaVisualizar;

        protected override async Task OnInitializedAsync()
        {
            await ConfigurarHttpClient();
            await CargarEvidencias();
        }

        private async Task ConfigurarHttpClient()
        {
            try
            {
                var token = await LocalStorage.GetItemAsync<string>("token");
                if (!string.IsNullOrEmpty(token))
                {
                    Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }
            catch (Exception ex)
            {
                ToastService.ShowError($"Error al configurar autenticación: {ex.Message}");
            }
        }

        private async Task CargarEvidencias()
        {
            try
            {
                isLoading = true;
                var response = await Http.GetAsync("api/evidencias-investigacion/admin/todas");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseDto = JsonSerializer.Deserialize<ResponseSolicitudesEvidenciasAdminDto>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (responseDto?.Exitoso == true)
                    {
                        evidencias = responseDto.Evidencias ?? new List<EvidenciaInvestigacionViewModel>();
                        ActualizarContadores();
                        AplicarFiltros();
                        ToastService.ShowSuccess($"Se cargaron {evidencias.Count} evidencias");
                    }
                    else
                    {
                        ToastService.ShowError(responseDto?.Mensaje ?? "Error al cargar evidencias");
                        evidencias = new List<EvidenciaInvestigacionViewModel>();
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ToastService.ShowError($"Error del servidor: {response.StatusCode}");
                    evidencias = new List<EvidenciaInvestigacionViewModel>();
                }
            }
            catch (Exception ex)
            {
                ToastService.ShowError($"Error al cargar evidencias: {ex.Message}");
                evidencias = new List<EvidenciaInvestigacionViewModel>();
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private void ActualizarContadores()
        {
            if (evidencias == null) return;

            cantidadPendientes = evidencias.Count(e => e.Estado == "Pendiente");
            cantidadAprobadas = evidencias.Count(e => e.Estado == "Aprobada");
            cantidadRechazadas = evidencias.Count(e => e.Estado == "Rechazada");
        }

        #region Filtros

        private void FiltrarPendientes()
        {
            filtroEstado = "Pendiente";
            AplicarFiltros();
        }

        private void FiltrarAprobadas()
        {
            filtroEstado = "Aprobada";
            AplicarFiltros();
        }

        private void FiltrarRechazadas()
        {
            filtroEstado = "Rechazada";
            AplicarFiltros();
        }

        private void MostrarTodas()
        {
            filtroEstado = "";
            AplicarFiltros();
        }

        private void AplicarFiltros()
        {
            if (evidencias == null)
            {
                evidenciasFiltradas = new List<EvidenciaInvestigacionViewModel>();
                return;
            }

            evidenciasFiltradas = evidencias.Where(e =>
                (string.IsNullOrEmpty(filtroEstado) || e.Estado == filtroEstado) &&
                (string.IsNullOrEmpty(filtroTipo) || e.TipoEvidencia.Contains(filtroTipo, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(busquedaTitulo) || e.TituloProyecto.Contains(busquedaTitulo, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(busquedaDocente) || e.DocenteCedula.Contains(busquedaDocente, StringComparison.OrdinalIgnoreCase))
            ).ToList();

            StateHasChanged();
        }

        private void LimpiarFiltros()
        {
            filtroEstado = "";
            filtroTipo = "";
            busquedaTitulo = "";
            busquedaDocente = "";
            AplicarFiltros();
        }

        #endregion

        #region Revisión de evidencias

        private async Task MostrarModalRevision(EvidenciaInvestigacionViewModel evidencia)
        {
            evidenciaEnRevision = evidencia;
            decisionAprobada = null;
            comentariosRevision = "";
            pdfDataUrl = "";
            
            await CargarPdfEvidencia(evidencia.Id);
            StateHasChanged();
        }

        private async Task CargarPdfEvidencia(Guid evidenciaId)
        {
            try
            {
                isLoadingPdf = true;
                StateHasChanged();

                var response = await Http.GetAsync($"api/evidencias-investigacion/{evidenciaId}/archivo");

                if (response.IsSuccessStatusCode)
                {
                    var contenidoBytes = await response.Content.ReadAsByteArrayAsync();
                    var contenidoBase64 = Convert.ToBase64String(contenidoBytes);
                    pdfDataUrl = $"data:application/pdf;base64,{contenidoBase64}";
                }
                else
                {
                    ToastService.ShowWarning("No se pudo cargar el archivo PDF. Código: " + (int)response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                ToastService.ShowError($"Error al cargar PDF: {ex.Message}");
            }
            finally
            {
                isLoadingPdf = false;
                StateHasChanged();
            }
        }

        private void CerrarModalRevision()
        {
            evidenciaEnRevision = null;
            decisionAprobada = null;
            comentariosRevision = "";
            pdfDataUrl = "";
            StateHasChanged();
        }

        private async Task GuardarRevision()
        {
            if (evidenciaEnRevision == null || decisionAprobada == null)
            {
                ToastService.ShowWarning("Debe seleccionar una decisión para continuar");
                return;
            }

            if (!decisionAprobada.Value && string.IsNullOrWhiteSpace(comentariosRevision))
            {
                ToastService.ShowWarning("Debe proporcionar un motivo para rechazar la evidencia");
                return;
            }

            try
            {
                isGuardandoRevision = true;
                StateHasChanged();

                var revisionDto = new RevisionSolicitudEvidenciaDto
                {
                    SolicitudId = evidenciaEnRevision.Id,
                    Accion = decisionAprobada.Value ? "Aprobar" : "Rechazar",
                    Comentarios = comentariosRevision
                };

                var json = JsonSerializer.Serialize(revisionDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await Http.PostAsync("api/evidencias-investigacion/admin/revisar", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseDto = JsonSerializer.Deserialize<ResponseGenericoEvidenciaDto>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (responseDto?.Exitoso == true)
                    {
                        ToastService.ShowSuccess($"Evidencia {(decisionAprobada.Value ? "aprobada" : "rechazada")} exitosamente");
                        CerrarModalRevision();
                        await CargarEvidencias(); // Recargar la lista
                    }
                    else
                    {
                        ToastService.ShowError(responseDto?.Mensaje ?? "Error al procesar la revisión");
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ToastService.ShowError($"Error del servidor: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                ToastService.ShowError($"Error al guardar revisión: {ex.Message}");
            }
            finally
            {
                isGuardandoRevision = false;
                StateHasChanged();
            }
        }

    /// <summary>
    /// Método para cargar archivo PDF de evidencia
    /// </summary>


    /// <summary>
    /// Aprobar una evidencia directamente sin confirmación
    /// </summary>
    private async Task AprobarEvidencia(EvidenciaInvestigacionViewModel evidencia)
    {
        try
        {
            isGuardandoRevision = true;
            StateHasChanged();

            var revisionDto = new RevisionSolicitudEvidenciaDto
            {
                SolicitudId = evidencia.Id,
                Accion = "Aprobar",
                Comentarios = "Evidencia aprobada por el administrador"
            };

            var json = JsonSerializer.Serialize(revisionDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await Http.PostAsync("api/evidencias-investigacion/admin/revisar", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseDto = JsonSerializer.Deserialize<ResponseGenericoEvidenciaDto>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (responseDto?.Exitoso == true)
                {
                    ToastService.ShowSuccess("Evidencia aprobada exitosamente");
                    await CargarEvidencias(); // Recargar la lista
                }
                else
                {
                    ToastService.ShowError(responseDto?.Mensaje ?? "Error al procesar la aprobación");
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ToastService.ShowError($"Error del servidor: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            ToastService.ShowError($"Error al aprobar evidencia: {ex.Message}");
        }
        finally
        {
            isGuardandoRevision = false;
            StateHasChanged();
        }
    }

        /// <summary>
        /// Rechazar una evidencia (abre modal para motivo)
        /// </summary>
        private void RechazarEvidencia(EvidenciaInvestigacionViewModel evidencia)
        {
            evidenciaEnRevision = evidencia;
            decisionAprobada = false;
            comentariosRevision = "";
            showModalRechazo = true;
            StateHasChanged();
        }

        private void CerrarModalRechazo()
        {
            showModalRechazo = false;
            evidenciaEnRevision = null;
            decisionAprobada = null;
            comentariosRevision = "";
            StateHasChanged();
        }

        private async Task ConfirmarRechazo()
        {
            if (evidenciaEnRevision == null || string.IsNullOrWhiteSpace(comentariosRevision))
            {
                ToastService.ShowWarning("Debe proporcionar un motivo para rechazar la evidencia");
                return;
            }

            try
            {
                isGuardandoRevision = true;
                StateHasChanged();

                var revisionDto = new RevisionSolicitudEvidenciaDto
                {
                    SolicitudId = evidenciaEnRevision.Id,
                    Accion = "Rechazar",
                    Comentarios = comentariosRevision
                };

                var json = JsonSerializer.Serialize(revisionDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await Http.PostAsync("api/evidencias-investigacion/admin/revisar", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseDto = JsonSerializer.Deserialize<ResponseGenericoEvidenciaDto>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (responseDto?.Exitoso == true)
                    {
                        ToastService.ShowSuccess("Evidencia rechazada exitosamente");
                        CerrarModalRechazo();
                        await CargarEvidencias(); // Recargar la lista
                    }
                    else
                    {
                        ToastService.ShowError(responseDto?.Mensaje ?? "Error al procesar el rechazo");
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ToastService.ShowError($"Error del servidor: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                ToastService.ShowError($"Error al rechazar evidencia: {ex.Message}");
            }
            finally
            {
                isGuardandoRevision = false;
                StateHasChanged();
            }
        }

        #endregion

        #region Visualización

        private async Task VisualizarEvidencia(EvidenciaInvestigacionViewModel evidencia)
        {
            evidenciaParaVisualizar = evidencia;
            showVisualizarModal = true;
            pdfDataUrl = "";
            
            await CargarPdfEvidencia(evidencia.Id);
            StateHasChanged();
        }

        private void CerrarModalVisualizarPDF()
        {
            showVisualizarModal = false;
            evidenciaParaVisualizar = null;
            pdfDataUrl = "";
            StateHasChanged();
        }

        private async Task AbrirPdfEnNuevaVentana()
        {
            if (!string.IsNullOrEmpty(pdfDataUrl))
            {
                await JSRuntime.InvokeVoidAsync("open", pdfDataUrl, "_blank");
            }
        }

        private void VerMotivoRechazo(EvidenciaInvestigacionViewModel evidencia)
        {
            evidenciaRechazada = evidencia;
            StateHasChanged();
        }

        private void CerrarMotivoRechazo()
        {
            evidenciaRechazada = null;
            StateHasChanged();
        }

        #endregion

        #region Métodos auxiliares

        #endregion
    }
}
