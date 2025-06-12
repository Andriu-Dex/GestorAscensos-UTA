using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities;
using SGA.Infrastructure.Data;

namespace SGA.Infrastructure.Repositories
{
    public interface IServicioExternoRepository
    {
        Task<ServicioExterno> GetByCodigoAsync(string codigo);
        Task<IEnumerable<ServicioExterno>> GetAllActivosAsync();
        Task<bool> ActualizarEstadoConexionAsync(int id, bool exito, string? error = null);
        Task AddAsync(ServicioExterno servicioExterno);
        Task UpdateAsync(ServicioExterno servicioExterno);
    }

    public class ServicioExternoRepository : IServicioExternoRepository
    {
        private readonly AppDbContext _context;

        public ServicioExternoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServicioExterno> GetByCodigoAsync(string codigo)
        {
            return await _context.ServiciosExternos
                .FirstOrDefaultAsync(s => s.Codigo == codigo);
        }

        public async Task<IEnumerable<ServicioExterno>> GetAllActivosAsync()
        {
            return await _context.ServiciosExternos
                .Where(s => s.Activo)
                .ToListAsync();
        }

        public async Task<bool> ActualizarEstadoConexionAsync(int id, bool exito, string? error = null)
        {
            var servicio = await _context.ServiciosExternos.FindAsync(id);
            if (servicio == null)
                return false;

            servicio.FechaUltimaConexion = DateTime.Now;
            
            if (!exito)
                servicio.UltimoError = error;
            else
                servicio.UltimoError = null;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task AddAsync(ServicioExterno servicioExterno)
        {
            await _context.ServiciosExternos.AddAsync(servicioExterno);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ServicioExterno servicioExterno)
        {
            _context.Entry(servicioExterno).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
