using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGA.Application.DTOs.Docentes;
using SGA.Application.DTOs.Responses;
using SGA.Application.Constants;
using SGA.Application.Interfaces;
using System.Security.Claims;

namespace SGA.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DocentesController : ControllerBase
{
    private readonly IDocenteService _docenteService;

    public DocentesController(IDocenteService docenteService)
    {
        _docenteService = docenteService;
    }

    [HttpGet("perfil")]
    public async Task<ActionResult> GetPerfil()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            // Crear un objeto compatible con DocentePerfilDto del frontend
            var perfilResponse = new
            {
                Id = docente.Id,
                Cedula = docente.Cedula,
                Nombres = docente.Nombres,
                Apellidos = docente.Apellidos,
                Email = docente.Email,
                TelefonoContacto = docente.Celular, // Mapear Celular a TelefonoContacto
                NivelActual = docente.NivelActual,
                FechaInicioNivelActual = docente.FechaInicioNivelActual,
                FechaUltimoAscenso = docente.FechaUltimoAscenso,
                NombreCompleto = docente.NombreCompleto,
                FechaNombramiento = docente.FechaNombramiento,
                PromedioEvaluaciones = docente.PromedioEvaluaciones,
                HorasCapacitacion = docente.HorasCapacitacion,
                NumeroObrasAcademicas = docente.NumeroObrasAcademicas,
                MesesInvestigacion = docente.MesesInvestigacion,
                FechaUltimaImportacion = docente.FechaUltimaImportacion,
                FotoPerfilBase64 = docente.FotoPerfilBase64,
                PuedeAscender = true, // Esto se puede calcular desde el dominio
                SiguienteNivel = "", // Esto se puede calcular desde el dominio
                Departamento = (string?)null,
                Facultad = (object?)null
            };

            return Ok(perfilResponse);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<DocenteDto>> GetDocente(Guid id)
    {
        try
        {
            var docente = await _docenteService.GetDocenteByIdAsync(id);
            if (docente == null)
                return NotFound("Docente no encontrado");

            return Ok(docente);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("importar-datos/tthh")]
    public async Task<ActionResult<ImportarDatosResponse>> ImportarDatosTTHH()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            var response = await _docenteService.ImportarDatosTTHHAsync(docente.Cedula);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("importar-datos/dac")]
    public async Task<ActionResult<ImportarDatosResponse>> ImportarDatosDAC()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            var response = await _docenteService.ImportarDatosDACAsync(docente.Cedula);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("importar-datos/ditic")]
    public async Task<ActionResult<ImportarDatosResponse>> ImportarDatosDITIC()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            var response = await _docenteService.ImportarDatosDITICAsync(docente.Cedula);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("importar-datos/dirinv")]
    public async Task<ActionResult<ImportarDatosResponse>> ImportarDatosDIRINV()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            var response = await _docenteService.ImportarDatosDIRINVAsync(docente.Cedula);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("importar-obras")]
    public async Task<ActionResult<ImportarDatosResponse>> ImportarObras()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            var response = await _docenteService.ImportarObrasAcademicasAsync(docente.Cedula);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("importar-evaluacion")]
    public async Task<ActionResult<ImportarDatosResponse>> ImportarEvaluacion()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            var response = await _docenteService.ImportarDatosDACAsync(docente.Cedula);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("importar-capacitacion")]
    public async Task<ActionResult<ImportarDatosResponse>> ImportarCapacitacion()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            var response = await _docenteService.ImportarDatosDITICAsync(docente.Cedula);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("importar-investigacion")]
    public async Task<ActionResult<ImportarDatosResponse>> ImportarInvestigacion()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            var response = await _docenteService.ImportarDatosDIRINVAsync(docente.Cedula);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("validar-requisitos/{nivelObjetivo}")]
    public async Task<ActionResult<ValidacionRequisitosDto>> ValidarRequisitos(string nivelObjetivo)
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            var validacion = await _docenteService.ValidarRequisitosAscensoAsync(docente.Cedula, nivelObjetivo);
            return Ok(validacion);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("indicadores")]
    public async Task<ActionResult<IndicadoresDocenteDto>> GetIndicadores()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            var indicadores = await _docenteService.GetIndicadoresAsync(docente.Cedula);
            return Ok(indicadores);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("actualizar-indicadores")]
    public async Task<ActionResult> ActualizarIndicadores()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            // Importar datos de todos los sistemas
            await _docenteService.ImportarDatosTTHHAsync(docente.Cedula);
            await _docenteService.ImportarDatosDACAsync(docente.Cedula);
            await _docenteService.ImportarDatosDITICAsync(docente.Cedula);
            await _docenteService.ImportarDatosDIRINVAsync(docente.Cedula);

            return Ok(new { message = "Indicadores actualizados correctamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("importar-tiempo-rol")]
    public async Task<ActionResult<ImportarDatosResponse>> ImportarTiempoRol()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            var response = await _docenteService.ImportarTiempoRolTTHHAsync(docente.Cedula);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("{id}/profile-photo")]
    [RequestSizeLimit(FileLimits.ProfileImages.MaxSizeBytes)]
    public async Task<ActionResult<FileUploadResponse>> UploadProfilePhoto(Guid id, IFormFile file)
    {
        try
        {
            // Verificar que el docente autenticado coincida con el ID solicitado
            var currentDocenteId = GetCurrentDocenteId();
            if (currentDocenteId == Guid.Empty)
            {
                return Unauthorized(FileUploadResponse.ErrorResponse(
                    "No se pudo identificar al docente",
                    FileUploadErrorType.DatabaseError));
            }

            if (currentDocenteId != id)
            {
                return Forbid();
            }

            var result = await _docenteService.UploadProfilePhotoAsync(id, file);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en upload de foto de perfil: {ex.Message}");
            return StatusCode(500, FileUploadResponse.ErrorResponse(
                "Error interno del servidor",
                FileUploadErrorType.ProcessingError));
        }
    }

    [HttpGet("upload-config")]
    public ActionResult<UploadConfigResponse> GetUploadConfig()
    {
        try
        {
            var config = _docenteService.GetUploadConfig();
            return Ok(config);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener configuración de carga: {ex.Message}");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    // Método auxiliar para obtener el ID del docente actual
    private Guid GetCurrentDocenteId()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Guid.Empty;

            // Obtener el docente por email de forma síncrona (no ideal, pero funcional)
            var docente = _docenteService.GetDocenteByEmailAsync(email).Result;
            return docente?.Id ?? Guid.Empty;
        }
        catch
        {
            return Guid.Empty;
        }
    }
}

// Nuevo controlador para endpoints específicos que el frontend espera
[ApiController]
[Route("api/docente")]
[Authorize]
public class DocenteController : ControllerBase
{
    private readonly IDocenteService _docenteService;

    public DocenteController(IDocenteService docenteService)
    {
        _docenteService = docenteService;
    }

    [HttpGet("indicadores")]
    public async Task<ActionResult<IndicadoresDocenteDto>> GetIndicadores()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            var indicadores = await _docenteService.GetIndicadoresAsync(docente.Cedula);
            return Ok(indicadores);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("requisitos")]
    public async Task<ActionResult<RequisitosAscensoDto>> GetRequisitos()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            // Obtener el siguiente nivel para validar requisitos
            var nivelActual = int.Parse(docente.NivelActual.ToString().Replace("Titular", ""));
            var siguienteNivel = $"Titular{nivelActual + 1}";

            var requisitos = await _docenteService.GetRequisitosAscensoAsync(docente.Cedula, siguienteNivel);
            return Ok(requisitos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPut("actualizar-perfil")]
    public async Task<ActionResult> ActualizarPerfil([FromBody] ActualizarPerfilFrontendDto dto)
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            // Convertir el DTO del frontend al DTO del servicio
            var actualizarDto = new ActualizarPerfilDto
            {
                Nombres = dto.Nombres,
                Apellidos = dto.Apellidos,
                Email = dto.Email,
                Celular = dto.TelefonoContacto
            };

            var resultado = await _docenteService.ActualizarPerfilAsync(docente.Id, actualizarDto);
            if (!resultado)
                return BadRequest("No se pudo actualizar el perfil");

            return Ok(new { message = "Perfil actualizado correctamente" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("actualizar-indicadores")]
    public async Task<ActionResult> ActualizarIndicadores()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            // Importar datos de todos los sistemas
            await _docenteService.ImportarDatosTTHHAsync(docente.Cedula);
            await _docenteService.ImportarDatosDACAsync(docente.Cedula);
            await _docenteService.ImportarDatosDITICAsync(docente.Cedula);
            await _docenteService.ImportarDatosDIRINVAsync(docente.Cedula);

            return Ok(new { message = "Indicadores actualizados correctamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
