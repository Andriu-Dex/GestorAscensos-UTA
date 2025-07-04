# Implementación de Reenvío de Solicitudes con Selección de Documentos

## Resumen

Se implementó la funcionalidad para que los usuarios puedan reenviar solicitudes de ascenso rechazadas, permitiendo la selección y modificación de documentos antes del reenvío, similar al flujo de obras académicas.

## Estado Actual del Código

### ❌ Problemas Detectados en Solicitudes.razor.txt

1. **Código Duplicado en ShowReenviarConfirmation**

   ```csharp
   // Línea ~1058: Código duplicado y conflictivo
   private void ShowReenviarConfirmation(SolicitudAscensoDto solicitud)
   {
       // Primera implementación
       showReenviarFormulario = true; // ❌ Variable no definida

       // Segunda implementación duplicada
       showReenviarConfirmation = true;
   }
   ```

2. **Variable No Definida**

   - `showReenviarFormulario` se usa pero no está declarada en las variables de estado

3. **Falta Modal de Selección de Documentos**

   - Solo existe el modal de confirmación simple
   - No hay integración con DocumentosSelector

4. **Métodos Incompletos**
   - Algunos métodos están truncados con `{…}`

### ✅ Funcionalidad Backend Implementada

#### 1. **Interfaz ISolicitudService**

```csharp
// Método agregado
Task<bool> ReenviarSolicitudConDocumentosAsync(
    Guid solicitudId,
    Guid docenteId,
    Dictionary<string, List<string>> documentosSeleccionados);
```

#### 2. **Servicio SolicitudService**

```csharp
public async Task<bool> ReenviarSolicitudConDocumentosAsync(
    Guid solicitudId,
    Guid docenteId,
    Dictionary<string, List<string>> documentosSeleccionados)
{
    // Elimina documentos existentes
    // Asocia nuevos documentos seleccionados
    // Cambia estado a Pendiente
    // Limpia datos de rechazo
}
```

#### 3. **Controller SolicitudesController**

```csharp
[HttpPut("{id}/reenviar-con-documentos")]
public async Task<IActionResult> ReenviarSolicitudConDocumentos(
    Guid id,
    [FromBody] ReenviarConDocumentosRequest request)
```

#### 4. **DTO ReenviarConDocumentosRequest**

```csharp
public class ReenviarConDocumentosRequest
{
    public Dictionary<string, List<string>> DocumentosSeleccionados { get; set; } = new();
}
```

## ✅ Cambios Necesarios para Corregir Frontend

### 1. **Crear archivo Solicitudes.razor corregido**

#### Variables de Estado a Agregar:

```csharp
// Estados para reenvío con documentos
private bool showReenviarFormulario = false;
private DocumentosSelector? reenviarDocumentosSelector;
private int totalDocumentosReenvio = 0;
private bool isReenviandoConDocumentos = false;
```

#### Modal de Reenvío con DocumentosSelector:

```html
<!-- Modal para reenvío con selección de documentos -->
@if (showReenviarFormulario && currentSolicitudInfo != null) {
<div
  class="modal fade show"
  style="display: block;"
  tabindex="-1"
  role="dialog"
>
  <div class="modal-dialog modal-xl" role="document">
    <div class="modal-content">
      <div
        class="modal-header"
        style="background-color: #8a1538; color: white;"
      >
        <h5 class="modal-title">
          <i class="bi bi-arrow-repeat"></i> Reenviar Solicitud - Seleccionar
          Documentos
        </h5>
        <button
          type="button"
          class="btn-close btn-close-white"
          @onclick="CloseReenviarFormulario"
        ></button>
      </div>
      <div class="modal-body">
        <!-- Información de la solicitud -->
        <div class="alert alert-info">
          <h6>
            Solicitud: SOL-@currentSolicitudInfo.Id.ToString().Substring(0, 8)
          </h6>
          <p>
            Seleccione los documentos que desea enviar con la solicitud
            reenviada.
          </p>
        </div>

        <!-- Componente selector de documentos -->
        <DocumentosSelector
          @ref="reenviarDocumentosSelector"
          OnSelectionChanged="OnReenvioDocumentSelectionChanged"
        />

        @if (totalDocumentosReenvio > 0) {
        <div class="alert alert-success mt-3">
          <i class="bi bi-check-circle"></i>
          Se han seleccionado
          <strong>@totalDocumentosReenvio</strong> documentos.
        </div>
        } else {
        <div class="alert alert-warning mt-3">
          <i class="bi bi-exclamation-triangle"></i>
          Debe seleccionar al menos un documento para reenviar la solicitud.
        </div>
        }
      </div>
      <div class="modal-footer">
        <button
          type="button"
          class="btn btn-secondary"
          @onclick="CloseReenviarFormulario"
          disabled="@isReenviandoConDocumentos"
        >
          <i class="bi bi-x-lg"></i> Cancelar
        </button>
        <button
          type="button"
          class="btn btn-primary"
          @onclick="ConfirmReenviarConDocumentos"
          disabled="@(isReenviandoConDocumentos || totalDocumentosReenvio == 0)"
          style="background-color: #8a1538; border-color: #8a1538;"
        >
          @if (isReenviandoConDocumentos) {
          <span class="spinner-border spinner-border-sm me-2"></span>
          @:Reenviando... } else {
          <i class="bi bi-arrow-repeat"></i>
          @: Reenviar con Documentos Seleccionados }
        </button>
      </div>
    </div>
  </div>
</div>
<div class="modal-backdrop fade show"></div>
}
```

#### Métodos a Implementar:

```csharp
private void ShowReenviarConfirmation(SolicitudAscensoDto solicitud)
{
    currentSolicitudInfo = new SolicitudData
    {
        Id = solicitud.Id,
        NivelActual = solicitud.NivelActual,
        NivelSolicitado = solicitud.NivelSolicitado,
        Estado = solicitud.Estado,
        FechaSolicitud = solicitud.FechaSolicitud
    };

    showReenviarFormulario = true;
    showDetails = false;
}

private void CloseReenviarFormulario()
{
    showReenviarFormulario = false;
    currentSolicitudInfo = null;
    totalDocumentosReenvio = 0;
    reenviarDocumentosSelector = null;
    isReenviandoConDocumentos = false;
}

private void OnReenvioDocumentSelectionChanged(Dictionary<string, bool> seleccionados)
{
    if (reenviarDocumentosSelector != null)
    {
        totalDocumentosReenvio = reenviarDocumentosSelector.GetTotalSelectedCount();
    }
    StateHasChanged();
}

private async Task ConfirmReenviarConDocumentos()
{
    if (currentSolicitudInfo == null || reenviarDocumentosSelector == null) return;

    isReenviandoConDocumentos = true;
    StateHasChanged();

    try
    {
        var token = await LocalStorage.GetItemAsStringAsync("authToken");
        Http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var documentosSeleccionados = reenviarDocumentosSelector.GetSelectedDocuments();
        var request = new { DocumentosSeleccionados = documentosSeleccionados };

        var response = await Http.PutAsJsonAsync($"api/solicitudascenso/{currentSolicitudInfo.Id}/reenviar-con-documentos", request);

        if (response.IsSuccessStatusCode)
        {
            ToastService.ShowSuccess("Solicitud reenviada exitosamente con los nuevos documentos");
            CloseReenviarFormulario();
            await LoadSolicitudes();
        }
        else
        {
            var errorMessage = await response.GetErrorMessageAsync();
            ToastService.ShowError($"Error al reenviar solicitud: {errorMessage}");
        }
    }
    catch (Exception ex)
    {
        ToastService.ShowError($"Error al reenviar solicitud: {ex.Message}");
    }
    finally
    {
        isReenviandoConDocumentos = false;
        StateHasChanged();
    }
}
```

### 2. **Cambio en SolicitudData**

```csharp
public class SolicitudData  // Cambiar nombre de SolicitudInfo a SolicitudData
{
    public Guid Id { get; set; }
    public string NivelActual { get; set; } = string.Empty;
    public string NivelSolicitado { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaSolicitud { get; set; }
}
```

## 🔧 Pasos para Completar la Implementación

1. **Crear archivo Solicitudes.razor limpio** - Eliminar duplicaciones y errores
2. **Agregar variables de estado faltantes** - Para el formulario de reenvío
3. **Implementar modal de selección de documentos** - Con DocumentosSelector
4. **Corregir métodos del frontend** - Eliminar código duplicado
5. **Probar integración completa** - Backend + Frontend

## 📋 Flujo de Usuario Implementado

1. **Usuario ve solicitud rechazada** → Clic en "Reenviar"
2. **Se abre modal de selección** → Muestra DocumentosSelector
3. **Usuario selecciona documentos** → Obras, certificados, evidencias
4. **Confirma reenvío** → Llama al endpoint con documentos
5. **Backend procesa** → Elimina documentos antiguos, asocia nuevos
6. **Solicitud cambia a Pendiente** → Lista se actualiza

## ✅ Ventajas de la Implementación

- **Reutilización de componentes**: Usa DocumentosSelector existente
- **Experiencia consistente**: Similar a creación de nueva solicitud
- **Flexibilidad**: Usuario puede cambiar documentos antes de reenviar
- **Integridad de datos**: Backend maneja correctamente las asociaciones
- **Feedback visual**: Modales informativos y estados de carga

## 🔄 Endpoints API Disponibles

- `PUT /api/solicitudascenso/{id}/reenviar` - Reenvío simple (ya existía)
- `PUT /api/solicitudascenso/{id}/reenviar-con-documentos` - Reenvío con documentos (nuevo)

La implementación está lista en el backend, solo falta corregir el frontend siguiendo esta documentación.
