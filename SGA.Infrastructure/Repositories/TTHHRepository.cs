using Microsoft.EntityFrameworkCore;
using SGA.Application.Interfaces.Repositories;
using SGA.Domain.Entities.External;
using SGA.Infrastructure.Data.External;

namespace SGA.Infrastructure.Repositories;

public class TTHHRepository : ITTHHRepository
{
    private readonly TTHHDbContext _tthhContext;

    public TTHHRepository(TTHHDbContext tthhContext)
    {
        _tthhContext = tthhContext;
    }

    public async Task<EmpleadoTTHH?> GetEmpleadoByCedulaAsync(string cedula)
    {
        return await _tthhContext.Empleados
            .FirstOrDefaultAsync(e => e.Cedula == cedula && e.EstaActivo);
    }

    public async Task<bool> ValidarEmpleadoActivoAsync(string cedula)
    {
        return await _tthhContext.Empleados
            .AnyAsync(e => e.Cedula == cedula && e.EstaActivo);
    }

    public async Task<List<EmpleadoTTHH>> GetAllEmpleadosActivosAsync()
    {
        return await _tthhContext.Empleados
            .Where(e => e.EstaActivo)
            .ToListAsync();
    }
}
