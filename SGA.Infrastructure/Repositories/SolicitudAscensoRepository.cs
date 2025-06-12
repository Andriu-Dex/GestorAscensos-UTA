using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities;
using SGA.Infrastructure.Data;

namespace SGA.Infrastructure.Repositories
{    public interface ISolicitudAscensoRepository
    {
        Task<SolicitudAscenso?> GetByIdAsync(int id);
        Task<IEnumerable<SolicitudAscenso>> GetByDocenteIdAsync(int docenteId);
        Task<IEnumerable<SolicitudAscenso>> GetAllPendientesAsync();
        Task<IEnumerable<SolicitudAscenso>> GetByEstadoAsync(int estadoId);
        Task AddAsync(SolicitudAscenso solicitud);
        Task UpdateAsync(SolicitudAscenso solicitud);
    }

    public class SolicitudAscensoRepository : ISolicitudAscensoRepository
    {
        private readonly AppDbContext _context;

        public SolicitudAscensoRepository(AppDbContext context)
        {
            _context = context;
        }        public async Task<SolicitudAscenso?> GetByIdAsync(int id)
        {
            return await _context.SolicitudesAscenso
                .Include(s => s.Docente)
                .Include(s => s.EstadoSolicitud)
                .Include(s => s.Documentos)
                    .ThenInclude(d => d.Documento)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<SolicitudAscenso>> GetByDocenteIdAsync(int docenteId)
        {
            return await _context.SolicitudesAscenso
                .Where(s => s.DocenteId == docenteId)
                .Include(s => s.Documentos)
                    .ThenInclude(d => d.Documento)
                .ToListAsync();
        }        public async Task<IEnumerable<SolicitudAscenso>> GetAllPendientesAsync()
        {
            // Estados Enviada=1 y EnProceso=2 según los datos seed
            return await _context.SolicitudesAscenso
                .Where(s => s.EstadoSolicitudId == 1 || s.EstadoSolicitudId == 2)
                .Include(s => s.Docente)
                .Include(s => s.EstadoSolicitud)
                .Include(s => s.Documentos)
                    .ThenInclude(d => d.Documento)
                .ToListAsync();
        }

        public async Task<IEnumerable<SolicitudAscenso>> GetByEstadoAsync(int estadoId)
        {
            return await _context.SolicitudesAscenso
                .Where(s => s.EstadoSolicitudId == estadoId)
                .Include(s => s.Docente)
                .Include(s => s.EstadoSolicitud)
                .Include(s => s.Documentos)
                    .ThenInclude(d => d.Documento)
                .ToListAsync();
        }

        public async Task AddAsync(SolicitudAscenso solicitud)
        {
            await _context.SolicitudesAscenso.AddAsync(solicitud);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SolicitudAscenso solicitud)
        {
            _context.SolicitudesAscenso.Update(solicitud);
            await _context.SaveChangesAsync();
        }
    }
}
