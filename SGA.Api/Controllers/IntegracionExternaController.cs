using Microsoft.AspNetCore.Mvc;
using SGA.Application.Interfaces;
using SGA.Application.DTOs.ExternalData;
using System.Threading.Tasks;

namespace SGA.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IntegracionExternaController : ControllerBase
    {
        private readonly IExternalDataService _externalDataService;

        public IntegracionExternaController(IExternalDataService externalDataService)
        {
            _externalDataService = externalDataService;
        }

        [HttpGet("tthh/{cedula}")]
        public async Task<IActionResult> ObtenerDatosTTHH(string cedula)
        {
            var datos = await _externalDataService.ImportarDatosTTHHAsync(cedula);
            if (datos == null)
                return NotFound($"No se encontraron datos de TTHH para la cédula {cedula}");
                
            return Ok(datos);
        }

        [HttpGet("dac/{cedula}")]
        public async Task<IActionResult> ObtenerDatosDAC(string cedula)
        {
            var datos = await _externalDataService.ImportarDatosDACAsync(cedula);
            if (datos == null)
                return NotFound($"No se encontraron datos de evaluación docente para la cédula {cedula}");
                
            return Ok(datos);
        }

        [HttpGet("ditic/{cedula}")]
        public async Task<IActionResult> ObtenerDatosDITIC(string cedula)
        {
            var datos = await _externalDataService.ImportarDatosDITICAsync(cedula);
            if (datos == null)
                return NotFound($"No se encontraron datos de capacitación para la cédula {cedula}");
                
            return Ok(datos);
        }

        [HttpGet("dirinv/{cedula}")]
        public async Task<IActionResult> ObtenerDatosDirInv(string cedula)
        {
            var datos = await _externalDataService.ImportarDatosDirInvAsync(cedula);
            if (datos == null)
                return NotFound($"No se encontraron datos de investigación para la cédula {cedula}");
                
            return Ok(datos);
        }

        [HttpGet("completo/{cedula}")]
        public async Task<IActionResult> ObtenerDatosCompletos(string cedula)
        {
            var datosTTHH = await _externalDataService.ImportarDatosTTHHAsync(cedula);
            if (datosTTHH == null)
                return NotFound($"No se encontraron datos de TTHH para la cédula {cedula}");
                
            var datosDAC = await _externalDataService.ImportarDatosDACAsync(cedula);
            var datosDITIC = await _externalDataService.ImportarDatosDITICAsync(cedula);
            var datosDirInv = await _externalDataService.ImportarDatosDirInvAsync(cedula);
            
            return Ok(new
            {
                TTHH = datosTTHH,
                DAC = datosDAC,
                DITIC = datosDITIC,
                DirInv = datosDirInv
            });
        }
    }
}
