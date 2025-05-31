using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities;
using SGA.Infrastructure.Data;

namespace SGA.Infrastructure.Repositories
{
    public interface IDocumentoRepository
    {
        Task<Documento> GetByIdAsync(int id);
        Task<IEnumerable<Documento>> GetByDocenteIdAsync(int docenteId);
        Task<IEnumerable<Documento>> GetByTipoAsync(int docenteId, TipoDocumento tipo);
        Task AddAsync(Documento documento);
        Task UpdateAsync(Documento documento);
        Task DeleteAsync(int id);
    }

    public class DocumentoRepository : IDocumentoRepository
    {
        private readonly AppDbContext _context;

        public DocumentoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Documento> GetByIdAsync(int id)
        {
            return await _context.Documentos.FindAsync(id);
        }

        public async Task<IEnumerable<Documento>> GetByDocenteIdAsync(int docenteId)
        {
            return await _context.Documentos
                .Where(d => d.DocenteId == docenteId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Documento>> GetByTipoAsync(int docenteId, TipoDocumento tipo)
        {
            return await _context.Documentos
                .Where(d => d.DocenteId == docenteId && d.Tipo == tipo)
                .ToListAsync();
        }

        public async Task AddAsync(Documento documento)
        {
            await _context.Documentos.AddAsync(documento);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Documento documento)
        {
            _context.Documentos.Update(documento);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var documento = await _context.Documentos.FindAsync(id);
            if (documento != null)
            {
                _context.Documentos.Remove(documento);
                await _context.SaveChangesAsync();
            }
        }
    }
}
