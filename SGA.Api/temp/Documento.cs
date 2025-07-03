using System;
using System.Collections.Generic;

namespace SGA.Api.temp;

public partial class Documento
{
    public Guid Id { get; set; }

    public string NombreArchivo { get; set; } = null!;

    public string RutaArchivo { get; set; } = null!;

    public long TamanoArchivo { get; set; }

    public string TipoDocumento { get; set; } = null!;

    public byte[] ContenidoArchivo { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public Guid? SolicitudAscensoId { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public Guid DocenteId { get; set; }

    public virtual Docente Docente { get; set; } = null!;

    public virtual SolicitudesAscenso? SolicitudAscenso { get; set; }
}
