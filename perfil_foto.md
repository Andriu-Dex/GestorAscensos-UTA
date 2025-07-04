# Implementación Completa del Sistema de Fotos de Perfil

## Descripción General

Este documento describe la implementación completa de un sistema robusto, modular y escalable para la gestión de fotos de perfil de docentes. El sistema almacena las imágenes en la base de datos (no localmente), sigue buenas prácticas de OOP, mantiene la normalización 3NF y proporciona endpoints claros y modulares.

## Características Principales

- **Almacenamiento en BD**: Las imágenes se guardan como BLOB en la base de datos
- **Validación robusta**: Tamaño máximo 5MB, formatos permitidos (JPEG, PNG, WebP)
- **Compresión automática**: Optimización de imágenes para reducir tamaño
- **Respuestas estructuradas**: JSON con mensajes claros para toast/modals
- **Middleware personalizado**: Control de límites por endpoint
- **Configuración dinámica**: El frontend obtiene límites desde el backend

---

## 1. Estructura de Base de Datos

### 1.1 Modificación de la tabla Docentes

```sql
-- Agregar columna para foto de perfil si no existe
ALTER TABLE Docentes
ADD FotoPerfil VARBINARY(MAX) NULL;

-- Opcional: Agregar metadatos de la imagen
ALTER TABLE Docentes
ADD FotoPerfilTipo NVARCHAR(50) NULL,
    FotoPerfilTamaño INT NULL,
    FotoPerfilFechaSubida DATETIME2 NULL;
```

### 1.2 Índices recomendados

```sql
-- Índice para consultas rápidas
CREATE INDEX IX_Docentes_FotoPerfil
ON Docentes (DocenteId)
WHERE FotoPerfil IS NOT NULL;
```

---

## 2. Backend - Dominio y Entidades

### 2.1 Actualizar entidad Docente

**Archivo**: `SGA.Domain/Entities/Docente.cs`

```csharp
public class Docente
{
    // ... propiedades existentes ...

    [Column(TypeName = "varbinary(max)")]
    public byte[]? FotoPerfil { get; set; }

    [MaxLength(50)]
    public string? FotoPerfilTipo { get; set; }

    public int? FotoPerfilTamaño { get; set; }

    public DateTime? FotoPerfilFechaSubida { get; set; }

    // Propiedad calculada para mostrar en frontend
    [NotMapped]
    public string? FotoPerfilBase64 => FotoPerfil != null && !string.IsNullOrEmpty(FotoPerfilTipo)
        ? $"data:{FotoPerfilTipo};base64,{Convert.ToBase64String(FotoPerfil)}"
        : null;
}
```

---

## 3. Backend - Constantes y Configuración

### 3.1 Crear constantes de límites de archivo

**Archivo**: `SGA.Application/Constants/FileLimits.cs`

```csharp
namespace SGA.Application.Constants
{
    public static class FileLimits
    {
        // Límites para fotos de perfil
        public static class ProfileImages
        {
            public const int MaxSizeBytes = 5 * 1024 * 1024; // 5MB
            public const int MaxSizeMB = 5;

            public static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
            public static readonly string[] AllowedMimeTypes = {
                "image/jpeg",
                "image/png",
                "image/webp"
            };

            // Configuración de compresión
            public const int MaxWidth = 800;
            public const int MaxHeight = 800;
            public const int JpegQuality = 85;
        }

        // Límites para otros tipos de archivos (futuro)
        public static class Documents
        {
            public const int MaxSizeBytes = 10 * 1024 * 1024; // 10MB
            public static readonly string[] AllowedExtensions = { ".pdf", ".doc", ".docx" };
        }
    }
}
```

---

## 4. Backend - DTOs

### 4.1 DTO para respuesta de carga de archivos

**Archivo**: `SGA.Application/DTOs/Responses/FileUploadResponse.cs`

```csharp
namespace SGA.Application.DTOs.Responses
{
    public class FileUploadResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public FileUploadError? Error { get; set; }

        // Factory methods
        public static FileUploadResponse SuccessResponse(string message, string? imageUrl = null)
        {
            return new FileUploadResponse
            {
                Success = true,
                Message = message,
                ImageUrl = imageUrl
            };
        }

        public static FileUploadResponse ErrorResponse(string message, FileUploadErrorType errorType)
        {
            return new FileUploadResponse
            {
                Success = false,
                Message = message,
                Error = new FileUploadError
                {
                    Type = errorType,
                    Details = message
                }
            };
        }
    }

    public class FileUploadError
    {
        public FileUploadErrorType Type { get; set; }
        public string Details { get; set; } = string.Empty;
    }

    public enum FileUploadErrorType
    {
        InvalidFile,
        FileTooLarge,
        InvalidFormat,
        ProcessingError,
        DatabaseError
    }
}
```

### 4.2 DTO para configuración de carga

**Archivo**: `SGA.Application/DTOs/Responses/UploadConfigResponse.cs`

```csharp
namespace SGA.Application.DTOs.Responses
{
    public class UploadConfigResponse
    {
        public int MaxSizeBytes { get; set; }
        public int MaxSizeMB { get; set; }
        public string[] AllowedExtensions { get; set; } = Array.Empty<string>();
        public string[] AllowedMimeTypes { get; set; } = Array.Empty<string>();
        public string AcceptAttribute { get; set; } = string.Empty;
    }
}
```

### 4.3 DTO para validación

**Archivo**: `SGA.Application/DTOs/Responses/ValidationResponse.cs`

```csharp
namespace SGA.Application.DTOs.Responses
{
    public class ValidationResponse
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        public string[] Errors { get; set; } = Array.Empty<string>();

        public static ValidationResponse Valid(string message = "Validación exitosa")
        {
            return new ValidationResponse { IsValid = true, Message = message };
        }

        public static ValidationResponse Invalid(string message, params string[] errors)
        {
            return new ValidationResponse
            {
                IsValid = false,
                Message = message,
                Errors = errors
            };
        }
    }
}
```

---

## 5. Backend - Servicios

### 5.1 Servicio de procesamiento de imágenes

**Archivo**: `SGA.Application/Services/ImageService.cs`

```csharp
using SGA.Application.Constants;
using SGA.Application.DTOs.Responses;
using SGA.Application.Interfaces;
using System.Drawing;
using System.Drawing.Imaging;

namespace SGA.Application.Services
{
    public class ImageService : IImageService
    {
        public ValidationResponse ValidateImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return ValidationResponse.Invalid("No se ha seleccionado ningún archivo");
            }

            // Validar tamaño
            if (file.Length > FileLimits.ProfileImages.MaxSizeBytes)
            {
                return ValidationResponse.Invalid(
                    $"El archivo es demasiado grande. Tamaño máximo permitido: {FileLimits.ProfileImages.MaxSizeMB}MB");
            }

            // Validar extensión
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!FileLimits.ProfileImages.AllowedExtensions.Contains(extension))
            {
                return ValidationResponse.Invalid(
                    $"Formato de archivo no permitido. Formatos permitidos: {string.Join(", ", FileLimits.ProfileImages.AllowedExtensions)}");
            }

            // Validar tipo MIME
            if (!FileLimits.ProfileImages.AllowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
            {
                return ValidationResponse.Invalid(
                    $"Tipo de contenido no válido: {file.ContentType}");
            }

            return ValidationResponse.Valid("Archivo válido");
        }

        public async Task<byte[]> ProcessImageAsync(IFormFile file)
        {
            using var inputStream = file.OpenReadStream();
            using var image = await Image.LoadAsync(inputStream);

            // Redimensionar si es necesario
            if (image.Width > FileLimits.ProfileImages.MaxWidth ||
                image.Height > FileLimits.ProfileImages.MaxHeight)
            {
                var (newWidth, newHeight) = CalculateNewDimensions(
                    image.Width, image.Height,
                    FileLimits.ProfileImages.MaxWidth,
                    FileLimits.ProfileImages.MaxHeight);

                image.Mutate(x => x.Resize(newWidth, newHeight));
            }

            // Convertir a JPEG para optimizar tamaño
            using var outputStream = new MemoryStream();
            await image.SaveAsJpegAsync(outputStream, new JpegEncoder
            {
                Quality = FileLimits.ProfileImages.JpegQuality
            });

            return outputStream.ToArray();
        }

        public string GetOptimizedMimeType(string originalMimeType)
        {
            // Convertir todo a JPEG para consistencia y optimización
            return "image/jpeg";
        }

        private static (int width, int height) CalculateNewDimensions(
            int originalWidth, int originalHeight,
            int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / originalWidth;
            var ratioY = (double)maxHeight / originalHeight;
            var ratio = Math.Min(ratioX, ratioY);

            return ((int)(originalWidth * ratio), (int)(originalHeight * ratio));
        }
    }
}
```

### 5.2 Actualizar servicio de docente

**Archivo**: `SGA.Application/Services/DocenteService.cs`

```csharp
// Agregar método para subir foto de perfil
public async Task<FileUploadResponse> UploadProfilePhotoAsync(int docenteId, IFormFile file)
{
    try
    {
        // Validar archivo
        var validation = _imageService.ValidateImage(file);
        if (!validation.IsValid)
        {
            return FileUploadResponse.ErrorResponse(validation.Message, FileUploadErrorType.InvalidFile);
        }

        // Obtener docente
        var docente = await _docenteRepository.GetByIdAsync(docenteId);
        if (docente == null)
        {
            return FileUploadResponse.ErrorResponse("Docente no encontrado", FileUploadErrorType.DatabaseError);
        }

        // Procesar imagen
        var processedImage = await _imageService.ProcessImageAsync(file);
        var optimizedMimeType = _imageService.GetOptimizedMimeType(file.ContentType);

        // Actualizar datos del docente
        docente.FotoPerfil = processedImage;
        docente.FotoPerfilTipo = optimizedMimeType;
        docente.FotoPerfilTamaño = processedImage.Length;
        docente.FotoPerfilFechaSubida = DateTime.UtcNow;

        // Guardar en base de datos
        await _docenteRepository.UpdateAsync(docente);

        // Generar URL de la imagen
        var imageUrl = $"data:{optimizedMimeType};base64,{Convert.ToBase64String(processedImage)}";

        return FileUploadResponse.SuccessResponse(
            "Foto de perfil actualizada correctamente",
            imageUrl);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al subir foto de perfil para docente {DocenteId}", docenteId);
        return FileUploadResponse.ErrorResponse(
            "Error interno del servidor al procesar la imagen",
            FileUploadErrorType.ProcessingError);
    }
}

// Método para obtener configuración de carga
public UploadConfigResponse GetUploadConfig()
{
    return new UploadConfigResponse
    {
        MaxSizeBytes = FileLimits.ProfileImages.MaxSizeBytes,
        MaxSizeMB = FileLimits.ProfileImages.MaxSizeMB,
        AllowedExtensions = FileLimits.ProfileImages.AllowedExtensions,
        AllowedMimeTypes = FileLimits.ProfileImages.AllowedMimeTypes,
        AcceptAttribute = string.Join(",", FileLimits.ProfileImages.AllowedMimeTypes)
    };
}
```

---

## 6. Backend - Interfaces

### 6.1 Interfaz del servicio de imágenes

**Archivo**: `SGA.Application/Interfaces/IImageService.cs`

```csharp
using SGA.Application.DTOs.Responses;

namespace SGA.Application.Interfaces
{
    public interface IImageService
    {
        ValidationResponse ValidateImage(IFormFile file);
        Task<byte[]> ProcessImageAsync(IFormFile file);
        string GetOptimizedMimeType(string originalMimeType);
    }
}
```

### 6.2 Actualizar interfaz del servicio de docente

**Archivo**: `SGA.Application/Interfaces/IDocenteService.cs`

```csharp
// Agregar métodos
Task<FileUploadResponse> UploadProfilePhotoAsync(int docenteId, IFormFile file);
UploadConfigResponse GetUploadConfig();
```

---

## 7. Backend - Middleware

### 7.1 Middleware para límites de archivos

**Archivo**: `SGA.Api/Middleware/FileUploadMiddleware.cs`

```csharp
using SGA.Application.Constants;

namespace SGA.Api.Middleware
{
    public class FileUploadMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<FileUploadMiddleware> _logger;

        public FileUploadMiddleware(RequestDelegate next, ILogger<FileUploadMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Solo aplicar a endpoints de carga de archivos
            if (context.Request.Path.StartsWithSegments("/api/docentes/upload-photo"))
            {
                var contentLength = context.Request.ContentLength;

                if (contentLength.HasValue && contentLength.Value > FileLimits.ProfileImages.MaxSizeBytes)
                {
                    _logger.LogWarning("Archivo demasiado grande: {Size} bytes", contentLength.Value);

                    context.Response.StatusCode = 413; // Payload Too Large
                    context.Response.ContentType = "application/json";

                    var errorResponse = new
                    {
                        success = false,
                        message = $"El archivo es demasiado grande. Tamaño máximo permitido: {FileLimits.ProfileImages.MaxSizeMB}MB",
                        error = new
                        {
                            type = "FileTooLarge",
                            details = $"Tamaño recibido: {contentLength.Value / (1024 * 1024)}MB"
                        }
                    };

                    await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(errorResponse));
                    return;
                }
            }

            await _next(context);
        }
    }
}
```

---

## 8. Backend - Controladores

### 8.1 Actualizar controlador de docentes

**Archivo**: `SGA.Api/Controllers/DocentesController.cs`

```csharp
[HttpPost("upload-photo")]
[RequestSizeLimit(FileLimits.ProfileImages.MaxSizeBytes)]
public async Task<ActionResult<FileUploadResponse>> UploadProfilePhoto(IFormFile file)
{
    try
    {
        // Obtener ID del docente del token
        var docenteId = GetCurrentDocenteId();
        if (docenteId == 0)
        {
            return Unauthorized(FileUploadResponse.ErrorResponse(
                "No se pudo identificar al docente",
                FileUploadErrorType.DatabaseError));
        }

        var result = await _docenteService.UploadProfilePhotoAsync(docenteId, file);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error en upload de foto de perfil");
        return StatusCode(500, FileUploadResponse.ErrorResponse(
            "Error interno del servidor",
            FileUploadErrorType.ProcessingError));
    }
}

[HttpGet("upload-config")]
public ActionResult<UploadConfigResponse> GetUploadConfig()
{
    try
    {
        var config = _docenteService.GetUploadConfig();
        return Ok(config);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al obtener configuración de carga");
        return StatusCode(500, "Error interno del servidor");
    }
}

// Método auxiliar para obtener el ID del docente actual
private int GetCurrentDocenteId()
{
    var userIdClaim = User.FindFirst("DocenteId")?.Value;
    return int.TryParse(userIdClaim, out var docenteId) ? docenteId : 0;
}
```

---

## 9. Backend - Configuración

### 9.1 Actualizar Program.cs

**Archivo**: `SGA.Api/Program.cs`

```csharp
// Configurar límites de archivos
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = FileLimits.ProfileImages.MaxSizeBytes;
    options.ValueLengthLimit = FileLimits.ProfileImages.MaxSizeBytes;
});

// Registrar servicios
builder.Services.AddScoped<IImageService, ImageService>();

// Configurar Kestrel para archivos grandes
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = FileLimits.ProfileImages.MaxSizeBytes;
});

// Agregar middleware
app.UseMiddleware<FileUploadMiddleware>();
```

### 9.2 Actualizar DependencyInjection.cs

**Archivo**: `SGA.Application/DependencyInjection.cs`

```csharp
public static IServiceCollection AddApplication(this IServiceCollection services)
{
    // ... servicios existentes ...

    services.AddScoped<IImageService, ImageService>();

    return services;
}
```

---

## 10. Frontend - Blazor

### 10.1 Componente de carga de foto de perfil

**Archivo**: `SGA.Web/Components/ProfilePhotoUpload.razor`

```html
@using SGA.Application.DTOs.Responses @inject HttpClient Http @inject IJSRuntime
JSRuntime

<div class="profile-photo-upload">
  <div class="current-photo mb-3">
    @if (!string.IsNullOrEmpty(CurrentPhotoUrl)) {
    <img
      src="@CurrentPhotoUrl"
      alt="Foto actual"
      class="profile-photo-preview"
    />
    } else {
    <div class="no-photo-placeholder">
      <i class="bi bi-person-circle"></i>
      <span>Sin foto</span>
    </div>
    }
  </div>

  <div class="upload-section">
    <InputFile
      OnChange="@OnFileSelected"
      accept="@acceptAttribute"
      class="form-control"
      disabled="@isUploading"
    />

    <div class="upload-info mt-2">
      <small class="text-muted">
        Formatos permitidos: @string.Join(", ", allowedExtensions)
        <br />
        Tamaño máximo: @maxSizeMB MB
      </small>
    </div>

    @if (isUploading) {
    <div class="upload-progress mt-3">
      <div class="d-flex align-items-center">
        <div class="spinner-border spinner-border-sm me-2" role="status"></div>
        <span>Subiendo imagen...</span>
      </div>
    </div>
    } @if (!string.IsNullOrEmpty(errorMessage)) {
    <div class="alert alert-danger mt-3">
      <i class="bi bi-exclamation-triangle me-2"></i>
      @errorMessage
    </div>
    }
  </div>
</div>

@code { [Parameter] public string? CurrentPhotoUrl { get; set; } [Parameter]
public EventCallback<string>
  OnPhotoUploaded { get; set; } private bool isUploading = false; private string
  errorMessage = string.Empty; private string acceptAttribute = string.Empty;
  private string[] allowedExtensions = Array.Empty<string
    >(); private int maxSizeMB = 5; protected override async Task
    OnInitializedAsync() { await LoadUploadConfig(); } private async Task
    LoadUploadConfig() { try { var response = await
    Http.GetAsync("api/docentes/upload-config"); if
    (response.IsSuccessStatusCode) { var config = await
    response.Content.ReadFromJsonAsync<UploadConfigResponse
      >(); if (config != null) { acceptAttribute = config.AcceptAttribute;
      allowedExtensions = config.AllowedExtensions; maxSizeMB =
      config.MaxSizeMB; } } } catch (Exception ex) { Console.WriteLine($"Error
      loading upload config: {ex.Message}"); } } private async Task
      OnFileSelected(InputFileChangeEventArgs e) { errorMessage = string.Empty;
      var file = e.File; if (file == null) return; // Validación en frontend if
      (file.Size > maxSizeMB * 1024 * 1024) { errorMessage = $"El archivo es
      demasiado grande. Tamaño máximo: {maxSizeMB}MB"; return; } var extension =
      Path.GetExtension(file.Name).ToLowerInvariant(); if
      (!allowedExtensions.Contains(extension)) { errorMessage = $"Formato no
      permitido. Use: {string.Join(", ", allowedExtensions)}"; return; } await
      UploadPhoto(file); } private async Task UploadPhoto(IBrowserFile file) {
      try { isUploading = true; StateHasChanged(); using var content = new
      MultipartFormDataContent(); using var stream =
      file.OpenReadStream(maxSizeMB * 1024 * 1024); using var streamContent =
      new StreamContent(stream); streamContent.Headers.ContentType = new
      MediaTypeHeaderValue(file.ContentType); content.Add(streamContent, "file",
      file.Name); var response = await
      Http.PostAsync("api/docentes/upload-photo", content); var responseContent
      = await response.Content.ReadAsStringAsync(); if
      (response.IsSuccessStatusCode) { var result =
      JsonSerializer.Deserialize<FileUploadResponse
        >(responseContent, new JsonSerializerOptions {
        PropertyNameCaseInsensitive = true }); if (result != null &&
        result.Success) { await JSRuntime.InvokeVoidAsync("showToast",
        result.Message, "success"); if (!string.IsNullOrEmpty(result.ImageUrl))
        { await OnPhotoUploaded.InvokeAsync(result.ImageUrl); } } else {
        errorMessage = result?.Message ?? "Error desconocido"; await
        JSRuntime.InvokeVoidAsync("showToast", errorMessage, "error"); } } else
        { var errorResult = JsonSerializer.Deserialize<FileUploadResponse
          >(responseContent, new JsonSerializerOptions {
          PropertyNameCaseInsensitive = true }); errorMessage =
          errorResult?.Message ?? $"Error del servidor: {response.StatusCode}";
          await JSRuntime.InvokeVoidAsync("showToast", errorMessage, "error"); }
          } catch (Exception ex) { errorMessage = $"Error al subir la imagen:
          {ex.Message}"; await JSRuntime.InvokeVoidAsync("showToast",
          errorMessage, "error"); } finally { isUploading = false;
          StateHasChanged(); } } }</FileUploadResponse
        ></FileUploadResponse
      ></UploadConfigResponse
    ></string
  ></string
>
```

### 10.2 CSS para el componente

**Archivo**: `SGA.Web/wwwroot/css/components/profile-photo-upload.css`

```css
.profile-photo-upload {
  max-width: 400px;
  margin: 0 auto;
}

.profile-photo-preview {
  width: 150px;
  height: 150px;
  border-radius: 50%;
  object-fit: cover;
  border: 3px solid #e9ecef;
  display: block;
  margin: 0 auto;
}

.no-photo-placeholder {
  width: 150px;
  height: 150px;
  border-radius: 50%;
  background-color: #f8f9fa;
  border: 3px solid #e9ecef;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  margin: 0 auto;
  color: #6c757d;
}

.no-photo-placeholder i {
  font-size: 4rem;
  margin-bottom: 0.5rem;
}

.upload-progress {
  text-align: center;
}

.upload-info {
  text-align: center;
}

@media (max-width: 576px) {
  .profile-photo-preview,
  .no-photo-placeholder {
    width: 120px;
    height: 120px;
  }

  .no-photo-placeholder i {
    font-size: 3rem;
  }
}
```

---

## 11. Integración en Páginas Existentes

### 11.1 Actualizar página de perfil

**Archivo**: `SGA.Web/Pages/Perfil.razor`

```html
<!-- Agregar en la sección de información personal -->
<div class="row">
  <div class="col-md-4">
    <div class="card">
      <div class="card-header">
        <h5><i class="bi bi-camera me-2"></i>Foto de Perfil</h5>
      </div>
      <div class="card-body">
        <ProfilePhotoUpload
          CurrentPhotoUrl="@userInfo?.FotoPerfilBase64"
          OnPhotoUploaded="@OnPhotoUpdated"
        />
      </div>
    </div>
  </div>
  <!-- ... resto del contenido ... -->
</div>

@code { private async Task OnPhotoUpdated(string newPhotoUrl) { // Actualizar la
información del usuario if (userInfo != null) { userInfo.FotoPerfil =
newPhotoUrl; StateHasChanged(); } // Opcional: Refrescar datos desde el servidor
await RefreshUserInfo(); } }
```

---

## 12. JavaScript Utilities

### 12.1 Funciones de soporte

**Archivo**: `SGA.Web/wwwroot/js/photo-upload.js`

```javascript
window.showToast = (message, type = "info") => {
  // Implementación usando Bootstrap Toast o librería de preferencia
  const toastElement = document.createElement("div");
  toastElement.className = `toast align-items-center text-white bg-${
    type === "success" ? "success" : "danger"
  } border-0`;
  toastElement.innerHTML = `
        <div class="d-flex">
            <div class="toast-body">
                ${message}
            </div>
            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
        </div>
    `;

  const container =
    document.getElementById("toast-container") || createToastContainer();
  container.appendChild(toastElement);

  const toast = new bootstrap.Toast(toastElement);
  toast.show();

  toastElement.addEventListener("hidden.bs.toast", () => {
    toastElement.remove();
  });
};

function createToastContainer() {
  const container = document.createElement("div");
  container.id = "toast-container";
  container.className = "toast-container position-fixed top-0 end-0 p-3";
  container.style.zIndex = "9999";
  document.body.appendChild(container);
  return container;
}
```

---

## 13. Testing

### 13.1 Tests unitarios para ImageService

**Archivo**: `SGA.Tests/Services/ImageServiceTests.cs`

```csharp
[TestClass]
public class ImageServiceTests
{
    private ImageService _imageService;

    [TestInitialize]
    public void Setup()
    {
        _imageService = new ImageService();
    }

    [TestMethod]
    public void ValidateImage_NullFile_ReturnsInvalid()
    {
        // Act
        var result = _imageService.ValidateImage(null);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Message.Contains("No se ha seleccionado"));
    }

    [TestMethod]
    public void ValidateImage_FileTooLarge_ReturnsInvalid()
    {
        // Arrange
        var mockFile = CreateMockFile("test.jpg", FileLimits.ProfileImages.MaxSizeBytes + 1);

        // Act
        var result = _imageService.ValidateImage(mockFile);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Message.Contains("demasiado grande"));
    }

    private IFormFile CreateMockFile(string fileName, long size)
    {
        var mock = new Mock<IFormFile>();
        mock.Setup(f => f.FileName).Returns(fileName);
        mock.Setup(f => f.Length).Returns(size);
        mock.Setup(f => f.ContentType).Returns("image/jpeg");
        return mock.Object;
    }
}
```

---

## 14. Configuración de Producción

### 14.1 Variables de entorno

```bash
# Límites de archivos
MAX_PROFILE_IMAGE_SIZE_MB=5
ALLOWED_IMAGE_FORMATS=jpg,jpeg,png,webp

# Configuración de compresión
IMAGE_MAX_WIDTH=800
IMAGE_MAX_HEIGHT=800
JPEG_QUALITY=85
```

### 14.2 Configuración de IIS

**Archivo**: `web.config`

```xml
<configuration>
  <system.webServer>
    <security>
      <requestFiltering>
        <requestLimits maxAllowedContentLength="5242880" /> <!-- 5MB -->
      </requestFiltering>
    </security>
  </system.webServer>
</configuration>
```

---

## 15. Monitoreo y Logs

### 15.1 Configuración de logging

**Archivo**: `appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "SGA.Application.Services.ImageService": "Information",
      "SGA.Api.Controllers.DocentesController": "Information",
      "SGA.Api.Middleware.FileUploadMiddleware": "Warning"
    }
  }
}
```

---

## 16. Checklist de Implementación

### Backend

- [ ] Modificar tabla `Docentes` con columnas de foto
- [ ] Crear constantes en `FileLimits.cs`
- [ ] Implementar DTOs de respuesta
- [ ] Crear `ImageService` con validación y compresión
- [ ] Actualizar `DocenteService` con métodos de foto
- [ ] Crear middleware `FileUploadMiddleware`
- [ ] Actualizar controlador con endpoints
- [ ] Configurar `Program.cs` y DI
- [ ] Agregar migraciones de base de datos

### Frontend

- [ ] Crear componente `ProfilePhotoUpload`
- [ ] Agregar estilos CSS
- [ ] Implementar funciones JavaScript
- [ ] Integrar en páginas existentes
- [ ] Configurar manejo de errores
- [ ] Añadir validaciones en cliente

### Testing y Producción

- [ ] Escribir tests unitarios
- [ ] Configurar variables de entorno
- [ ] Configurar servidor web
- [ ] Implementar monitoreo
- [ ] Documentar cambios

---

## 17. Consideraciones de Seguridad

1. **Validación de archivos**: Doble validación (cliente y servidor)
2. **Límites de tamaño**: Configurados en múltiples niveles
3. **Tipos MIME**: Verificación estricta de formatos
4. **Compresión**: Reducir superficie de ataque
5. **Autenticación**: Verificar identidad del usuario
6. **Logs**: Registrar intentos de carga para auditoría

---

## 18. Mantenimiento

### Tareas regulares

- Monitorear tamaño de base de datos
- Revisar logs de errores de carga
- Actualizar límites según necesidades
- Optimizar consultas de imágenes
- Backup regular de datos

### Mejoras futuras

- Implementar CDN para imágenes
- Agregar thumbnails automáticos
- Implementar caché de imágenes
- Soporte para más formatos
- Compresión adaptativa por dispositivo

---

Este documento proporciona una guía completa para implementar el sistema de fotos de perfil desde cero, siguiendo las mejores prácticas de desarrollo y asegurando un sistema robusto, escalable y mantenible.
