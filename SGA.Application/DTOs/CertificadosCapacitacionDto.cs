using System.ComponentModel.DataAnnotations;

namespace SGA.Application.DTOs;

// DTO para crear un nuevo certificado de capacitación
public class NuevoCertificadoCapacitacionDto
{
    [Required(ErrorMessage = "El nombre del curso es requerido")]
    [StringLength(500, ErrorMessage = "El nombre del curso no puede exceder 500 caracteres")]
    public string NombreCurso { get; set; } = string.Empty;

    [Required(ErrorMessage = "La institución oferente es requerida")]
    [StringLength(255, ErrorMessage = "La institución oferente no puede exceder 255 caracteres")]
    public string InstitucionOfertante { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tipo de capacitación es requerido")]
    [StringLength(100, ErrorMessage = "El tipo de capacitación no puede exceder 100 caracteres")]
    public string TipoCapacitacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de inicio es requerida")]
    public DateTime FechaInicio { get; set; }

    [Required(ErrorMessage = "La fecha de fin es requerida")]
    public DateTime FechaFin { get; set; }

    [Required(ErrorMessage = "Las horas de duración son requeridas")]
    [Range(1, 500, ErrorMessage = "Las horas de duración deben estar entre 1 y 500")]
    public int HorasDuracion { get; set; }

    [Required(ErrorMessage = "La modalidad es requerida")]
    [StringLength(50, ErrorMessage = "La modalidad no puede exceder 50 caracteres")]
    public string Modalidad { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "El número de registro no puede exceder 100 caracteres")]
    public string? NumeroRegistro { get; set; }

    [StringLength(200, ErrorMessage = "El área temática no puede exceder 200 caracteres")]
    public string? AreaTematica { get; set; }

    [StringLength(2000, ErrorMessage = "La descripción no puede exceder 2000 caracteres")]
    public string? Descripcion { get; set; }

    // Para el archivo PDF del certificado
    [StringLength(255, ErrorMessage = "El nombre del archivo no puede exceder 255 caracteres")]
    public string? ArchivoNombre { get; set; }

    public string? ArchivoContenido { get; set; } // Base64
    public string? ArchivoTipo { get; set; } = "application/pdf";
}

// DTO para solicitud de múltiples certificados
public class SolicitudCertificadosCapacitacionDto
{
    [Required(ErrorMessage = "Al menos un certificado es requerido")]
    [MinLength(1, ErrorMessage = "Debe incluir al menos un certificado")]
    public List<NuevoCertificadoCapacitacionDto> Certificados { get; set; } = new();

    [StringLength(1000, ErrorMessage = "Los comentarios no pueden exceder 1000 caracteres")]
    public string? Comentarios { get; set; }
}

// DTO para mostrar detalles de un certificado
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
    public DateTime? FechaActualizacion { get; set; }
    public DateTime? FechaRevision { get; set; }
    public string? Estado { get; set; }
    public string? ComentariosRevision { get; set; }
    public string? MotivoRechazo { get; set; }
    public Guid? SolicitudId { get; set; }
}

// DTO para respuesta de certificados
public class ResponseCertificadosCapacitacionDto
{
    public bool Exitoso { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public List<CertificadoCapacitacionDetalleDto> Certificados { get; set; } = new();
    public int TotalCertificados { get; set; }
}

// DTOs para administración
public class SolicitudCertificadoCapacitacionAdminDto
{
    public Guid Id { get; set; }
    public Guid SolicitudGrupoId { get; set; }
    public string DocenteCedula { get; set; } = string.Empty;
    public string DocenteNombre { get; set; } = string.Empty;
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
    public string Estado { get; set; } = string.Empty;
    public string? ComentariosRevision { get; set; }
    public string? MotivoRechazo { get; set; }
    public string? ComentariosSolicitud { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaRevision { get; set; }
}

public class ResponseSolicitudesCertificadosAdminDto
{
    public bool Exitoso { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public List<SolicitudCertificadoCapacitacionAdminDto> Solicitudes { get; set; } = new();
    public int TotalSolicitudes { get; set; }
}

public class RevisionSolicitudCertificadoDto
{
    public Guid SolicitudId { get; set; }
    public string Accion { get; set; } = string.Empty; // "Aprobar" o "Rechazar"
    public string Comentarios { get; set; } = string.Empty;
    public string? ComentariosAdicionales { get; set; }
}

// DTOs para gestión de documentos del usuario (similar a obras académicas)
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
    public string ArchivoContenido { get; set; } = string.Empty; // Base64
    public string ArchivoTipo { get; set; } = "application/pdf";
}

public class GestionDocumentoCertificadoDto
{
    public Guid SolicitudId { get; set; }
    public string Accion { get; set; } = string.Empty;
    public EditarMetadatosCertificadoDto? Metadatos { get; set; }
    public ReemplazarArchivoCertificadoDto? Archivo { get; set; }
    public string? Comentario { get; set; }
}

public class ResponseGenericoCertificadoDto
{
    public bool Exitoso { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public object? Datos { get; set; }
}

// DTO para crear un certificado (usado por el frontend)
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

// DTO que usa el frontend para solicitar certificados
public class SolicitarCertificadosCapacitacionDto
{
    public List<CrearCertificadoCapacitacionDto> Certificados { get; set; } = new();
    public string? Comentarios { get; set; }
}
