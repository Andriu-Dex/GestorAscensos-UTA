using System;
using System.Collections.Generic;

namespace SGA.Api.temp;

public partial class LogsAuditorium
{
    public Guid Id { get; set; }

    public string Accion { get; set; } = null!;

    public string? UsuarioId { get; set; }

    public string? UsuarioEmail { get; set; }

    public string? EntidadAfectada { get; set; }

    public string? ValoresAnteriores { get; set; }

    public string? ValoresNuevos { get; set; }

    public string? DireccionIp { get; set; }

    public DateTime FechaAccion { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }
}
