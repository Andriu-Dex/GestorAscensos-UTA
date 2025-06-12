using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities;
using SGA.Domain.Interfaces;
using SGA.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGA.Infrastructure.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly AppDbContext _context;

        public DocumentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Document> GetByIdAsync(int id)
        {
            return await _context.Documents
                .Include(d => d.Teacher)
                .Include(d => d.Reviewer)
                .Include(d => d.Requirement)
                .Include(d => d.Observations)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<Document>> GetByTeacherIdAsync(int teacherId)
        {
            return await _context.Documents
                .Include(d => d.Observations)
                .Include(d => d.Requirement)
                .Where(d => d.TeacherId == teacherId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Document>> GetByRequirementIdAsync(int requirementId)
        {
            return await _context.Documents
                .Include(d => d.Teacher)
                .Include(d => d.Observations)
                .Where(d => d.RequirementId == requirementId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Document>> GetByDocumentTypeAsync(string documentType)
        {
            return await _context.Documents
                .Include(d => d.Teacher)
                .Include(d => d.Observations)
                .Where(d => d.DocumentType == documentType)
                .ToListAsync();
        }

        public async Task<IEnumerable<Document>> GetAllAsync()
        {
            return await _context.Documents
                .Include(d => d.Teacher)
                .Include(d => d.Requirement)
                .ToListAsync();
        }

        public async Task AddAsync(Document document)
        {
            await _context.Documents.AddAsync(document);
            await SaveChangesAsync();
        }

        public async Task UpdateAsync(Document document)
        {
            _context.Documents.Update(document);
            await SaveChangesAsync();
        }

        public async Task DeleteAsync(Document document)
        {
            _context.Documents.Remove(document);
            await SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
