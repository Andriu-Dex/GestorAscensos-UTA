using SGA.Domain.Entities.External;

namespace SGA.Application.Interfaces.Repositories;

public interface ITTHHRepository
{
    Task<EmpleadoTTHH?> GetEmpleadoByCedulaAsync(string cedula);
    Task<bool> ValidarEmpleadoActivoAsync(string cedula);
    Task<List<EmpleadoTTHH>> GetAllEmpleadosActivosAsync();
}
