using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGA.Application.DTOs.Documentos;
using SGA.Application.Interfaces;
using System.Security.Claims;

namespace SGA.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DocumentoController : ControllerBase
{
    private readonly IDocumentoService _documentoService;
    private readonly IDocenteService _docenteService;

    public DocumentoController(IDocumentoService documentoService, IDocenteService docenteService)
    {
        _documentoService = documentoService;
        _docenteService = docenteService;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> SubirDocumento([FromForm] IFormFile archivo, [FromForm] string nombre, [FromForm] string? tipoDocumento = null, [FromForm] string? solicitudId = null)
    {
        try
        {
            if (archivo == null || archivo.Length == 0)
                return BadRequest("Archivo requerido");

            if (string.IsNullOrEmpty(nombre))
                return BadRequest("Nombre del documento requerido");

            // Validar que sea PDF
            if (!archivo.ContentType.Contains("pdf", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Solo se permiten archivos PDF");

            using var memoryStream = new MemoryStream();
            await archivo.CopyToAsync(memoryStream);

            // Crear una solicitud temporal si no se proporciona
            var solicitudGuid = string.IsNullOrEmpty(solicitudId) ? Guid.NewGuid() : Guid.Parse(solicitudId);
            var tipoDoc = string.IsNullOrEmpty(tipoDocumento) ? 
                Domain.Enums.TipoDocumento.Otro : 
                Enum.Parse<Domain.Enums.TipoDocumento>(tipoDocumento);

            var documento = new SubirDocumentoRequestDto
            {
                Nombre = nombre,
                Tipo = tipoDoc,
                SolicitudId = solicitudGuid,
                Contenido = memoryStream.ToArray(),
                TipoContenido = archivo.ContentType
            };

            var documentoId = await _documentoService.SubirDocumentoAsync(solicitudGuid, documento);
            return Ok(new { Id = documentoId, Mensaje = "Documento subido exitosamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("mis-documentos")]
    public async Task<ActionResult<List<DocumentoResponseDto>>> GetMisDocumentos()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            // Obtener documentos del docente desde el repositorio
            var documentoRepository = HttpContext.RequestServices.GetRequiredService<Application.Interfaces.Repositories.IDocumentoRepository>();
            var documentos = await documentoRepository.GetByDocenteIdAsync(docente.Id);

            var documentosDto = documentos.Select(d => new DocumentoResponseDto
            {
                Id = d.Id,
                Nombre = d.NombreArchivo,
                Tipo = d.TipoDocumento,
                RutaArchivo = d.RutaArchivo,
                TamanoArchivo = d.TamanoArchivo,
                TipoContenido = d.ContentType,
                FechaSubida = d.FechaCreacion,
                SolicitudId = d.SolicitudAscensoId,
                EsValido = true,
                Observaciones = null,
                FechaValidacion = null,
                ValidadoPor = null
            }).ToList();

            return Ok(documentosDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("{id}/download")]
    public async Task<ActionResult> DescargarDocumento(Guid id)
    {
        try
        {
            var contenido = await _documentoService.DescargarDocumentoAsync(id);
            if (contenido == null)
                return NotFound("Documento no encontrado");

            return File(contenido, "application/pdf");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("{id}/view")]
    public async Task<ActionResult> VisualizarDocumento(Guid id)
    {
        try
        {
            var contenido = await _documentoService.DescargarDocumentoAsync(id);
            if (contenido == null)
                return NotFound("Documento no encontrado");

            return File(contenido, "application/pdf");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> EliminarDocumento(Guid id)
    {
        try
        {
            var eliminado = await _documentoService.EliminarDocumentoAsync(id);
            if (!eliminado)
                return NotFound("Documento no encontrado");

            return Ok(new { message = "Documento eliminado correctamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
