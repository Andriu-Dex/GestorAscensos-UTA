using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities;
using SGA.Domain.Enums;
using SGA.Domain.Interfaces;
using SGA.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGA.Infrastructure.Repositories
{
    public class PromotionRequestRepository : IPromotionRequestRepository
    {
        private readonly AppDbContext _context;

        public PromotionRequestRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<PromotionRequest> GetByIdAsync(int id)
        {
            return await _context.PromotionRequests
                .Include(pr => pr.Teacher)
                .FirstOrDefaultAsync(pr => pr.Id == id);
        }

        public async Task<IEnumerable<PromotionRequest>> GetByTeacherIdAsync(int teacherId)
        {
            return await _context.PromotionRequests
                .Where(pr => pr.TeacherId == teacherId)
                .OrderByDescending(pr => pr.CreatedAt)
                .ToListAsync();
        }

        public async Task<PromotionRequest> GetActiveRequestByTeacherIdAsync(int teacherId)
        {
            return await _context.PromotionRequests
                .Where(pr => pr.TeacherId == teacherId && 
                      (pr.Status == PromotionRequestStatus.Pending || 
                       pr.Status == PromotionRequestStatus.InProgress))
                .OrderByDescending(pr => pr.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PromotionRequest>> GetByStatusAsync(PromotionRequestStatus status)
        {
            return await _context.PromotionRequests
                .Include(pr => pr.Teacher)
                .Where(pr => pr.Status == status)
                .OrderByDescending(pr => pr.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<PromotionRequest>> GetAllAsync()
        {
            return await _context.PromotionRequests
                .Include(pr => pr.Teacher)
                .OrderByDescending(pr => pr.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(PromotionRequest promotionRequest)
        {
            await _context.PromotionRequests.AddAsync(promotionRequest);
            await SaveChangesAsync();
        }

        public async Task UpdateAsync(PromotionRequest promotionRequest)
        {
            _context.PromotionRequests.Update(promotionRequest);
            await SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
