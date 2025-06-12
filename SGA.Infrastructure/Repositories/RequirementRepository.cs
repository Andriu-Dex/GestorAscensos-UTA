using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities;
using SGA.Domain.Interfaces;
using SGA.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGA.Infrastructure.Repositories
{
    public class RequirementRepository : IRequirementRepository
    {
        private readonly AppDbContext _context;

        public RequirementRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Requirement> GetByIdAsync(int id)
        {
            return await _context.Requirements.FindAsync(id);
        }

        public async Task<Requirement> GetByNameAsync(string name)
        {
            return await _context.Requirements
                .FirstOrDefaultAsync(x => x.Name == name);
        }

        public async Task<IEnumerable<Requirement>> GetAllAsync()
        {
            return await _context.Requirements.ToListAsync();
        }

        public async Task AddAsync(Requirement requirement)
        {
            await _context.Requirements.AddAsync(requirement);
            await SaveChangesAsync();
        }

        public async Task UpdateAsync(Requirement requirement)
        {
            _context.Requirements.Update(requirement);
            await SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
