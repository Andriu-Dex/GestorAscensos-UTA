using SGA.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SGA.BlazorClient.Services
{
    public class AcademicDegreeService : IAcademicDegreeService
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "api/academicdegrees";

        public AcademicDegreeService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }        public async Task<IEnumerable<AcademicDegreeDto>?> GetAllAcademicDegreesAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<AcademicDegreeDto>>(ApiUrl);
        }

        public async Task<AcademicDegreeDto?> GetAcademicDegreeByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<AcademicDegreeDto>($"{ApiUrl}/{id}");
        }

        public async Task<IEnumerable<AcademicDegreeDto>?> GetAcademicDegreesByTeacherIdAsync(int teacherId)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<AcademicDegreeDto>>($"{ApiUrl}/teacher/{teacherId}");
        }
    }
}
