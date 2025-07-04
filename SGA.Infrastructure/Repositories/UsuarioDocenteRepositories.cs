using Microsoft.EntityFrameworkCore;
using SGA.Application.Interfaces.Repositories;
using SGA.Domain.Entities;
using SGA.Infrastructure.Data;

namespace SGA.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly ApplicationDbContext _context;

    public UsuarioRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> GetByIdAsync(Guid id)
    {
        return await _context.Usuarios
            .Include(u => u.Docente)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        return await _context.Usuarios
            .Include(u => u.Docente)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<Usuario> CreateAsync(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        return usuario;
    }

    public async Task<Usuario> UpdateAsync(Usuario usuario)
    {
        usuario.FechaModificacion = DateTime.UtcNow;
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();
        return usuario;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null) return false;

        _context.Usuarios.Remove(usuario);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Usuarios.AnyAsync(u => u.Id == id);
    }

    public async Task<List<Usuario>> GetAdministradoresAsync()
    {
        return await _context.Usuarios
            .Where(u => u.Rol == Domain.Enums.RolUsuario.Administrador && u.EstaActivo)
            .ToListAsync();
    }
}

public class DocenteRepository : IDocenteRepository
{
    private readonly ApplicationDbContext _context;

    public DocenteRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Docente?> GetByIdAsync(Guid id)
    {
        return await _context.Docentes
            .Include(d => d.Usuario)
            .Include(d => d.SolicitudesAscenso)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<Docente?> GetByCedulaAsync(string cedula)
    {
        return await _context.Docentes
            .Include(d => d.Usuario)
            .Include(d => d.SolicitudesAscenso)
            .FirstOrDefaultAsync(d => d.Cedula == cedula);
    }

    public async Task<Docente?> GetByUsuarioIdAsync(Guid usuarioId)
    {
        return await _context.Docentes
            .Include(d => d.Usuario)
            .Include(d => d.SolicitudesAscenso)
            .FirstOrDefaultAsync(d => d.UsuarioId == usuarioId);
    }

    public async Task<Docente> CreateAsync(Docente docente)
    {
        _context.Docentes.Add(docente);
        await _context.SaveChangesAsync();
        return docente;
    }

    public async Task<Docente> UpdateAsync(Docente docente)
    {
        docente.FechaModificacion = DateTime.UtcNow;
        _context.Docentes.Update(docente);
        await _context.SaveChangesAsync();
        return docente;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var docente = await _context.Docentes.FindAsync(id);
        if (docente == null) return false;

        _context.Docentes.Remove(docente);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Docente>> GetAllAsync()
    {
        return await _context.Docentes
            .Include(d => d.Usuario)
            .Include(d => d.SolicitudesAscenso)
            .ToListAsync();
    }

    // Método optimizado para consultas simples sin includes
    public async Task<Docente?> GetByCedulaSimpleAsync(string cedula)
    {
        return await _context.Docentes
            .FirstOrDefaultAsync(d => d.Cedula == cedula);
    }

    // Método optimizado para consultas simples sin includes
    public async Task<Docente?> GetByIdSimpleAsync(Guid id)
    {
        return await _context.Docentes
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<Docente?> GetByEmailAsync(string email)
    {
        return await _context.Docentes
            .Include(d => d.Usuario)
            .Include(d => d.SolicitudesAscenso)
            .FirstOrDefaultAsync(d => d.Email == email);
    }
}
