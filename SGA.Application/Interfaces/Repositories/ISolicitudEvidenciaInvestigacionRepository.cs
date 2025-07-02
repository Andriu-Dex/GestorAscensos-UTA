using SGA.Domain.Entities;
using SGA.Application.Interfaces.Repositories;

namespace SGA.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface para gestión de evidencias de investigación
/// Sigue el patrón de ISolicitudCertificadoCapacitacionRepository
/// </summary>
public interface ISolicitudEvidenciaInvestigacionRepository : IRepository<SolicitudEvidenciaInvestigacion>
{
    Task<List<SolicitudEvidenciaInvestigacion>> GetByDocenteCedulaAsync(string cedula);
    Task<List<SolicitudEvidenciaInvestigacion>> GetPendientesAsync();
    Task<List<SolicitudEvidenciaInvestigacion>> GetTodasAsync();
    Task<SolicitudEvidenciaInvestigacion?> GetByIdWithDocenteAsync(Guid id);
}
