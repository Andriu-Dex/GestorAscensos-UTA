using System;
using System.Collections.Generic;

namespace SGA.Api.temp;

public partial class Docente
{
    public Guid Id { get; set; }

    public string Cedula { get; set; } = null!;

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string NivelActual { get; set; } = null!;

    public DateTime FechaInicioNivelActual { get; set; }

    public DateTime? FechaUltimoAscenso { get; set; }

    public bool EstaActivo { get; set; }

    public DateTime? FechaNombramiento { get; set; }

    public decimal? PromedioEvaluaciones { get; set; }

    public int? HorasCapacitacion { get; set; }

    public int? NumeroObrasAcademicas { get; set; }

    public int? MesesInvestigacion { get; set; }

    public DateTime? FechaUltimaImportacion { get; set; }

    public Guid UsuarioId { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public virtual ICollection<Documento> Documentos { get; set; } = new List<Documento>();

    public virtual ICollection<ObrasAcademica> ObrasAcademicas { get; set; } = new List<ObrasAcademica>();

    public virtual ICollection<SolicitudesAscenso> SolicitudesAscensos { get; set; } = new List<SolicitudesAscenso>();

    public virtual ICollection<SolicitudesCertificadosCapacitacion> SolicitudesCertificadosCapacitacions { get; set; } = new List<SolicitudesCertificadosCapacitacion>();

    public virtual ICollection<SolicitudesEvidenciasInvestigacion> SolicitudesEvidenciasInvestigacions { get; set; } = new List<SolicitudesEvidenciasInvestigacion>();

    public virtual ICollection<SolicitudesObrasAcademica> SolicitudesObrasAcademicas { get; set; } = new List<SolicitudesObrasAcademica>();

    public virtual Usuario Usuario { get; set; } = null!;
}
