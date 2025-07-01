using SGA.Domain.Entities;

namespace SGA.Application.Interfaces.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByIdAsync(Guid id);
    Task<Usuario?> GetByEmailAsync(string email);
    Task<Usuario> CreateAsync(Usuario usuario);
    Task<Usuario> UpdateAsync(Usuario usuario);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}

public interface IDocenteRepository
{
    Task<Docente?> GetByIdAsync(Guid id);
    Task<Docente?> GetByCedulaAsync(string cedula);
    Task<Docente?> GetByUsuarioIdAsync(Guid usuarioId);
    Task<Docente> CreateAsync(Docente docente);
    Task<Docente> UpdateAsync(Docente docente);
    Task<bool> DeleteAsync(Guid id);
    Task<List<Docente>> GetAllAsync();
    // Métodos optimizados sin includes
    Task<Docente?> GetByCedulaSimpleAsync(string cedula);
    Task<Docente?> GetByIdSimpleAsync(Guid id);
}

public interface ISolicitudAscensoRepository
{
    Task<SolicitudAscenso?> GetByIdAsync(Guid id);
    Task<List<SolicitudAscenso>> GetByDocenteIdAsync(Guid docenteId);
    Task<List<SolicitudAscenso>> GetAllAsync();
    Task<SolicitudAscenso> CreateAsync(SolicitudAscenso solicitud);
    Task<SolicitudAscenso> UpdateAsync(SolicitudAscenso solicitud);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> TieneDocumenteSolicitudActivaAsync(Guid docenteId);
}

public interface IDocumentoRepository
{
    Task<Documento?> GetByIdAsync(Guid id);
    Task<List<Documento>> GetBySolicitudIdAsync(Guid solicitudId);
    Task<List<Documento>> GetByDocenteIdAsync(Guid docenteId);
    Task<Documento> CreateAsync(Documento documento);
    Task<Documento> UpdateAsync(Documento documento);
    Task<bool> DeleteAsync(Guid id);
}

public interface ILogAuditoriaRepository
{
    Task<LogAuditoria> CreateAsync(LogAuditoria log);
    Task<List<LogAuditoria>> GetByFechaRangoAsync(DateTime? fechaInicio, DateTime? fechaFin);
}
