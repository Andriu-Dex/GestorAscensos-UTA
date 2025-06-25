using SGA.Application.DTOs.Auditorias;

namespace SGA.Application.Interfaces;

public interface IAuditoriaService
{
    Task RegistrarAccionAsync(string accion, string? usuarioId, string? usuarioEmail, 
        string? entidadAfectada, string? valoresAnteriores, string? valoresNuevos, string? direccionIP);
    Task<List<object>> GetLogAuditoriaAsync(DateTime? fechaInicio = null, DateTime? fechaFin = null);
}
