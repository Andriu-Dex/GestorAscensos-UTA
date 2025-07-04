using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGA.Application.DTOs.Admin;
using SGA.Application.Interfaces;
using SGA.Domain.Enums;
using System.Security.Claims;

namespace SGA.Api.Controllers;

/// <summary>
/// Controlador para administración de configuraciones de requisitos de ascenso
/// </summary>
[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "Administrador")]
public class ConfiguracionRequisitosController : ControllerBase
{
    private readonly IConfiguracionRequisitoService _configuracionService;
    private readonly ILogger<ConfiguracionRequisitosController> _logger;

    public ConfiguracionRequisitosController(
        IConfiguracionRequisitoService configuracionService,
        ILogger<ConfiguracionRequisitosController> logger)
    {
        _configuracionService = configuracionService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todas las configuraciones de requisitos
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<ConfiguracionRequisitoDto>>> GetAll()
    {
        try
        {
            var configuraciones = await _configuracionService.GetAllAsync();
            return Ok(configuraciones);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener configuraciones de requisitos");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene solo las configuraciones activas
    /// </summary>
    [HttpGet("activas")]
    public async Task<ActionResult<List<ConfiguracionRequisitoDto>>> GetActivas()
    {
        try
        {
            var configuraciones = await _configuracionService.GetActivasAsync();
            return Ok(configuraciones);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener configuraciones activas");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene resumen de configuraciones para administración
    /// </summary>
    [HttpGet("resumen")]
    public async Task<ActionResult<List<ConfiguracionRequisitoResumenDto>>> GetResumen()
    {
        try
        {
            var resumen = await _configuracionService.GetResumenAsync();
            return Ok(resumen);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener resumen de configuraciones");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene configuración por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ConfiguracionRequisitoDto>> GetById(Guid id)
    {
        try
        {
            var configuracion = await _configuracionService.GetByIdAsync(id);
            if (configuracion == null)
            {
                return NotFound(new { message = "Configuración no encontrada" });
            }

            return Ok(configuracion);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener configuración {Id}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene configuración para niveles específicos
    /// </summary>
    [HttpGet("niveles/{nivelActual}/{nivelSolicitado}")]
    public async Task<ActionResult<ConfiguracionRequisitoDto>> GetByNiveles(NivelTitular nivelActual, NivelTitular nivelSolicitado)
    {
        try
        {
            var configuracion = await _configuracionService.GetByNivelesAsync(nivelActual, nivelSolicitado);
            if (configuracion == null)
            {
                return NotFound(new { message = $"No se encontró configuración para ascenso de {nivelActual} a {nivelSolicitado}" });
            }

            return Ok(configuracion);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener configuración para niveles {NivelActual}-{NivelSolicitado}", 
                nivelActual, nivelSolicitado);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Crea una nueva configuración de requisitos
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ConfiguracionRequisitoDto>> Create([FromBody] CrearActualizarConfiguracionRequisitoDto dto)
    {
        try
        {
            var usuarioEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "Sistema";
            var configuracion = await _configuracionService.CreateAsync(dto, usuarioEmail);
            
            return CreatedAtAction(nameof(GetById), new { id = configuracion.Id }, configuracion);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear configuración de requisitos");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Actualiza una configuración existente
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ConfiguracionRequisitoDto>> Update(Guid id, [FromBody] CrearActualizarConfiguracionRequisitoDto dto)
    {
        try
        {
            var usuarioEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "Sistema";
            var configuracion = await _configuracionService.UpdateAsync(id, dto, usuarioEmail);
            
            return Ok(configuracion);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar configuración {Id}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Elimina una configuración
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var usuarioEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "Sistema";
            var eliminada = await _configuracionService.DeleteAsync(id, usuarioEmail);
            
            if (!eliminada)
            {
                return NotFound(new { message = "Configuración no encontrada" });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar configuración {Id}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Activa o desactiva una configuración
    /// </summary>
    [HttpPatch("{id:guid}/toggle-activo")]
    public async Task<ActionResult<bool>> ToggleActivo(Guid id)
    {
        try
        {
            var usuarioEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "Sistema";
            var nuevoEstado = await _configuracionService.ToggleActivoAsync(id, usuarioEmail);
            
            return Ok(new { estaActivo = nuevoEstado });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar estado de configuración {Id}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Valida todas las configuraciones del sistema
    /// </summary>
    [HttpGet("validar")]
    public async Task<ActionResult<ValidacionConfiguracionesDto>> ValidarConfiguraciones()
    {
        try
        {
            var validacion = await _configuracionService.ValidarConfiguracionesAsync();
            return Ok(validacion);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar configuraciones");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Inicializa configuraciones por defecto
    /// </summary>
    [HttpPost("inicializar-defecto")]
    public async Task<IActionResult> InicializarConfiguracionesPorDefecto()
    {
        try
        {
            var usuarioEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "Sistema";
            await _configuracionService.InicializarConfiguracionesPorDefectoAsync(usuarioEmail);
            
            return Ok(new { message = "Configuraciones por defecto inicializadas correctamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al inicializar configuraciones por defecto");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Exporta configuraciones a JSON
    /// </summary>
    [HttpGet("exportar")]
    public async Task<IActionResult> ExportarConfiguraciones()
    {
        try
        {
            var json = await _configuracionService.ExportarConfiguracionesAsync();
            var fileName = $"configuraciones_requisitos_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json";
            
            return File(System.Text.Encoding.UTF8.GetBytes(json), "application/json", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al exportar configuraciones");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Importa configuraciones desde JSON
    /// </summary>
    [HttpPost("importar")]
    public async Task<IActionResult> ImportarConfiguraciones([FromBody] string jsonConfiguraciones)
    {
        try
        {
            var usuarioEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "Sistema";
            var resultado = await _configuracionService.ImportarConfiguracionesAsync(jsonConfiguraciones, usuarioEmail);
            
            if (resultado)
            {
                return Ok(new { message = "Configuraciones importadas correctamente" });
            }
            else
            {
                return BadRequest(new { message = "Error al importar configuraciones" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al importar configuraciones");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene historial de cambios en configuraciones
    /// </summary>
    [HttpGet("historial")]
    public async Task<ActionResult<List<HistorialConfiguracionDto>>> GetHistorialCambios([FromQuery] int limite = 50)
    {
        try
        {
            var historial = await _configuracionService.GetHistorialCambiosAsync(limite);
            return Ok(historial);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener historial de cambios");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }
}
