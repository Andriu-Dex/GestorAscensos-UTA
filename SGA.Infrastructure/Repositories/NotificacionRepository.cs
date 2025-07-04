using Microsoft.EntityFrameworkCore;
using SGA.Application.Interfaces;
using SGA.Domain.Entities;
using SGA.Infrastructure.Data;

namespace SGA.Infrastructure.Repositories;

/// <summary>
/// Repositorio para gestionar notificaciones en tiempo real
/// </summary>
public class NotificacionRepository : INotificacionRepository
{
    private readonly ApplicationDbContext _context;

    public NotificacionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Notificacion> CreateAsync(Notificacion notificacion)
    {
        _context.Notificaciones.Add(notificacion);
        await _context.SaveChangesAsync();
        return notificacion;
    }

    public async Task<List<Notificacion>> GetByUsuarioIdAsync(Guid usuarioId, int limit = 20)
    {
        return await _context.Notificaciones
            .Where(n => n.UsuarioId == usuarioId)
            .OrderByDescending(n => n.FechaCreacion)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<List<Notificacion>> GetNoLeidasByUsuarioIdAsync(Guid usuarioId)
    {
        return await _context.Notificaciones
            .Where(n => n.UsuarioId == usuarioId && !n.Leida)
            .OrderByDescending(n => n.FechaCreacion)
            .ToListAsync();
    }

    public async Task<int> GetCountNoLeidasByUsuarioIdAsync(Guid usuarioId)
    {
        return await _context.Notificaciones
            .CountAsync(n => n.UsuarioId == usuarioId && !n.Leida);
    }

    public async Task<Notificacion?> GetByIdAsync(Guid id)
    {
        return await _context.Notificaciones
            .FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task UpdateAsync(Notificacion notificacion)
    {
        _context.Notificaciones.Update(notificacion);
        await _context.SaveChangesAsync();
    }

    public async Task MarcarComoLeidaAsync(Guid notificacionId)
    {
        var notificacion = await GetByIdAsync(notificacionId);
        if (notificacion != null)
        {
            notificacion.Leida = true;
            notificacion.FechaLeida = DateTime.UtcNow;
            await UpdateAsync(notificacion);
        }
    }

    public async Task MarcarTodasComoLeidasAsync(Guid usuarioId)
    {
        var notificaciones = await _context.Notificaciones
            .Where(n => n.UsuarioId == usuarioId && !n.Leida)
            .ToListAsync();

        foreach (var notificacion in notificaciones)
        {
            notificacion.Leida = true;
            notificacion.FechaLeida = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var notificacion = await GetByIdAsync(id);
        if (notificacion != null)
        {
            _context.Notificaciones.Remove(notificacion);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteOlderThanAsync(DateTime fecha)
    {
        var notificacionesAntiguas = await _context.Notificaciones
            .Where(n => n.FechaCreacion < fecha)
            .ToListAsync();

        _context.Notificaciones.RemoveRange(notificacionesAntiguas);
        await _context.SaveChangesAsync();
    }
}
