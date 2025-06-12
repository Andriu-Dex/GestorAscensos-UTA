using SGA.Application.DTOs;
using SGA.Application.Models;
using SGA.Domain.Enums;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SGA.BlazorClient.Services
{
    public class PromotionRequestService : IPromotionRequestService
    {
        private readonly HttpClient _httpClient;
        private const string ApiEndpoint = "api/promotionrequests";

        public PromotionRequestService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }        public async Task<IEnumerable<PromotionRequestDto>?> GetAllRequestsAsync()
        {
            var requests = await _httpClient.GetFromJsonAsync<IEnumerable<PromotionRequestDto>>(ApiEndpoint);
            return requests ?? new List<PromotionRequestDto>();
        }

        public async Task<PromotionRequestDto?> GetRequestByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<PromotionRequestDto>($"{ApiEndpoint}/{id}");
        }

        public async Task<IEnumerable<PromotionRequestDto>?> GetRequestsByTeacherIdAsync(int teacherId)
        {
            var requests = await _httpClient.GetFromJsonAsync<IEnumerable<PromotionRequestDto>>($"{ApiEndpoint}/teacher/{teacherId}");
            return requests ?? new List<PromotionRequestDto>();
        }        public async Task<IEnumerable<PromotionRequestDto>?> GetRequestsByStatusAsync(PromotionRequestStatus status)
        {
            var requests = await _httpClient.GetFromJsonAsync<IEnumerable<PromotionRequestDto>>($"{ApiEndpoint}/status/{(int)status}");
            return requests ?? new List<PromotionRequestDto>();
        }public async Task<bool> ProcessRequestAsync(int requestId, PromotionRequestStatus newStatus, string comments)
        {
            var processRequest = new ProcessPromotionRequestDto
            {
                NewStatus = newStatus,
                Comments = comments
            };

            var response = await _httpClient.PutAsJsonAsync($"{ApiEndpoint}/{requestId}/process", processRequest);
            return response.IsSuccessStatusCode;
        }
        
        public async Task<int> CreateRequestAsync(int teacherId)
        {            var response = await _httpClient.PostAsync($"{ApiEndpoint}/teacher/{teacherId}", null);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<PromotionRequestResult>();
            return result?.PromotionRequestId ?? 0;
        }
        
        public async Task<int> CreateRequestWithDocumentAsync(int teacherId, int documentId)
        {
            var response = await _httpClient.PostAsync($"{ApiEndpoint}/teacher/{teacherId}/withdocument/{documentId}", null);
            response.EnsureSuccessStatusCode();
              var result = await response.Content.ReadFromJsonAsync<PromotionRequestResult>();
            return result?.PromotionRequestId ?? 0;
        }
    }
}
