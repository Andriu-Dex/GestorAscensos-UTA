using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SGA.Application.Interfaces;
using SGA.Application.DTOs.Docentes;
using SGA.Domain.Enums;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SGA.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ValidacionController : ControllerBase
    {
        private readonly IValidacionAscensoService _validacionService;
        private readonly IDocenteService _docenteService;

        public ValidacionController(IValidacionAscensoService validacionService, IDocenteService docenteService)
        {
            _validacionService = validacionService;
            _docenteService = docenteService;
        }

        [HttpGet("requisitos")]
        public async Task<ActionResult<RequisitosAscensoDto>> GetRequisitos([FromQuery] int nivelActual)
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(email))
                    return Unauthorized();

                var docente = await _docenteService.GetDocenteByEmailAsync(email);
                if (docente == null)
                    return NotFound("Docente no encontrado");

                // Calcular el siguiente nivel
                var nivelSiguiente = nivelActual + 1;
                var nivelDestinoString = $"Titular{nivelSiguiente}";
                
                // Usar el método existente que ya devuelve RequisitosAscensoDto
                var requisitos = await _docenteService.GetRequisitosAscensoAsync(docente.Cedula, nivelDestinoString);
                
                return Ok(requisitos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al obtener requisitos: {ex.Message}" });
            }
        }

        [HttpGet("validar-ascenso/{cedula}")]
        public async Task<IActionResult> ValidarAscenso(string cedula, [FromQuery] NivelTitular? nivelDestino = null)
        {
            try
            {
                var resultado = await _validacionService.ValidarRequisitosConDatosExternosAsync(cedula, nivelDestino);
                return Ok(resultado);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al validar requisitos: {ex.Message}");
            }
        }

        [HttpGet("puede-ascender/{cedula}")]
        public async Task<IActionResult> PuedeAscender(string cedula)
        {
            try
            {
                var requisitos = await _validacionService.ValidarRequisitosConDatosExternosAsync(cedula);
                return Ok(new { 
                    Cedula = cedula,
                    PuedeAscender = requisitos.PuedeAscender,
                    NivelActual = requisitos.NivelActual,
                    NivelDestino = requisitos.NivelDestino,
                    Cumplimientos = new
                    {
                        Antiguedad = requisitos.CumpleAntiguedad,
                        Evaluacion = requisitos.CumpleEvaluacion,
                        Obras = requisitos.CumpleObras,
                        Capacitacion = requisitos.CumpleCapacitacion,
                        Investigacion = requisitos.CumpleInvestigacion
                    }
                });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al verificar elegibilidad: {ex.Message}");
            }
        }
    }
}
