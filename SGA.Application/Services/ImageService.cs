using SGA.Application.Constants;
using SGA.Application.DTOs.Responses;
using SGA.Application.Interfaces;
using Microsoft.AspNetCore.Http;

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
                    $"Formato no permitido. Use: {string.Join(", ", FileLimits.ProfileImages.AllowedExtensions)}");
            }

            // Validar tipo MIME
            if (!FileLimits.ProfileImages.AllowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
            {
                return ValidationResponse.Invalid(
                    $"Tipo de archivo no permitido. Use: {string.Join(", ", FileLimits.ProfileImages.AllowedMimeTypes)}");
            }

            return ValidationResponse.Valid("Archivo válido");
        }

        public async Task<byte[]> ProcessImageAsync(IFormFile file)
        {
            // Por ahora, simplemente retorna la imagen tal como viene
            // En una implementación más avanzada, aquí se redimensionaría y optimizaría
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            return stream.ToArray();
        }

        public string GetOptimizedMimeType(string originalMimeType)
        {
            // Mantener el tipo MIME original por simplicidad
            return originalMimeType;
        }
    }
}
