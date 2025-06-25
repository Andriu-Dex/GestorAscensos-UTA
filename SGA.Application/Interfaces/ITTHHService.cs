using SGA.Domain.Entities.External;

namespace SGA.Application.Interfaces;

public interface ITTHHService
{
    Task<EmpleadoTTHH?> GetEmpleadoByCedulaAsync(string cedula);
    Task<bool> ValidarEmpleadoActivoAsync(string cedula);
    Task<List<EmpleadoTTHH>> GetAllEmpleadosActivosAsync();
}
