using SGA.Application.DTOs;
using SGA.Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGA.BlazorClient.Services
{
    public interface IPromotionRequestService
    {
        Task<IEnumerable<PromotionRequestDto>> GetAllRequestsAsync();
        Task<PromotionRequestDto> GetRequestByIdAsync(int id);
        Task<IEnumerable<PromotionRequestDto>> GetRequestsByTeacherIdAsync(int teacherId);
        Task<IEnumerable<PromotionRequestDto>> GetRequestsByStatusAsync(PromotionRequestStatus status);
        Task<bool> ProcessRequestAsync(int requestId, PromotionRequestStatus newStatus, string comments);
        Task<int> CreateRequestAsync(int teacherId);
        Task<int> CreateRequestWithDocumentAsync(int teacherId, int documentId);
    }
}
