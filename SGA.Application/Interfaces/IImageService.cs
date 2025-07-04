using SGA.Application.DTOs.Responses;
using Microsoft.AspNetCore.Http;

namespace SGA.Application.Interfaces
{
    public interface IImageService
    {
        ValidationResponse ValidateImage(IFormFile file);
        Task<byte[]> ProcessImageAsync(IFormFile file);
        string GetOptimizedMimeType(string originalMimeType);
    }
}
