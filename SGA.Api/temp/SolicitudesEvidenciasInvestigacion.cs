using System;
using System.Collections.Generic;

namespace SGA.Api.temp;

public partial class SolicitudesEvidenciasInvestigacion
{
    public Guid Id { get; set; }

    public Guid DocenteId { get; set; }

    public string DocenteCedula { get; set; } = null!;

    public string TipoEvidencia { get; set; } = null!;

    public string TituloProyecto { get; set; } = null!;

    public string InstitucionFinanciadora { get; set; } = null!;

    public string RolInvestigador { get; set; } = null!;

    public DateTime FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public int MesesDuracion { get; set; }

    public string? CodigoProyecto { get; set; }

    public string? AreaTematica { get; set; }

    public string? Descripcion { get; set; }

    public string ArchivoNombre { get; set; } = null!;

    public long ArchivoTamano { get; set; }

    public string ArchivoTipo { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public string? ComentariosRevision { get; set; }

    public string? MotivoRechazo { get; set; }

    public DateTime? FechaRevision { get; set; }

    public string? ComentariosSolicitud { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public byte[] ArchivoContenido { get; set; } = null!;

    public bool ArchivoEstaComprimido { get; set; }

    public long ArchivoTamanoComprimido { get; set; }

    public virtual Docente Docente { get; set; } = null!;
}
