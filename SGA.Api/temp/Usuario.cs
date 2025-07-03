using System;
using System.Collections.Generic;

namespace SGA.Api.temp;

public partial class Usuario
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Rol { get; set; } = null!;

    public bool EstaActivo { get; set; }

    public int IntentosLogin { get; set; }

    public DateTime? UltimoBloqueado { get; set; }

    public DateTime UltimoLogin { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public virtual Docente? Docente { get; set; }

    public virtual ICollection<SolicitudesAscenso> SolicitudesAscensos { get; set; } = new List<SolicitudesAscenso>();

    public virtual ICollection<SolicitudesCertificadosCapacitacion> SolicitudesCertificadosCapacitacions { get; set; } = new List<SolicitudesCertificadosCapacitacion>();

    public virtual ICollection<SolicitudesObrasAcademica> SolicitudesObrasAcademicas { get; set; } = new List<SolicitudesObrasAcademica>();
}
