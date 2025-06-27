using SGA.Application.DTOs.ExternalData;

namespace SGA.Application.Interfaces;

public interface IDIRINVDataService
{
    Task<List<ObraAcademicaDto>> GetObrasDocenteAsync(string cedula);
    Task AddObraAcademicaAsync(string cedula, ObraAcademicaDto obra);
}
