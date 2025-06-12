using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities;
using SGA.Domain.Interfaces;
using SGA.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGA.Infrastructure.Repositories
{
    public class PromotionObservationRepository : IPromotionObservationRepository
    {
        private readonly AppDbContext _context;

        public PromotionObservationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PromotionObservation> GetByIdAsync(int id)
        {
            return await _context.PromotionObservations
                .Include(o => o.PromotionRequest)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<PromotionObservation>> GetByPromotionRequestIdAsync(int promotionRequestId)
        {
            return await _context.PromotionObservations
                .Where(o => o.PromotionRequestId == promotionRequestId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<PromotionObservation>> GetAllAsync()
        {
            return await _context.PromotionObservations
                .Include(o => o.PromotionRequest)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(PromotionObservation observation)
        {
            await _context.PromotionObservations.AddAsync(observation);
            await SaveChangesAsync();
        }

        public async Task UpdateAsync(PromotionObservation observation)
        {
            _context.PromotionObservations.Update(observation);
            await SaveChangesAsync();
        }

        public async Task DeleteAsync(PromotionObservation observation)
        {
            _context.PromotionObservations.Remove(observation);
            await SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
