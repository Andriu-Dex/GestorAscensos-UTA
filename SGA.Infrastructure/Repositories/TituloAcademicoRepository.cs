using Microsoft.EntityFrameworkCore;
using SGA.Application.Interfaces;
using SGA.Domain.Entities;
using SGA.Infrastructure.Data;

namespace SGA.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio para títulos académicos
/// </summary>
public class TituloAcademicoRepository : ITituloAcademicoRepository
{
    private readonly ApplicationDbContext _context;

    public TituloAcademicoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TituloAcademico?> GetByIdAsync(Guid id)
    {
        return await _context.TitulosAcademicos
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<TituloAcademico>> GetAllAsync()
    {
        return await _context.TitulosAcademicos
            .OrderBy(t => t.OrdenJerarquico)
            .ThenBy(t => t.Nombre)
            .ToListAsync();
    }

    public async Task<TituloAcademico> CreateAsync(TituloAcademico titulo)
    {
        _context.TitulosAcademicos.Add(titulo);
        await _context.SaveChangesAsync();
        return titulo;
    }

    public async Task<TituloAcademico> UpdateAsync(TituloAcademico titulo)
    {
        _context.TitulosAcademicos.Update(titulo);
        await _context.SaveChangesAsync();
        return titulo;
    }

    public async Task DeleteAsync(Guid id)
    {
        var titulo = await GetByIdAsync(id);
        if (titulo != null)
        {
            _context.TitulosAcademicos.Remove(titulo);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<TituloAcademico>> GetActivosOrdenadosAsync()
    {
        return await _context.TitulosAcademicos
            .Where(t => t.EstaActivo)
            .OrderBy(t => t.OrdenJerarquico)
            .ThenBy(t => t.Nombre)
            .ToListAsync();
    }

    public async Task<TituloAcademico?> GetByCodigoAsync(string codigo)
    {
        return await _context.TitulosAcademicos
            .FirstOrDefaultAsync(t => t.Codigo.ToLower() == codigo.ToLower());
    }

    public async Task<bool> ExisteCodigoAsync(string codigo, Guid? excluirId = null)
    {
        var query = _context.TitulosAcademicos
            .Where(t => t.Codigo.ToLower() == codigo.ToLower());

        if (excluirId.HasValue)
        {
            query = query.Where(t => t.Id != excluirId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<bool> ExisteNombreAsync(string nombre, Guid? excluirId = null)
    {
        var query = _context.TitulosAcademicos
            .Where(t => t.Nombre.ToLower() == nombre.ToLower());

        if (excluirId.HasValue)
        {
            query = query.Where(t => t.Id != excluirId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<List<TituloAcademico>> GetByRangoJerarquicoAsync(int ordenMinimo, int ordenMaximo)
    {
        return await _context.TitulosAcademicos
            .Where(t => t.EstaActivo && 
                       t.OrdenJerarquico >= ordenMinimo && 
                       t.OrdenJerarquico <= ordenMaximo)
            .OrderBy(t => t.OrdenJerarquico)
            .ToListAsync();
    }

    public async Task<List<TituloAcademico>> GetPosiblesAscensosAsync(Guid tituloActualId)
    {
        var tituloActual = await GetByIdAsync(tituloActualId);
        if (tituloActual == null)
            return new List<TituloAcademico>();

        return await _context.TitulosAcademicos
            .Where(t => t.EstaActivo && 
                       t.OrdenJerarquico > tituloActual.OrdenJerarquico)
            .OrderBy(t => t.OrdenJerarquico)
            .ToListAsync();
    }

    public async Task<List<TituloAcademico>> GetEquivalentesANivelAsync(int nivelEnum)
    {
        return await _context.TitulosAcademicos
            .Where(t => t.EstaActivo && t.NivelEquivalente == nivelEnum)
            .OrderBy(t => t.OrdenJerarquico)
            .ToListAsync();
    }

    public async Task<bool> EstaOrdenDisponibleAsync(int orden, Guid? excluirId = null)
    {
        var query = _context.TitulosAcademicos
            .Where(t => t.OrdenJerarquico == orden);

        if (excluirId.HasValue)
        {
            query = query.Where(t => t.Id != excluirId.Value);
        }

        return !await query.AnyAsync();
    }

    public async Task<int> GetSiguienteOrdenDisponibleAsync()
    {
        var maxOrden = await _context.TitulosAcademicos
            .MaxAsync(t => (int?)t.OrdenJerarquico) ?? 0;

        // Buscar el primer hueco disponible
        for (int i = 1; i <= maxOrden + 1; i++)
        {
            if (await EstaOrdenDisponibleAsync(i))
                return i;
        }

        return maxOrden + 1;
    }
}
