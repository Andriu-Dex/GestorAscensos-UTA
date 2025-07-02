using System.ComponentModel.DataAnnotations;

namespace SGA.Application.DTOs;

public class NuevaObraAcademicaDto
{
    [Required(ErrorMessage = "El título es requerido")]
    [StringLength(500, ErrorMessage = "El título no puede exceder 500 caracteres")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tipo de obra es requerido")]
    [StringLength(100, ErrorMessage = "El tipo de obra no puede exceder 100 caracteres")]
    public string TipoObra { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de publicación es requerida")]
    public DateTime FechaPublicacion { get; set; }

    [StringLength(255, ErrorMessage = "La editorial no puede exceder 255 caracteres")]
    public string? Editorial { get; set; }

    [StringLength(255, ErrorMessage = "La revista no puede exceder 255 caracteres")]
    public string? Revista { get; set; }

    [StringLength(50, ErrorMessage = "El ISBN/ISSN no puede exceder 50 caracteres")]
    public string? ISBN_ISSN { get; set; }

    [StringLength(200, ErrorMessage = "El DOI no puede exceder 200 caracteres")]
    public string? DOI { get; set; }

    public bool EsIndexada { get; set; }

    [StringLength(100, ErrorMessage = "El índice de indexación no puede exceder 100 caracteres")]
    public string? IndiceIndexacion { get; set; }

    [StringLength(1000, ErrorMessage = "Los autores no pueden exceder 1000 caracteres")]
    public string? Autores { get; set; }

    [StringLength(2000, ErrorMessage = "La descripción no puede exceder 2000 caracteres")]
    public string? Descripcion { get; set; }

    // Para el archivo PDF
    public string? ArchivoNombre { get; set; }
    public string? ArchivoContenido { get; set; } // Base64
    public string? ArchivoTipo { get; set; }
}

public class SolicitudObrasAcademicasDto
{
    [Required(ErrorMessage = "Al menos una obra es requerida")]
    [MinLength(1, ErrorMessage = "Debe incluir al menos una obra")]
    public List<NuevaObraAcademicaDto> Obras { get; set; } = new();

    [StringLength(1000, ErrorMessage = "Los comentarios no pueden exceder 1000 caracteres")]
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
    public string? Autores { get; set; }
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

// DTOs para administración
public class SolicitudObraAcademicaAdminDto
{
    public Guid Id { get; set; }
    public Guid SolicitudGrupoId { get; set; }
    public string DocenteCedula { get; set; } = string.Empty;
    public string DocenteNombre { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string TipoObra { get; set; } = string.Empty;
    public DateTime FechaPublicacion { get; set; }
    public string? Editorial { get; set; }
    public string? Revista { get; set; }
    public string? ISBN_ISSN { get; set; }
    public string? DOI { get; set; }
    public bool EsIndexada { get; set; }
    public string? IndiceIndexacion { get; set; }
    public string? Autores { get; set; }
    public string? Descripcion { get; set; }
    public string? ArchivoNombre { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string? ComentariosRevision { get; set; }
    public string? MotivoRechazo { get; set; }
    public string? ComentariosSolicitud { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaRevision { get; set; }
}

public class ResponseSolicitudesAdminDto
{
    public bool Exitoso { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public List<SolicitudObraAcademicaAdminDto> Solicitudes { get; set; } = new();
}

public class RevisionSolicitudDto
{
    public Guid SolicitudId { get; set; }
    public string Accion { get; set; } = string.Empty; // "Aprobar" o "Rechazar"
    public string? ComentariosRevision { get; set; }
    public string? MotivoRechazo { get; set; }
}

// DTOs para gestión de documentos del usuario
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
    public string ArchivoContenido { get; set; } = string.Empty; // Base64
    public string ArchivoTipo { get; set; } = "application/pdf";
}

public class GestionDocumentoDto
{
    public Guid SolicitudId { get; set; }
    public string Accion { get; set; } = string.Empty; // "Eliminar", "EditarMetadatos", "ReemplazarArchivo", "AgregarComentario", "Reenviar"
    public EditarMetadatosSolicitudDto? Metadatos { get; set; }
    public ReemplazarArchivoDto? Archivo { get; set; }
    public string? Comentario { get; set; }
}

public class ResponseGenericoDto
{
    public bool Exitoso { get; set; }
    public string Mensaje { get; set; } = string.Empty;
}
