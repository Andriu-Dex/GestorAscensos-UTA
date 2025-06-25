using SGA.Application.DTOs.Docentes;
using SGA.Application.DTOs.ExternalData;

namespace SGA.Application.Interfaces;

public interface IExternalDataService
{
    Task<ImportarDatosResponse> ImportarDesdeTTHHAsync(string cedula);
    Task<ImportarDatosResponse> ImportarDesdeDADACAsync(string cedula);
    Task<ImportarDatosResponse> ImportarDesdeDITICAsync(string cedula);
    Task<ImportarDatosResponse> ImportarDesdeDIRINVAsync(string cedula);
    Task<DocenteTTHHDto?> ObtenerDatosDocenteTTHHAsync(string identificacion);
    Task<bool> ValidarDocenteEnTTHHAsync(string identificacion);
    Task<IEnumerable<DocenteTTHHDto>> ObtenerTodosDocentesTTHHAsync();
    Task SincronizarDatosDocentesAsync();
}
