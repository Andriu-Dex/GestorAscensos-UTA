using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SGA.Application.DTOs.DocumentoImportacion;
using SGA.Application.Interfaces;
using System.Security.Claims;

namespace SGA.Api.Controllers;

[ApiController]
[Route("api/documentos-importacion")]
[Authorize]
public class DocumentoImportacionController : ControllerBase
{
    private readonly IDocumentoImportacionService _importacionService;
    private readonly ILogger<DocumentoImportacionController> _logger;

    public DocumentoImportacionController(
        IDocumentoImportacionService importacionService,
        ILogger<DocumentoImportacionController> logger)
    {
        _importacionService = importacionService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene la cédula del usuario autenticado desde los claims del JWT
    /// </summary>
    private string? ObtenerCedulaUsuario()
    {
        _logger.LogDebug("Claims disponibles: {Claims}", 
            string.Join(", ", User.Claims.Select(c => $"{c.Type}={c.Value}")));
        
        var cedula = User.FindFirst("cedula")?.Value;
        _logger.LogDebug("Cédula obtenida: {Cedula}", cedula);
        
        return cedula;
    }

    /// <summary>
    /// Buscar documentos disponibles para importación
    /// </summary>
    [HttpGet("buscar")]
    public async Task<ActionResult<List<DocumentoImportacionDto>>> BuscarDocumentosImportables(
        [FromQuery] string? tipoDocumento = null,
        [FromQuery] DateTime? fechaDesde = null,
        [FromQuery] DateTime? fechaHasta = null,
        [FromQuery] string? textoBusqueda = null,
        [FromQuery] bool soloImportables = true)
    {
        try
        {
            var cedula = ObtenerCedulaUsuario();
            if (string.IsNullOrEmpty(cedula))
                return Unauthorized("No se pudo obtener la cédula del usuario");

            var filtros = new FiltrosImportacionDto
            {
                TipoDocumento = tipoDocumento,
                FechaDesde = fechaDesde,
                FechaHasta = fechaHasta,
                TextoBusqueda = textoBusqueda,
                SoloImportables = soloImportables
            };

            var documentos = await _importacionService.BuscarDocumentosImportablesAsync(cedula, filtros);
            return Ok(documentos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error al buscar documentos: {ex.Message}" });
        }
    }

    /// <summary>
    /// Importar documentos seleccionados
    /// </summary>
    [HttpPost("importar")]
    [Authorize(Roles = "Docente,Administrador")]
    public async Task<ActionResult<ImportarDocumentosResponseDto>> ImportarDocumentosSeleccionados(
        [FromBody] List<Guid> documentosIds)
    {
        try
        {
            var cedula = ObtenerCedulaUsuario();
            if (string.IsNullOrEmpty(cedula))
                return Unauthorized("No se pudo obtener la cédula del usuario");

            if (documentosIds == null || !documentosIds.Any())
                return BadRequest("Debe seleccionar al menos un documento para importar");

            var resultado = await _importacionService.ImportarDocumentosSeleccionadosAsync(cedula, documentosIds);
            
            if (resultado.Exitoso)
                return Ok(resultado);
            else
                return BadRequest(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error al importar documentos: {ex.Message}" });
        }
    }

    /// <summary>
    /// Obtener detalle de un documento específico
    /// </summary>
    [HttpGet("{documentoId}")]
    public async Task<ActionResult<DocumentoImportacionDto>> ObtenerDetalleDocumento(Guid documentoId)
    {
        try
        {
            var cedula = ObtenerCedulaUsuario();
            if (string.IsNullOrEmpty(cedula))
                return Unauthorized("No se pudo obtener la cédula del usuario");

            var documento = await _importacionService.ObtenerDetalleDocumentoAsync(documentoId, cedula);
            
            if (documento == null)
                return NotFound("Documento no encontrado");

            return Ok(documento);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error al obtener detalle del documento: {ex.Message}" });
        }
    }

    /// <summary>
    /// Validar si un documento es importable
    /// </summary>
    [HttpGet("{documentoId}/validar")]
    public async Task<ActionResult<bool>> ValidarDocumentoImportable(Guid documentoId)
    {
        try
        {
            var cedula = ObtenerCedulaUsuario();
            if (string.IsNullOrEmpty(cedula))
                return Unauthorized("No se pudo obtener la cédula del usuario");

            var esImportable = await _importacionService.ValidarDocumentoImportableAsync(documentoId, cedula);
            return Ok(esImportable);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error al validar documento: {ex.Message}" });
        }
    }

    /// <summary>
    /// Descargar/Visualizar documento
    /// </summary>
    [HttpGet("{documentoId}/descargar")]
    public async Task<ActionResult> DescargarDocumento(Guid documentoId)
    {
        try
        {
            var cedula = ObtenerCedulaUsuario();
            if (string.IsNullOrEmpty(cedula))
                return Unauthorized("No se pudo obtener la cédula del usuario");

            // Verificar que el documento esté disponible para el usuario
            var documento = await _importacionService.ObtenerDetalleDocumentoAsync(documentoId, cedula);
            if (documento == null)
                return NotFound("Documento no encontrado");

            // Obtener el contenido del documento desde el servicio de documentos
            var contenidoDocumento = await _importacionService.ObtenerContenidoDocumentoAsync(documentoId);
            if (contenidoDocumento == null)
                return NotFound("Contenido del documento no encontrado");

            return File(contenidoDocumento, "application/pdf", documento.Nombre);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error al descargar documento: {ex.Message}" });
        }
    }
}
