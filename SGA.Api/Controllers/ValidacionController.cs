using Microsoft.AspNetCore.Mvc;
using SGA.Application.Interfaces;
using SGA.Domain.Enums;
using System.Threading.Tasks;

namespace SGA.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValidacionController : ControllerBase
    {
        private readonly IValidacionAscensoService _validacionService;

        public ValidacionController(IValidacionAscensoService validacionService)
        {
            _validacionService = validacionService;
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
