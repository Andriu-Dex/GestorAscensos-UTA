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
/// Servicio para gestión de títulos académicos dinámicos
/// </summary>
public class TituloAcademicoService : ITituloAcademicoService
{
    private readonly ITituloAcademicoRepository _repository;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IMapper _mapper;
    private readonly ILogger<TituloAcademicoService> _logger;

    public TituloAcademicoService(
        ITituloAcademicoRepository repository,
        IAuditoriaService auditoriaService,
        IMapper mapper,
        ILogger<TituloAcademicoService> logger)
    {
        _repository = repository;
        _auditoriaService = auditoriaService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<TituloAcademicoDto>> GetAllAsync()
    {
        var titulos = await _repository.GetAllAsync();
        var dtos = _mapper.Map<List<TituloAcademicoDto>>(titulos);
        
        foreach (var dto in dtos)
        {
            dto.RepresentacionCompleta = ObtenerRepresentacionCompleta(dto);
        }
        
        return dtos;
    }

    public async Task<List<TituloAcademicoDto>> GetActivosAsync()
    {
        var titulos = await _repository.GetActivosOrdenadosAsync();
        var dtos = _mapper.Map<List<TituloAcademicoDto>>(titulos);
        
        foreach (var dto in dtos)
        {
            dto.RepresentacionCompleta = ObtenerRepresentacionCompleta(dto);
        }
        
        return dtos;
    }

    public async Task<TituloAcademicoDto?> GetByIdAsync(Guid id)
    {
        var titulo = await _repository.GetByIdAsync(id);
        if (titulo == null) return null;
        
        var dto = _mapper.Map<TituloAcademicoDto>(titulo);
        dto.RepresentacionCompleta = ObtenerRepresentacionCompleta(dto);
        
        return dto;
    }

    public async Task<TituloAcademicoDto?> GetByCodigoAsync(string codigo)
    {
        var titulo = await _repository.GetByCodigoAsync(codigo);
        if (titulo == null) return null;
        
        var dto = _mapper.Map<TituloAcademicoDto>(titulo);
        dto.RepresentacionCompleta = ObtenerRepresentacionCompleta(dto);
        
        return dto;
    }

    public async Task<TituloAcademicoDto> CreateAsync(CrearActualizarTituloAcademicoDto dto, string usuarioEmail)
    {
        // Validaciones de negocio
        await ValidarCreacion(dto);
        
        var titulo = _mapper.Map<TituloAcademico>(dto);
        titulo.ModificadoPor = usuarioEmail;
        titulo.FechaCreacion = DateTime.UtcNow;
        
        // Si no se especifica orden, obtener el siguiente disponible
        if (titulo.OrdenJerarquico <= 0)
        {
            titulo.OrdenJerarquico = await _repository.GetSiguienteOrdenDisponibleAsync();
        }
        
        var resultado = await _repository.CreateAsync(titulo);
        
        // Auditoría
        await _auditoriaService.RegistrarAccionAsync(
            "Crear TituloAcademico",
            null,
            usuarioEmail,
            $"TituloAcademico:{resultado.Id}",
            string.Empty,
            JsonSerializer.Serialize(resultado),
            null
        );
        
        _logger.LogInformation("Título académico creado: {Nombre} por {Usuario}", 
            resultado.Nombre, usuarioEmail);
        
        var dtoResultado = _mapper.Map<TituloAcademicoDto>(resultado);
        dtoResultado.RepresentacionCompleta = ObtenerRepresentacionCompleta(dtoResultado);
        
        return dtoResultado;
    }

    public async Task<TituloAcademicoDto> UpdateAsync(Guid id, CrearActualizarTituloAcademicoDto dto, string usuarioEmail)
    {
        var tituloExistente = await _repository.GetByIdAsync(id);
        if (tituloExistente == null)
        {
            throw new ArgumentException($"Título académico con ID {id} no encontrado");
        }
        
        if (tituloExistente.EsTituloSistema && (dto.Nombre != tituloExistente.Nombre || dto.Codigo != tituloExistente.Codigo))
        {
            throw new InvalidOperationException("No se puede cambiar el nombre o código de un título del sistema");
        }
        
        // Validaciones de negocio
        await ValidarActualizacion(dto, id);
        
        // Guardar valores anteriores para auditoría
        var valoresAnteriores = JsonSerializer.Serialize(new
        {
            tituloExistente.Nombre,
            tituloExistente.Codigo,
            tituloExistente.Descripcion,
            tituloExistente.OrdenJerarquico,
            tituloExistente.EstaActivo,
            tituloExistente.ColorHex
        });
        
        // Actualizar propiedades
        _mapper.Map(dto, tituloExistente);
        tituloExistente.ModificadoPor = usuarioEmail;
        tituloExistente.FechaModificacion = DateTime.UtcNow;
        
        var resultado = await _repository.UpdateAsync(tituloExistente);
        
        // Auditoría
        await _auditoriaService.RegistrarAccionAsync(
            "Actualizar TituloAcademico",
            null,
            usuarioEmail,
            $"TituloAcademico:{id}",
            valoresAnteriores,
            JsonSerializer.Serialize(resultado),
            null
        );
        
        _logger.LogInformation("Título académico actualizado: {Nombre} por {Usuario}", 
            resultado.Nombre, usuarioEmail);
        
        var dtoResultado = _mapper.Map<TituloAcademicoDto>(resultado);
        dtoResultado.RepresentacionCompleta = ObtenerRepresentacionCompleta(dtoResultado);
        
        return dtoResultado;
    }

    public async Task<bool> DeleteAsync(Guid id, string usuarioEmail)
    {
        var titulo = await _repository.GetByIdAsync(id);
        if (titulo == null)
        {
            return false;
        }
        
        if (titulo.EsTituloSistema)
        {
            throw new InvalidOperationException("No se puede eliminar un título del sistema");
        }
        
        // Verificar que no esté en uso
        if (!await PuedeSerEliminadoAsync(id))
        {
            throw new InvalidOperationException("No se puede eliminar el título porque está siendo utilizado en configuraciones de requisitos");
        }
        
        var valoresAnteriores = JsonSerializer.Serialize(titulo);
        
        await _repository.DeleteAsync(id);
        
        // Auditoría
        await _auditoriaService.RegistrarAccionAsync(
            "Eliminar TituloAcademico",
            null,
            usuarioEmail,
            $"TituloAcademico:{id}",
            valoresAnteriores,
            string.Empty,
            null
        );
        
        _logger.LogInformation("Título académico eliminado: {Nombre} por {Usuario}", 
            titulo.Nombre, usuarioEmail);
        
        return true;
    }

    public async Task<bool> ToggleActivoAsync(Guid id, string usuarioEmail)
    {
        var titulo = await _repository.GetByIdAsync(id);
        if (titulo == null)
        {
            throw new ArgumentException($"Título académico con ID {id} no encontrado");
        }
        
        var estadoAnterior = titulo.EstaActivo;
        titulo.EstaActivo = !titulo.EstaActivo;
        titulo.ModificadoPor = usuarioEmail;
        titulo.FechaModificacion = DateTime.UtcNow;
        
        await _repository.UpdateAsync(titulo);
        
        // Auditoría
        await _auditoriaService.RegistrarAccionAsync(
            $"{(titulo.EstaActivo ? "Activar" : "Desactivar")} TituloAcademico",
            null,
            usuarioEmail,
            $"TituloAcademico:{id}",
            JsonSerializer.Serialize(new { EstaActivo = estadoAnterior }),
            JsonSerializer.Serialize(new { EstaActivo = titulo.EstaActivo }),
            null
        );
        
        _logger.LogInformation("Título académico {Accion}: {Nombre} por {Usuario}", 
            titulo.EstaActivo ? "activado" : "desactivado", titulo.Nombre, usuarioEmail);
        
        return titulo.EstaActivo;
    }

    public async Task<List<TituloAcademicoOpcionDto>> GetOpcionesAsync(bool soloActivos = true)
    {
        var titulos = soloActivos 
            ? await _repository.GetActivosOrdenadosAsync()
            : await _repository.GetAllAsync();
            
        return _mapper.Map<List<TituloAcademicoOpcionDto>>(titulos);
    }

    public async Task<List<TituloAcademicoResumenDto>> GetResumenAsync()
    {
        var titulos = await _repository.GetAllAsync();
        return _mapper.Map<List<TituloAcademicoResumenDto>>(titulos);
    }

    public async Task<List<NivelAcademicoHibridoDto>> GetNivelesHibridosAsync()
    {
        var niveles = new List<NivelAcademicoHibridoDto>();
        
        // Agregar niveles del enum
        foreach (NivelTitular nivel in Enum.GetValues<NivelTitular>())
        {
            niveles.Add(new NivelAcademicoHibridoDto
            {
                Id = $"enum_{(int)nivel}",
                Nombre = nivel.GetDescription(),
                Codigo = nivel.ToString(),
                OrdenJerarquico = (int)nivel,
                EsEnum = true,
                EsTituloSistema = true,
                ColorHex = "#8a1538"
            });
        }
        
        // Agregar títulos dinámicos activos
        var titulosDinamicos = await _repository.GetActivosOrdenadosAsync();
        foreach (var titulo in titulosDinamicos)
        {
            niveles.Add(new NivelAcademicoHibridoDto
            {
                Id = $"titulo_{titulo.Id}",
                Nombre = titulo.Nombre,
                Codigo = titulo.Codigo,
                OrdenJerarquico = titulo.OrdenJerarquico,
                EsEnum = false,
                EsTituloSistema = titulo.EsTituloSistema,
                ColorHex = titulo.ColorHex ?? "#6c757d"
            });
        }
        
        return niveles.OrderBy(n => n.OrdenJerarquico).ToList();
    }

    public async Task<List<TituloAcademicoOpcionDto>> GetPosiblesAscensosAsync(Guid tituloActualId)
    {
        var posibles = await _repository.GetPosiblesAscensosAsync(tituloActualId);
        return _mapper.Map<List<TituloAcademicoOpcionDto>>(posibles);
    }

    public async Task<bool> PuedeSerEliminadoAsync(Guid id)
    {
        // Verificar si el título está siendo usado en ConfiguracionRequisito
        // Para esto necesitaríamos acceso al repositorio de configuraciones
        // Por ahora implementamos una verificación básica
        
        var titulo = await _repository.GetByIdAsync(id);
        if (titulo == null)
            return false;

        // TODO: Implementar verificación con ConfiguracionRequisitoRepository
        // var configuraciones = await _configuracionRepository.GetByTituloIdAsync(id);
        // return !configuraciones.Any();
        
        return true; // Temporal - permitir eliminación hasta implementar verificación completa
    }

    public async Task InicializarTitulosPorDefectoAsync(string usuarioEmail)
    {
        var titulosExistentes = await _repository.GetAllAsync();
        
        // Solo crear si no existen títulos equivalentes al enum
        foreach (NivelTitular nivel in Enum.GetValues<NivelTitular>())
        {
            var equivalente = titulosExistentes.FirstOrDefault(t => t.NivelEquivalente == (int)nivel);
            if (equivalente == null)
            {
                var titulo = new TituloAcademico
                {
                    Nombre = nivel.GetDescription(),
                    Codigo = nivel.ToString().ToUpper(),
                    Descripcion = $"Título equivalente al nivel {nivel.GetDescription()}",
                    OrdenJerarquico = (int)nivel,
                    EstaActivo = true,
                    EsTituloSistema = true,
                    NivelEquivalente = (int)nivel,
                    ModificadoPor = usuarioEmail,
                    FechaCreacion = DateTime.UtcNow,
                    ColorHex = "#8a1538"
                };
                
                await _repository.CreateAsync(titulo);
            }
        }
        
        _logger.LogInformation("Títulos académicos por defecto inicializados por {Usuario}", usuarioEmail);
    }

    public async Task<bool> EsCodigoUnicoAsync(string codigo, Guid? excluirId = null)
    {
        return !await _repository.ExisteCodigoAsync(codigo, excluirId);
    }

    public async Task<bool> EsNombreUnicoAsync(string nombre, Guid? excluirId = null)
    {
        return !await _repository.ExisteNombreAsync(nombre, excluirId);
    }

    #region Métodos privados de validación

    private async Task ValidarCreacion(CrearActualizarTituloAcademicoDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre))
            throw new ArgumentException("El nombre del título es requerido");
            
        if (string.IsNullOrWhiteSpace(dto.Codigo))
            throw new ArgumentException("El código del título es requerido");
            
        if (dto.OrdenJerarquico <= 0 || dto.OrdenJerarquico > 100)
            throw new ArgumentException("El orden jerárquico debe estar entre 1 y 100");
            
        if (!await EsCodigoUnicoAsync(dto.Codigo))
            throw new InvalidOperationException($"Ya existe un título con el código '{dto.Codigo}'");
            
        if (!await EsNombreUnicoAsync(dto.Nombre))
            throw new InvalidOperationException($"Ya existe un título con el nombre '{dto.Nombre}'");
            
        if (dto.OrdenJerarquico > 0 && !await _repository.EstaOrdenDisponibleAsync(dto.OrdenJerarquico))
            throw new InvalidOperationException($"El orden jerárquico {dto.OrdenJerarquico} ya está en uso");
    }
    
    private async Task ValidarActualizacion(CrearActualizarTituloAcademicoDto dto, Guid id)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre))
            throw new ArgumentException("El nombre del título es requerido");
            
        if (string.IsNullOrWhiteSpace(dto.Codigo))
            throw new ArgumentException("El código del título es requerido");
            
        if (dto.OrdenJerarquico <= 0 || dto.OrdenJerarquico > 100)
            throw new ArgumentException("El orden jerárquico debe estar entre 1 y 100");
            
        if (!await EsCodigoUnicoAsync(dto.Codigo, id))
            throw new InvalidOperationException($"Ya existe un título con el código '{dto.Codigo}'");
            
        if (!await EsNombreUnicoAsync(dto.Nombre, id))
            throw new InvalidOperationException($"Ya existe un título con el nombre '{dto.Nombre}'");
            
        if (!await _repository.EstaOrdenDisponibleAsync(dto.OrdenJerarquico, id))
            throw new InvalidOperationException($"El orden jerárquico {dto.OrdenJerarquico} ya está en uso");
    }
    
    private static string ObtenerRepresentacionCompleta(TituloAcademicoDto dto)
    {
        var partes = new List<string> { dto.Nombre };
        
        if (!string.IsNullOrEmpty(dto.Descripcion))
            partes.Add($"({dto.Descripcion})");
            
        partes.Add($"[Orden: {dto.OrdenJerarquico}]");
        
        if (dto.EsTituloSistema)
            partes.Add("[Sistema]");
            
        return string.Join(" ", partes);
    }

    #endregion
}
