using System;
using System.Collections.Generic;

namespace SGA.Api.temp;

public partial class SolicitudesObrasAcademica
{
    public Guid Id { get; set; }

    public Guid DocenteId { get; set; }

    public string DocenteCedula { get; set; } = null!;

    public string Titulo { get; set; } = null!;

    public string TipoObra { get; set; } = null!;

    public DateTime FechaPublicacion { get; set; }

    public string? Editorial { get; set; }

    public string? Revista { get; set; }

    public string? IsbnIssn { get; set; }

    public string? Doi { get; set; }

    public bool EsIndexada { get; set; }

    public string? IndiceIndexacion { get; set; }

    public string? Autores { get; set; }

    public string? Descripcion { get; set; }

    public string? ArchivoNombre { get; set; }

    public string? ArchivoRuta { get; set; }

    public string? ArchivoTipo { get; set; }

    public long? ArchivoTamano { get; set; }

    public string Estado { get; set; } = null!;

    public string? ComentariosRevision { get; set; }

    public Guid? RevisadoPorId { get; set; }

    public DateTime? FechaRevision { get; set; }

    public string? MotivoRechazo { get; set; }

    public string? ComentariosSolicitud { get; set; }

    public Guid SolicitudGrupoId { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public long? ArchivoTamanoOriginal { get; set; }

    public byte[]? ContenidoArchivo { get; set; }

    public virtual Docente Docente { get; set; } = null!;

    public virtual Usuario? RevisadoPor { get; set; }
}
