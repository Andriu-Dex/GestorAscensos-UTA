using Microsoft.EntityFrameworkCore;
using SGA.Application.Interfaces.Repositories;
using SGA.Domain.Entities;
using SGA.Infrastructure.Data;

namespace SGA.Infrastructure.Repositories;

public class ObraAcademicaRepository : IObraAcademicaRepository
{
    private readonly ApplicationDbContext _context;

    public ObraAcademicaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ObraAcademica?> GetByIdAsync(Guid id)
    {
        return await _context.ObrasAcademicas
            .Include(o => o.Docente)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IEnumerable<ObraAcademica>> GetAllAsync()
    {
        return await _context.ObrasAcademicas
            .Include(o => o.Docente)
            .OrderByDescending(o => o.FechaPublicacion)
            .ToListAsync();
    }

    public async Task<ObraAcademica> CreateAsync(ObraAcademica obra)
    {
        obra.Id = Guid.NewGuid();
        obra.FechaCreacion = DateTime.UtcNow;
        obra.FechaModificacion = DateTime.UtcNow;
        
        _context.ObrasAcademicas.Add(obra);
        await _context.SaveChangesAsync();
        return obra;
    }

    public async Task<ObraAcademica> UpdateAsync(ObraAcademica obra)
    {
        obra.FechaModificacion = DateTime.UtcNow;
        _context.ObrasAcademicas.Update(obra);
        await _context.SaveChangesAsync();
        return obra;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var obra = await _context.ObrasAcademicas.FindAsync(id);
        if (obra == null)
            return false;

        _context.ObrasAcademicas.Remove(obra);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<ObraAcademica>> GetByDocenteIdAsync(Guid docenteId)
    {
        return await _context.ObrasAcademicas
            .Where(o => o.DocenteId == docenteId)
            .OrderByDescending(o => o.FechaPublicacion)
            .ToListAsync();
    }

    public async Task<IEnumerable<ObraAcademica>> GetByDocenteCedulaAsync(string cedula)
    {
        return await _context.ObrasAcademicas
            .Include(o => o.Docente)
            .Where(o => o.Docente.Cedula == cedula)
            .OrderByDescending(o => o.FechaPublicacion)
            .ToListAsync();
    }

    public async Task<int> CountByDocenteIdAsync(Guid docenteId)
    {
        return await _context.ObrasAcademicas
            .CountAsync(o => o.DocenteId == docenteId && o.EsVerificada);
    }

    public async Task<ObraAcademica?> GetByTituloYDocenteAsync(string titulo, Guid docenteId)
    {
        return await _context.ObrasAcademicas
            .FirstOrDefaultAsync(o => o.Titulo == titulo && o.DocenteId == docenteId);
    }

    public async Task<bool> ExisteObraAsync(string titulo, Guid docenteId, DateTime fechaPublicacion)
    {
        return await _context.ObrasAcademicas
            .AnyAsync(o => o.Titulo == titulo && 
                          o.DocenteId == docenteId && 
                          o.FechaPublicacion.Date == fechaPublicacion.Date);
    }

    public async Task<IEnumerable<ObraAcademica>> GetObrasRecientesAsync(Guid docenteId, DateTime fechaDesde)
    {
        return await _context.ObrasAcademicas
            .Where(o => o.DocenteId == docenteId && o.FechaPublicacion >= fechaDesde)
            .OrderByDescending(o => o.FechaPublicacion)
            .ToListAsync();
    }
}
