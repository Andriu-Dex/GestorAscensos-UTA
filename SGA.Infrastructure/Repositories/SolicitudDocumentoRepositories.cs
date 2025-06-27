using Microsoft.EntityFrameworkCore;
using SGA.Application.Interfaces.Repositories;
using SGA.Domain.Entities;
using SGA.Domain.Enums;
using SGA.Infrastructure.Data;

namespace SGA.Infrastructure.Repositories;

public class SolicitudAscensoRepository : ISolicitudAscensoRepository
{
    private readonly ApplicationDbContext _context;

    public SolicitudAscensoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SolicitudAscenso?> GetByIdAsync(Guid id)
    {
        return await _context.SolicitudesAscenso
            .Include(s => s.Docente)
            .Include(s => s.AprobadoPor)
            .Include(s => s.Documentos)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<SolicitudAscenso>> GetByDocenteIdAsync(Guid docenteId)
    {
        return await _context.SolicitudesAscenso
            .Include(s => s.Docente)
            .Include(s => s.AprobadoPor)
            .Include(s => s.Documentos)
            .Where(s => s.DocenteId == docenteId)
            .OrderByDescending(s => s.FechaSolicitud)
            .ToListAsync();
    }

    public async Task<List<SolicitudAscenso>> GetAllAsync()
    {
        return await _context.SolicitudesAscenso
            .Include(s => s.Docente)
            .Include(s => s.AprobadoPor)
            .Include(s => s.Documentos)
            .OrderByDescending(s => s.FechaSolicitud)
            .ToListAsync();
    }

    public async Task<SolicitudAscenso> CreateAsync(SolicitudAscenso solicitud)
    {
        _context.SolicitudesAscenso.Add(solicitud);
        await _context.SaveChangesAsync();
        return solicitud;
    }

    public async Task<SolicitudAscenso> UpdateAsync(SolicitudAscenso solicitud)
    {
        solicitud.FechaModificacion = DateTime.UtcNow;
        _context.SolicitudesAscenso.Update(solicitud);
        await _context.SaveChangesAsync();
        return solicitud;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var solicitud = await _context.SolicitudesAscenso.FindAsync(id);
        if (solicitud == null) return false;

        _context.SolicitudesAscenso.Remove(solicitud);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> TieneDocumenteSolicitudActivaAsync(Guid docenteId)
    {
        return await _context.SolicitudesAscenso
            .AnyAsync(s => s.DocenteId == docenteId && 
                          (s.Estado == EstadoSolicitud.Pendiente || s.Estado == EstadoSolicitud.EnProceso));
    }
}

public class DocumentoRepository : IDocumentoRepository
{
    private readonly ApplicationDbContext _context;

    public DocumentoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Documento?> GetByIdAsync(Guid id)
    {
        return await _context.Documentos
            .Include(d => d.SolicitudAscenso)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<List<Documento>> GetBySolicitudIdAsync(Guid solicitudId)
    {
        return await _context.Documentos
            .Where(d => d.SolicitudAscensoId == solicitudId)
            .ToListAsync();
    }

    public async Task<List<Documento>> GetByDocenteIdAsync(Guid docenteId)
    {
        return await _context.Documentos
            .Include(d => d.SolicitudAscenso)
            .Where(d => d.SolicitudAscenso.DocenteId == docenteId)
            .OrderByDescending(d => d.FechaCreacion)
            .ToListAsync();
    }

    public async Task<Documento> CreateAsync(Documento documento)
    {
        _context.Documentos.Add(documento);
        await _context.SaveChangesAsync();
        return documento;
    }

    public async Task<Documento> UpdateAsync(Documento documento)
    {
        documento.FechaModificacion = DateTime.UtcNow;
        _context.Documentos.Update(documento);
        await _context.SaveChangesAsync();
        return documento;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var documento = await _context.Documentos.FindAsync(id);
        if (documento == null) return false;

        _context.Documentos.Remove(documento);
        await _context.SaveChangesAsync();
        return true;
    }
}

public class LogAuditoriaRepository : ILogAuditoriaRepository
{
    private readonly ApplicationDbContext _context;

    public LogAuditoriaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LogAuditoria> CreateAsync(LogAuditoria log)
    {
        _context.LogsAuditoria.Add(log);
        await _context.SaveChangesAsync();
        return log;
    }

    public async Task<List<LogAuditoria>> GetByFechaRangoAsync(DateTime? fechaInicio, DateTime? fechaFin)
    {
        var query = _context.LogsAuditoria.AsQueryable();

        if (fechaInicio.HasValue)
            query = query.Where(l => l.FechaAccion >= fechaInicio.Value);

        if (fechaFin.HasValue)
            query = query.Where(l => l.FechaAccion <= fechaFin.Value);

        return await query
            .OrderByDescending(l => l.FechaAccion)
            .ToListAsync();
    }
}
