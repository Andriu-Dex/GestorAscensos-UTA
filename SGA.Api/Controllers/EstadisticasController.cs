using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGA.Application.DTOs.Admin;
using SGA.Application.Interfaces;
using System.Security.Claims;

namespace SGA.Api.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Administrador")]
    public class EstadisticasController : ControllerBase
    {
        private readonly IEstadisticasService _estadisticasService;
        private readonly ILogger<EstadisticasController> _logger;

        public EstadisticasController(
            IEstadisticasService estadisticasService,
            ILogger<EstadisticasController> logger)
        {
            _estadisticasService = estadisticasService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene estadísticas completas del sistema
        /// </summary>
        [HttpGet("estadisticas-completas")]
        public async Task<ActionResult<EstadisticasCompletasDto>> GetEstadisticasCompletas()
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                _logger.LogInformation("Admin {Email} consultando estadísticas completas", email);

                var estadisticas = await _estadisticasService.GetEstadisticasCompletasAsync();
                return Ok(estadisticas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas completas");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene estadísticas generales del dashboard
        /// </summary>
        [HttpGet("estadisticas")]
        public async Task<ActionResult<EstadisticasGeneralesDto>> GetEstadisticasGenerales()
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                _logger.LogInformation("Admin {Email} consultando estadísticas generales", email);

                var estadisticas = await _estadisticasService.GetEstadisticasGeneralesAsync();
                return Ok(estadisticas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas generales");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene estadísticas por facultad
        /// </summary>
        [HttpGet("estadisticas-por-facultad")]
        public async Task<ActionResult<List<EstadisticasFacultadDto>>> GetEstadisticasPorFacultad()
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                _logger.LogInformation("Admin {Email} consultando estadísticas por facultad", email);

                var estadisticas = await _estadisticasService.GetEstadisticasPorFacultadAsync();
                return Ok(estadisticas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas por facultad");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene estadísticas por nivel académico
        /// </summary>
        [HttpGet("estadisticas-por-nivel")]
        public async Task<ActionResult<List<EstadisticasNivelDto>>> GetEstadisticasPorNivel()
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                _logger.LogInformation("Admin {Email} consultando estadísticas por nivel", email);

                var estadisticas = await _estadisticasService.GetEstadisticasPorNivelAsync();
                return Ok(estadisticas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas por nivel");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene estadísticas de actividad mensual
        /// </summary>
        [HttpGet("estadisticas-actividad-mensual")]
        public async Task<ActionResult<List<EstadisticasActividadMensualDto>>> GetEstadisticasActividadMensual()
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                _logger.LogInformation("Admin {Email} consultando estadísticas de actividad mensual", email);

                var estadisticas = await _estadisticasService.GetEstadisticasActividadMensualAsync();
                return Ok(estadisticas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas de actividad mensual");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene lista de facultades
        /// </summary>
        [HttpGet("facultades")]
        public async Task<ActionResult<List<string>>> GetFacultades()
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                _logger.LogInformation("Admin {Email} consultando facultades", email);

                var facultades = await _estadisticasService.GetFacultadesAsync();
                return Ok(facultades);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener facultades");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}
