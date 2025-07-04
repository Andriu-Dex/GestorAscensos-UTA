using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace SGA.Api.Hubs;

/// <summary>
/// Hub de SignalR para notificaciones en tiempo real
/// </summary>
[Authorize]
public class NotificacionesHub : Hub
{
    private readonly ILogger<NotificacionesHub> _logger;

    public NotificacionesHub(ILogger<NotificacionesHub> logger)
    {
        _logger = logger;
    }

    public async Task JoinUserGroup()
    {
        try
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            _logger.LogInformation("👤 SIGNALR: Intentando unir usuario a grupo personal");
            _logger.LogInformation("👤 SIGNALR: ConnectionId: {ConnectionId}", Context.ConnectionId);
            _logger.LogInformation("👤 SIGNALR: UserId del claim: {UserId}", userId ?? "NULL");
            
            // Si no hay userId en las claims, usar el email como fallback
            if (string.IsNullOrEmpty(userId))
            {
                var email = Context.User?.FindFirst(ClaimTypes.Email)?.Value 
                           ?? Context.User?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
                
                _logger.LogInformation("👤 SIGNALR: Email encontrado como fallback: {Email}", email ?? "NULL");
                
                if (!string.IsNullOrEmpty(email))
                {
                    userId = email; // Usar email como identificador temporal
                    _logger.LogInformation("👤 SIGNALR: Usando email como userId: {Email}", email);
                }
            }
            
            if (!string.IsNullOrEmpty(userId))
            {
                var groupName = $"User_{userId}";
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                _logger.LogInformation("✅ SIGNALR: Usuario {UserId} agregado al grupo {GroupName} con conexión {ConnectionId}", 
                    userId, groupName, Context.ConnectionId);
                
                // También agregar al grupo con el userId sin formato para casos donde se usa distinto formato
                if (userId.Contains('@'))
                {
                    var sanitizedId = userId.Replace("@", "_at_");
                    var altGroupName = $"User_{sanitizedId}";
                    await Groups.AddToGroupAsync(Context.ConnectionId, altGroupName);
                    _logger.LogInformation("✅ SIGNALR: Usuario también agregado a grupo alternativo: {AltGroupName}", altGroupName);
                }
            }
            else
            {
                _logger.LogWarning("⚠️ SIGNALR: No se pudo obtener ID de usuario para la conexión {ConnectionId}", Context.ConnectionId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ SIGNALR: Error al unir usuario al grupo de notificaciones");
        }
    }

    public async Task JoinAdminGroup()
    {
        try
        {
            var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
            _logger.LogInformation("👨‍💼 SIGNALR: Verificando si el usuario es administrador");
            _logger.LogInformation("👨‍💼 SIGNALR: ConnectionId: {ConnectionId}", Context.ConnectionId);
            _logger.LogInformation("👨‍💼 SIGNALR: Rol del usuario: {Role}", userRole ?? "NULL");
            
            if (userRole == "Administrador")
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
                _logger.LogInformation("✅ SIGNALR: Administrador agregado al grupo 'Admins' con conexión {ConnectionId}", Context.ConnectionId);
            }
            else
            {
                _logger.LogInformation("ℹ️ SIGNALR: Usuario no es administrador, no se agrega al grupo 'Admins'");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ SIGNALR: Error al unir administrador al grupo");
        }
    }

    public override async Task OnConnectedAsync()
    {
        try
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = Context.User?.FindFirst(ClaimTypes.Email)?.Value;
            var role = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
            
            _logger.LogInformation("🔗 SIGNALR: Nueva conexión establecida");
            _logger.LogInformation("🔗 SIGNALR: ConnectionId: {ConnectionId}", Context.ConnectionId);
            _logger.LogInformation("🔗 SIGNALR: Usuario ID: {UserId}", userId ?? "NO ENCONTRADO");
            _logger.LogInformation("🔗 SIGNALR: Email: {Email}", email ?? "NO ENCONTRADO");
            _logger.LogInformation("🔗 SIGNALR: Rol: {Role}", role ?? "NO ENCONTRADO");
            
            await JoinUserGroup();
            await JoinAdminGroup();
            await base.OnConnectedAsync();
            
            _logger.LogInformation("✅ SIGNALR: Conexión completada para {ConnectionId}", Context.ConnectionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ SIGNALR: Error en conexión");
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                _logger.LogInformation("Usuario {UserId} desconectado del hub de notificaciones", userId);
            }

            await base.OnDisconnectedAsync(exception);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en desconexión de SignalR");
        }
    }

    /// <summary>
    /// Marcar notificación como leída
    /// </summary>
    public async Task MarcarNotificacionLeida(string notificacionId)
    {
        try
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("Marcando notificación {NotificacionId} como leída para usuario {UserId}", 
                notificacionId, userId);
            
            // Enviar una respuesta de confirmación al cliente
            await Clients.Caller.SendAsync("NotificacionMarcadaLeida", notificacionId);
            
            // Nota: La persistencia real en BD debe hacerse desde el controlador API,
            // este método solo sirve para notificar en tiempo real al frontend
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al marcar notificación como leída");
            throw; // Re-lanzar excepción para que el cliente sepa que hubo un error
        }
    }
}
