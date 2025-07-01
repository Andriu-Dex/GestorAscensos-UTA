using SGA.Domain.Entities;

namespace SGA.Application.Interfaces.Repositories;

public interface IObraAcademicaRepository
{
    Task<ObraAcademica?> GetByIdAsync(Guid id);
    Task<IEnumerable<ObraAcademica>> GetAllAsync();
    Task<ObraAcademica> CreateAsync(ObraAcademica obra);
    Task<ObraAcademica> UpdateAsync(ObraAcademica obra);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<ObraAcademica>> GetByDocenteIdAsync(Guid docenteId);
    Task<IEnumerable<ObraAcademica>> GetByDocenteCedulaAsync(string cedula);
    Task<int> CountByDocenteIdAsync(Guid docenteId);
    Task<ObraAcademica?> GetByTituloYDocenteAsync(string titulo, Guid docenteId);
    Task<bool> ExisteObraAsync(string titulo, Guid docenteId, DateTime fechaPublicacion);
    Task<IEnumerable<ObraAcademica>> GetObrasRecientesAsync(Guid docenteId, DateTime fechaDesde);
}
