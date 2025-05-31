using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities;
using SGA.Infrastructure.Data;

namespace SGA.Infrastructure.Repositories
{
    public interface IDocenteRepository
    {
        Task<Docente> GetByIdAsync(int id);
        Task<Docente> GetByCedulaAsync(string cedula);
        Task<Docente> GetByUsernameAsync(string username);
        Task<IEnumerable<Docente>> GetAllAsync();
        Task AddAsync(Docente docente);
        Task UpdateAsync(Docente docente);
        Task<bool> ExistsAsync(int id);
    }

    public class DocenteRepository : IDocenteRepository
    {
        private readonly AppDbContext _context;

        public DocenteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Docente> GetByIdAsync(int id)
        {
            return await _context.Docentes
                .Include(d => d.Documentos)
                .Include(d => d.Solicitudes)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Docente> GetByCedulaAsync(string cedula)
        {
            return await _context.Docentes
                .Include(d => d.Documentos)
                .Include(d => d.Solicitudes)
                .FirstOrDefaultAsync(d => d.Cedula == cedula);
        }

        public async Task<Docente> GetByUsernameAsync(string username)
        {
            return await _context.Docentes
                .Include(d => d.Documentos)
                .Include(d => d.Solicitudes)
                .FirstOrDefaultAsync(d => d.NombreUsuario == username);
        }

        public async Task<IEnumerable<Docente>> GetAllAsync()
        {
            return await _context.Docentes.ToListAsync();
        }

        public async Task AddAsync(Docente docente)
        {
            await _context.Docentes.AddAsync(docente);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Docente docente)
        {
            _context.Docentes.Update(docente);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Docentes.AnyAsync(d => d.Id == id);
        }
    }
}
