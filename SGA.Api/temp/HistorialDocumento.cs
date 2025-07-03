using System;
using System.Collections.Generic;

namespace SGA.Api.temp;

public partial class HistorialDocumento
{
    public Guid Id { get; set; }

    public Guid ObraAcademicaId { get; set; }

    public string Accion { get; set; } = null!;

    public string EstadoAnterior { get; set; } = null!;

    public string EstadoNuevo { get; set; } = null!;

    public string? Comentarios { get; set; }

    public string? DetallesAdicionales { get; set; }

    public Guid RealizadoPorId { get; set; }

    public string RealizadoPorNombre { get; set; } = null!;

    public string RolUsuario { get; set; } = null!;

    public DateTime FechaAccion { get; set; }

    public string? DireccionIp { get; set; }

    public string? UserAgent { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public virtual ObrasAcademica ObraAcademica { get; set; } = null!;
}
