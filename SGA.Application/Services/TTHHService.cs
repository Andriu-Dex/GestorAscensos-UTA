using SGA.Application.Interfaces;
using SGA.Application.Interfaces.Repositories;
using SGA.Domain.Entities.External;

namespace SGA.Application.Services;

public class TTHHService : ITTHHService
{
    private readonly ITTHHRepository _tthhRepository;

    public TTHHService(ITTHHRepository tthhRepository)
    {
        _tthhRepository = tthhRepository;
    }

    public async Task<EmpleadoTTHH?> GetEmpleadoByCedulaAsync(string cedula)
    {
        return await _tthhRepository.GetEmpleadoByCedulaAsync(cedula);
    }

    public async Task<bool> ValidarEmpleadoActivoAsync(string cedula)
    {
        return await _tthhRepository.ValidarEmpleadoActivoAsync(cedula);
    }

    public async Task<List<EmpleadoTTHH>> GetAllEmpleadosActivosAsync()
    {
        return await _tthhRepository.GetAllEmpleadosActivosAsync();
    }
}
