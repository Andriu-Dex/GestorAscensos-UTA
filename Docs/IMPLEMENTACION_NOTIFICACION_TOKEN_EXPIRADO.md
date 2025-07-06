# Implementación de Notificación de Token Expirado

## Descripción

Se ha implementado un sistema robusto y profesional de notificación de expiración de tokens JWT que proporciona una experiencia de usuario clara y consistente cuando la sesión expira. El sistema utiliza notificaciones Toast prominentes y maneja automáticamente la limpieza de sesión y redirección al login.

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

### Paso 1: Crear TokenValidationComponent

### Paso 2: Integrar en App.razor

### Paso 3: Implementar AuthService.HandleTokenExpiration

### Paso 4: Optimizar AuthorizationMessageHandler

## Características de la Implementación

### Experiencia de Usuario Mejorada

- **Notificación Personalizada:** Toast con saludo personalizado usando el nombre del usuario (ej: "Hola Carlos, tu sesión ha expirado. Por favor, inicia sesión nuevamente.")
- **Mensaje Único:** Solo un Toast para evitar saturación visual y confusión
- **Redirección Automática:** Transición suave al login después de 4 segundos
- **Limpieza Completa:** Eliminación total de datos de sesión y caché
- **Experiencia Consistente:** Comportamiento uniforme en toda la aplicación
- **Obtención Inteligente de Nombre:** Sistema que busca el nombre del usuario en diferentes claims como fallback

### Características Técnicas

- **Validación Local:** Parseo de JWT sin peticiones adicionales al servidor
- **Timer Optimizado:** Verificación cada 15 segundos con inicio retardado de 5 segundos
- **Manejo de Excepciones:** Logging detallado y fallbacks robustos
- **Integración Nativa:** Uso completo del ecosistema Blazor y Blazored.Toast
- **Arquitectura Modular:** Separación clara de responsabilidades entre componentes

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

- Notificación personalizada con saludo directo al usuario por su nombre
- Mensaje único y claro sin saturación de notificaciones múltiples
- Redirección automática y consistente al login
- Limpieza automática y completa de datos de sesión
- Experiencia uniforme en toda la aplicación
- Sistema inteligente de obtención de nombre con fallbacks robustos

## Archivos de la Implementación

### Componentes Principales

1. **`SGA.Web/Shared/TokenValidationComponent.razor`**

   - Componente global de validación periódica
   - Manejo de notificaciones Toast
   - Control del ciclo de vida de validación

2. **`SGA.Web/Services/AuthService.cs`**

   - Método `HandleTokenExpiration()` centralizado
   - Limpieza de sesión y datos de usuario
   - Integración con localStorage y AuthenticationStateProvider

3. **`SGA.Web/Services/AuthorizationMessageHandler.cs`**

   - Interceptor HTTP optimizado para respuestas 401
   - Validación previa de tokens antes de peticiones
   - Delegación al AuthService para manejo centralizado

4. **`SGA.Web/App.razor`**
   - Integración global del TokenValidationComponent
   - Configuración de BlazoredToasts
   - Configuración del enrutamiento con autenticación

### Configuración de Dependencias

Asegurar que las siguientes dependencias estén configuradas en `Program.cs`:

```csharp
builder.Services.AddBlazoredToast();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AuthorizationMessageHandler>();
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
[TOKEN VALIDATION] Token expirado detectado en verificación periódica
[TOKEN VALIDATION] Manejando expiración de token desde componente
[AUTH DEBUG] HandleTokenExpiration llamado
[AUTH DEBUG] Token expirado - sesión limpiada exitosamente
[TOKEN VALIDATION] Toasts mostrados exitosamente
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
