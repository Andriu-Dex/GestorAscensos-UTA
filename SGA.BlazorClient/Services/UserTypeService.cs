using SGA.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SGA.BlazorClient.Services
{
    public class UserTypeService : IUserTypeService
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "api/usertypes";

        public UserTypeService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<IEnumerable<UserTypeDto>> GetAllUserTypesAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<UserTypeDto>>(ApiUrl);
        }

        public async Task<UserTypeDto> GetUserTypeByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<UserTypeDto>($"{ApiUrl}/{id}");
        }
    }
}
