# Implementación de Notificación de Token Expirado

## Descripción

Se ha implementado un sistema completo de notificación y manejo de expiración de tokens JWT para mejorar la experiencia del usuario cuando su sesión expira.

## Componentes Implementados

### 1. AuthorizationMessageHandler Mejorado

**Archivo:** `SGA.Web/Services/AuthorizationMessageHandler.cs`

**Funcionalidades:**

- Intercepta todas las respuestas HTTP 401 (Unauthorized)
- Maneja la expiración de tokens de manera centralizada
- Evita múltiples ejecuciones simultáneas con un sistema de bloqueo
- Limpia automáticamente los datos de autenticación
- Muestra notificación toast personalizada
- Redirige automáticamente al login después de 4 segundos

**Características técnicas:**

- Usa `DelegatingHandler` para interceptar todas las peticiones HTTP
- Implementa un patrón de singleton para evitar múltiples notificaciones
- Manejo robusto de errores con fallbacks

### 2. Script de Validación de Tokens (JavaScript)

**Archivo:** `SGA.Web/wwwroot/js/token-validation.js`

**Funcionalidades:**

- Parsea tokens JWT del lado del cliente
- Verifica expiración de tokens sin hacer peticiones al servidor
- Monitoreo periódico automático cada 2 minutos
- Modal de notificación personalizado con cuenta regresiva
- Funciones utilitarias para verificar tokens antes de peticiones importantes

**Características técnicas:**

- Validación local de JWT sin dependencias externas
- Sistema de notificación modal nativo con estilos personalizados
- Integración automática con localStorage

### 3. Estilos CSS Personalizados

**Archivo:** `SGA.Web/wwwroot/css/token-expiration.css`

**Funcionalidades:**

- Estilos especiales para toasts de expiración de token
- Animaciones llamativas (pulse, shake)
- Responsive design para móviles
- Colores corporativos (#8a1538)
- Efectos visuales mejorados (sombras, gradientes)

### 4. Integración con Blazored.Toast

**Configuración:** Modificaciones en `AuthorizationMessageHandler`

**Características:**

- Toast posicionado en el centro superior
- Duración de 5 segundos con barra de progreso
- Mensaje personalizado con emojis
- Sin botón de cierre para forzar la atención del usuario

## Flujo de Funcionamiento

### Escenario 1: Token Expira Durante Petición HTTP

1. Usuario hace una petición HTTP (ej: cargar perfil, actualizar datos)
2. El servidor responde con 401 (Unauthorized)
3. `AuthorizationMessageHandler` intercepta la respuesta
4. Se ejecuta `HandleTokenExpiration()`:
   - Limpia token del localStorage
   - Marca usuario como desautenticado
   - Limpia caché del AuthService
   - Muestra toast de notificación
   - Espera 4 segundos
   - Redirige a `/login`

### Escenario 2: Token Expira Por Tiempo (Monitoreo Pasivo)

1. Script JavaScript verifica tokens cada 2 minutos
2. Detecta token expirado localmente
3. Muestra modal personalizado con cuenta regresiva
4. Limpia localStorage
5. Redirige a `/login` automáticamente

### Escenario 3: Token Expira Durante Navegación

1. Usuario navega entre páginas
2. Componentes verifican autenticación
3. Si detectan token inválido, llaman a `HandleAuthenticationError`
4. Se delega al sistema de interceptor para manejo consistente

## Mejoras de Experiencia de Usuario

### Antes de la Implementación

- Errores 401 no tenían manejo centralizado
- Usuario veía errores técnicos confusos
- No había notificación clara de sesión expirada
- Redirección manual o inconsistente al login

### Después de la Implementación

- Notificación clara y profesional de sesión expirada
- Redirección automática al login
- Limpieza automática de datos de sesión
- Experiencia consistente en toda la aplicación
- Prevención de múltiples notificaciones

## Configuración y Mantenimiento

### Personalización de Mensajes

Para cambiar el mensaje de expiración, modificar:

```csharp
// En AuthorizationMessageHandler.cs
toastService.ShowError("Su mensaje personalizado aquí", settings => { ... });
```

### Configuración de Timing

```csharp
// Duración del toast (milisegundos)
settings.Timeout = 5;

// Delay antes de redirección
await Task.Delay(4000);
```

### Monitoreo JavaScript

```javascript
// Frecuencia de verificación (minutos)
window.tokenValidation.startTokenMonitoring(2);
```

## Consideraciones de Seguridad

1. **Limpieza Completa:** Se limpia tanto localStorage como memoria
2. **No Exposición de Tokens:** Los tokens no se muestran en logs públicos
3. **Redirección Forzada:** El usuario es redirigido automáticamente
4. **Estado Consistente:** Todos los servicios se sincronizan

## Compatibilidad

- ✅ Blazor WebAssembly
- ✅ Bootstrap 5
- ✅ Blazored.Toast
- ✅ Navegadores modernos (ES6+)
- ✅ Responsive (móviles y desktop)

## Archivos Modificados

1. `SGA.Web/Services/AuthorizationMessageHandler.cs` - Handler principal
2. `SGA.Web/wwwroot/js/token-validation.js` - Validación JavaScript
3. `SGA.Web/wwwroot/css/token-expiration.css` - Estilos personalizados
4. `SGA.Web/wwwroot/index.html` - Referencias a scripts y CSS
5. `SGA.Web/Pages/Perfil.razor` - Método `HandleAuthenticationError` simplificado

## Testing

Para probar la funcionalidad:

1. **Forzar Expiración:**

   - Modificar token en localStorage manualmente
   - Usar herramientas de desarrollador para modificar fecha de expiración

2. **Verificar Respuesta 401:**

   - Usar un token inválido
   - Hacer peticiones después de que expire el token real

3. **Probar Redirección:**
   - Verificar que se redirige a `/login`
   - Confirmar que los datos se limpian correctamente

## Notas de Desarrollo

- La implementación es modular y puede extenderse fácilmente
- Los estilos siguen la paleta de colores corporativa
- El código incluye manejo robusto de errores
- Compatible con el patrón de inyección de dependencias de Blazor
