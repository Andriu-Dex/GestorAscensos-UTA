using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGA.Application.DTOs;
using SGA.Application.Services;
using SGA.Domain.Entities;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace SGA.Api.Controllers
{
    /// <summary>
    /// DTO para validar documentos - definido localmente para evitar problemas de referencia
    /// </summary>
    public class ValidarDocumentoDto
    {
        /// <summary>
        /// Indica si el documento es válido
        /// </summary>
        [Required(ErrorMessage = "El campo Validado es requerido")]
        public bool Validado { get; set; }

        /// <summary>
        /// Observaciones sobre la validación del documento
        /// </summary>
        [MaxLength(1000, ErrorMessage = "Las observaciones no pueden exceder 1000 caracteres")]
        public string? Observaciones { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DocumentoController : ControllerBase
    {
        private readonly IDocumentoService _documentoService;
        private readonly ILogger<DocumentoController> _logger;

        public DocumentoController(IDocumentoService documentoService, ILogger<DocumentoController> logger)
        {
            _documentoService = documentoService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetMisDocumentos()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int docenteId))
                {
                    return Unauthorized(new { message = "Token no válido" });
                }

                var documentos = await _documentoService.GetDocumentosByDocenteIdAsync(docenteId);
                var documentosDto = documentos.Select(d => new DocumentoResponseDto
                {
                    Id = d.Id,
                    DocenteId = d.DocenteId,
                    Nombre = d.Nombre,
                    Descripcion = d.Descripcion,
                    TipoDocumentoId = d.TipoDocumentoId,
                    ContentType = d.ContentType,
                    TamanioBytes = d.TamanioBytes,
                    FechaSubida = d.FechaSubida,
                    FechaModificacion = d.FechaModificacion,
                    Validado = d.Validado,
                    FechaValidacion = d.FechaValidacion,
                    ValidadoPorId = d.ValidadoPorId,
                    ObservacionesValidacion = d.ObservacionesValidacion,
                    HashSHA256 = d.HashSHA256,
                    Activo = d.Activo
                });

                return Ok(documentosDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener documentos del docente: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDocumento(int id)
        {
            try
            {
                var documento = await _documentoService.GetDocumentoByIdAsync(id);
                if (documento == null)
                {
                    return NotFound(new { message = "Documento no encontrado" });
                }

                // Verificar que el usuario tiene acceso a este documento
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int docenteId))
                {
                    return Unauthorized(new { message = "Token no válido" });
                }

                if (documento.DocenteId != docenteId && !User.IsInRole("Administrador"))
                {
                    return Forbid("No tiene permisos para ver este documento");
                }

                var documentoDto = new DocumentoResponseDto
                {
                    Id = documento.Id,
                    DocenteId = documento.DocenteId,
                    Nombre = documento.Nombre,
                    Descripcion = documento.Descripcion,
                    TipoDocumentoId = documento.TipoDocumentoId,
                    ContentType = documento.ContentType,
                    TamanioBytes = documento.TamanioBytes,
                    FechaSubida = documento.FechaSubida,
                    FechaModificacion = documento.FechaModificacion,
                    Validado = documento.Validado,
                    FechaValidacion = documento.FechaValidacion,
                    ValidadoPorId = documento.ValidadoPorId,
                    ObservacionesValidacion = documento.ObservacionesValidacion,
                    HashSHA256 = documento.HashSHA256,
                    Activo = documento.Activo
                };

                return Ok(documentoDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener documento con ID {id}: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }        [HttpGet("{id}/descargar")]
        public async Task<IActionResult> DescargarDocumento(int id)
        {
            try
            {
                var documento = await _documentoService.GetDocumentoByIdAsync(id);
                if (documento == null)
                {
                    return NotFound(new { message = "Documento no encontrado" });
                }

                // Verificar que el usuario tiene acceso a este documento
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int docenteId))
                {
                    return Unauthorized(new { message = "Token no válido" });
                }

                if (documento.DocenteId != docenteId && !User.IsInRole("Administrador"))
                {
                    return Forbid("No tiene permisos para descargar este documento");
                }

                // Obtener contenido descifrado
                var contenido = await _documentoService.ObtenerContenidoDocumentoAsync(id);

                // Devolver archivo
                return File(contenido, documento.ContentType, documento.Nombre);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al descargar documento con ID {id}: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubirDocumento([FromForm] SubirDocumentoDto subirDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int docenteId))
                {
                    return Unauthorized(new { message = "Token no válido" });
                }                if (subirDto.Archivo == null || subirDto.Archivo.Length == 0)
                {
                    return BadRequest(new { message = "No se ha proporcionado un archivo" });
                }

                using var memoryStream = new MemoryStream();
                await subirDto.Archivo.CopyToAsync(memoryStream);
                memoryStream.Position = 0; // Reset position for reading

                var documento = await _documentoService.SubirDocumentoAsync(
                    docenteId,
                    subirDto.Nombre,
                    subirDto.Descripcion ?? string.Empty,
                    subirDto.TipoDocumentoId,
                    memoryStream,
                    subirDto.Archivo.ContentType
                );

                var documentoDto = new DocumentoResponseDto
                {
                    Id = documento.Id,
                    DocenteId = documento.DocenteId,
                    Nombre = documento.Nombre,
                    Descripcion = documento.Descripcion,
                    TipoDocumentoId = documento.TipoDocumentoId,
                    ContentType = documento.ContentType,
                    TamanioBytes = documento.TamanioBytes,
                    FechaSubida = documento.FechaSubida,
                    Validado = documento.Validado,
                    HashSHA256 = documento.HashSHA256,
                    Activo = documento.Activo
                };

                return CreatedAtAction(nameof(GetDocumento), new { id = documento.Id }, documentoDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al subir documento: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarDocumento(int id, [FromBody] ActualizarDocumentoDto actualizarDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int docenteId))
                {
                    return Unauthorized(new { message = "Token no válido" });
                }

                await _documentoService.ActualizarDocumentoAsync(id, docenteId, actualizarDto.Nombre, actualizarDto.Descripcion ?? string.Empty);

                return Ok(new { message = "Documento actualizado exitosamente" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar documento con ID {id}: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarDocumento(int id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int docenteId))
                {
                    return Unauthorized(new { message = "Token no válido" });
                }

                await _documentoService.EliminarDocumentoAsync(id);

                return Ok(new { message = "Documento eliminado exitosamente" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar documento con ID {id}: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("tipo/{tipoId}")]
        public async Task<IActionResult> GetDocumentosPorTipo(int tipoId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int docenteId))
                {
                    return Unauthorized(new { message = "Token no válido" });
                }

                var documentos = await _documentoService.GetDocumentosByTipoAsync(docenteId, tipoId);
                var documentosDto = documentos.Select(d => new DocumentoResponseDto
                {
                    Id = d.Id,
                    DocenteId = d.DocenteId,
                    Nombre = d.Nombre,
                    Descripcion = d.Descripcion,
                    TipoDocumentoId = d.TipoDocumentoId,
                    ContentType = d.ContentType,
                    TamanioBytes = d.TamanioBytes,
                    FechaSubida = d.FechaSubida,
                    FechaModificacion = d.FechaModificacion,
                    Validado = d.Validado,
                    FechaValidacion = d.FechaValidacion,
                    ValidadoPorId = d.ValidadoPorId,
                    ObservacionesValidacion = d.ObservacionesValidacion,
                    HashSHA256 = d.HashSHA256,
                    Activo = d.Activo
                });

                return Ok(documentosDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener documentos por tipo {tipoId}: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPut("{id}/validar")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ValidarDocumento(int id, [FromBody] ValidarDocumentoDto validarDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int validadorId))
                {
                    return Unauthorized(new { message = "Token no válido" });
                }

                await _documentoService.ValidarDocumentoAsync(id, validadorId, validarDto.Validado, validarDto.Observaciones);

                return Ok(new { message = "Documento validado exitosamente" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al validar documento con ID {id}: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

                // Endpoint eliminado para evitar duplicación
    }
}
