using SGA.Application.Interfaces;
using SGA.Domain.Enums;
using SGA.Domain.Extensions;

namespace SGA.Application.Services;

/// <summary>
/// Servicio para obtener requisitos de ascenso de forma dinámica desde la base de datos
/// </summary>
public class RequisitosDinamicosService : IRequisitosDinamicosService
{
    private readonly IConfiguracionRequisitoRepository _configuracionRepository;

    public RequisitosDinamicosService(IConfiguracionRequisitoRepository configuracionRepository)
    {
        _configuracionRepository = configuracionRepository;
    }

    public async Task<RequisitoAscenso?> ObtenerRequisitosAsync(NivelTitular nivelActual, NivelTitular nivelSolicitado)
    {
        var configuracion = await _configuracionRepository.GetByNivelesAsync(nivelActual, nivelSolicitado);
        return configuracion != null ? RequisitoAscenso.FromConfiguracion(configuracion) : null;
    }

    public async Task<RequisitoAscenso?> ObtenerRequisitosParaSiguienteNivelAsync(NivelTitular nivelActual)
    {
        var siguienteNivel = nivelActual.GetSiguienteNivel();
        if (!siguienteNivel.HasValue)
        {
            return null; // Ya está en el nivel máximo
        }

        return await ObtenerRequisitosAsync(nivelActual, siguienteNivel.Value);
    }

    public async Task<bool> TieneRequisitosConfiguradosAsync(NivelTitular nivelActual, NivelTitular nivelSolicitado)
    {
        return await _configuracionRepository.ExisteConfiguracionAsync(nivelActual, nivelSolicitado);
    }

    public async Task<List<RequisitoAscenso>> ObtenerTodosLosRequisitosActivosAsync()
    {
        var configuraciones = await _configuracionRepository.GetActivasAsync();
        return configuraciones.Select(RequisitoAscenso.FromConfiguracion).ToList();
    }
}
