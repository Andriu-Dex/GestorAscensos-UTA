using System;
using System.Collections.Generic;

namespace SGA.Api.temp;

public partial class ObrasAcademica
{
    public Guid Id { get; set; }

    public Guid DocenteId { get; set; }

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

    public string? NombreArchivo { get; set; }

    public byte[]? ContenidoArchivoPdf { get; set; }

    public long? TamanoArchivo { get; set; }

    public string? ContentType { get; set; }

    public string OrigenDatos { get; set; } = null!;

    public bool EsVerificada { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public string? ComentariosAdmin { get; set; }

    public string? ComentariosDocente { get; set; }

    public string Estado { get; set; } = null!;

    public DateTime? FechaUltimaModificacion { get; set; }

    public Guid? ModificadoPorId { get; set; }

    public virtual Docente Docente { get; set; } = null!;

    public virtual ICollection<HistorialDocumento> HistorialDocumentos { get; set; } = new List<HistorialDocumento>();
}
