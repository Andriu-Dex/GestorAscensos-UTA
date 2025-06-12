using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGA.Application.DTOs;
using SGA.Application.Services;
using SGA.Domain.Entities;
using System.Security.Claims;

namespace SGA.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DocenteController : ControllerBase
    {
        private readonly IDocenteService _docenteService;
        private readonly ILogger<DocenteController> _logger;

        public DocenteController(IDocenteService docenteService, ILogger<DocenteController> logger)
        {
            _docenteService = docenteService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDocente(int id)
        {
            try
            {
                var docente = await _docenteService.GetDocenteByIdAsync(id);
                if (docente == null)
                {
                    return NotFound(new { message = "Docente no encontrado" });
                }

                var docenteDto = new DocenteResponseDto
                {
                    Id = docente.Id,
                    Cedula = docente.Cedula,
                    Nombres = docente.Nombres,
                    Apellidos = docente.Apellidos,
                    Email = docente.Email,
                    TelefonoContacto = docente.TelefonoContacto,
                    FacultadId = docente.FacultadId,
                    NivelActual = docente.NivelActual,
                    FechaIngresoNivelActual = docente.FechaIngresoNivelActual,
                    EsAdministrador = docente.EsAdministrador,
                    Activo = docente.Activo,
                    FechaRegistro = docente.FechaRegistro
                };

                return Ok(docenteDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener docente con ID {id}: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("perfil")]
        public async Task<IActionResult> GetMiPerfil()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int docenteId))
                {
                    return Unauthorized(new { message = "Token no válido" });
                }

                var docente = await _docenteService.GetDocenteByIdAsync(docenteId);
                if (docente == null)
                {
                    return NotFound(new { message = "Docente no encontrado" });
                }

                var docenteDto = new DocenteResponseDto
                {
                    Id = docente.Id,
                    Cedula = docente.Cedula,
                    Nombres = docente.Nombres,
                    Apellidos = docente.Apellidos,
                    Email = docente.Email,
                    TelefonoContacto = docente.TelefonoContacto,
                    FacultadId = docente.FacultadId,
                    NivelActual = docente.NivelActual,
                    FechaIngresoNivelActual = docente.FechaIngresoNivelActual,
                    EsAdministrador = docente.EsAdministrador,
                    Activo = docente.Activo,
                    FechaRegistro = docente.FechaRegistro
                };

                return Ok(docenteDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener perfil del docente: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPut("perfil")]
        public async Task<IActionResult> ActualizarPerfil([FromBody] ActualizarDocenteDto actualizarDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int docenteId))
                {
                    return Unauthorized(new { message = "Token no válido" });
                }

                var docente = await _docenteService.GetDocenteByIdAsync(docenteId);
                if (docente == null)
                {
                    return NotFound(new { message = "Docente no encontrado" });
                }

                // Actualizar solo los campos permitidos
                docente.Email = actualizarDto.Email;
                docente.TelefonoContacto = actualizarDto.TelefonoContacto;

                await _docenteService.UpdateDocenteAsync(docente);

                return Ok(new { message = "Perfil actualizado exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar perfil del docente: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("estado-requisitos")]
        public async Task<IActionResult> GetEstadoRequisitos()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int docenteId))
                {
                    return Unauthorized(new { message = "Token no válido" });
                }

                var estadoRequisitos = await _docenteService.ObtenerEstadoRequisitosAsync(docenteId);
                return Ok(estadoRequisitos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener estado de requisitos: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("validar-requisitos")]
        public async Task<IActionResult> ValidarRequisitos()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int docenteId))
                {
                    return Unauthorized(new { message = "Token no válido" });
                }

                var cumpleRequisitos = await _docenteService.ValidarRequisitosAscensoAsync(docenteId);
                return Ok(new { cumpleRequisitos });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al validar requisitos: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}
