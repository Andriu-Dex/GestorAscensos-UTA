using SGA.Domain.Common;

namespace SGA.Domain.Entities;

public class LogAuditoria : BaseEntity
{
    public string Accion { get; set; } = string.Empty;
    public string? UsuarioId { get; set; }
    public string? UsuarioEmail { get; set; }
    public string? EntidadAfectada { get; set; }
    public string? ValoresAnteriores { get; set; }
    public string? ValoresNuevos { get; set; }
    public string? DireccionIP { get; set; }
    public DateTime FechaAccion { get; set; }
}
