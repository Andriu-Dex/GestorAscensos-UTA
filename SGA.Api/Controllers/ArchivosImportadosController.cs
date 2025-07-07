using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGA.Application.DTOs.DocumentoImportacion;
using SGA.Application.Interfaces.Repositories;
using System.Security.Claims;

namespace SGA.Api.Controllers
{
    [ApiController]
    [Route("api/archivos-importados")]
    public class ArchivosImportadosController : ControllerBase
    {
        private readonly IDocumentoRepository _documentoRepository;
        private readonly IDocenteRepository _docenteRepository;
        private readonly ILogger<ArchivosImportadosController> _logger;

        public ArchivosImportadosController(
            IDocumentoRepository documentoRepository,
            IDocenteRepository docenteRepository,
            ILogger<ArchivosImportadosController> logger)
        {
            _documentoRepository = documentoRepository;
            _docenteRepository = docenteRepository;
            _logger = logger;
        }

        /// <summary>
        /// Obtener archivos importados del docente
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Docente")]
        public async Task<ActionResult<List<ArchivoImportadoDto>>> ObtenerArchivosImportados()
        {
            try
            {
                var cedula = ObtenerCedulaUsuario();
                
                if (string.IsNullOrEmpty(cedula))
                    return Unauthorized("No se pudo obtener la cédula del usuario");

                var docente = await _docenteRepository.GetByCedulaAsync(cedula);
                
                if (docente == null)
                    return NotFound("Docente no encontrado");

                // Obtener documentos clonados (importados) del docente
                var documentosImportados = await _documentoRepository
                    .GetDocumentosImportadosByDocenteAsync(docente.Id);

                var archivos = documentosImportados.Select(doc => new ArchivoImportadoDto
                {
                    Id = doc.Id,
                    NombreArchivo = doc.NombreArchivo,
                    TipoDocumento = MapearTipoDocumento(doc.TipoDocumento),
                    TamanoArchivo = doc.TamanoArchivo,
                    FechaImportacion = doc.FechaCreacion,
                    Estado = DeterminarEstado(doc),
                    FechaEnvioValidacion = doc.SolicitudAscensoId.HasValue ? doc.FechaModificacion : null,
                    SolicitudId = doc.SolicitudAscensoId
                }).ToList();

                return Ok(archivos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener archivos importados para docente {Cedula}", 
                    ObtenerCedulaUsuario());
                return StatusCode(500, new { message = $"Error interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Enviar archivo importado para validación
        /// </summary>
        [HttpPost("{archivoId}/enviar-validacion")]
        [Authorize(Roles = "Docente")]
        public async Task<ActionResult> EnviarArchivoParaValidacion(
            Guid archivoId, 
            [FromBody] EnviarValidacionRequestDto request)
        {
            try
            {
                var cedula = ObtenerCedulaUsuario();
                if (string.IsNullOrEmpty(cedula))
                    return Unauthorized("No se pudo obtener la cédula del usuario");

                var docente = await _docenteRepository.GetByCedulaAsync(cedula);
                if (docente == null)
                    return NotFound("Docente no encontrado");

                var documento = await _documentoRepository.GetByIdAsync(archivoId);
                if (documento == null)
                    return NotFound("Archivo no encontrado");

                if (documento.DocenteId != docente.Id)
                    return Forbid("No tiene permisos para enviar este archivo");

                if (documento.SolicitudAscensoId.HasValue)
                    return BadRequest("Este archivo ya fue enviado para validación");

                // Aquí se debería crear una nueva solicitud o agregar a una existente
                // Por ahora, simulamos que se envía marcando el documento
                documento.SolicitudAscensoId = Guid.NewGuid(); // Temporal
                documento.FechaModificacion = DateTime.UtcNow;
                
                await _documentoRepository.UpdateAsync(documento);

                return Ok(new { message = "Archivo enviado para validación exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar archivo {ArchivoId} para validación", archivoId);
                return StatusCode(500, new { message = $"Error interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Visualizar archivo importado
        /// </summary>
        [HttpGet("{archivoId}/visualizar")]
        [Authorize(Roles = "Docente")]
        public async Task<ActionResult> VisualizarArchivo(Guid archivoId)
        {
            try
            {
                var cedula = ObtenerCedulaUsuario();
                if (string.IsNullOrEmpty(cedula))
                    return Unauthorized("No se pudo obtener la cédula del usuario");

                var docente = await _docenteRepository.GetByCedulaAsync(cedula);
                if (docente == null)
                    return NotFound("Docente no encontrado");

                var documento = await _documentoRepository.GetByIdAsync(archivoId);
                if (documento == null)
                    return NotFound("Archivo no encontrado");

                if (documento.DocenteId != docente.Id)
                    return Forbid("No tiene permisos para ver este archivo");

                if (documento.ContenidoArchivo == null || documento.ContenidoArchivo.Length == 0)
                    return NotFound("Contenido del archivo no encontrado");

                // Configurar headers para visualización en iframe
                Response.Headers["Content-Disposition"] = "inline";
                Response.Headers["Content-Type"] = "application/pdf";
                Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                Response.Headers["Pragma"] = "no-cache";
                Response.Headers["Expires"] = "0";

                return File(documento.ContenidoArchivo, "application/pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al visualizar archivo {ArchivoId}", archivoId);
                return StatusCode(500, new { message = $"Error interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Descargar archivo importado
        /// </summary>
        [HttpGet("{archivoId}/descargar")]
        [Authorize(Roles = "Docente")]
        public async Task<ActionResult> DescargarArchivo(Guid archivoId)
        {
            try
            {
                var cedula = ObtenerCedulaUsuario();
                if (string.IsNullOrEmpty(cedula))
                    return Unauthorized("No se pudo obtener la cédula del usuario");

                var docente = await _docenteRepository.GetByCedulaAsync(cedula);
                if (docente == null)
                    return NotFound("Docente no encontrado");

                var documento = await _documentoRepository.GetByIdAsync(archivoId);
                if (documento == null)
                    return NotFound("Archivo no encontrado");

                if (documento.DocenteId != docente.Id)
                    return Forbid("No tiene permisos para descargar este archivo");

                if (documento.ContenidoArchivo == null || documento.ContenidoArchivo.Length == 0)
                    return NotFound("Contenido del archivo no encontrado");

                // Configurar headers para descarga
                Response.Headers["Content-Disposition"] = $"attachment; filename=\"{documento.NombreArchivo}\"";
                Response.Headers["Content-Type"] = "application/pdf";

                return File(documento.ContenidoArchivo, "application/pdf", documento.NombreArchivo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al descargar archivo {ArchivoId}", archivoId);
                return StatusCode(500, new { message = $"Error interno: {ex.Message}" });
            }
        }

        private string ObtenerCedulaUsuario()
        {
            return User.FindFirst("cedula")?.Value ?? string.Empty;
        }

        private static string MapearTipoDocumento(SGA.Domain.Enums.TipoDocumento tipo)
        {
            return tipo switch
            {
                SGA.Domain.Enums.TipoDocumento.ObrasAcademicas => "Obra Académica",
                SGA.Domain.Enums.TipoDocumento.CertificadosCapacitacion => "Certificado de Capacitación",
                SGA.Domain.Enums.TipoDocumento.CertificadoInvestigacion => "Evidencia de Investigación",
                SGA.Domain.Enums.TipoDocumento.CertificadoTrabajo => "Certificado de Trabajo",
                SGA.Domain.Enums.TipoDocumento.EvaluacionesDocentes => "Evaluación Docente",
                _ => "Otro"
            };
        }

        private static string DeterminarEstado(SGA.Domain.Entities.Documento documento)
        {
            if (!documento.SolicitudAscensoId.HasValue)
                return "Importado";
            
            // Aquí se debería verificar el estado real de la solicitud
            // Por ahora retornamos un estado simulado
            return "En Revisión";
        }
    }
}
