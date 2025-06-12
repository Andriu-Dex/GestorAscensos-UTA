using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities;
using SGA.Domain.Interfaces;
using SGA.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGA.Infrastructure.Repositories
{
    public class DocumentObservationRepository : IDocumentObservationRepository
    {
        private readonly AppDbContext _context;

        public DocumentObservationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DocumentObservation> GetByIdAsync(int id)
        {
            return await _context.DocumentObservations
                .Include(o => o.Document)
                .Include(o => o.Reviewer)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<DocumentObservation>> GetByDocumentIdAsync(int documentId)
        {
            return await _context.DocumentObservations
                .Include(o => o.Reviewer)
                .Where(o => o.DocumentId == documentId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<DocumentObservation>> GetByReviewerIdAsync(int reviewerId)
        {
            return await _context.DocumentObservations
                .Include(o => o.Document)
                .Where(o => o.ReviewerId == reviewerId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<DocumentObservation>> GetAllAsync()
        {
            return await _context.DocumentObservations
                .Include(o => o.Document)
                .Include(o => o.Reviewer)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(DocumentObservation observation)
        {
            await _context.DocumentObservations.AddAsync(observation);
            await SaveChangesAsync();
        }

        public async Task UpdateAsync(DocumentObservation observation)
        {
            _context.DocumentObservations.Update(observation);
            await SaveChangesAsync();
        }

        public async Task DeleteAsync(DocumentObservation observation)
        {
            _context.DocumentObservations.Remove(observation);
            await SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
