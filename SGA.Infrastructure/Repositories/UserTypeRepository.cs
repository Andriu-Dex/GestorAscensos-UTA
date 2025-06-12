using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities;
using SGA.Domain.Interfaces;
using SGA.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGA.Infrastructure.Repositories
{
    public class UserTypeRepository : IUserTypeRepository
    {
        private readonly AppDbContext _context;

        public UserTypeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserType> GetByIdAsync(int id)
        {
            return await _context.UserTypes.FindAsync(id);
        }

        public async Task<UserType> GetByNameAsync(string name)
        {
            return await _context.UserTypes
                .FirstOrDefaultAsync(x => x.Name == name);
        }

        public async Task<IEnumerable<UserType>> GetAllAsync()
        {
            return await _context.UserTypes.ToListAsync();
        }

        public async Task AddAsync(UserType userType)
        {
            await _context.UserTypes.AddAsync(userType);
            await SaveChangesAsync();
        }

        public async Task UpdateAsync(UserType userType)
        {
            _context.UserTypes.Update(userType);
            await SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
