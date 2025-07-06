using System.Net.Http.Json;
using SGA.Web.Models.Admin;

namespace SGA.Web.Services;

/// <summary>
/// Servicio para interactuar con la API de títulos académicos desde el frontend
/// </summary>
public class TitulosAcademicosService
{
    private readonly HttpClient _httpClient;

    public TitulosAcademicosService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Obtiene todos los títulos académicos activos para selectores
    /// </summary>
    public async Task<List<TituloAcademicoOpcionDto>> GetOpcionesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/TitulosAcademicos/opciones");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<TituloAcademicoOpcionDto>>() ?? new();
            }
            return new List<TituloAcademicoOpcionDto>();
        }
        catch
        {
            return new List<TituloAcademicoOpcionDto>();
        }
    }

    /// <summary>
    /// Obtiene títulos académicos posibles según un título actual para ascensos
    /// </summary>
    public async Task<List<TituloAcademicoOpcionDto>> GetPosiblesAscensosAsync(Guid tituloActualId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/TitulosAcademicos/{tituloActualId}/posibles-ascensos");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<TituloAcademicoOpcionDto>>() ?? new();
            }
            return new List<TituloAcademicoOpcionDto>();
        }
        catch
        {
            return new List<TituloAcademicoOpcionDto>();
        }
    }

    /// <summary>
    /// Verifica si un título puede ser eliminado
    /// </summary>
    public async Task<bool> PuedeSerEliminadoAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/TitulosAcademicos/{id}/puede-ser-eliminado");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<bool>();
            }
            return false;
        }
        catch
        {
            return false;
        }
    }
}
