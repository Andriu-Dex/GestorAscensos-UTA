using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.Toast.Services;
using System.Text.Json;
using System.Security.Claims;

namespace SGA.Web.Services;

/// <summary>
/// Servicio para gestionar notificaciones en tiempo real en Blazor
/// </summary>
public class NotificacionesService : IAsyncDisposable
{
    private readonly HttpClient _httpClient;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly IToastService _toastService;
    private readonly ILogger<NotificacionesService> _logger;
    private readonly SGA.Web.Services.ILocalStorageService _localStorage;
    private HubConnection? _hubConnection;
    
    // Eventos para notificaciones
    public event Action<NotificacionDto>? NotificacionRecibida;
    public event Action<int>? ContadorActualizado;

    public NotificacionesService(
        HttpClient httpClient,
        AuthenticationStateProvider authStateProvider,
        IToastService toastService,
        ILogger<NotificacionesService> logger,
        SGA.Web.Services.ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _authStateProvider = authStateProvider;
        _toastService = toastService;
        _logger = logger;
        _localStorage = localStorage;
    }

    /// <summary>
    /// Inicializar conexión SignalR
    /// </summary>
    public async Task InicializarAsync()
    {
        try
        {
            _logger.LogInformation("Iniciando configuración de SignalR para notificaciones en tiempo real");
            
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            if (!authState.User?.Identity?.IsAuthenticated == true)
            {
                _logger.LogWarning("No se puede inicializar SignalR: Usuario no autenticado");
                return;
            }

            var token = await ObtenerTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("No se puede inicializar SignalR: No se pudo obtener token");
                return;
            }

            // Usar URL relativa para que funcione tanto en desarrollo como en producción
            var hubUrl = "/notificacionesHub";
            
            _logger.LogInformation("Inicializando conexión SignalR a: {HubUrl} con token: {TokenStart}...", 
                hubUrl, token.Substring(0, Math.Min(token.Length, 20)));

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl, options =>
                {
                    // Pasar token como query string ya que WebSockets no soporta headers
                    options.AccessTokenProvider = () => Task.FromResult<string?>(token);
                })
                .WithAutomaticReconnect(new[] { TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(2), 
                                               TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10) })
                .Build();

            // Configurar eventos del hub
            _hubConnection.On<object>("RecibirNotificacion", OnNotificacionRecibida);
            _hubConnection.On<string>("NotificacionMarcadaLeida", id => 
                _logger.LogInformation("Notificación {Id} marcada como leída", id));
            
            // Eventos de conexión
            _hubConnection.Reconnecting += OnReconnecting;
            _hubConnection.Reconnected += OnReconnected;
            _hubConnection.Closed += OnClosed;

            await _hubConnection.StartAsync();
            _logger.LogInformation("Conexión SignalR iniciada correctamente con ConnectionId: {ConnectionId}", 
                _hubConnection.ConnectionId);
            
            // Unirse a grupos después de conectar
            await UnirseAGrupos();
            
            _logger.LogInformation("✅ Conexión SignalR establecida con ID: {ConnectionId}", 
                _hubConnection.ConnectionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al inicializar conexión SignalR");
        }
    }

    /// <summary>
    /// Obtener notificaciones del usuario
    /// </summary>
    public async Task<List<NotificacionDto>> ObtenerNotificacionesAsync(int limit = 20)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/notificaciones?limit={limit}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<NotificacionDto>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<NotificacionDto>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener notificaciones");
        }
        
        return new List<NotificacionDto>();
    }

    /// <summary>
    /// Obtener contador de notificaciones no leídas
    /// </summary>
    public async Task<int> ObtenerContadorNoLeidasAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/notificaciones/contador-no-leidas");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ContadorDto>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return result?.Contador ?? 0;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener contador de notificaciones");
        }
        
        return 0;
    }

    /// <summary>
    /// Marcar notificación como leída
    /// </summary>
    public async Task MarcarComoLeidaAsync(Guid notificacionId)
    {
        try
        {
            await _httpClient.PutAsync($"api/notificaciones/{notificacionId}/marcar-leida", null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al marcar notificación como leída");
        }
    }

    /// <summary>
    /// Marcar todas las notificaciones como leídas
    /// </summary>
    public async Task MarcarTodasComoLeidasAsync()
    {
        try
        {
            await _httpClient.PutAsync("api/notificaciones/marcar-todas-leidas", null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al marcar todas las notificaciones como leídas");
        }
    }

    private async Task OnNotificacionRecibida(object notificacionObj)
    {
        try
        {
            var json = JsonSerializer.Serialize(notificacionObj);
            var notificacion = JsonSerializer.Deserialize<NotificacionDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (notificacion != null)
            {
                // Mostrar toast
                var tipoToast = notificacion.Tipo?.ToLower() switch
                {
                    "exito" or "solicitudaprobada" or "ascensoaprobado" or "certificadoaprobado" or "obraaprobada" or "evidenciaaprobada" => Blazored.Toast.Services.ToastLevel.Success,
                    "error" or "solicitudrechazada" or "ascensorechazado" or "certificadorechazado" or "obrarechazada" or "evidenciarechazada" => Blazored.Toast.Services.ToastLevel.Error,
                    "advertencia" => Blazored.Toast.Services.ToastLevel.Warning,
                    _ => Blazored.Toast.Services.ToastLevel.Info
                };

                _toastService.ShowToast(tipoToast, notificacion.Mensaje);

                // Notificar a componentes suscritos
                NotificacionRecibida?.Invoke(notificacion);
                
                // Actualizar contador
                var nuevoContador = await ObtenerContadorNoLeidasAsync();
                ContadorActualizado?.Invoke(nuevoContador);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar notificación recibida");
        }
    }

    private async Task UnirseAGrupos()
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            try
            {
                _logger.LogInformation("Intentando unirse a grupos de SignalR...");
                
                await _hubConnection.InvokeAsync("JoinUserGroup");
                _logger.LogInformation("Usuario unido al grupo de usuarios específico");
                
                await _hubConnection.InvokeAsync("JoinAdminGroup"); 
                _logger.LogInformation("Solicitud de unión al grupo de administradores enviada (solo se unirá si es admin)");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al unirse a grupos SignalR");
            }
        }
        else
        {
            _logger.LogWarning("No se pudo unir a grupos: SignalR no está conectado. Estado actual: {Estado}", 
                _hubConnection?.State.ToString() ?? "null");
        }
    }

    private async Task<string?> ObtenerTokenAsync()
    {
        try
        {
            // Obtener token desde localStorage (fuente primaria)
            var token = await _localStorage.GetItemAsync<string>("authToken");
            
            if (!string.IsNullOrEmpty(token))
            {
                _logger.LogInformation("✅ Token obtenido desde localStorage para SignalR: {TokenStart}...", 
                    token.Substring(0, Math.Min(token.Length, 20)));
                return token;
            }
            
            // Log para depuración
            _logger.LogWarning("⚠️ No se encontró token en localStorage. Intentando obtenerlo de AuthStateProvider...");
            
            // Si no hay token en localStorage, no crear tokens temporales
            // ya que podrían no ser aceptados por el hub
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener token para SignalR");
            return null;
        }
    }

    private async Task<string?> GetTokenFromAuthStateAsync()
    {
        try
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            if (authState.User?.Identity?.IsAuthenticated == true)
            {
                return await ObtenerTokenAsync();
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    private Task OnReconnecting(Exception? exception)
    {
        _logger.LogWarning("🔄 Reconectando SignalR...");
        return Task.CompletedTask;
    }

    private async Task OnReconnected(string? connectionId)
    {
        _logger.LogInformation("✅ SignalR reconectado");
        await UnirseAGrupos();
    }

    private Task OnClosed(Exception? exception)
    {
        _logger.LogWarning("❌ Conexión SignalR cerrada");
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.DisposeAsync();
        }
    }
}

public class NotificacionDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public string? Tipo { get; set; }
    public string? UrlAccion { get; set; }
    public DateTime FechaCreacion { get; set; }
    public string? Icono { get; set; }
    public string? Color { get; set; }
    public bool Leida { get; set; }
}

public class ContadorDto
{
    public int Contador { get; set; }
}
