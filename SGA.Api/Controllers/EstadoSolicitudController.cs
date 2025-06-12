using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities;
using SGA.Infrastructure.Data;
using SGA.Application.DTOs;
using System.ComponentModel.DataAnnotations;

namespace SGA.Api.Controllers
{
    /// <summary>
    /// Controlador para la gestión de estados de solicitudes de ascenso
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class EstadoSolicitudController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<EstadoSolicitudController> _logger;

        public EstadoSolicitudController(AppDbContext context, ILogger<EstadoSolicitudController> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene todos los estados de solicitud
        /// </summary>
        /// <returns>Lista de estados de solicitud</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EstadoSolicitudResponseDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<EstadoSolicitudResponseDto>>> GetEstadosSolicitud()
        {
            try
            {
                var estados = await _context.EstadosSolicitud
                    .AsNoTracking()
                    .OrderBy(e => e.Orden)
                    .ToListAsync();

                var response = estados.Select(e => new EstadoSolicitudResponseDto
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    Codigo = e.Codigo,
                    Descripcion = e.Descripcion,
                    Color = e.Color,
                    Orden = e.Orden,
                    EsEstadoFinal = e.EsEstadoFinal,
                    RequiereRevision = e.RequiereRevision,
                    EsActivo = e.EsActivo,
                    FechaCreacion = e.FechaCreacion,
                    FechaActualizacion = e.FechaActualizacion,
                    // Propiedades de compatibilidad para controladores legacy
                    ColorHex = e.Color
                }).ToList();

                _logger.LogInformation("Se obtuvieron {Count} estados de solicitud", response.Count);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estados de solicitud");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene un estado de solicitud por ID
        /// </summary>
        /// <param name="id">ID del estado de solicitud</param>
        /// <returns>Estado de solicitud</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(EstadoSolicitudResponseDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<EstadoSolicitudResponseDto>> GetEstadoSolicitud(int id)
        {
            try
            {
                var estado = await _context.EstadosSolicitud
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (estado == null)
                {
                    _logger.LogWarning("No se encontró el estado de solicitud con ID {Id}", id);
                    return NotFound(new { message = $"No se encontró el estado de solicitud con ID {id}" });
                }

                var response = new EstadoSolicitudResponseDto
                {
                    Id = estado.Id,
                    Nombre = estado.Nombre,
                    Codigo = estado.Codigo,
                    Descripcion = estado.Descripcion,
                    Color = estado.Color,
                    Orden = estado.Orden,
                    EsEstadoFinal = estado.EsEstadoFinal,
                    RequiereRevision = estado.RequiereRevision,
                    EsActivo = estado.EsActivo,
                    FechaCreacion = estado.FechaCreacion,
                    FechaActualizacion = estado.FechaActualizacion,
                    // Propiedades de compatibilidad para controladores legacy
                    ColorHex = estado.Color
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el estado de solicitud con ID {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Crea un nuevo estado de solicitud
        /// </summary>
        /// <param name="createDto">Datos del estado de solicitud a crear</param>
        /// <returns>Estado de solicitud creado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(EstadoSolicitudResponseDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<EstadoSolicitudResponseDto>> CreateEstadoSolicitud([FromBody] CreateEstadoSolicitudDto createDto)
        {
            try
            {
                // Validar que no exista un estado con el mismo código
                var existeEstado = await _context.EstadosSolicitud
                    .AnyAsync(e => e.Codigo == createDto.Codigo);

                if (existeEstado)
                {
                    return BadRequest(new { message = $"Ya existe un estado con el código '{createDto.Codigo}'" });
                }

                // Validar que no exista un estado con el mismo orden
                var existeOrden = await _context.EstadosSolicitud
                    .AnyAsync(e => e.Orden == createDto.Orden);

                if (existeOrden)
                {
                    return BadRequest(new { message = $"Ya existe un estado con el orden {createDto.Orden}" });
                }

                var estadoSolicitud = new EstadoSolicitud
                {
                    Codigo = createDto.Codigo.Trim().ToUpper(),
                    Nombre = createDto.Nombre.Trim(),
                    Descripcion = createDto.Descripcion?.Trim(),
                    Color = createDto.Color?.Trim(),
                    Orden = createDto.Orden,
                    EsEstadoFinal = createDto.EsEstadoFinal,
                    RequiereRevision = createDto.RequiereRevision,
                    EsActivo = true,
                    FechaCreacion = DateTime.UtcNow
                };

                _context.EstadosSolicitud.Add(estadoSolicitud);
                await _context.SaveChangesAsync();

                var response = new EstadoSolicitudResponseDto
                {
                    Id = estadoSolicitud.Id,
                    Nombre = estadoSolicitud.Nombre,
                    Codigo = estadoSolicitud.Codigo,
                    Descripcion = estadoSolicitud.Descripcion,
                    Color = estadoSolicitud.Color,
                    Orden = estadoSolicitud.Orden,
                    EsEstadoFinal = estadoSolicitud.EsEstadoFinal,
                    RequiereRevision = estadoSolicitud.RequiereRevision,
                    EsActivo = estadoSolicitud.EsActivo,
                    FechaCreacion = estadoSolicitud.FechaCreacion,
                    FechaActualizacion = estadoSolicitud.FechaActualizacion,
                    ColorHex = estadoSolicitud.Color
                };

                _logger.LogInformation("Se creó el estado de solicitud '{Nombre}' con ID {Id}", estadoSolicitud.Nombre, estadoSolicitud.Id);
                return CreatedAtAction(nameof(GetEstadoSolicitud), new { id = estadoSolicitud.Id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el estado de solicitud");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Actualiza un estado de solicitud existente
        /// </summary>
        /// <param name="id">ID del estado de solicitud</param>
        /// <param name="updateDto">Datos actualizados del estado de solicitud</param>
        /// <returns>Estado de solicitud actualizado</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(EstadoSolicitudResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<EstadoSolicitudResponseDto>> UpdateEstadoSolicitud(int id, [FromBody] UpdateEstadoSolicitudDto updateDto)
        {
            try
            {
                var estadoSolicitud = await _context.EstadosSolicitud
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (estadoSolicitud == null)
                {
                    return NotFound(new { message = $"No se encontró el estado de solicitud con ID {id}" });
                }

                // Validar que no exista otro estado con el mismo código
                if (!string.IsNullOrEmpty(updateDto.Codigo))
                {
                    var existeNombre = await _context.EstadosSolicitud
                        .AnyAsync(e => e.Codigo == updateDto.Codigo && e.Id != id);

                    if (existeNombre)
                    {
                        return BadRequest(new { message = $"Ya existe otro estado con el código '{updateDto.Codigo}'" });
                    }
                }

                // Validar que no exista otro estado con el mismo orden
                var existeOrden = await _context.EstadosSolicitud
                    .AnyAsync(e => e.Orden == updateDto.Orden && e.Id != id);

                if (existeOrden)
                {
                    return BadRequest(new { message = $"Ya existe otro estado con el orden {updateDto.Orden}" });
                }

                // Actualizar propiedades
                if (!string.IsNullOrEmpty(updateDto.Codigo))
                    estadoSolicitud.Codigo = updateDto.Codigo.Trim().ToUpper();
                
                if (!string.IsNullOrEmpty(updateDto.Nombre))
                    estadoSolicitud.Nombre = updateDto.Nombre.Trim();
                
                estadoSolicitud.Descripcion = updateDto.Descripcion?.Trim();
                estadoSolicitud.Color = updateDto.Color?.Trim();
                estadoSolicitud.Orden = updateDto.Orden;
                estadoSolicitud.EsEstadoFinal = updateDto.EsEstadoFinal;
                estadoSolicitud.RequiereRevision = updateDto.RequiereRevision;
                estadoSolicitud.EsActivo = updateDto.EsActivo;
                estadoSolicitud.FechaActualizacion = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var response = new EstadoSolicitudResponseDto
                {
                    Id = estadoSolicitud.Id,
                    Nombre = estadoSolicitud.Nombre,
                    Codigo = estadoSolicitud.Codigo,
                    Descripcion = estadoSolicitud.Descripcion,
                    Color = estadoSolicitud.Color,
                    Orden = estadoSolicitud.Orden,
                    EsEstadoFinal = estadoSolicitud.EsEstadoFinal,
                    RequiereRevision = estadoSolicitud.RequiereRevision,
                    EsActivo = estadoSolicitud.EsActivo,
                    FechaCreacion = estadoSolicitud.FechaCreacion,
                    FechaActualizacion = estadoSolicitud.FechaActualizacion,
                    ColorHex = estadoSolicitud.Color
                };

                _logger.LogInformation("Se actualizó el estado de solicitud con ID {Id}", id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el estado de solicitud con ID {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Elimina (desactiva) un estado de solicitud
        /// </summary>
        /// <param name="id">ID del estado de solicitud</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> DeleteEstadoSolicitud(int id)
        {
            try
            {
                var estadoSolicitud = await _context.EstadosSolicitud
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (estadoSolicitud == null)
                {
                    return NotFound(new { message = $"No se encontró el estado de solicitud con ID {id}" });
                }

                // Verificar si el estado está siendo usado por alguna solicitud
                var esUsado = await _context.SolicitudesAscenso
                    .AnyAsync(s => s.EstadoSolicitudId == id);

                if (esUsado)
                {
                    return BadRequest(new { message = "No se puede eliminar el estado porque está siendo usado por solicitudes existentes" });
                }

                // En lugar de eliminar físicamente, desactivamos el estado
                estadoSolicitud.EsActivo = false;
                estadoSolicitud.FechaActualizacion = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Se eliminó (desactivó) el estado de solicitud con ID {Id}", id);
                return Ok(new { message = "Estado de solicitud eliminado correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el estado de solicitud con ID {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene solo los estados de solicitud activos
        /// </summary>
        /// <returns>Lista de estados de solicitud activos</returns>
        [HttpGet("activos")]
        [ProducesResponseType(typeof(IEnumerable<EstadoSolicitudResponseDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<EstadoSolicitudResponseDto>>> GetEstadosSolicitudActivos()
        {
            try
            {
                var estadosActivos = await _context.EstadosSolicitud
                    .Where(e => e.EsActivo)
                    .AsNoTracking()
                    .OrderBy(e => e.Orden)
                    .ToListAsync();

                var response = estadosActivos.Select(e => new EstadoSolicitudResponseDto
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    Codigo = e.Codigo,
                    Descripcion = e.Descripcion,
                    Color = e.Color,
                    Orden = e.Orden,
                    EsEstadoFinal = e.EsEstadoFinal,
                    RequiereRevision = e.RequiereRevision,
                    EsActivo = e.EsActivo,
                    FechaCreacion = e.FechaCreacion,
                    FechaActualizacion = e.FechaActualizacion,
                    ColorHex = e.Color
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estados de solicitud activos");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene el workflow de estados (orden de transición)
        /// </summary>
        /// <returns>Lista ordenada de estados para el workflow</returns>
        [HttpGet("workflow")]
        [ProducesResponseType(typeof(IEnumerable<EstadoSolicitudResponseDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<EstadoSolicitudResponseDto>>> GetWorkflowEstados()
        {
            try
            {
                var estados = await _context.EstadosSolicitud
                    .Where(e => e.EsActivo)
                    .AsNoTracking()
                    .OrderBy(e => e.Orden)
                    .ToListAsync();

                var response = estados.Select(e => new EstadoSolicitudResponseDto
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    Codigo = e.Codigo,
                    Descripcion = e.Descripcion,
                    Color = e.Color,
                    Orden = e.Orden,
                    EsEstadoFinal = e.EsEstadoFinal,
                    RequiereRevision = e.RequiereRevision,
                    EsActivo = e.EsActivo,
                    FechaCreacion = e.FechaCreacion,
                    FechaActualizacion = e.FechaActualizacion,
                    ColorHex = e.Color
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el workflow de estados");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene los estados siguientes válidos para una transición desde un estado dado
        /// </summary>
        /// <param name="estadoActualId">ID del estado actual</param>
        /// <returns>Lista de estados válidos para la transición</returns>
        [HttpGet("siguientes/{estadoActualId:int}")]
        [ProducesResponseType(typeof(IEnumerable<EstadoSolicitudResponseDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<EstadoSolicitudResponseDto>>> GetEstadosSiguientes(int estadoActualId)
        {
            try
            {
                var estadoActual = await _context.EstadosSolicitud
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id == estadoActualId);

                if (estadoActual == null)
                {
                    return NotFound(new { message = $"No se encontró el estado con ID {estadoActualId}" });
                }

                // Para simplicidad, devolvemos el siguiente estado en orden
                // En una implementación más compleja, esto podría consultar una tabla de transiciones
                var siguientesEstados = await _context.EstadosSolicitud
                    .Where(e => e.EsActivo && e.Orden > estadoActual.Orden && !e.EsEstadoFinal)
                    .AsNoTracking()
                    .OrderBy(e => e.Orden)
                    .ToListAsync();

                var response = siguientesEstados.Select(e => new EstadoSolicitudResponseDto
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    Codigo = e.Codigo,
                    Descripcion = e.Descripcion,
                    Color = e.Color,
                    Orden = e.Orden,
                    EsEstadoFinal = e.EsEstadoFinal,
                    RequiereRevision = e.RequiereRevision,
                    EsActivo = e.EsActivo,
                    FechaCreacion = e.FechaCreacion,
                    FechaActualizacion = e.FechaActualizacion,
                    ColorHex = e.Color
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estados siguientes para el estado {EstadoId}", estadoActualId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene estadísticas de estados de solicitud
        /// </summary>
        /// <returns>Estadísticas de estados</returns>
        [HttpGet("estadisticas")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> GetEstadisticasEstados()
        {
            try
            {
                var estadisticas = new
                {
                    TotalEstados = await _context.EstadosSolicitud.CountAsync(),
                    EstadosActivos = await _context.EstadosSolicitud.CountAsync(e => e.EsActivo),
                    EstadosInactivos = await _context.EstadosSolicitud.CountAsync(e => !e.EsActivo),
                    EstadosFinales = await _context.EstadosSolicitud.CountAsync(e => e.EsEstadoFinal),
                    EstadosConRevision = await _context.EstadosSolicitud.CountAsync(e => e.RequiereRevision),
                    SolicitudesPorEstado = await _context.EstadosSolicitud
                        .Where(e => e.EsActivo)
                        .Select(e => new
                        {
                            EstadoId = e.Id,
                            EstadoNombre = e.Nombre,
                            CantidadSolicitudes = e.SolicitudesAscenso.Count()
                        })
                        .ToListAsync()
                };

                return Ok(estadisticas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas de estados de solicitud");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}