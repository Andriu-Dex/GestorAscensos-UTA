using System;
using System.Collections.Generic;

namespace SGA.Api.temp;

public partial class SolicitudesAscenso
{
    public Guid Id { get; set; }

    public Guid DocenteId { get; set; }

    public string NivelActual { get; set; } = null!;

    public string NivelSolicitado { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public string? MotivoRechazo { get; set; }

    public string? Observaciones { get; set; }

    public DateTime FechaSolicitud { get; set; }

    public DateTime? FechaAprobacion { get; set; }

    public Guid? AprobadoPorId { get; set; }

    public decimal PromedioEvaluaciones { get; set; }

    public int HorasCapacitacion { get; set; }

    public int NumeroObrasAcademicas { get; set; }

    public int MesesInvestigacion { get; set; }

    public int TiempoEnNivelDias { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public virtual Usuario? AprobadoPor { get; set; }

    public virtual Docente Docente { get; set; } = null!;

    public virtual ICollection<Documento> Documentos { get; set; } = new List<Documento>();
}
