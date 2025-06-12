using SGA.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGA.BlazorClient.Services
{
    public interface IDocumentService
    {
        Task<IEnumerable<DocumentDto>?> GetAllDocumentsAsync();
        Task<DocumentDto?> GetDocumentByIdAsync(int id);
        Task<IEnumerable<DocumentDto>?> GetDocumentsByTeacherIdAsync(int teacherId);
        Task<IEnumerable<DocumentDto>?> GetDocumentsByRequirementIdAsync(int requirementId);
        Task<IEnumerable<DocumentDto>?> GetDocumentsByTypeAsync(string documentType);
        Task<int> UploadDocumentAsync(UploadDocumentDto document);
        Task<int> CreateDocumentAsync(DocumentDto document);
        Task AddObservationAsync(int documentId, AddDocumentObservationDto observation);
        Task<byte[]> DownloadDocumentAsync(int id);
    }
}
