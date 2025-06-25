using SGA.Application.Interfaces;
using SGA.Application.Interfaces.Repositories;
using SGA.Domain.Entities;

namespace SGA.Application.Services;

public class AuditoriaService : IAuditoriaService
{
    private readonly ILogAuditoriaRepository _logRepository;

    public AuditoriaService(ILogAuditoriaRepository logRepository)
    {
        _logRepository = logRepository;
    }

    public async Task RegistrarAccionAsync(string accion, string? usuarioId, string? usuarioEmail, 
        string? entidadAfectada, string? valoresAnteriores, string? valoresNuevos, string? direccionIP)
    {
        var log = new LogAuditoria
        {
            Accion = accion,
            UsuarioId = usuarioId,
            UsuarioEmail = usuarioEmail,
            EntidadAfectada = entidadAfectada,
            ValoresAnteriores = valoresAnteriores,
            ValoresNuevos = valoresNuevos,
            DireccionIP = direccionIP,
            FechaAccion = DateTime.UtcNow
        };

        await _logRepository.CreateAsync(log);
    }

    public async Task<List<object>> GetLogAuditoriaAsync(DateTime? fechaInicio = null, DateTime? fechaFin = null)
    {
        var logs = await _logRepository.GetByFechaRangoAsync(fechaInicio, fechaFin);
        
        return logs.Select(log => new
        {
            log.Id,
            log.Accion,
            log.UsuarioEmail,
            log.EntidadAfectada,
            log.FechaAccion,
            log.DireccionIP
        }).Cast<object>().ToList();
    }
}
