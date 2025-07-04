using AutoMapper;
using Microsoft.Extensions.Logging;
using SGA.Application.DTOs.Admin;
using SGA.Application.Interfaces;
using SGA.Domain.Entities;
using SGA.Domain.Enums;
using SGA.Domain.Extensions;
using System.Text.Json;

namespace SGA.Application.Services;

/// <summary>
/// Servicio para gestión de configuración de requisitos de ascenso
/// </summary>
public class ConfiguracionRequisitoService : IConfiguracionRequisitoService
{
    private readonly IConfiguracionRequisitoRepository _repository;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IMapper _mapper;
    private readonly ILogger<ConfiguracionRequisitoService> _logger;

    public ConfiguracionRequisitoService(
        IConfiguracionRequisitoRepository repository,
        IAuditoriaService auditoriaService,
        IMapper mapper,
        ILogger<ConfiguracionRequisitoService> logger)
    {
        _repository = repository;
        _auditoriaService = auditoriaService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<ConfiguracionRequisitoDto>> GetAllAsync()
    {
        var configuraciones = await _repository.GetAllAsync();
        return _mapper.Map<List<ConfiguracionRequisitoDto>>(configuraciones);
    }

    public async Task<List<ConfiguracionRequisitoDto>> GetActivasAsync()
    {
        var configuraciones = await _repository.GetActivasAsync();
        return _mapper.Map<List<ConfiguracionRequisitoDto>>(configuraciones);
    }

    public async Task<ConfiguracionRequisitoDto?> GetByIdAsync(Guid id)
    {
        var configuracion = await _repository.GetByIdAsync(id);
        return configuracion != null ? _mapper.Map<ConfiguracionRequisitoDto>(configuracion) : null;
    }

    public async Task<ConfiguracionRequisitoDto?> GetByNivelesAsync(NivelTitular nivelActual, NivelTitular nivelSolicitado)
    {
        var configuracion = await _repository.GetByNivelesAsync(nivelActual, nivelSolicitado);
        return configuracion != null ? _mapper.Map<ConfiguracionRequisitoDto>(configuracion) : null;
    }

    public async Task<List<ConfiguracionRequisitoResumenDto>> GetResumenAsync()
    {
        var configuraciones = await _repository.GetAllAsync();
        return _mapper.Map<List<ConfiguracionRequisitoResumenDto>>(configuraciones);
    }

    public async Task<ConfiguracionRequisitoDto> CreateAsync(CrearActualizarConfiguracionRequisitoDto dto, string usuarioEmail)
    {
        // Validar que no exista ya una configuración para esta transición
        var existeConfiguracion = await _repository.ExisteConfiguracionAsync(dto.NivelActual, dto.NivelSolicitado);
        if (existeConfiguracion)
        {
            throw new InvalidOperationException($"Ya existe una configuración para el ascenso de {dto.NivelActual.GetDescription()} a {dto.NivelSolicitado.GetDescription()}");
        }

        // Validar que el ascenso sea válido
        if (!dto.NivelActual.EsAscensoValido(dto.NivelSolicitado))
        {
            throw new ArgumentException($"El ascenso de {dto.NivelActual.GetDescription()} a {dto.NivelSolicitado.GetDescription()} no es válido");
        }

        var configuracion = _mapper.Map<ConfiguracionRequisito>(dto);
        configuracion.ModificadoPor = usuarioEmail;

        // Validar la configuración
        if (!configuracion.EsConfiguracionValida())
        {
            throw new ArgumentException("La configuración contiene valores inválidos");
        }

        var resultado = await _repository.CreateAsync(configuracion);

        // Auditoría
        await _auditoriaService.RegistrarAccionAsync(
            "Crear ConfiguracionRequisito",
            null, // usuarioId - no disponible desde aquí
            usuarioEmail,
            $"ConfiguracionRequisito:{resultado.Id}",
            null, // valores anteriores - nueva entidad
            JsonSerializer.Serialize(dto),
            null // direccionIP - no disponible desde servicio
        );

        _logger.LogInformation("Configuración de requisito creada: {NombreAscenso} por {Usuario}", 
            resultado.NombreAscenso, usuarioEmail);

        return _mapper.Map<ConfiguracionRequisitoDto>(resultado);
    }

    public async Task<ConfiguracionRequisitoDto> UpdateAsync(Guid id, CrearActualizarConfiguracionRequisitoDto dto, string usuarioEmail)
    {
        var configuracionExistente = await _repository.GetByIdAsync(id);
        if (configuracionExistente == null)
        {
            throw new ArgumentException("Configuración no encontrada");
        }

        // Validar que no exista otra configuración para esta transición (excepto la actual)
        var existeOtraConfiguracion = await _repository.ExisteConfiguracionAsync(dto.NivelActual, dto.NivelSolicitado, id);
        if (existeOtraConfiguracion)
        {
            throw new InvalidOperationException($"Ya existe otra configuración para el ascenso de {dto.NivelActual.GetDescription()} a {dto.NivelSolicitado.GetDescription()}");
        }

        // Validar que el ascenso sea válido
        if (!dto.NivelActual.EsAscensoValido(dto.NivelSolicitado))
        {
            throw new ArgumentException($"El ascenso de {dto.NivelActual.GetDescription()} a {dto.NivelSolicitado.GetDescription()} no es válido");
        }

        // Guardar valores anteriores para auditoría
        var valoresAnteriores = JsonSerializer.Serialize(new
        {
            configuracionExistente.NivelActual,
            configuracionExistente.NivelSolicitado,
            configuracionExistente.TiempoMinimoMeses,
            configuracionExistente.ObrasMinimas,
            configuracionExistente.PuntajeEvaluacionMinimo,
            configuracionExistente.HorasCapacitacionMinimas,
            configuracionExistente.TiempoInvestigacionMinimo,
            configuracionExistente.EstaActivo,
            configuracionExistente.Descripcion
        });

        // Actualizar propiedades
        _mapper.Map(dto, configuracionExistente);
        configuracionExistente.ModificadoPor = usuarioEmail;

        // Validar la configuración
        if (!configuracionExistente.EsConfiguracionValida())
        {
            throw new ArgumentException("La configuración contiene valores inválidos");
        }

        var resultado = await _repository.UpdateAsync(configuracionExistente);

        // Auditoría
        await _auditoriaService.RegistrarAccionAsync(
            "Actualizar ConfiguracionRequisito",
            null, // usuarioId
            usuarioEmail,
            $"ConfiguracionRequisito:{resultado.Id}",
            valoresAnteriores,
            JsonSerializer.Serialize(dto),
            null // direccionIP
        );

        _logger.LogInformation("Configuración de requisito actualizada: {NombreAscenso} por {Usuario}", 
            resultado.NombreAscenso, usuarioEmail);

        return _mapper.Map<ConfiguracionRequisitoDto>(resultado);
    }

    public async Task<bool> DeleteAsync(Guid id, string usuarioEmail)
    {
        var configuracion = await _repository.GetByIdAsync(id);
        if (configuracion == null)
        {
            return false;
        }

        var valoresAnteriores = JsonSerializer.Serialize(configuracion);

        await _repository.DeleteAsync(id);

        // Auditoría
        await _auditoriaService.RegistrarAccionAsync(
            "Eliminar ConfiguracionRequisito",
            null, // usuarioId
            usuarioEmail,
            $"ConfiguracionRequisito:{id}",
            valoresAnteriores,
            string.Empty,
            null // direccionIP
        );

        _logger.LogInformation("Configuración de requisito eliminada: {NombreAscenso} por {Usuario}", 
            configuracion.NombreAscenso, usuarioEmail);

        return true;
    }

    public async Task<bool> ToggleActivoAsync(Guid id, string usuarioEmail)
    {
        var configuracion = await _repository.GetByIdAsync(id);
        if (configuracion == null)
        {
            throw new ArgumentException("Configuración no encontrada");
        }

        var estadoAnterior = configuracion.EstaActivo;
        var nuevoEstado = await _repository.ToggleActivoAsync(id);

        // Auditoría
        await _auditoriaService.RegistrarAccionAsync(
            nuevoEstado ? "Activar ConfiguracionRequisito" : "Desactivar ConfiguracionRequisito",
            null, // usuarioId
            usuarioEmail,
            $"ConfiguracionRequisito:{id}",
            JsonSerializer.Serialize(new { EstaActivo = estadoAnterior }),
            JsonSerializer.Serialize(new { EstaActivo = nuevoEstado }),
            null // direccionIP
        );

        _logger.LogInformation("Configuración de requisito {Accion}: {NombreAscenso} por {Usuario}", 
            nuevoEstado ? "activada" : "desactivada", configuracion.NombreAscenso, usuarioEmail);

        return nuevoEstado;
    }

    public async Task<ValidacionConfiguracionesDto> ValidarConfiguracionesAsync()
    {
        var configuraciones = await GetAllAsync();
        var cobertura = await _repository.ValidarCoberturaNivelesAsync();
        
        var resultado = new ValidacionConfiguracionesDto
        {
            TotalConfiguraciones = configuraciones.Count,
            ConfiguracionesActivas = configuraciones.Count(c => c.EstaActivo),
            TodasConfiguradas = cobertura.Values.All(v => v)
        };

        foreach (var config in configuraciones)
        {
            // Validar configuración
            var configuracionEntidad = _mapper.Map<ConfiguracionRequisito>(config);
            
            if (configuracionEntidad.EsConfiguracionValida())
            {
                resultado.ConfiguracionesValidas.Add(config);
            }
            else
            {
                resultado.ConfiguracionesInvalidas.Add(config);
                resultado.Errores.Add($"Configuración inválida para {config.NombreAscenso}");
            }
        }

        // Verificar cobertura completa
        foreach (var kvp in cobertura.Where(c => !c.Value))
        {
            resultado.Errores.Add($"Falta configuración para: {kvp.Key}");
        }

        return resultado;
    }

    public async Task InicializarConfiguracionesPorDefectoAsync(string usuarioEmail)
    {
        var configuracionesExistentes = await _repository.GetAllAsync();
        if (configuracionesExistentes.Any())
        {
            _logger.LogInformation("Ya existen configuraciones de requisitos, no se inicializarán valores por defecto");
            return;
        }

        // Configuraciones por defecto basadas en los requisitos originales
        var configuracionesPorDefecto = new[]
        {
            new ConfiguracionRequisito
            {
                NivelActual = NivelTitular.Titular1,
                NivelSolicitado = NivelTitular.Titular2,
                TiempoMinimoMeses = 48, // 4 años
                ObrasMinimas = 1,
                PuntajeEvaluacionMinimo = 75m,
                HorasCapacitacionMinimas = 96,
                TiempoInvestigacionMinimo = 0,
                EstaActivo = true,
                ModificadoPor = usuarioEmail,
                Descripcion = "Requisitos para ascenso de Titular Auxiliar 1 a Titular Auxiliar 2"
            },
            new ConfiguracionRequisito
            {
                NivelActual = NivelTitular.Titular2,
                NivelSolicitado = NivelTitular.Titular3,
                TiempoMinimoMeses = 48, // 4 años
                ObrasMinimas = 2,
                PuntajeEvaluacionMinimo = 75m,
                HorasCapacitacionMinimas = 96,
                TiempoInvestigacionMinimo = 12,
                EstaActivo = true,
                ModificadoPor = usuarioEmail,
                Descripcion = "Requisitos para ascenso de Titular Auxiliar 2 a Titular Auxiliar 3"
            },
            new ConfiguracionRequisito
            {
                NivelActual = NivelTitular.Titular3,
                NivelSolicitado = NivelTitular.Titular4,
                TiempoMinimoMeses = 48, // 4 años
                ObrasMinimas = 3,
                PuntajeEvaluacionMinimo = 75m,
                HorasCapacitacionMinimas = 128,
                TiempoInvestigacionMinimo = 24,
                EstaActivo = true,
                ModificadoPor = usuarioEmail,
                Descripcion = "Requisitos para ascenso de Titular Auxiliar 3 a Titular Auxiliar 4"
            },
            new ConfiguracionRequisito
            {
                NivelActual = NivelTitular.Titular4,
                NivelSolicitado = NivelTitular.Titular5,
                TiempoMinimoMeses = 48, // 4 años
                ObrasMinimas = 5,
                PuntajeEvaluacionMinimo = 75m,
                HorasCapacitacionMinimas = 160,
                TiempoInvestigacionMinimo = 24,
                EstaActivo = true,
                ModificadoPor = usuarioEmail,
                Descripcion = "Requisitos para ascenso de Titular Auxiliar 4 a Titular Auxiliar 5"
            }
        };

        foreach (var config in configuracionesPorDefecto)
        {
            await _repository.CreateAsync(config);
        }

        _logger.LogInformation("Configuraciones de requisitos por defecto inicializadas por {Usuario}", usuarioEmail);
    }

    public async Task<string> ExportarConfiguracionesAsync()
    {
        var configuraciones = await GetAllAsync();
        return JsonSerializer.Serialize(configuraciones, new JsonSerializerOptions { WriteIndented = true });
    }

    public async Task<bool> ImportarConfiguracionesAsync(string jsonConfiguraciones, string usuarioEmail)
    {
        try
        {
            var configuraciones = JsonSerializer.Deserialize<List<ConfiguracionRequisitoDto>>(jsonConfiguraciones);
            if (configuraciones == null || !configuraciones.Any())
            {
                return false;
            }

            foreach (var configDto in configuraciones)
            {
                var existeConfiguracion = await _repository.ExisteConfiguracionAsync(configDto.NivelActual, configDto.NivelSolicitado);
                
                if (!existeConfiguracion)
                {
                    var crearDto = new CrearActualizarConfiguracionRequisitoDto
                    {
                        NivelActual = configDto.NivelActual,
                        NivelSolicitado = configDto.NivelSolicitado,
                        TiempoMinimoMeses = configDto.TiempoMinimoMeses,
                        ObrasMinimas = configDto.ObrasMinimas,
                        PuntajeEvaluacionMinimo = configDto.PuntajeEvaluacionMinimo,
                        HorasCapacitacionMinimas = configDto.HorasCapacitacionMinimas,
                        TiempoInvestigacionMinimo = configDto.TiempoInvestigacionMinimo,
                        EstaActivo = configDto.EstaActivo,
                        Descripcion = configDto.Descripcion
                    };

                    await CreateAsync(crearDto, usuarioEmail);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al importar configuraciones de requisitos");
            return false;
        }
    }

    public Task<List<HistorialConfiguracionDto>> GetHistorialCambiosAsync(int limite = 50)
    {
        // Esta implementación requeriría un servicio de auditoría más específico
        // Por ahora devolvemos una lista vacía
        return Task.FromResult(new List<HistorialConfiguracionDto>());
    }
}
