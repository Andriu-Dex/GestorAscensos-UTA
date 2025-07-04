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
    /// Inicializar conexión SignalR con validación previa del servidor
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

            // Usar la URL base del HttpClient para construir la URL absoluta del hub
            var baseAddress = _httpClient.BaseAddress?.ToString().TrimEnd('/') ?? "https://localhost:7030";
            var hubUrl = $"{baseAddress}/notificacionesHub";
            
            _logger.LogInformation("Inicializando conexión SignalR a: {HubUrl}", hubUrl);

            // Primero verificar si el servidor está disponible
            if (!await VerificarServidorDisponibleAsync(baseAddress))
            {
                _logger.LogWarning("Servidor no está disponible, postergando inicialización de SignalR");
                return;
            }

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl, options =>
                {
                    // Pasar token como query string ya que WebSockets no soporta headers
                    options.AccessTokenProvider = () => Task.FromResult<string?>(token);
                    // Configurar transporte específico para WebAssembly
                    options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets | 
                                        Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling;
                    // Configurar headers adicionales
                    options.Headers.Add("User-Agent", "SGA-Blazor-Client");
                })
                .WithAutomaticReconnect(new[] { TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(2), 
                                               TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(30) })
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
        catch (HttpRequestException httpEx)
        {
            _logger.LogError(httpEx, "❌ Error de conectividad SignalR - Servidor no disponible. La aplicación funcionará sin notificaciones en tiempo real.");
            // No relanzar para permitir que la aplicación continue sin SignalR
        }
        catch (TaskCanceledException tcEx)
        {
            _logger.LogError(tcEx, "❌ Timeout en conexión SignalR. La aplicación funcionará sin notificaciones en tiempo real.");
            // No relanzar para permitir que la aplicación continue sin SignalR
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al inicializar conexión SignalR. La aplicación funcionará sin notificaciones en tiempo real.");
            // No relanzar para permitir que la aplicación continue sin SignalR
        }
    }

    /// <summary>
    /// Verificar si el servidor API está disponible antes de intentar conectar SignalR
    /// </summary>
    private async Task<bool> VerificarServidorDisponibleAsync(string baseAddress)
    {
        try
        {
            _logger.LogInformation("Verificando disponibilidad del servidor: {BaseAddress}", baseAddress);
            
            // Usar el endpoint de salud para verificar disponibilidad
            var response = await _httpClient.GetAsync($"{baseAddress}/api/health");
            var isAvailable = response.IsSuccessStatusCode;
            
            _logger.LogInformation("Servidor {Estado}: {StatusCode}", 
                isAvailable ? "DISPONIBLE" : "NO DISPONIBLE", response.StatusCode);
                
            return isAvailable;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "No se pudo verificar disponibilidad del servidor");
            return false;
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
                _logger.LogInformation("📨 Nueva notificación recibida: {Titulo}", notificacion.Titulo);

                // Notificar a componentes suscritos primero
                NotificacionRecibida?.Invoke(notificacion);
                
                // Actualizar contador
                var nuevoContador = await ObtenerContadorNoLeidasAsync();
                ContadorActualizado?.Invoke(nuevoContador);

                _logger.LogInformation("✅ Notificación procesada correctamente");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al procesar notificación recibida");
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

    /// <summary>
    /// Intentar reconectar SignalR si no está conectado
    /// </summary>
    public async Task IntentarReconectarAsync()
    {
        if (_hubConnection == null || _hubConnection.State == HubConnectionState.Disconnected)
        {
            _logger.LogInformation("Intentando reconectar SignalR...");
            await InicializarAsync();
        }
        else
        {
            _logger.LogInformation("SignalR ya está conectado. Estado: {Estado}", _hubConnection.State);
        }
    }

    /// <summary>
    /// Verificar el estado de la conexión SignalR
    /// </summary>
    public HubConnectionState? ObtenerEstadoConexion()
    {
        return _hubConnection?.State;
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
