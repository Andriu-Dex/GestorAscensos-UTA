using SGA.Domain.Common;
using SGA.Domain.Enums;

namespace SGA.Domain.Entities;

public class Usuario : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public RolUsuario Rol { get; set; }
    public bool EstaActivo { get; set; } = true;
    public int IntentosLogin { get; set; } = 0;
    public DateTime? UltimoBloqueado { get; set; }
    public DateTime UltimoLogin { get; set; }
    
    // Relación con Docente
    public virtual Docente? Docente { get; set; }
    
    // Relación con Notificaciones
    public virtual ICollection<Notificacion> Notificaciones { get; set; } = new List<Notificacion>();
}
