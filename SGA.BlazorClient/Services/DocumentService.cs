using SGA.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SGA.BlazorClient.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "api/documents";

        public DocumentService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }        public async Task<IEnumerable<DocumentDto>?> GetAllDocumentsAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<DocumentDto>>(ApiUrl);
        }

        public async Task<DocumentDto?> GetDocumentByIdAsync(int id)
        {            return await _httpClient.GetFromJsonAsync<DocumentDto>($"{ApiUrl}/{id}");
        }

        public async Task<IEnumerable<DocumentDto>?> GetDocumentsByTeacherIdAsync(int teacherId)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<DocumentDto>>($"{ApiUrl}/teacher/{teacherId}");
        }

        public async Task<IEnumerable<DocumentDto>?> GetDocumentsByRequirementIdAsync(int requirementId)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<DocumentDto>>($"{ApiUrl}/requirement/{requirementId}");
        }

        public async Task<IEnumerable<DocumentDto>?> GetDocumentsByTypeAsync(string documentType)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<DocumentDto>>($"{ApiUrl}/type/{documentType}");
        }

        public async Task<int> UploadDocumentAsync(UploadDocumentDto document)
        {
            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(document.Name), "Name");
            formData.Add(new StringContent(document.Description), "Description");
            formData.Add(new StringContent(document.DocumentType), "DocumentType");
            formData.Add(new StringContent(document.StartDate.ToString("yyyy-MM-dd")), "StartDate");
            formData.Add(new StringContent(document.EndDate.ToString("yyyy-MM-dd")), "EndDate");
            formData.Add(new StringContent(document.Department), "Department");
            formData.Add(new StringContent(document.IssuingInstitution), "IssuingInstitution");
            
            if (document.DurationHours.HasValue)
                formData.Add(new StringContent(document.DurationHours.Value.ToString()), "DurationHours");
                
            formData.Add(new StringContent(document.TeacherId.ToString()), "TeacherId");
            
            if (document.RequirementId.HasValue)
                formData.Add(new StringContent(document.RequirementId.Value.ToString()), "RequirementId");
                
            // Agregar el archivo
            var fileContent = new StreamContent(document.File.OpenReadStream());
            formData.Add(fileContent, "File", document.File.FileName);
              var response = await _httpClient.PostAsync(ApiUrl, formData);
            response.EnsureSuccessStatusCode();
            
            var createdDocument = await response.Content.ReadFromJsonAsync<DocumentDto>();
            return createdDocument?.Id ?? 0;
        }

        public async Task<int> CreateDocumentAsync(DocumentDto document)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiUrl, document);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<DocumentDto>();
            return result?.Id ?? 0;
        }        public async Task AddObservationAsync(int documentId, AddDocumentObservationDto observation)
        {
            // Asegurar que el DocumentId esté establecido
            observation.DocumentId = documentId;
            var response = await _httpClient.PostAsJsonAsync($"{ApiUrl}/{documentId}/observation", observation);
            response.EnsureSuccessStatusCode();
        }

        public async Task<byte[]> DownloadDocumentAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{ApiUrl}/{id}/download");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}
