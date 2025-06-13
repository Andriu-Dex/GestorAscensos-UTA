using Microsoft.AspNetCore.Mvc;
using SGA.Application.Services;

namespace SGA.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailNormalizationController : ControllerBase
    {
        private readonly IEmailNormalizationService _emailNormalizationService;
        private readonly IDatosTTHHService _datosTTHHService;

        public EmailNormalizationController(
            IEmailNormalizationService emailNormalizationService,
            IDatosTTHHService datosTTHHService)
        {
            _emailNormalizationService = emailNormalizationService;
            _datosTTHHService = datosTTHHService;
        }

        /// <summary>
        /// Normaliza todos los correos existentes en la base de datos de TTHH
        /// </summary>
        /// <returns>Número de registros actualizados</returns>
        [HttpPost("normalizar-existentes")]
        public async Task<ActionResult<NormalizacionResult>> NormalizarCorreosExistentes()
        {
            try
            {
                var registrosActualizados = await _emailNormalizationService.NormalizarCorreosExistentesAsync();
                
                return Ok(new NormalizacionResult
                {
                    RegistrosActualizados = registrosActualizados,
                    Mensaje = $"Se han normalizado {registrosActualizados} correos electrónicos",
                    Exito = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new NormalizacionResult
                {
                    RegistrosActualizados = 0,
                    Mensaje = $"Error al normalizar correos: {ex.Message}",
                    Exito = false
                });
            }
        }

        /// <summary>
        /// Obtiene todos los datos de TTHH con sus correos normalizados
        /// </summary>
        /// <returns>Lista de datos de TTHH</returns>
        [HttpGet("listar-datos-tthh")]
        public async Task<ActionResult> ListarDatosTTHH()
        {
            try
            {
                var datos = await _datosTTHHService.GetAllAsync();
                return Ok(datos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = $"Error al obtener datos: {ex.Message}" });
            }
        }

        /// <summary>
        /// Normaliza el correo de un docente específico por cédula
        /// </summary>
        /// <param name="cedula">Cédula del docente</param>
        /// <returns>Resultado de la normalización</returns>
        [HttpPost("normalizar-por-cedula/{cedula}")]
        public async Task<ActionResult<NormalizacionResult>> NormalizarCorreoPorCedula(string cedula)
        {
            try
            {
                var datosTTHH = await _datosTTHHService.GetDatosByCedulaAsync(cedula);
                if (datosTTHH == null)
                {
                    return NotFound(new NormalizacionResult
                    {
                        RegistrosActualizados = 0,
                        Mensaje = $"No se encontraron datos para la cédula {cedula}",
                        Exito = false
                    });
                }

                await _emailNormalizationService.NormalizarCorreoDocente(datosTTHH);
                
                return Ok(new NormalizacionResult
                {
                    RegistrosActualizados = 1,
                    Mensaje = $"Correo normalizado para {datosTTHH.Nombres} {datosTTHH.Apellidos}: {datosTTHH.EmailInstitucional}",
                    Exito = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new NormalizacionResult
                {
                    RegistrosActualizados = 0,
                    Mensaje = $"Error al normalizar correo: {ex.Message}",
                    Exito = false
                });
            }
        }
    }

    public class NormalizacionResult
    {
        public int RegistrosActualizados { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public bool Exito { get; set; }
    }
}
