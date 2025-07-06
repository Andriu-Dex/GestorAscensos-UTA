using Microsoft.EntityFrameworkCore;
using SGA.Application.Interfaces;
using SGA.Domain.Entities;
using SGA.Domain.Enums;
using SGA.Domain.Extensions;
using SGA.Infrastructure.Data;

namespace SGA.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio para configuración de requisitos
/// </summary>
public class ConfiguracionRequisitoRepository : IConfiguracionRequisitoRepository
{
    private readonly ApplicationDbContext _context;

    public ConfiguracionRequisitoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ConfiguracionRequisito>> GetAllAsync()
    {
        return await _context.ConfiguracionesRequisitos
            .Include(c => c.TituloActual)
            .Include(c => c.TituloSolicitado)
            .OrderBy(c => c.NivelActual)
            .ThenBy(c => c.NivelSolicitado)
            .ToListAsync();
    }

    public async Task<List<ConfiguracionRequisito>> GetActivasAsync()
    {
        return await _context.ConfiguracionesRequisitos
            .Include(c => c.TituloActual)
            .Include(c => c.TituloSolicitado)
            .Where(c => c.EstaActivo)
            .OrderBy(c => c.NivelActual)
            .ThenBy(c => c.NivelSolicitado)
            .ToListAsync();
    }

    public async Task<ConfiguracionRequisito?> GetByIdAsync(Guid id)
    {
        return await _context.ConfiguracionesRequisitos
            .Include(c => c.TituloActual)
            .Include(c => c.TituloSolicitado)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<ConfiguracionRequisito?> GetByNivelesAsync(NivelTitular nivelActual, NivelTitular nivelSolicitado)
    {
        return await _context.ConfiguracionesRequisitos
            .Include(c => c.TituloActual)
            .Include(c => c.TituloSolicitado)
            .Where(c => c.NivelActual == nivelActual && c.NivelSolicitado == nivelSolicitado && c.EstaActivo)
            .FirstOrDefaultAsync();
    }

    public async Task<List<ConfiguracionRequisito>> GetByNivelActualAsync(NivelTitular nivelActual)
    {
        return await _context.ConfiguracionesRequisitos
            .Include(c => c.TituloActual)
            .Include(c => c.TituloSolicitado)
            .Where(c => c.NivelActual == nivelActual && c.EstaActivo)
            .OrderBy(c => c.NivelSolicitado)
            .ToListAsync();
    }

    public async Task<ConfiguracionRequisito> CreateAsync(ConfiguracionRequisito configuracion)
    {
        configuracion.FechaCreacion = DateTime.UtcNow;
        
        _context.ConfiguracionesRequisitos.Add(configuracion);
        await _context.SaveChangesAsync();
        
        return configuracion;
    }

    public async Task<ConfiguracionRequisito> UpdateAsync(ConfiguracionRequisito configuracion)
    {
        configuracion.FechaModificacion = DateTime.UtcNow;
        
        _context.ConfiguracionesRequisitos.Update(configuracion);
        await _context.SaveChangesAsync();
        
        return configuracion;
    }

    public async Task DeleteAsync(Guid id)
    {
        var configuracion = await GetByIdAsync(id);
        if (configuracion != null)
        {
            _context.ConfiguracionesRequisitos.Remove(configuracion);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ToggleActivoAsync(Guid id)
    {
        var configuracion = await GetByIdAsync(id);
        if (configuracion == null)
            return false;

        configuracion.EstaActivo = !configuracion.EstaActivo;
        configuracion.FechaModificacion = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        return configuracion.EstaActivo;
    }

    public async Task<bool> ExisteConfiguracionAsync(NivelTitular nivelActual, NivelTitular nivelSolicitado, Guid? excludeId = null)
    {
        var query = _context.ConfiguracionesRequisitos
            .Where(c => c.NivelActual == nivelActual && c.NivelSolicitado == nivelSolicitado);

        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<List<ConfiguracionRequisito>> GetOrderedByNivelAsync()
    {
        return await _context.ConfiguracionesRequisitos
            .Include(c => c.TituloActual)
            .Include(c => c.TituloSolicitado)
            .OrderBy(c => c.NivelActual.HasValue ? (int)c.NivelActual.Value : 999)
            .ThenBy(c => c.NivelSolicitado.HasValue ? (int)c.NivelSolicitado.Value : 999)
            .ThenBy(c => c.TituloActual != null ? c.TituloActual.OrdenJerarquico : 999)
            .ThenBy(c => c.TituloSolicitado != null ? c.TituloSolicitado.OrdenJerarquico : 999)
            .ToListAsync();
    }

    public async Task<Dictionary<string, bool>> ValidarCoberturaNivelesAsync()
    {
        var configuracionesActivas = await GetActivasAsync();
        var cobertura = new Dictionary<string, bool>();

        // Definir todas las transiciones posibles
        var transicionesPosibles = new[]
        {
            (NivelTitular.Titular1, NivelTitular.Titular2),
            (NivelTitular.Titular2, NivelTitular.Titular3),
            (NivelTitular.Titular3, NivelTitular.Titular4),
            (NivelTitular.Titular4, NivelTitular.Titular5)
        };

        foreach (var (nivelActual, nivelSolicitado) in transicionesPosibles)
        {
            var nombreTransicion = $"{nivelActual.GetDescription()} → {nivelSolicitado.GetDescription()}";
            var tieneConfiguracion = configuracionesActivas.Any(c => 
                c.NivelActual == nivelActual && c.NivelSolicitado == nivelSolicitado);
            
            cobertura[nombreTransicion] = tieneConfiguracion;
        }

        return cobertura;
    }
}
