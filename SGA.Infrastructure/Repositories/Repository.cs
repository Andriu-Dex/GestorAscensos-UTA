using Microsoft.EntityFrameworkCore;
using SGA.Application.Interfaces.Repositories;
using SGA.Infrastructure.Data;

namespace SGA.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T> AddAsync(T entity)
    {
        try
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        catch (Exception ex)
        {
            // Log detallado del error con toda la cadena de inner exceptions
            var entityName = typeof(T).Name;
            var errorDetails = GetFullExceptionDetails(ex);
            
            Console.WriteLine($"====== ERROR EN REPOSITORY.ADDASYNC ======");
            Console.WriteLine($"Entidad: {entityName}");
            Console.WriteLine($"Fecha: {DateTime.Now}");
            Console.WriteLine($"Detalles completos del error:");
            Console.WriteLine(errorDetails);
            Console.WriteLine($"==========================================");
            
            throw new Exception($"Error al guardar {entityName}: {ex.Message} | Inner: {ex.InnerException?.Message}", ex);
        }
    }

    private string GetFullExceptionDetails(Exception ex)
    {
        var details = new System.Text.StringBuilder();
        var currentEx = ex;
        int level = 0;
        
        while (currentEx != null)
        {
            details.AppendLine($"Nivel {level}: {currentEx.GetType().Name}");
            details.AppendLine($"  Mensaje: {currentEx.Message}");
            if (currentEx.Data?.Count > 0)
            {
                details.AppendLine($"  Data:");
                foreach (System.Collections.DictionaryEntry item in currentEx.Data)
                {
                    details.AppendLine($"    {item.Key}: {item.Value}");
                }
            }
            if (!string.IsNullOrEmpty(currentEx.StackTrace))
            {
                details.AppendLine($"  Stack Trace: {currentEx.StackTrace}");
            }
            
            currentEx = currentEx.InnerException;
            level++;
        }
        
        return details.ToString();
    }

    public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await _dbSet.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return entities;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<T> UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        return entity != null;
    }
}
