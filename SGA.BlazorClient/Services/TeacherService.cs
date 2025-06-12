using SGA.Application.DTOs;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SGA.BlazorClient.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly HttpClient _httpClient;
        private const string ApiEndpoint = "api/teachers";

        public TeacherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }        public async Task<IEnumerable<TeacherDto>?> GetAllTeachersAsync()
        {
            var teachers = await _httpClient.GetFromJsonAsync<IEnumerable<TeacherDto>>(ApiEndpoint);
            return teachers ?? new List<TeacherDto>();
        }

        public async Task<TeacherDto?> GetTeacherByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<TeacherDto>($"{ApiEndpoint}/{id}");
        }

        public async Task<PromotionEligibilityResultDto?> CheckEligibilityAsync(int teacherId)
        {
            return await _httpClient.GetFromJsonAsync<PromotionEligibilityResultDto>($"{ApiEndpoint}/{teacherId}/eligibility");
        }

        public async Task<PromotionRequestDto?> CreatePromotionRequestAsync(int teacherId)
        {
            var response = await _httpClient.PostAsync($"{ApiEndpoint}/{teacherId}/promotion-requests", null);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PromotionRequestDto>();
        }
    }
}
