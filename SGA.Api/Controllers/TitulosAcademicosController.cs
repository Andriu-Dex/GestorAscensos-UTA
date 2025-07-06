using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGA.Application.DTOs.Admin;
using SGA.Application.Interfaces;
using System.Security.Claims;

namespace SGA.Api.Controllers;

/// <summary>
/// Controlador para administración de títulos académicos dinámicos
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class TitulosAcademicosController : ControllerBase
{
    private readonly ITituloAcademicoService _tituloService;
    private readonly ILogger<TitulosAcademicosController> _logger;

    public TitulosAcademicosController(
        ITituloAcademicoService tituloService,
        ILogger<TitulosAcademicosController> logger)
    {
        _tituloService = tituloService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los títulos académicos
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<TituloAcademicoDto>>> GetAll()
    {
        try
        {
            var titulos = await _tituloService.GetAllAsync();
            return Ok(titulos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener títulos académicos");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene solo los títulos académicos activos
    /// </summary>
    [HttpGet("activos")]
    public async Task<ActionResult<List<TituloAcademicoDto>>> GetActivos()
    {
        try
        {
            var titulos = await _tituloService.GetActivosAsync();
            return Ok(titulos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener títulos académicos activos");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene resumen de títulos para administración
    /// </summary>
    [HttpGet("resumen")]
    public async Task<ActionResult<List<TituloAcademicoResumenDto>>> GetResumen()
    {
        try
        {
            var resumen = await _tituloService.GetResumenAsync();
            return Ok(resumen);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener resumen de títulos académicos");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene título académico por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TituloAcademicoDto>> GetById(Guid id)
    {
        try
        {
            var titulo = await _tituloService.GetByIdAsync(id);
            if (titulo == null)
            {
                return NotFound(new { message = "Título académico no encontrado" });
            }

            return Ok(titulo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener título académico {Id}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene título académico por código
    /// </summary>
    [HttpGet("codigo/{codigo}")]
    public async Task<ActionResult<TituloAcademicoDto>> GetByCodigo(string codigo)
    {
        try
        {
            var titulo = await _tituloService.GetByCodigoAsync(codigo);
            if (titulo == null)
            {
                return NotFound(new { message = $"Título académico con código '{codigo}' no encontrado" });
            }

            return Ok(titulo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener título académico por código {Codigo}", codigo);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Crea un nuevo título académico
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TituloAcademicoDto>> Create([FromBody] CrearActualizarTituloAcademicoDto dto)
    {
        try
        {
            var usuarioEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "Sistema";
            var titulo = await _tituloService.CreateAsync(dto, usuarioEmail);
            
            return CreatedAtAction(nameof(GetById), new { id = titulo.Id }, titulo);
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
            _logger.LogError(ex, "Error al crear título académico");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Actualiza un título académico existente
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TituloAcademicoDto>> Update(Guid id, [FromBody] CrearActualizarTituloAcademicoDto dto)
    {
        try
        {
            var usuarioEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "Sistema";
            var titulo = await _tituloService.UpdateAsync(id, dto, usuarioEmail);
            
            return Ok(titulo);
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
            _logger.LogError(ex, "Error al actualizar título académico {Id}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Elimina un título académico
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var usuarioEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "Sistema";
            var eliminado = await _tituloService.DeleteAsync(id, usuarioEmail);
            
            if (!eliminado)
            {
                return NotFound(new { message = "Título académico no encontrado" });
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar título académico {Id}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Activa o desactiva un título académico
    /// </summary>
    [HttpPatch("{id:guid}/toggle-activo")]
    public async Task<ActionResult<bool>> ToggleActivo(Guid id)
    {
        try
        {
            var usuarioEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "Sistema";
            var nuevoEstado = await _tituloService.ToggleActivoAsync(id, usuarioEmail);
            
            return Ok(new { estaActivo = nuevoEstado });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar estado de título académico {Id}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene títulos como opciones de selección
    /// </summary>
    [HttpGet("opciones")]
    public async Task<ActionResult<List<TituloAcademicoOpcionDto>>> GetOpciones([FromQuery] bool soloActivos = true)
    {
        try
        {
            var opciones = await _tituloService.GetOpcionesAsync(soloActivos);
            return Ok(opciones);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener opciones de títulos académicos");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene niveles híbridos (enum + títulos dinámicos) para formularios
    /// </summary>
    [HttpGet("niveles-hibridos")]
    public async Task<ActionResult<List<NivelAcademicoHibridoDto>>> GetNivelesHibridos()
    {
        try
        {
            var niveles = await _tituloService.GetNivelesHibridosAsync();
            return Ok(niveles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener niveles híbridos");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene posibles ascensos para un título dado
    /// </summary>
    [HttpGet("{id:guid}/posibles-ascensos")]
    public async Task<ActionResult<List<TituloAcademicoOpcionDto>>> GetPosiblesAscensos(Guid id)
    {
        try
        {
            var ascensos = await _tituloService.GetPosiblesAscensosAsync(id);
            return Ok(ascensos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener posibles ascensos para título {Id}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Verifica si un título puede ser eliminado
    /// </summary>
    [HttpGet("{id:guid}/puede-eliminar")]
    public async Task<ActionResult<bool>> PuedeSerEliminado(Guid id)
    {
        try
        {
            var puedeEliminar = await _tituloService.PuedeSerEliminadoAsync(id);
            return Ok(new { puedeEliminar });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar si el título {Id} puede ser eliminado", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Inicializa títulos académicos por defecto equivalentes al enum
    /// </summary>
    [HttpPost("inicializar-defecto")]
    public async Task<IActionResult> InicializarTitulosPorDefecto()
    {
        try
        {
            var usuarioEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "Sistema";
            await _tituloService.InicializarTitulosPorDefectoAsync(usuarioEmail);
            
            return Ok(new { message = "Títulos académicos por defecto inicializados correctamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al inicializar títulos académicos por defecto");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Valida si un código es único
    /// </summary>
    [HttpGet("validar-codigo/{codigo}")]
    public async Task<ActionResult<bool>> ValidarCodigo(string codigo, [FromQuery] Guid? excluirId = null)
    {
        try
        {
            var esUnico = await _tituloService.EsCodigoUnicoAsync(codigo, excluirId);
            return Ok(new { esUnico });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar código {Codigo}", codigo);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Valida si un nombre es único
    /// </summary>
    [HttpGet("validar-nombre/{nombre}")]
    public async Task<ActionResult<bool>> ValidarNombre(string nombre, [FromQuery] Guid? excluirId = null)
    {
        try
        {
            var esUnico = await _tituloService.EsNombreUnicoAsync(nombre, excluirId);
            return Ok(new { esUnico });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar nombre {Nombre}", nombre);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene títulos para la aplicación frontend con formato específico
    /// </summary>
    [HttpGet("frontend")]
    public async Task<ActionResult<List<object>>> GetForFrontend([FromQuery] bool soloActivos = false)
    {
        try
        {
            var titulos = soloActivos 
                ? await _tituloService.GetActivosAsync()
                : await _tituloService.GetAllAsync();
            
            // Mapear a formato que espera el frontend
            var titulosFrontend = titulos.Select(t => new
            {
                t.Id,
                t.Nombre,
                t.Codigo,
                t.OrdenJerarquico,
                t.EsTituloSistema,
                t.EstaActivo,
                t.ColorHex,
                t.Descripcion
            }).ToList();
            
            return Ok(titulosFrontend);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener títulos para frontend");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }
}
