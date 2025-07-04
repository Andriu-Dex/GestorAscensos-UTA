using SGA.Domain.Common;
using SGA.Domain.Enums;

namespace SGA.Domain.Entities;

/// <summary>
/// Entidad para gestionar notificaciones en tiempo real del sistema
/// </summary>
public class Notificacion : BaseEntity
{
    public Guid UsuarioId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public TipoNotificacion Tipo { get; set; }
    public bool Leida { get; set; } = false;
    public DateTime FechaLeida { get; set; }
    public string? DatosAdicionales { get; set; } // JSON con datos extra si se necesita
    public string? UrlAccion { get; set; } // URL para redireccionar si el usuario hace clic
    
    // Relaciones
    public virtual Usuario Usuario { get; set; } = null!;
}
