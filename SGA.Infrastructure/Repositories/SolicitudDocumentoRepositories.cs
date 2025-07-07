using Microsoft.EntityFrameworkCore;
using SGA.Application.DTOs.DocumentoImportacion;
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
        var solicitud = await _context.SolicitudesAscenso
            .Include(s => s.Documentos)
            .FirstOrDefaultAsync(s => s.Id == id);
            
        if (solicitud == null) return false;

        // Eliminar primero los documentos asociados
        if (solicitud.Documentos.Any())
        {
            _context.Documentos.RemoveRange(solicitud.Documentos);
        }

        // Luego eliminar la solicitud
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
            .Where(d => d.SolicitudAscenso != null && d.SolicitudAscenso.DocenteId == docenteId)
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

    /// <summary>
    /// Obtener documentos disponibles para importación
    /// </summary>
    public async Task<List<Documento>> GetDocumentosImportablesAsync(Guid docenteId, FiltrosImportacionDto filtros)
    {
        var query = _context.Documentos
            .Include(d => d.SolicitudAscenso)
            .Include(d => d.Docente)
            .Where(d => d.DocenteId == docenteId || 
                       (d.SolicitudAscenso != null && d.SolicitudAscenso.DocenteId == docenteId));

        // Log para debug
        Console.WriteLine($"[DEBUG] Filtros recibidos: TipoDocumento='{filtros.TipoDocumento}', FechaDesde={filtros.FechaDesde}, FechaHasta={filtros.FechaHasta}, TextoBusqueda='{filtros.TextoBusqueda}', SoloImportables={filtros.SoloImportables}");

        // Aplicar filtros
        if (!string.IsNullOrEmpty(filtros.TipoDocumento))
        {
            if (Enum.TryParse<TipoDocumento>(filtros.TipoDocumento, out var tipoEnum))
            {
                Console.WriteLine($"[DEBUG] Aplicando filtro de tipo: {tipoEnum}");
                query = query.Where(d => d.TipoDocumento == tipoEnum);
            }
            else
            {
                Console.WriteLine($"[DEBUG] No se pudo parsear el tipo de documento: {filtros.TipoDocumento}");
            }
        }

        if (filtros.FechaDesde.HasValue)
        {
            query = query.Where(d => d.FechaCreacion >= filtros.FechaDesde.Value);
        }

        if (filtros.FechaHasta.HasValue)
        {
            query = query.Where(d => d.FechaCreacion <= filtros.FechaHasta.Value);
        }

        if (!string.IsNullOrWhiteSpace(filtros.TextoBusqueda))
        {
            var textoBusqueda = filtros.TextoBusqueda.ToLower();
            query = query.Where(d => d.NombreArchivo.ToLower().Contains(textoBusqueda));
        }

        if (filtros.SoloImportables)
        {
            query = query.Where(d => !d.FueUtilizadoEnSolicitudAprobada);
        }

        var resultados = await query
            .OrderByDescending(d => d.FechaCreacion)
            .ToListAsync();

        Console.WriteLine($"[DEBUG] Documentos encontrados después de filtros: {resultados.Count}");
        
        return resultados;
    }

    /// <summary>
    /// Obtener documentos no utilizados en solicitudes aprobadas
    /// </summary>
    public async Task<List<Documento>> GetDocumentosNoUtilizadosAsync(Guid docenteId)
    {
        return await _context.Documentos
            .Include(d => d.SolicitudAscenso)
            .Where(d => (d.DocenteId == docenteId || 
                        (d.SolicitudAscenso != null && d.SolicitudAscenso.DocenteId == docenteId)) &&
                       !d.FueUtilizadoEnSolicitudAprobada)
            .OrderByDescending(d => d.FechaCreacion)
            .ToListAsync();
    }

    /// <summary>
    /// Clonar un documento existente
    /// </summary>
    public async Task<Documento> ClonarDocumentoAsync(Documento documentoOriginal, Guid? nuevaSolicitudId = null)
    {
        var documentoClonado = new Documento
        {
            Id = Guid.NewGuid(),
            NombreArchivo = documentoOriginal.NombreArchivo,
            RutaArchivo = documentoOriginal.RutaArchivo,
            TamanoArchivo = documentoOriginal.TamanoArchivo,
            TipoDocumento = documentoOriginal.TipoDocumento,
            ContenidoArchivo = documentoOriginal.ContenidoArchivo,
            ContentType = documentoOriginal.ContentType,
            SolicitudAscensoId = nuevaSolicitudId,
            DocenteId = documentoOriginal.DocenteId,
            FueUtilizadoEnSolicitudAprobada = false,
            SolicitudAprobadaId = null,
            FechaUtilizacion = null,
            FechaCreacion = DateTime.UtcNow,
            FechaModificacion = DateTime.UtcNow
        };

        _context.Documentos.Add(documentoClonado);
        await _context.SaveChangesAsync();
        return documentoClonado;
    }

    /// <summary>
    /// Verificar si un documento está disponible para importación
    /// </summary>
    public async Task<bool> EsDocumentoDisponibleParaImportacionAsync(Guid documentoId, Guid docenteId)
    {
        var documento = await _context.Documentos
            .Include(d => d.SolicitudAscenso)
            .FirstOrDefaultAsync(d => d.Id == documentoId);

        if (documento == null) return false;

        // Verificar que el documento pertenece al docente
        var perteneceAlDocente = documento.DocenteId == docenteId || 
                                (documento.SolicitudAscenso != null && documento.SolicitudAscenso.DocenteId == docenteId);

        if (!perteneceAlDocente) return false;

        // Verificar que no haya sido utilizado en solicitud aprobada
        return !documento.FueUtilizadoEnSolicitudAprobada;
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
