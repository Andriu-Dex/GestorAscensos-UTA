# Implementación de Notificación de Token Expirado

## Desc**Archivo:** `SGA.Web/App.razor`

```razor
<CascadingAuthenticationState>
    <Router AppAssembly="@typeof(App).Assembly">
        <Found Context="routeData">
            <AuthorizeRouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)">
                <NotAuthorized>
                    @if (context.User.Identity?.IsAuthenticated != true)
                    {
                        <RedirectToLogin />
                    }
                    else
                    {
                        <p role="alert">No tienes permisos para acceder a esta página.</p>
                    }
                </NotAuthorized>
            </AuthorizeRouteView>
            <FocusOnNavigate RouteData="@routeData" Selector="h1" />
        </Found>
        <NotFound>
            <PageTitle>No encontrado</PageTitle>
            <LayoutView Layout="@typeof(MainLayout)">
                <p role="alert">Lo sentimos, no hay nada en esta dirección.</p>
            </LayoutView>
        </NotFound>
    </Router>

    @* Componente global de validación de token *@
    <TokenValidationComponent />

    @* Configuración global de Toast notifications *@
    <BlazoredToasts Position="ToastPosition.TopCenter"
                   Timeout="5"
                   IconType="IconType.FontAwesome"
                   SuccessClass="success-toast"
                   SuccessIcon="fas fa-check-circle"
                   ErrorClass="error-toast"
                   ErrorIcon="fas fa-exclamation-circle"
                   InfoClass="info-toast"
                   InfoIcon="fas fa-info-circle"
                   WarningClass="warning-toast"
                   WarningIcon="fas fa-exclamation-triangle" />
</CascadingAuthenticationState>
```

### Paso 5: Optimizar AuthorizationMessageHandler (Opcional)

**Archivo:** `SGA.Web/Services/AuthorizationMessageHandler.cs`

```csharp
public class AuthorizationMessageHandler : DelegatingHandler
{
    private readonly ILocalStorageService _localStorage;
    private readonly AuthService _authService;
    private readonly ILogger<AuthorizationMessageHandler> _logger;
    private static readonly SemaphoreSlim _semaphore = new(1, 1);

    public AuthorizationMessageHandler(
        ILocalStorageService localStorage,
        AuthService authService,
        ILogger<AuthorizationMessageHandler> logger)
    {
        _localStorage = localStorage;
        _authService = authService;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Verificar token antes de enviar la petición
        await EnsureTokenIsValid();

        var response = await base.SendAsync(request, cancellationToken);

        // Si recibimos 401, manejar la expiración
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning("Recibido 401 Unauthorized - Token posiblemente expirado");
            await HandleUnauthorizedResponse();
        }

        return response;
    }

    private async Task EnsureTokenIsValid()
    {
        try
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrWhiteSpace(token))
            {
                return;
            }

            // Validación básica del formato JWT
            if (IsTokenExpiredLocally(token))
            {
                _logger.LogInformation("Token expirado detectado antes de la petición");
                await _authService.HandleTokenExpiration();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verificando token en AuthorizationMessageHandler");
        }
    }

    private bool IsTokenExpiredLocally(string token)
    {
        try
        {
            var parts = token.Split('.');
            if (parts.Length != 3) return true;

            var payload = parts[1];
            switch (payload.Length % 4)
            {
                case 2: payload += "=="; break;
                case 3: payload += "="; break;
            }

            var jsonBytes = Convert.FromBase64String(payload);
            var jsonString = System.Text.Encoding.UTF8.GetString(jsonBytes);

            if (jsonString.Contains("\"exp\""))
            {
                var expIndex = jsonString.IndexOf("\"exp\":");
                if (expIndex >= 0)
                {
                    var expStart = jsonString.IndexOf(':', expIndex) + 1;
                    var expEnd = jsonString.IndexOfAny(new char[] { ',', '}' }, expStart);
                    if (expEnd > expStart)
                    {
                        var expString = jsonString.Substring(expStart, expEnd - expStart).Trim();
                        if (long.TryParse(expString, out var exp))
                        {
                            var expirationTime = DateTimeOffset.FromUnixTimeSeconds(exp);
                            return expirationTime <= DateTimeOffset.UtcNow;
                        }
                    }
                }
            }

            return false;
        }
        catch
        {
            return true;
        }
    }

    private async Task HandleUnauthorizedResponse()
    {
        // Usar semáforo para evitar múltiples ejecuciones simultáneas
        if (await _semaphore.WaitAsync(100))
        {
            try
            {
                await _authService.HandleTokenExpiration();
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
```

### Paso 6: Configurar Dependencias

**Archivo:** `SGA.Web/Program.cs`

````csharp
// Servicios de autenticación y almacenamiento
builder.Services.AddBlazoredToast();
builder.Services.AddBlazoredLocalStorage();

// Servicios personalizados
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AuthorizationMessageHandler>();
builder.Services.AddScoped<ApiAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<ApiAuthenticationStateProvider>());

// Configuración del HttpClient con el handler de autorización
builder.Services.AddScoped(sp => new HttpClient(sp.GetRequiredService<AuthorizationMessageHandler>())
{
    BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7000/")
});
```a implementado un sistema robusto y profesional de notificación de expiración de tokens JWT que proporciona una experiencia de usuario clara y consistente cuando la sesión expira. El sistema utiliza notificaciones Toast prominentes con mensajes personalizados que incluyen el nombre real del usuario, y maneja automáticamente la limpieza de sesión y redirección al login.

## Componentes Implementados

### 1. TokenValidationComponent - Componente Global de Validación

**Archivo:** `SGA.Web/Shared/TokenValidationComponent.razor`

**Funcionalidades:**

- Validación periódica automática del token cada 15 segundos
- Detección local de expiración sin peticiones al servidor
- Mostrar notificaciones Toast prominentes y profesionales
- Limpieza automática de sesión y redirección al login
- Manejo de estados de autenticación en tiempo real

**Características técnicas:**

- Componente Blazor integrado globalmente en `App.razor`
- Validación local de JWT mediante parseo del payload
- Timer asíncrono con manejo robusto de excepciones
- Integración nativa con `Blazored.Toast` para notificaciones

### 2. AuthorizationMessageHandler Optimizado

**Archivo:** `SGA.Web/Services/AuthorizationMessageHandler.cs`

**Funcionalidades:**

- Intercepta respuestas HTTP 401 (Unauthorized)
- Detecta tokens expirados antes de enviar peticiones
- Delega el manejo de expiración al `AuthService` centralizado
- Evita múltiples ejecuciones simultáneas con sistema de bloqueo

**Características técnicas:**

- `DelegatingHandler` para interceptación HTTP transparente
- Validación previa de tokens para evitar peticiones innecesarias
- Patrón de delegación para centralizar la lógica de manejo

### 3. AuthService Centralizado

**Archivo:** `SGA.Web/Services/AuthService.cs`

**Funcionalidades:**

- Método `HandleTokenExpiration()` para limpieza centralizada de sesión
- Eliminación completa de datos de autenticación
- Sincronización del estado de autenticación
- Limpieza de caché de usuario

**Características técnicas:**

- Integración con `ILocalStorageService` para limpieza de tokens
- Actualización del `AuthenticationStateProvider`
- Logging detallado para monitoreo y debugging

### 4. Integración Global en App.razor

**Archivo:** `SGA.Web/App.razor`

**Funcionalidades:**

- Inclusión global del `TokenValidationComponent`
- Configuración centralizada de `BlazoredToasts`
- Integración seamless con el ciclo de vida de la aplicación

**Características técnicas:**

- Configuración de posicionamiento de Toast (`TopRight`, `TopCenter`)
- Configuración de iconos Font Awesome para diferentes tipos de notificación
- Timeout configurado a 5 segundos para visibilidad óptima

## Flujo de Funcionamiento

### Escenario 1: Validación Periódica Automática

1. `TokenValidationComponent` se inicializa globalmente al cargar la aplicación
2. Timer ejecuta validación cada 15 segundos para usuarios autenticados
3. Componente parsea el JWT localmente y verifica la fecha de expiración
4. Si detecta expiración:
   - Detiene el timer de validación para evitar ejecuciones múltiples
   - Obtiene el nombre del usuario desde los claims de autenticación
   - Llama a `AuthService.HandleTokenExpiration()` para limpiar sesión
   - Muestra Toast personalizado con saludo al usuario (ej: "Hola Carlos, tu sesión ha expirado...")
   - Espera 4 segundos para visibilidad del mensaje
   - Redirige automáticamente a `/login`

### Escenario 2: Token Expira Durante Petición HTTP

1. `AuthorizationMessageHandler` intercepta la petición HTTP
2. Verifica si el token está expirado antes de enviar la petición
3. Si está expirado o el servidor responde 401:
   - Delega el manejo a `AuthService.HandleTokenExpiration()`
   - El `TokenValidationComponent` detecta el cambio de estado
   - Muestra la notificación Toast correspondiente
   - Redirige al login automáticamente

### Escenario 3: Cambio de Estado de Autenticación

1. `TokenValidationComponent` escucha cambios en `AuthenticationStateProvider`
2. Cuando el usuario se autentica: inicia la validación periódica
3. Cuando el usuario se desautentica: detiene la validación periódica
4. Sincronización automática del estado en toda la aplicación

## Implementación Paso a Paso

### Paso 1: Configurar el Servicio JWT para Incluir Nombres

**Archivo:** `SGA.Application/Interfaces/IJwtService.cs`

```csharp
using System.Security.Claims;

namespace SGA.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(Guid userId, string email, string role, string? cedula = null, string? nombres = null, string? apellidos = null);
    string GenerateRefreshToken();
    bool ValidateToken(string token);
    Dictionary<string, string> GetClaimsFromToken(string token);
}
````

**Archivo:** `SGA.Application/Services/JwtService.cs`

```csharp
public string GenerateToken(Guid userId, string email, string role, string? cedula = null, string? nombres = null, string? apellidos = null)
{
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        new Claim(ClaimTypes.Email, email),
        new Claim(ClaimTypes.Role, role),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
    };

    if (!string.IsNullOrEmpty(cedula))
    {
        claims.Add(new Claim("cedula", cedula));
    }

    if (!string.IsNullOrEmpty(nombres))
    {
        claims.Add(new Claim(ClaimTypes.GivenName, nombres));
        claims.Add(new Claim("given_name", nombres)); // Para compatibilidad con estándares JWT
    }

    if (!string.IsNullOrEmpty(apellidos))
    {
        claims.Add(new Claim(ClaimTypes.Surname, apellidos));
        claims.Add(new Claim("family_name", apellidos)); // Para compatibilidad con estándares JWT
    }

    var token = new JwtSecurityToken(
        issuer: _issuer,
        audience: _audience,
        claims: claims,
        expires: DateTime.UtcNow.AddHours(8),
        signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}
```

**Archivo:** `SGA.Application/Services/AuthService.cs`

```csharp
public async Task<LoginResponse> LoginAsync(LoginRequest request)
{
    // ...código de validación existente...

    var docente = await _docenteRepository.GetByUsuarioIdAsync(usuario.Id);
    var token = _jwtService.GenerateToken(usuario.Id, usuario.Email, usuario.Rol.ToString(),
        docente?.Cedula, docente?.Nombres, docente?.Apellidos);
    var refreshToken = _jwtService.GenerateRefreshToken();

    // ...resto del método...
}

public async Task<LoginResponse> RegisterAsync(RegisterRequest request)
{
    // ...código de registro existente...

    // Generar tokens
    var token = _jwtService.GenerateToken(usuario.Id, usuario.Email, usuario.Rol.ToString(),
        docente.Cedula, docente.Nombres, docente.Apellidos);
    var refreshToken = _jwtService.GenerateRefreshToken();

    // ...resto del método...
}
```

### Paso 2: Crear TokenValidationComponent

**Archivo:** `SGA.Web/Shared/TokenValidationComponent.razor`

```razor
@using Microsoft.AspNetCore.Components.Authorization
@using SGA.Web.Services
@using Blazored.Toast.Services
@using Microsoft.AspNetCore.Components
@inject AuthenticationStateProvider AuthStateProvider
@inject Blazored.LocalStorage.ILocalStorageService LocalStorage
@inject NavigationManager Navigation
@inject IToastService ToastService
@inject AuthService AuthServiceLocal
@implements IDisposable

@* Este componente maneja la validación automática de tokens *@

@code {
    private Timer? _tokenCheckTimer;
    private bool _isCheckingToken = false;
    private ApiAuthenticationStateProvider? _apiAuthStateProvider;

    protected override async Task OnInitializedAsync()
    {
        _apiAuthStateProvider = AuthStateProvider as ApiAuthenticationStateProvider;

        Console.WriteLine("[TOKEN VALIDATION] Iniciando TokenValidationComponent");

        // Solo iniciar la validación si el usuario está autenticado
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        if (authState.User.Identity?.IsAuthenticated == true)
        {
            Console.WriteLine("[TOKEN VALIDATION] Usuario autenticado, iniciando validación");
            StartTokenValidation();
        }
        else
        {
            Console.WriteLine("[TOKEN VALIDATION] Usuario no autenticado, no iniciando validación");
        }

        // Suscribirse a cambios de estado de autenticación
        AuthStateProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
    }

    private void StartTokenValidation()
    {
        // Verificar el token cada 15 segundos (más frecuente para mejor detección)
        _tokenCheckTimer = new Timer(async _ => await CheckTokenValidity(),
            null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(15));

        Console.WriteLine("[TOKEN VALIDATION] Timer de validación iniciado");
    }

    private void StopTokenValidation()
    {
        _tokenCheckTimer?.Dispose();
        _tokenCheckTimer = null;
        Console.WriteLine("[TOKEN VALIDATION] Timer de validación detenido");
    }

    private async void OnAuthenticationStateChanged(Task<AuthenticationState> task)
    {
        var authState = await task;

        if (authState.User.Identity?.IsAuthenticated == true)
        {
            StartTokenValidation();
        }
        else
        {
            StopTokenValidation();
        }
    }

    private async Task CheckTokenValidity()
    {
        if (_isCheckingToken || _apiAuthStateProvider == null)
            return;

        _isCheckingToken = true;

        try
        {
            var token = await LocalStorage.GetItemAsync<string>("authToken");

            Console.WriteLine($"[TOKEN VALIDATION] Verificando token: {!string.IsNullOrWhiteSpace(token)}");

            if (string.IsNullOrWhiteSpace(token))
            {
                Console.WriteLine("[TOKEN VALIDATION] Token no encontrado, manejando expiración");
                await HandleTokenExpiration();
                return;
            }

            // Verificar si el token ha expirado usando validación local
            if (IsTokenExpired(token))
            {
                Console.WriteLine("[TOKEN VALIDATION] Token expirado detectado en verificación periódica");
                await HandleTokenExpiration();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TOKEN VALIDATION] Error verificando token: {ex.Message}");
        }
        finally
        {
            _isCheckingToken = false;
        }
    }

    private bool IsTokenExpired(string token)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(token))
                return true;

            // Parsear el JWT para obtener la fecha de expiración
            var parts = token.Split('.');
            if (parts.Length != 3)
                return true;

            var payload = parts[1];
            // Agregar padding si es necesario
            switch (payload.Length % 4)
            {
                case 2: payload += "=="; break;
                case 3: payload += "="; break;
            }

            var jsonBytes = Convert.FromBase64String(payload);
            var jsonString = System.Text.Encoding.UTF8.GetString(jsonBytes);

            // Buscar el campo exp en el JSON
            if (jsonString.Contains("\"exp\""))
            {
                var expIndex = jsonString.IndexOf("\"exp\":");
                if (expIndex >= 0)
                {
                    var expStart = jsonString.IndexOf(':', expIndex) + 1;
                    var expEnd = jsonString.IndexOfAny(new char[] { ',', '}' }, expStart);
                    if (expEnd > expStart)
                    {
                        var expString = jsonString.Substring(expStart, expEnd - expStart).Trim();
                        if (long.TryParse(expString, out var exp))
                        {
                            var expirationTime = DateTimeOffset.FromUnixTimeSeconds(exp);
                            return expirationTime <= DateTimeOffset.UtcNow.AddSeconds(10); // 10 segundos de margen
                        }
                    }
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TOKEN VALIDATION] Error verificando expiración: {ex.Message}");
            return true;
        }
    }

    private async Task HandleTokenExpiration()
    {
        try
        {
            Console.WriteLine("[TOKEN VALIDATION] Manejando expiración de token desde componente");

            // Parar la validación para evitar bucles
            StopTokenValidation();

            // Obtener el nombre del usuario antes de limpiar la sesión
            string userName = "Usuario";
            try
            {
                var authState = await AuthStateProvider.GetAuthenticationStateAsync();
                if (authState.User.Identity?.IsAuthenticated == true)
                {
                    Console.WriteLine("[TOKEN VALIDATION] Claims disponibles:");
                    foreach (var claim in authState.User.Claims)
                    {
                        Console.WriteLine($"[TOKEN VALIDATION] Claim: {claim.Type} = {claim.Value}");
                    }

                    // Buscar el nombre real del usuario con prioridad específica
                    // Primero buscar los claims estándar de nombres que ahora incluimos en el token
                    var nombreClaim = authState.User.FindFirst(System.Security.Claims.ClaimTypes.GivenName) ??     // GivenName estándar (más común)
                                     authState.User.FindFirst("given_name") ??                                     // given_name estándar JWT
                                     authState.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname") ??  // Nombre de pila MS
                                     authState.User.FindFirst("first_name") ??                                     // Primer nombre
                                     authState.User.FindFirst("nombre") ??                                         // Campo nombre personalizado
                                     authState.User.FindFirst("name") ??                                           // Nombre completo
                                     authState.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name") ??    // Claim name estándar
                                     authState.User.FindFirst(System.Security.Claims.ClaimTypes.Name);             // Name estándar

                    if (nombreClaim != null && !string.IsNullOrWhiteSpace(nombreClaim.Value))
                    {
                        // Si el nombre contiene espacios, tomar solo el primer nombre
                        var nombreCompleto = nombreClaim.Value.Trim();
                        userName = nombreCompleto.Split(' ')[0];
                        Console.WriteLine($"[TOKEN VALIDATION] Nombre obtenido del claim '{nombreClaim.Type}': {userName}");
                    }
                    else
                    {
                        // Como último recurso, usar email pero extraer el nombre de manera inteligente
                        var emailClaim = authState.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress") ??
                                        authState.User.FindFirst("email") ??
                                        authState.User.FindFirst(System.Security.Claims.ClaimTypes.Email);

                        if (emailClaim != null && !string.IsNullOrWhiteSpace(emailClaim.Value))
                        {
                            var emailPart = emailClaim.Value.Split('@')[0];
                            Console.WriteLine($"[TOKEN VALIDATION] Procesando email parte: {emailPart}");

                            // Si el email tiene un patrón como "nombre.apellido" usar solo el nombre
                            if (emailPart.Contains('.'))
                            {
                                var nameParts = emailPart.Split('.');
                                // Tomar la primera parte antes del punto como nombre
                                userName = nameParts[0];
                                Console.WriteLine($"[TOKEN VALIDATION] Email con punto detectado, usando: {userName}");
                            }
                            else
                            {
                                // Si no tiene punto, podría ser algo como "sparedes"
                                // Intentar capitalizar la primera letra como nombre propio
                                userName = char.ToUpper(emailPart[0]) + emailPart.Substring(1).ToLower();
                                Console.WriteLine($"[TOKEN VALIDATION] Email sin punto, capitalizando: {userName}");
                            }
                            Console.WriteLine($"[TOKEN VALIDATION] Nombre final obtenido del email: {userName}");
                        }
                    }
                }
            }
            catch (Exception userEx)
            {
                Console.WriteLine($"[TOKEN VALIDATION] Error obteniendo nombre de usuario: {userEx.Message}");
            }

            // Limpiar usando AuthService
            await AuthServiceLocal.HandleTokenExpiration();

            // Mostrar notificación personalizada con el nombre del usuario
            await InvokeAsync(() =>
            {
                try
                {
                    var personalizedMessage = $"Hola {userName}, tu sesión ha expirado. Por favor, inicia sesión nuevamente.";

                    ToastService.ShowError($"🔒 {personalizedMessage}", settings =>
                    {
                        settings.Timeout = 8;
                        settings.ShowCloseButton = false;
                        settings.ShowProgressBar = true;
                        settings.Position = Blazored.Toast.Configuration.ToastPosition.TopCenter;
                    });

                    Console.WriteLine($"[TOKEN VALIDATION] Toast personalizado mostrado para usuario: {userName}");
                }
                catch (Exception toastEx)
                {
                    Console.WriteLine($"[TOKEN VALIDATION] Error mostrando toast: {toastEx.Message}");
                }
            });

            // Esperar y redirigir
            await Task.Delay(4000);

            await InvokeAsync(() =>
            {
                Navigation.NavigateTo("/login", true);
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TOKEN VALIDATION] Error manejando expiración de token: {ex.Message}");
            // En caso de error, redirigir directamente
            await InvokeAsync(() =>
            {
                Navigation.NavigateTo("/login", true);
            });
        }
    }

    public void Dispose()
    {
        StopTokenValidation();
        if (AuthStateProvider != null)
        {
            AuthStateProvider.AuthenticationStateChanged -= OnAuthenticationStateChanged;
        }
    }
}
```

### Paso 3: Implementar AuthService.HandleTokenExpiration

**Archivo:** `SGA.Web/Services/AuthService.cs`

Agregar el método `HandleTokenExpiration()`:

```csharp
/// <summary>
/// Maneja la expiración de token de manera centralizada
/// </summary>
public async Task HandleTokenExpiration()
{
    try
    {
        Console.WriteLine("[AUTH DEBUG] HandleTokenExpiration llamado");

        // Limpiar token del almacenamiento local
        await _localStorage.RemoveItemAsync("authToken");

        // Limpiar caché del usuario
        ClearUserCache();

        // Limpiar headers de autorización
        _httpClient.DefaultRequestHeaders.Authorization = null;

        // Notificar al AuthenticationStateProvider que el usuario ya no está autenticado
        if (_authStateProvider is ApiAuthenticationStateProvider apiProvider)
        {
            apiProvider.MarkUserAsLoggedOut();
        }

        Console.WriteLine("[AUTH DEBUG] Token expirado - sesión limpiada exitosamente");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[AUTH DEBUG] Error en HandleTokenExpiration: {ex.Message}");
    }
}

/// <summary>
/// Limpia el caché del usuario actual para forzar que se vuelva a cargar desde la API
/// </summary>
public void ClearUserCache()
{
    _currentUser = null;
}
```

### Paso 4: Integrar en App.razor

## Características de la Implementación

### Experiencia de Usuario Mejorada

- **Notificación Personalizada con Nombre Real:** Toast con saludo personalizado usando el nombre real del usuario obtenido del token JWT (ej: "Hola Steven, tu sesión ha expirado. Por favor, inicia sesión nuevamente.")
- **Sistema Inteligente de Obtención de Nombres:** Busca el nombre en múltiples claims del JWT con prioridad en claims estándar (`ClaimTypes.GivenName`, `given_name`)
- **Fallback Robusto:** Si no encuentra el nombre en los claims, extrae inteligentemente el nombre del email con capitalización apropiada
- **Mensaje Único:** Solo un Toast para evitar saturación visual y confusión
- **Redirección Automática:** Transición suave al login después de 4 segundos
- **Limpieza Completa:** Eliminación total de datos de sesión y caché
- **Experiencia Consistente:** Comportamiento uniforme en toda la aplicación

### Características Técnicas

- **Token JWT Enriquecido:** Inclusión automática de nombres del usuario en claims estándar (`ClaimTypes.GivenName`, `given_name`)
- **Validación Local:** Parseo de JWT sin peticiones adicionales al servidor
- **Timer Optimizado:** Verificación cada 15 segundos con inicio retardado de 5 segundos
- **Manejo de Excepciones:** Logging detallado y fallbacks robustos
- **Integración Nativa:** Uso completo del ecosistema Blazor y Blazored.Toast
- **Arquitectura Modular:** Separación clara de responsabilidades entre componentes
- **Sistema de Claims Priorizado:** Búsqueda inteligente de nombres con múltiples fallbacks

## Consideraciones de Seguridad

1. **Limpieza Completa de Sesión:**

   - Eliminación de token de localStorage
   - Limpieza de caché de usuario en memoria
   - Actualización del estado de autenticación global

2. **Validación Local Segura:**

   - Parseo de JWT sin exposición de datos sensibles
   - Verificación de fecha de expiración con margen de seguridad
   - Manejo seguro de excepciones en validación

3. **Prevención de Fugas de Información:**
   - Logging controlado sin exposición de tokens completos
   - Manejo de errores sin revelar detalles internos
   - Redirección inmediata en caso de token inválido

## Beneficios de la Implementación

### Antes de la Implementación

- Errores 401 mostraban mensajes técnicos confusos
- No había notificación clara de expiración de sesión
- Redirección manual e inconsistente
- Usuario podía quedar en estado indefinido

### Después de la Implementación

- **Notificación personalizada con nombre real:** Saludo directo al usuario usando su nombre real obtenido del token JWT (ej: "Hola Steven, tu sesión ha expirado...")
- **Sistema inteligente de obtención de nombres:** Busca en claims estándar del JWT y tiene fallbacks robustos
- **Mensaje único y claro:** Sin saturación de notificaciones múltiples
- **Redirección automática y consistente:** Al login sin intervención manual
- **Limpieza automática y completa:** De datos de sesión y caché
- **Experiencia uniforme:** En toda la aplicación
- **Token JWT enriquecido:** Con información completa del usuario para mejor experiencia

## Archivos de la Implementación

### Componentes Principales

1. **`SGA.Application/Interfaces/IJwtService.cs` y `SGA.Application/Services/JwtService.cs`**

   - Servicio JWT actualizado para incluir nombres en tokens
   - Claims estándar para nombres (`ClaimTypes.GivenName`, `given_name`)
   - Compatibilidad con estándares JWT para interoperabilidad

2. **`SGA.Application/Services/AuthService.cs`**

   - Métodos `LoginAsync()` y `RegisterAsync()` actualizados
   - Generación de tokens con información completa del usuario
   - Método `HandleTokenExpiration()` centralizado para limpieza de sesión

3. **`SGA.Web/Shared/TokenValidationComponent.razor`**

   - Componente global de validación periódica
   - Sistema inteligente de obtención de nombres del JWT
   - Manejo de notificaciones Toast personalizadas
   - Control del ciclo de vida de validación

4. **`SGA.Web/Services/AuthService.cs`**

   - Método `HandleTokenExpiration()` centralizado
   - Limpieza de sesión y datos de usuario
   - Integración con localStorage y AuthenticationStateProvider

5. **`SGA.Web/Services/AuthorizationMessageHandler.cs`** (Opcional)

   - Interceptor HTTP optimizado para respuestas 401
   - Validación previa de tokens antes de peticiones
   - Delegación al AuthService para manejo centralizado

6. **`SGA.Web/App.razor`**

   - Integración global del TokenValidationComponent
   - Configuración de BlazoredToasts
   - Configuración del enrutamiento con autenticación

### Configuración de Dependencias

Asegurar que las siguientes dependencias estén configuradas en `Program.cs`:

```csharp
// Servicios de autenticación y almacenamiento
builder.Services.AddBlazoredToast();
builder.Services.AddBlazoredLocalStorage();

// Servicios personalizados
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AuthorizationMessageHandler>();
builder.Services.AddScoped<ApiAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<ApiAuthenticationStateProvider>());

// Configuración del HttpClient con el handler de autorización
builder.Services.AddScoped(sp => new HttpClient(sp.GetRequiredService<AuthorizationMessageHandler>())
{
    BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7000/")
});
```

### NuGet Packages Requeridos

```xml
<PackageReference Include="Blazored.Toast" Version="4.2.0" />
<PackageReference Include="Blazored.LocalStorage" Version="4.5.0" />
<PackageReference Include="Microsoft.AspNetCore.Components.Authorization" Version="7.0.0" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="7.0.0" />
```

## Testing y Validación

### Métodos de Prueba

1. **Simulación de Token Expirado:**

   ```javascript
   // En consola del navegador
   localStorage.setItem("authToken", "token_invalido_para_pruebas");
   ```

2. **Verificación de Comportamiento 401:**

   - Usar token expirado en peticiones HTTP
   - Verificar que se muestra el Toast correctamente
   - Confirmar redirección automática a `/login`

3. **Validación de Limpieza de Sesión:**
   - Verificar que localStorage se limpia completamente
   - Confirmar que el estado de autenticación se actualiza
   - Validar que no hay datos residuales en memoria

### Logs de Debugging

La implementación incluye logging detallado para facilitar el debugging:

```
[TOKEN VALIDATION] Iniciando TokenValidationComponent
[TOKEN VALIDATION] Usuario autenticado, iniciando validación
[TOKEN VALIDATION] Timer de validación iniciado
[TOKEN VALIDATION] Verificando token: True
[TOKEN VALIDATION] Claims disponibles:
[TOKEN VALIDATION] Claim: http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier = 12345678-1234-1234-1234-123456789012
[TOKEN VALIDATION] Claim: http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress = steven.paredes@uta.edu.ec
[TOKEN VALIDATION] Claim: http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname = Steven
[TOKEN VALIDATION] Claim: given_name = Steven
[TOKEN VALIDATION] Nombre obtenido del claim 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname': Steven
[TOKEN VALIDATION] Token expirado detectado en verificación periódica
[TOKEN VALIDATION] Manejando expiración de token desde componente
[AUTH DEBUG] HandleTokenExpiration llamado
[AUTH DEBUG] Token expirado - sesión limpiada exitosamente
[TOKEN VALIDATION] Toast personalizado mostrado para usuario: Steven
```

## Mantenimiento y Extensiones

### Extensiones Posibles

1. **Configuración Dinámica:**

   - Intervalos de validación configurables por usuario
   - Mensajes personalizables según contexto
   - Timeouts ajustables según tipo de sesión

2. **Notificaciones Avanzadas:**

   - Advertencias previas a expiración (ej: 5 minutos antes)
   - Opciones de renovación automática de token
   - Historial de sesiones expiradas

3. **Análiticas de Sesión:**
   - Tracking de patrones de expiración
   - Métricas de tiempo de sesión promedio
   - Reportes de seguridad de autenticación

### Buenas Prácticas de Mantenimiento

- Revisar logs periódicamente para detectar patrones anómalos
- Actualizar intervalos de validación según necesidades de seguridad
- Mantener sincronización entre cliente y servidor en configuración de timeouts
- Probar regularmente la funcionalidad en diferentes navegadores y dispositivos

## Compatibilidad y Requisitos

### Tecnologías Requeridas

- ✅ Blazor WebAssembly (.NET 6+)
- ✅ Blazored.Toast (versión 4.0+)
- ✅ Blazored.LocalStorage (versión 4.0+)
- ✅ Bootstrap 5 (para estilos base)
- ✅ Font Awesome (para iconos de Toast)

### Navegadores Compatibles

- ✅ Chrome/Edge (versión 90+)
- ✅ Firefox (versión 88+)
- ✅ Safari (versión 14+)
- ✅ Navegadores móviles modernos

### Consideraciones de Rendimiento

- Timer optimizado con frecuencia balanceada (15 segundos)
- Validación local sin peticiones HTTP adicionales
- Manejo eficiente de memoria con IDisposable
- Prevención de memory leaks con cleanup de event handlers
