using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities;
using SGA.Infrastructure.Data;

namespace SGA.Infrastructure.Repositories
{
    public interface IDatosTTHHRepository
    {
        Task<DatosTTHH> GetByCedulaAsync(string cedula);
        Task<bool> ExistsByCedulaAsync(string cedula);
        Task AddAsync(DatosTTHH datosTTHH);
        Task<IEnumerable<DatosTTHH>> GetAllAsync();
    }

    public class DatosTTHHRepository : IDatosTTHHRepository
    {
        private readonly AppDbContext _context;

        public DatosTTHHRepository(AppDbContext context)
        {
            _context = context;
        }        public async Task<DatosTTHH> GetByCedulaAsync(string cedula)
        {
            return await _context.DatosTTHH
                .Include(d => d.Facultad)
                .FirstOrDefaultAsync(d => d.Cedula == cedula);
        }

        public async Task<bool> ExistsByCedulaAsync(string cedula)
        {
            return await _context.DatosTTHH
                .AnyAsync(d => d.Cedula == cedula);
        }

        public async Task AddAsync(DatosTTHH datosTTHH)
        {
            await _context.DatosTTHH.AddAsync(datosTTHH);
            await _context.SaveChangesAsync();
        }        public async Task<IEnumerable<DatosTTHH>> GetAllAsync()
        {
            return await _context.DatosTTHH
                .Include(d => d.Facultad)
                .ToListAsync();
        }
    }
}
