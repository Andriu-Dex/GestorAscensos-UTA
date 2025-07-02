using Microsoft.EntityFrameworkCore;
using SGA.Application.Interfaces.Repositories;
using SGA.Domain.Entities;
using SGA.Infrastructure.Data;

namespace SGA.Infrastructure.Repositories;

/// <summary>
/// Implementación del repository para evidencias de investigación
/// Sigue el patrón establecido en el sistema
/// </summary>
public class SolicitudEvidenciaInvestigacionRepository : Repository<SolicitudEvidenciaInvestigacion>, ISolicitudEvidenciaInvestigacionRepository
{
    public SolicitudEvidenciaInvestigacionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<SolicitudEvidenciaInvestigacion>> GetByDocenteCedulaAsync(string cedula)
    {
        return await _context.SolicitudesEvidenciasInvestigacion
            .Include(e => e.Docente)
            .Where(e => e.DocenteCedula == cedula)
            .OrderByDescending(e => e.FechaCreacion)
            .ToListAsync();
    }

    public async Task<List<SolicitudEvidenciaInvestigacion>> GetPendientesAsync()
    {
        return await _context.SolicitudesEvidenciasInvestigacion
            .Include(e => e.Docente)
            .Where(e => e.Estado == "Pendiente")
            .OrderBy(e => e.FechaCreacion)
            .ToListAsync();
    }

    public async Task<List<SolicitudEvidenciaInvestigacion>> GetTodasAsync()
    {
        return await _context.SolicitudesEvidenciasInvestigacion
            .Include(e => e.Docente)
            .OrderByDescending(e => e.FechaCreacion)
            .ToListAsync();
    }

    public async Task<SolicitudEvidenciaInvestigacion?> GetByIdWithDocenteAsync(Guid id)
    {
        return await _context.SolicitudesEvidenciasInvestigacion
            .Include(e => e.Docente)
            .FirstOrDefaultAsync(e => e.Id == id);
    }
}
