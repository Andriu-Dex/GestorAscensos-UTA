using SGA.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SGA.BlazorClient.Services
{
    public class RequirementService : IRequirementService
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "api/requirements";

        public RequirementService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }        public async Task<IEnumerable<RequirementDto>?> GetAllRequirementsAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<RequirementDto>>(ApiUrl);
        }

        public async Task<RequirementDto?> GetRequirementByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<RequirementDto>($"{ApiUrl}/{id}");
        }
    }
}
