using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities;
using SGA.Domain.Interfaces;
using SGA.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGA.Infrastructure.Repositories
{
    public class AcademicDegreeRepository : IAcademicDegreeRepository
    {
        private readonly AppDbContext _context;

        public AcademicDegreeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AcademicDegree> GetByIdAsync(int id)
        {
            return await _context.AcademicDegrees
                .Include(a => a.Teacher)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<AcademicDegree>> GetByTeacherIdAsync(int teacherId)
        {
            return await _context.AcademicDegrees
                .Where(a => a.TeacherId == teacherId)
                .ToListAsync();
        }

        public async Task<IEnumerable<AcademicDegree>> GetByDegreeTypeAsync(string degreeType)
        {
            return await _context.AcademicDegrees
                .Include(a => a.Teacher)
                .Where(a => a.DegreeType == degreeType)
                .ToListAsync();
        }

        public async Task<IEnumerable<AcademicDegree>> GetAllAsync()
        {
            return await _context.AcademicDegrees
                .Include(a => a.Teacher)
                .ToListAsync();
        }

        public async Task AddAsync(AcademicDegree academicDegree)
        {
            await _context.AcademicDegrees.AddAsync(academicDegree);
            await SaveChangesAsync();
        }

        public async Task UpdateAsync(AcademicDegree academicDegree)
        {
            _context.AcademicDegrees.Update(academicDegree);
            await SaveChangesAsync();
        }

        public async Task DeleteAsync(AcademicDegree academicDegree)
        {
            _context.AcademicDegrees.Remove(academicDegree);
            await SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
