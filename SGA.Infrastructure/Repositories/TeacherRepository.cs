using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities;
using SGA.Domain.Interfaces;
using SGA.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGA.Infrastructure.Repositories
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly AppDbContext _context;

        public TeacherRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Teacher> GetByIdAsync(int id)
        {
            return await _context.Teachers
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Teacher> GetByIdentificationNumberAsync(string identificationNumber)
        {
            return await _context.Teachers
                .FirstOrDefaultAsync(t => t.IdentificationNumber == identificationNumber);
        }

        public async Task<IEnumerable<Teacher>> GetAllAsync()
        {
            return await _context.Teachers.ToListAsync();
        }

        public async Task AddAsync(Teacher teacher)
        {
            await _context.Teachers.AddAsync(teacher);
            await SaveChangesAsync();
        }

        public async Task UpdateAsync(Teacher teacher)
        {
            _context.Teachers.Update(teacher);
            await SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
