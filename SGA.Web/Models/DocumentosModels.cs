namespace SGA.Web.Models
{
    // DTOs para obras académicas
    public class NuevaObraAcademicaDto
    {
        public string Titulo { get; set; } = string.Empty;
        public string TipoObra { get; set; } = string.Empty;
        public DateTime FechaPublicacion { get; set; } = DateTime.Today;
        public string? Editorial { get; set; }
        public string? Revista { get; set; }
        public string? ISBN_ISSN { get; set; }
        public string? DOI { get; set; }
        public bool EsIndexada { get; set; }
        public string? IndiceIndexacion { get; set; }
        public string? Autores { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? ArchivoNombre { get; set; }
        public string? ArchivoContenido { get; set; }
        public string? ArchivoTipo { get; set; }
    }

    public class SolicitudObrasAcademicasDto
    {
        public List<NuevaObraAcademicaDto> Obras { get; set; } = new();
        public string? Comentarios { get; set; }
    }

    public class ObraAcademicaDetalleDto
    {
        public int Id { get; set; }
        public Guid? SolicitudId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string TipoObra { get; set; } = string.Empty;
        public DateTime FechaPublicacion { get; set; }
        public string? Editorial { get; set; }
        public string? Revista { get; set; }
        public string? ISBN_ISSN { get; set; }
        public string? DOI { get; set; }
        public bool EsIndexada { get; set; }
        public string? IndiceIndexacion { get; set; }
        public string? Autores { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? ArchivoNombre { get; set; }
        public bool TieneArchivo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public DateTime? FechaRevision { get; set; }
        public string? Estado { get; set; }
        public string? ComentariosRevision { get; set; }
        public string? MotivoRechazo { get; set; }
    }

    public class ResponseObrasAcademicasDto
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public List<ObraAcademicaDetalleDto> Obras { get; set; } = new();
        public int TotalObras { get; set; }
    }

    public class EditarMetadatosSolicitudDto
    {
        public string? Titulo { get; set; }
        public string? Descripcion { get; set; }
        public string? TipoObra { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        public string? Editorial { get; set; }
        public string? Revista { get; set; }
        public string? ISBN_ISSN { get; set; }
        public string? DOI { get; set; }
        public bool? EsIndexada { get; set; }
        public string? IndiceIndexacion { get; set; }
        public string? Autores { get; set; }
    }

    public class ReemplazarArchivoDto
    {
        public string ArchivoNombre { get; set; } = string.Empty;
        public string ArchivoContenido { get; set; } = string.Empty;
        public string ArchivoTipo { get; set; } = "application/pdf";
    }

    // DTOs para certificados de capacitación
    public class CertificadoCapacitacionDetalleDto
    {
        public Guid Id { get; set; }
        public string NombreCurso { get; set; } = string.Empty;
        public string InstitucionOfertante { get; set; } = string.Empty;
        public string TipoCapacitacion { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int HorasDuracion { get; set; }
        public string Modalidad { get; set; } = string.Empty;
        public string? NumeroRegistro { get; set; }
        public string? AreaTematica { get; set; }
        public string? Descripcion { get; set; }
        public string? ArchivoNombre { get; set; }
        public bool TieneArchivo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? MotivoRechazo { get; set; }
        public string? ComentariosRevision { get; set; }
    }

    public class CrearCertificadoCapacitacionDto
    {
        public string NombreCurso { get; set; } = string.Empty;
        public string InstitucionOfertante { get; set; } = string.Empty;
        public string TipoCapacitacion { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; } = DateTime.Now;
        public DateTime FechaFin { get; set; } = DateTime.Now;
        public int HorasDuracion { get; set; }
        public string Modalidad { get; set; } = string.Empty;
        public string? NumeroRegistro { get; set; }
        public string? AreaTematica { get; set; }
        public string? Descripcion { get; set; }
        public string? ArchivoNombre { get; set; }
        public string? ArchivoContenido { get; set; }
        public string ArchivoTipo { get; set; } = "application/pdf";
    }

    public class SolicitarCertificadosCapacitacionDto
    {
        public List<CrearCertificadoCapacitacionDto> Certificados { get; set; } = new();
    }

    public class ResponseCertificadosCapacitacionDto
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public List<CertificadoCapacitacionDetalleDto> Certificados { get; set; } = new();
        public int TotalCertificados { get; set; }
    }

    public class EditarMetadatosCertificadoDto
    {
        public string? NombreCurso { get; set; }
        public string? InstitucionOfertante { get; set; }
        public string? TipoCapacitacion { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? HorasDuracion { get; set; }
        public string? Modalidad { get; set; }
        public string? NumeroRegistro { get; set; }
        public string? AreaTematica { get; set; }
        public string? Descripcion { get; set; }
    }

    public class ReemplazarArchivoCertificadoDto
    {
        public string ArchivoNombre { get; set; } = string.Empty;
        public string ArchivoContenido { get; set; } = string.Empty;
        public string ArchivoTipo { get; set; } = "application/pdf";
    }

    // DTOs para documentos
    public class DocumentoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string RutaArchivo { get; set; } = string.Empty;
        public int TipoDocumentoId { get; set; }
        public TipoDocumentoDto? TipoDocumento { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Verificado { get; set; }
    }

    public class TipoDocumentoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }

    // DTOs genéricos
    public class ResponseGenericoDto
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }

    public class ResponseGenericoCertificadoDto
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public object? Datos { get; set; }
    }

    // DTOs para evidencias de investigación
    public class EvidenciaInvestigacionViewModel
    {
        public Guid Id { get; set; }
        public string DocenteCedula { get; set; } = string.Empty;
        public string TipoEvidencia { get; set; } = string.Empty;
        public string TituloProyecto { get; set; } = string.Empty;
        public string InstitucionFinanciadora { get; set; } = string.Empty;
        public string RolInvestigador { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int MesesDuracion { get; set; }
        public string? CodigoProyecto { get; set; }
        public string? AreaTematica { get; set; }
        public string? Descripcion { get; set; }
        public string ArchivoNombre { get; set; } = string.Empty;
        public bool TieneArchivo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? MotivoRechazo { get; set; }
        public string? ComentariosRevision { get; set; }
        public string? ComentariosSolicitud { get; set; }
        public DateTime? FechaRevision { get; set; }
    }

    public class CrearEvidenciaInvestigacionDto
    {
        public string TipoEvidencia { get; set; } = string.Empty;
        public string TituloProyecto { get; set; } = string.Empty;
        public string InstitucionFinanciadora { get; set; } = string.Empty;
        public string RolInvestigador { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; } = DateTime.Now;
        public DateTime? FechaFin { get; set; }
        public int MesesDuracion { get; set; }
        public string? CodigoProyecto { get; set; }
        public string? AreaTematica { get; set; }
        public string? Descripcion { get; set; }
        public string? ComentariosSolicitud { get; set; }
        public string? ArchivoNombre { get; set; }
        public string? ArchivoContenido { get; set; }
        public string ArchivoTipo { get; set; } = "application/pdf";
    }

    public class SolicitarEvidenciasInvestigacionDto
    {
        public List<CrearEvidenciaInvestigacionDto> Evidencias { get; set; } = new();
    }

    public class ResponseEvidenciasInvestigacionDto
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public List<EvidenciaInvestigacionViewModel> Evidencias { get; set; } = new();
        public int TotalEvidencias { get; set; }
    }

    public class EditarMetadatosEvidenciaDto
    {
        public string? TipoEvidencia { get; set; }
        public string? TituloProyecto { get; set; }
        public string? InstitucionFinanciadora { get; set; }
        public string? RolInvestigador { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? MesesDuracion { get; set; }
        public string? CodigoProyecto { get; set; }
        public string? AreaTematica { get; set; }
        public string? Descripcion { get; set; }
    }

    public class ReemplazarArchivoEvidenciaDto
    {
        public string ArchivoNombre { get; set; } = string.Empty;
        public string ArchivoContenido { get; set; } = string.Empty;
        public string ArchivoTipo { get; set; } = "application/pdf";
    }

    public class ResponseGenericoEvidenciaDto
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public object? Datos { get; set; }
    }

    public class ResponseSolicitudesEvidenciasAdminDto
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public List<EvidenciaInvestigacionViewModel> Evidencias { get; set; } = new();
        public int TotalEvidencias { get; set; }
    }

    public class RevisionSolicitudEvidenciaDto
    {
        public Guid SolicitudId { get; set; }
        public string Accion { get; set; } = string.Empty; // "Aprobar" o "Rechazar"
        public string? Comentarios { get; set; }
    }
}
