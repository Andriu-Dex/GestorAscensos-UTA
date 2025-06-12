using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities;
using SGA.Infrastructure.Data;
using SGA.Application.DTOs;
using System.ComponentModel.DataAnnotations;

namespace SGA.Api.Controllers
{
    /// <summary>
    /// Controlador para la gestión de tipos de documentos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TipoDocumentoController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TipoDocumentoController> _logger;

        public TipoDocumentoController(AppDbContext context, ILogger<TipoDocumentoController> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene todos los tipos de documentos
        /// </summary>
        /// <returns>Lista de tipos de documentos</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TipoDocumentoResponseDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<TipoDocumentoResponseDto>>> GetTiposDocumento()
        {
            try
            {
                var tiposDocumento = await _context.TiposDocumento
                    .AsNoTracking()
                    .ToListAsync();

                var response = tiposDocumento.Select(td => new TipoDocumentoResponseDto
                {
                    Id = td.Id,
                    Nombre = td.Nombre,
                    Descripcion = td.Descripcion,
                    RequiereValidacion = td.RequiereValidacion,
                    FormatoEsperado = td.FormatoEsperado,
                    TamanoMaximoMB = td.TamanoMaximoMB,
                    EsActivo = td.EsActivo,
                    FechaCreacion = td.FechaCreacion,
                    FechaActualizacion = td.FechaActualizacion,
                    // Propiedades de compatibilidad para controladores legacy
                    EsObligatorio = td.RequiereValidacion,
                    ValidacionRequerida = td.RequiereValidacion,
                    FormatosSoportados = td.FormatoEsperado,
                    TamanioMaximoMB = td.TamanoMaximoMB
                }).ToList();

                _logger.LogInformation("Se obtuvieron {Count} tipos de documento", response.Count);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tipos de documento");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene un tipo de documento por ID
        /// </summary>
        /// <param name="id">ID del tipo de documento</param>
        /// <returns>Tipo de documento</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TipoDocumentoResponseDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<TipoDocumentoResponseDto>> GetTipoDocumento(int id)
        {
            try
            {
                var tipoDocumento = await _context.TiposDocumento
                    .AsNoTracking()
                    .FirstOrDefaultAsync(td => td.Id == id);

                if (tipoDocumento == null)
                {
                    _logger.LogWarning("No se encontró el tipo de documento con ID {Id}", id);
                    return NotFound(new { message = $"No se encontró el tipo de documento con ID {id}" });
                }

                var response = new TipoDocumentoResponseDto
                {
                    Id = tipoDocumento.Id,
                    Nombre = tipoDocumento.Nombre,
                    Descripcion = tipoDocumento.Descripcion,
                    RequiereValidacion = tipoDocumento.RequiereValidacion,
                    FormatoEsperado = tipoDocumento.FormatoEsperado,
                    TamanoMaximoMB = tipoDocumento.TamanoMaximoMB,
                    EsActivo = tipoDocumento.EsActivo,
                    FechaCreacion = tipoDocumento.FechaCreacion,
                    FechaActualizacion = tipoDocumento.FechaActualizacion,
                    // Propiedades de compatibilidad
                    EsObligatorio = tipoDocumento.RequiereValidacion,
                    ValidacionRequerida = tipoDocumento.RequiereValidacion,
                    FormatosSoportados = tipoDocumento.FormatoEsperado,
                    TamanioMaximoMB = tipoDocumento.TamanoMaximoMB
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el tipo de documento con ID {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Crea un nuevo tipo de documento
        /// </summary>
        /// <param name="createDto">Datos del tipo de documento a crear</param>
        /// <returns>Tipo de documento creado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(TipoDocumentoResponseDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<TipoDocumentoResponseDto>> CreateTipoDocumento([FromBody] CreateTipoDocumentoDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Verificar si ya existe un tipo de documento con el mismo nombre
                var existeTipoDocumento = await _context.TiposDocumento
                    .AnyAsync(td => td.Nombre.ToLower() == createDto.Nombre.ToLower());

                if (existeTipoDocumento)
                {
                    return Conflict(new { message = $"Ya existe un tipo de documento con el nombre '{createDto.Nombre}'" });
                }                var tipoDocumento = new TipoDocumento
                {
                    Codigo = createDto.Codigo?.Trim() ?? createDto.Nombre.Trim().ToUpper().Replace(" ", "_"),
                    Nombre = createDto.Nombre.Trim(),
                    Descripcion = createDto.Descripcion?.Trim(),
                    RequiereValidacion = createDto.RequiereValidacion,
                    FormatoEsperado = createDto.FormatoEsperado?.Trim(),
                    TamanoMaximoMB = createDto.TamanoMaximoMB,
                    EsActivo = createDto.EsActivo,
                    FechaCreacion = DateTime.UtcNow
                };

                _context.TiposDocumento.Add(tipoDocumento);
                await _context.SaveChangesAsync();

                var response = new TipoDocumentoResponseDto
                {
                    Id = tipoDocumento.Id,
                    Nombre = tipoDocumento.Nombre,
                    Descripcion = tipoDocumento.Descripcion,
                    RequiereValidacion = tipoDocumento.RequiereValidacion,
                    FormatoEsperado = tipoDocumento.FormatoEsperado,
                    TamanoMaximoMB = tipoDocumento.TamanoMaximoMB,
                    EsActivo = tipoDocumento.EsActivo,
                    FechaCreacion = tipoDocumento.FechaCreacion,
                    FechaActualizacion = tipoDocumento.FechaActualizacion,
                    // Propiedades de compatibilidad
                    EsObligatorio = tipoDocumento.RequiereValidacion,
                    ValidacionRequerida = tipoDocumento.RequiereValidacion,
                    FormatosSoportados = tipoDocumento.FormatoEsperado,
                    TamanioMaximoMB = tipoDocumento.TamanoMaximoMB
                };

                _logger.LogInformation("Se creó el tipo de documento '{Nombre}' con ID {Id}", tipoDocumento.Nombre, tipoDocumento.Id);
                return CreatedAtAction(nameof(GetTipoDocumento), new { id = tipoDocumento.Id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el tipo de documento");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Actualiza un tipo de documento existente
        /// </summary>
        /// <param name="id">ID del tipo de documento</param>
        /// <param name="updateDto">Datos actualizados del tipo de documento</param>
        /// <returns>Tipo de documento actualizado</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TipoDocumentoResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<TipoDocumentoResponseDto>> UpdateTipoDocumento(int id, [FromBody] UpdateTipoDocumentoDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var tipoDocumento = await _context.TiposDocumento
                    .FirstOrDefaultAsync(td => td.Id == id);

                if (tipoDocumento == null)
                {
                    return NotFound(new { message = $"No se encontró el tipo de documento con ID {id}" });
                }

                // Verificar si el nuevo nombre ya existe en otro registro
                if (!string.IsNullOrWhiteSpace(updateDto.Nombre) && 
                    updateDto.Nombre.ToLower() != tipoDocumento.Nombre.ToLower())
                {
                    var existeNombre = await _context.TiposDocumento
                        .AnyAsync(td => td.Id != id && td.Nombre.ToLower() == updateDto.Nombre.ToLower());

                    if (existeNombre)
                    {
                        return Conflict(new { message = $"Ya existe otro tipo de documento con el nombre '{updateDto.Nombre}'" });
                    }
                }

                // Actualizar propiedades
                if (!string.IsNullOrWhiteSpace(updateDto.Nombre))
                    tipoDocumento.Nombre = updateDto.Nombre.Trim();

                if (updateDto.Descripcion != null)
                    tipoDocumento.Descripcion = updateDto.Descripcion.Trim();

                tipoDocumento.RequiereValidacion = updateDto.RequiereValidacion;

                if (updateDto.FormatoEsperado != null)
                    tipoDocumento.FormatoEsperado = updateDto.FormatoEsperado.Trim();

                if (updateDto.TamanoMaximoMB.HasValue)
                    tipoDocumento.TamanoMaximoMB = updateDto.TamanoMaximoMB.Value;

                tipoDocumento.EsActivo = updateDto.EsActivo;
                tipoDocumento.FechaActualizacion = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var response = new TipoDocumentoResponseDto
                {
                    Id = tipoDocumento.Id,
                    Nombre = tipoDocumento.Nombre,
                    Descripcion = tipoDocumento.Descripcion,
                    RequiereValidacion = tipoDocumento.RequiereValidacion,
                    FormatoEsperado = tipoDocumento.FormatoEsperado,
                    TamanoMaximoMB = tipoDocumento.TamanoMaximoMB,
                    EsActivo = tipoDocumento.EsActivo,
                    FechaCreacion = tipoDocumento.FechaCreacion,
                    FechaActualizacion = tipoDocumento.FechaActualizacion,
                    // Propiedades de compatibilidad
                    EsObligatorio = tipoDocumento.RequiereValidacion,
                    ValidacionRequerida = tipoDocumento.RequiereValidacion,
                    FormatosSoportados = tipoDocumento.FormatoEsperado,
                    TamanioMaximoMB = tipoDocumento.TamanoMaximoMB
                };

                _logger.LogInformation("Se actualizó el tipo de documento con ID {Id}", id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el tipo de documento con ID {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Elimina un tipo de documento (soft delete)
        /// </summary>
        /// <param name="id">ID del tipo de documento</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteTipoDocumento(int id)
        {
            try
            {
                var tipoDocumento = await _context.TiposDocumento
                    .FirstOrDefaultAsync(td => td.Id == id);

                if (tipoDocumento == null)
                {
                    return NotFound(new { message = $"No se encontró el tipo de documento con ID {id}" });
                }

                // Verificar si el tipo de documento está siendo usado
                var esUsado = await _context.Documentos
                    .AnyAsync(d => d.TipoDocumentoId == id);

                if (esUsado)
                {
                    return Conflict(new { message = "No se puede eliminar el tipo de documento porque está siendo utilizado en documentos existentes" });
                }

                // Soft delete
                tipoDocumento.EsActivo = false;
                tipoDocumento.FechaActualizacion = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Se eliminó (desactivó) el tipo de documento con ID {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el tipo de documento con ID {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene tipos de documentos activos
        /// </summary>
        /// <returns>Lista de tipos de documentos activos</returns>
        [HttpGet("activos")]
        [ProducesResponseType(typeof(IEnumerable<TipoDocumentoResponseDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<TipoDocumentoResponseDto>>> GetTiposDocumentoActivos()
        {
            try
            {
                var tiposDocumentoActivos = await _context.TiposDocumento
                    .Where(td => td.EsActivo)
                    .AsNoTracking()
                    .ToListAsync();

                var response = tiposDocumentoActivos.Select(td => new TipoDocumentoResponseDto
                {
                    Id = td.Id,
                    Nombre = td.Nombre,
                    Descripcion = td.Descripcion,
                    RequiereValidacion = td.RequiereValidacion,
                    FormatoEsperado = td.FormatoEsperado,
                    TamanoMaximoMB = td.TamanoMaximoMB,
                    EsActivo = td.EsActivo,
                    FechaCreacion = td.FechaCreacion,
                    FechaActualizacion = td.FechaActualizacion,
                    // Propiedades de compatibilidad
                    EsObligatorio = td.RequiereValidacion,
                    ValidacionRequerida = td.RequiereValidacion,
                    FormatosSoportados = td.FormatoEsperado,
                    TamanioMaximoMB = td.TamanoMaximoMB
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tipos de documento activos");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene estadísticas de tipos de documentos
        /// </summary>
        /// <returns>Estadísticas de tipos de documentos</returns>
        [HttpGet("estadisticas")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> GetEstadisticas()
        {
            try
            {
                var estadisticas = new
                {
                    TotalTiposDocumento = await _context.TiposDocumento.CountAsync(),
                    TiposDocumentoActivos = await _context.TiposDocumento.CountAsync(td => td.EsActivo),
                    TiposDocumentoInactivos = await _context.TiposDocumento.CountAsync(td => !td.EsActivo),
                    TiposConValidacion = await _context.TiposDocumento.CountAsync(td => td.RequiereValidacion),
                    TiposSinValidacion = await _context.TiposDocumento.CountAsync(td => !td.RequiereValidacion)
                };

                return Ok(estadisticas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas de tipos de documento");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}