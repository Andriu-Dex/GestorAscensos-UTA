using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities;
using SGA.Infrastructure.Data;
using SGA.Application.DTOs;
using System.ComponentModel.DataAnnotations;

namespace SGA.Api.Controllers
{
    /// <summary>
    /// Controlador para la gestión de facultades
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class FacultadController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<FacultadController> _logger;

        public FacultadController(AppDbContext context, ILogger<FacultadController> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene todas las facultades
        /// </summary>
        /// <returns>Lista de facultades</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FacultadResponseDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<FacultadResponseDto>>> GetFacultades()
        {
            try
            {
                var facultades = await _context.Facultades
                    .AsNoTracking()
                    .ToListAsync();

                var response = facultades.Select(f => new FacultadResponseDto
                {
                    Id = f.Id,
                    Nombre = f.Nombre,
                    Codigo = f.Codigo,
                    Descripcion = f.Descripcion,
                    EsActiva = f.EsActiva,
                    Color = f.Color,
                    FechaCreacion = f.FechaCreacion,
                    FechaActualizacion = f.FechaActualizacion,
                    // Propiedad de compatibilidad para controladores legacy
                    Activa = f.EsActiva
                }).ToList();

                _logger.LogInformation("Se obtuvieron {Count} facultades", response.Count);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener facultades");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene una facultad por ID
        /// </summary>
        /// <param name="id">ID de la facultad</param>
        /// <returns>Facultad</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FacultadResponseDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<FacultadResponseDto>> GetFacultad(int id)
        {
            try
            {
                var facultad = await _context.Facultades
                    .AsNoTracking()
                    .FirstOrDefaultAsync(f => f.Id == id);

                if (facultad == null)
                {
                    _logger.LogWarning("No se encontró la facultad con ID {Id}", id);
                    return NotFound(new { message = $"No se encontró la facultad con ID {id}" });
                }

                var response = new FacultadResponseDto
                {
                    Id = facultad.Id,
                    Nombre = facultad.Nombre,
                    Codigo = facultad.Codigo,
                    Descripcion = facultad.Descripcion,
                    EsActiva = facultad.EsActiva,
                    Color = facultad.Color,
                    FechaCreacion = facultad.FechaCreacion,
                    FechaActualizacion = facultad.FechaActualizacion,
                    // Propiedad de compatibilidad
                    Activa = facultad.EsActiva
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la facultad con ID {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Crea una nueva facultad
        /// </summary>
        /// <param name="createDto">Datos de la facultad a crear</param>
        /// <returns>Facultad creada</returns>
        [HttpPost]
        [ProducesResponseType(typeof(FacultadResponseDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<FacultadResponseDto>> CreateFacultad([FromBody] CreateFacultadDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Verificar si ya existe una facultad con el mismo nombre o código
                var existeFacultad = await _context.Facultades
                    .AnyAsync(f => f.Nombre.ToLower() == createDto.Nombre.ToLower() || 
                                  f.Codigo.ToLower() == createDto.Codigo.ToLower());

                if (existeFacultad)
                {
                    return Conflict(new { message = $"Ya existe una facultad con el nombre '{createDto.Nombre}' o código '{createDto.Codigo}'" });
                }

                var facultad = new Facultad
                {
                    Nombre = createDto.Nombre.Trim(),
                    Codigo = createDto.Codigo.Trim().ToUpper(),
                    Descripcion = createDto.Descripcion?.Trim(),
                    EsActiva = createDto.EsActiva,
                    Color = createDto.Color?.Trim(),
                    FechaCreacion = DateTime.UtcNow
                };

                _context.Facultades.Add(facultad);
                await _context.SaveChangesAsync();

                var response = new FacultadResponseDto
                {
                    Id = facultad.Id,
                    Nombre = facultad.Nombre,
                    Codigo = facultad.Codigo,
                    Descripcion = facultad.Descripcion,
                    EsActiva = facultad.EsActiva,
                    Color = facultad.Color,
                    FechaCreacion = facultad.FechaCreacion,
                    FechaActualizacion = facultad.FechaActualizacion,
                    // Propiedad de compatibilidad
                    Activa = facultad.EsActiva
                };

                _logger.LogInformation("Se creó la facultad '{Nombre}' con ID {Id}", facultad.Nombre, facultad.Id);
                return CreatedAtAction(nameof(GetFacultad), new { id = facultad.Id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la facultad");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Actualiza una facultad existente
        /// </summary>
        /// <param name="id">ID de la facultad</param>
        /// <param name="updateDto">Datos actualizados de la facultad</param>
        /// <returns>Facultad actualizada</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(FacultadResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<FacultadResponseDto>> UpdateFacultad(int id, [FromBody] UpdateFacultadDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var facultad = await _context.Facultades
                    .FirstOrDefaultAsync(f => f.Id == id);

                if (facultad == null)
                {
                    return NotFound(new { message = $"No se encontró la facultad con ID {id}" });
                }

                // Verificar si el nuevo nombre o código ya existe en otro registro
                if (!string.IsNullOrWhiteSpace(updateDto.Nombre) && 
                    updateDto.Nombre.ToLower() != facultad.Nombre.ToLower())
                {
                    var existeNombre = await _context.Facultades
                        .AnyAsync(f => f.Id != id && f.Nombre.ToLower() == updateDto.Nombre.ToLower());

                    if (existeNombre)
                    {
                        return Conflict(new { message = $"Ya existe otra facultad con el nombre '{updateDto.Nombre}'" });
                    }
                }

                if (!string.IsNullOrWhiteSpace(updateDto.Codigo) && 
                    updateDto.Codigo.ToLower() != facultad.Codigo.ToLower())
                {
                    var existeCodigo = await _context.Facultades
                        .AnyAsync(f => f.Id != id && f.Codigo.ToLower() == updateDto.Codigo.ToLower());

                    if (existeCodigo)
                    {
                        return Conflict(new { message = $"Ya existe otra facultad con el código '{updateDto.Codigo}'" });
                    }
                }

                // Actualizar propiedades
                if (!string.IsNullOrWhiteSpace(updateDto.Nombre))
                    facultad.Nombre = updateDto.Nombre.Trim();

                if (!string.IsNullOrWhiteSpace(updateDto.Codigo))
                    facultad.Codigo = updateDto.Codigo.Trim().ToUpper();

                if (updateDto.Descripcion != null)
                    facultad.Descripcion = updateDto.Descripcion.Trim();

                facultad.EsActiva = updateDto.EsActiva;

                if (updateDto.Color != null)
                    facultad.Color = updateDto.Color.Trim();

                facultad.FechaActualizacion = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var response = new FacultadResponseDto
                {
                    Id = facultad.Id,
                    Nombre = facultad.Nombre,
                    Codigo = facultad.Codigo,
                    Descripcion = facultad.Descripcion,
                    EsActiva = facultad.EsActiva,
                    Color = facultad.Color,
                    FechaCreacion = facultad.FechaCreacion,
                    FechaActualizacion = facultad.FechaActualizacion,
                    // Propiedad de compatibilidad
                    Activa = facultad.EsActiva
                };

                _logger.LogInformation("Se actualizó la facultad con ID {Id}", id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la facultad con ID {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Elimina una facultad (soft delete)
        /// </summary>
        /// <param name="id">ID de la facultad</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteFacultad(int id)
        {
            try
            {
                var facultad = await _context.Facultades
                    .FirstOrDefaultAsync(f => f.Id == id);

                if (facultad == null)
                {
                    return NotFound(new { message = $"No se encontró la facultad con ID {id}" });
                }

                // Verificar si la facultad está siendo usada por docentes
                var esUsada = await _context.Docentes
                    .AnyAsync(d => d.FacultadId == id);

                if (esUsada)
                {
                    return Conflict(new { message = "No se puede eliminar la facultad porque tiene docentes asociados" });
                }

                // Soft delete
                facultad.EsActiva = false;
                facultad.FechaActualizacion = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Se eliminó (desactivó) la facultad con ID {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la facultad con ID {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene facultades activas
        /// </summary>
        /// <returns>Lista de facultades activas</returns>
        [HttpGet("activas")]
        [ProducesResponseType(typeof(IEnumerable<FacultadResponseDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<FacultadResponseDto>>> GetFacultadesActivas()
        {
            try
            {
                var facultadesActivas = await _context.Facultades
                    .Where(f => f.EsActiva)
                    .AsNoTracking()
                    .ToListAsync();

                var response = facultadesActivas.Select(f => new FacultadResponseDto
                {
                    Id = f.Id,
                    Nombre = f.Nombre,
                    Codigo = f.Codigo,
                    Descripcion = f.Descripcion,
                    EsActiva = f.EsActiva,
                    Color = f.Color,
                    FechaCreacion = f.FechaCreacion,
                    FechaActualizacion = f.FechaActualizacion,
                    // Propiedad de compatibilidad
                    Activa = f.EsActiva
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener facultades activas");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene facultades por código
        /// </summary>
        /// <param name="codigo">Código de la facultad</param>
        /// <returns>Facultad con el código especificado</returns>
        [HttpGet("codigo/{codigo}")]
        [ProducesResponseType(typeof(FacultadResponseDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<FacultadResponseDto>> GetFacultadPorCodigo(string codigo)
        {
            try
            {
                var facultad = await _context.Facultades
                    .AsNoTracking()
                    .FirstOrDefaultAsync(f => f.Codigo.ToLower() == codigo.ToLower());

                if (facultad == null)
                {
                    return NotFound(new { message = $"No se encontró la facultad con código '{codigo}'" });
                }

                var response = new FacultadResponseDto
                {
                    Id = facultad.Id,
                    Nombre = facultad.Nombre,
                    Codigo = facultad.Codigo,
                    Descripcion = facultad.Descripcion,
                    EsActiva = facultad.EsActiva,
                    Color = facultad.Color,
                    FechaCreacion = facultad.FechaCreacion,
                    FechaActualizacion = facultad.FechaActualizacion,
                    // Propiedad de compatibilidad
                    Activa = facultad.EsActiva
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la facultad con código {Codigo}", codigo);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene estadísticas de facultades
        /// </summary>
        /// <returns>Estadísticas de facultades</returns>
        [HttpGet("estadisticas")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> GetEstadisticas()
        {
            try
            {
                var estadisticas = new
                {
                    TotalFacultades = await _context.Facultades.CountAsync(),
                    FacultadesActivas = await _context.Facultades.CountAsync(f => f.EsActiva),
                    FacultadesInactivas = await _context.Facultades.CountAsync(f => !f.EsActiva),
                    FacultadesConDocentes = await _context.Facultades
                        .Where(f => f.Docentes.Any())
                        .CountAsync(),
                    DocentesPorFacultad = await _context.Facultades
                        .Where(f => f.EsActiva)
                        .Select(f => new 
                        {
                            FacultadId = f.Id,
                            Nombre = f.Nombre,
                            Codigo = f.Codigo,
                            TotalDocentes = f.Docentes.Count()
                        })
                        .ToListAsync()
                };

                return Ok(estadisticas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas de facultades");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}