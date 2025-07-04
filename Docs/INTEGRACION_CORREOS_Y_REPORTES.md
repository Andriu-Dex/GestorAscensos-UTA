# Sistema de Gestión de Ascensos - Integración de Correos Electrónicos y Reportes

## 📄 Resumen Ejecutivo

Este documento detalla la implementación completa del sistema de notificaciones por correo electrónico y la generación de reportes PDF en el Sistema de Gestión de Ascensos Docentes de la Universidad Técnica de Ambato.

## 📧 Sistema de Notificaciones por Correo Electrónico

### Arquitectura del Sistema de Correos

El sistema de correos está construido usando una arquitectura modular con los siguientes componentes:

```
┌─────────────────────────────────────────────┐
│             EmailService                    │
│          (Servicio Principal)               │
│   • Envío de correos SMTP                  │
│   • Plantillas HTML/Texto                  │
│   • Configuración desde appsettings.json   │
└─────────────────┬───────────────────────────┘
                  │
┌─────────────────▼───────────────────────────┐
│          NotificationService                │
│        (Orquestador de Eventos)            │
│   • Lógica de negocio                      │
│   • Eventos de solicitudes                 │
│   • Logging y manejo de errores            │
└─────────────────┬───────────────────────────┘
                  │
┌─────────────────▼───────────────────────────┐
│         Controladores/Servicios            │
│   • SolicitudService                       │
│   • ObrasAcademicasService                 │
│   • CertificadosCapacitacionService        │
│   • EvidenciasInvestigacionService         │
└─────────────────────────────────────────────┘
```

### Configuración del Sistema de Correos

#### 1. Dependencias Utilizadas

- **MailKit**: Librería principal para envío de correos SMTP
- **MimeKit**: Para construcción de mensajes MIME
- **Microsoft.Extensions.Logging**: Para logging integrado

#### 2. Configuración en `appsettings.json`

```json
{
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUser": "andriudex@gmail.com",
    "SmtpPass": "dvqxrhapdjbgjvde",
    "EnableSsl": true,
    "FromName": "Sistema de Gestión de Ascensos",
    "FromEmail": "andriudex@gmail.com"
  }
}
```

**Nota de Seguridad**: La contraseña mostrada es una "App Password" de Gmail específica para aplicaciones, no la contraseña principal de la cuenta.

#### 3. Inyección de Dependencias

En `SGA.Application/DependencyInjection.cs`:

```csharp
// Servicio de email
services.AddScoped<IEmailService, EmailService>();

// Servicio de notificaciones
services.AddScoped<INotificationService, NotificationService>();
```

### Implementación Técnica

#### Interface IEmailService

```csharp
public interface IEmailService
{
    Task<bool> EnviarCorreoAsync(string destinatario, string asunto, string cuerpoHtml, string? cuerpoTexto = null);
    Task<bool> EnviarCorreoFelicitacionAscensoAsync(string emailDocente, string nombreDocente, string nivelAnterior, string nivelNuevo);
    Task<bool> EnviarCorreoRechazoAscensoAsync(string emailDocente, string nombreDocente, string nivelSolicitado, string motivo);
}
```

#### Servicio de Email Principal

**Archivo**: `SGA.Application/Services/EmailService.cs`

**Características principales**:

- Soporte para correos HTML y texto plano
- Configuración SMTP flexible
- Autenticación segura
- Logging detallado
- Manejo robusto de errores

**Métodos clave**:

1. **EnviarCorreoAsync**: Método genérico para envío de correos
2. **EnviarCorreoFelicitacionAscensoAsync**: Correos de aprobación de ascensos
3. **EnviarCorreoRechazoAscensoAsync**: Correos de rechazo de ascensos
4. **GenerarPlantillaFelicitacion**: Plantilla HTML para felicitaciones
5. **GenerarPlantillaRechazo**: Plantilla HTML para rechazos

#### Interface INotificationService

```csharp
public interface INotificationService
{
    // Notificaciones para obras académicas
    Task NotificarNuevaSolicitudObrasAsync(string nombreDocente, int cantidadObras);
    Task NotificarAprobacionObraAsync(string emailDocente, string tituloObra, string comentarios);
    Task NotificarRechazoObraAsync(string emailDocente, string tituloObra, string motivo);

    // Notificaciones para certificados de capacitación
    Task NotificarNuevaSolicitudCertificadosAsync(string nombreDocente, int cantidadCertificados);
    Task NotificarAprobacionCertificadoAsync(string emailDocente, string nombreCurso, string comentarios);
    Task NotificarRechazoCertificadoAsync(string emailDocente, string nombreCurso, string motivo);

    // Notificaciones generales
    Task NotificarCambioEstadoSolicitudAsync(string emailDocente, string tipoSolicitud, string nuevoEstado);

    // Notificaciones para solicitudes de ascenso
    Task NotificarAprobacionAscensoAsync(string emailDocente, string nombreDocente, string nivelAnterior, string nivelNuevo);
    Task NotificarRechazoAscensoAsync(string emailDocente, string nombreDocente, string nivelSolicitado, string motivo);
}
```

### Plantillas de Correo

#### Plantilla de Felicitación por Ascenso

La plantilla incluye:

- Header institucional con logo UTA simulado
- Mensaje de felicitación personalizado
- Detalles del ascenso (nivel anterior → nivel nuevo)
- Información sobre reinicio de contadores
- Fecha de aprobación
- Footer institucional

**Características del diseño**:

- CSS inline para compatibilidad
- Colores institucionales (#8a1538)
- Responsive design
- Emojis para mejor experiencia visual

#### Plantilla de Rechazo de Solicitud

La plantilla incluye:

- Header institucional
- Mensaje explicativo del rechazo
- Motivos específicos del rechazo
- Recomendaciones para futuras solicitudes
- Información de contacto
- Footer institucional

### Casos de Uso Implementados

#### 1. Aprobación de Solicitud de Ascenso

**Flujo**:

1. Admin aprueba solicitud en `AdminSolicitudes.razor`
2. Se llama a `SolicitudService.AprobarSolicitudAsync()`
3. Se actualiza el estado y se reinician contadores
4. Se invoca `NotificationService.NotificarAprobacionAscensoAsync()`
5. Se envía correo de felicitación al docente

**Ejemplo de implementación**:

```csharp
// En SolicitudService.cs
public async Task<ResponseGenericoDto> AprobarSolicitudAsync(Guid solicitudId, string adminEmail)
{
    // ... lógica de aprobación ...

    // Enviar notificación
    await _notificationService.NotificarAprobacionAscensoAsync(
        docente.Email,
        $"{docente.Nombres} {docente.Apellidos}",
        solicitud.NivelActual.ToString(),
        solicitud.NivelSolicitado.ToString()
    );

    return response;
}
```

#### 2. Rechazo de Solicitud de Ascenso

**Flujo**:

1. Admin rechaza solicitud con motivo
2. Se actualiza estado a "Rechazada"
3. Se envía correo explicativo al docente

#### 3. Notificaciones de Obras Académicas

**Estados soportados**:

- Nueva solicitud (para administradores)
- Aprobación de obra
- Rechazo de obra

#### 4. Notificaciones de Certificados de Capacitación

**Estados soportados**:

- Nueva solicitud de certificados
- Aprobación de certificado
- Rechazo de certificado

### Logging y Monitoreo

El sistema incluye logging detallado:

```csharp
_logger.LogInformation("📧 Correo enviado exitosamente a {Destinatario} con asunto: {Asunto}", destinatario, asunto);
_logger.LogError(ex, "❌ Error al enviar correo a {Destinatario}: {Error}", destinatario, ex.Message);
```

## 📊 Sistema de Reportes PDF

### Arquitectura del Sistema de Reportes

```
┌─────────────────────────────────────────────┐
│            ReportesController               │
│   • Endpoints para generar reportes        │
│   • Autenticación y autorización           │
│   • Streaming de archivos PDF              │
└─────────────────┬───────────────────────────┘
                  │
┌─────────────────▼───────────────────────────┐
│             ReporteService                  │
│   • Generación de PDFs con iText7          │
│   • Plantillas de documentos               │
│   • Acceso a datos de repositorios         │
└─────────────────┬───────────────────────────┘
                  │
┌─────────────────▼───────────────────────────┐
│         Repositorios de Datos               │
│   • DocenteRepository                      │
│   • SolicitudAscensoRepository             │
│   • DocumentoRepository                    │
└─────────────────────────────────────────────┘
```

### Dependencias y Configuración

#### Librería iText7

```xml
<PackageReference Include="itext7" Version="8.0.2" />
```

**Características de iText7**:

- Generación de PDFs programática
- Soporte para estilos y formatos avanzados
- Tablas, imágenes y elementos gráficos
- Control total sobre el layout

#### Inyección de Dependencias

```csharp
services.AddScoped<IReporteService, ReporteService>();
```

### Tipos de Reportes Implementados

#### 1. Hoja de Vida Académica

**Endpoint**: `GET /api/reportes/hoja-vida`

**Contenido del reporte**:

- Header institucional con logo UTA
- Datos personales del docente
- Datos académicos (nivel, evaluaciones, etc.)
- Historial de solicitudes de ascenso
- Estadísticas de tiempo en nivel
- Fecha de generación

**Implementación técnica**:

```csharp
public async Task<byte[]> GenerarHojaVidaAsync(Guid docenteId)
{
    var docente = await _docenteRepository.GetByIdAsync(docenteId);

    using var memoryStream = new MemoryStream();
    using var writer = new PdfWriter(memoryStream);
    using var pdf = new PdfDocument(writer);
    var document = new Document(pdf);

    // Configuración de estilos
    var tituloEstilo = new Style()
        .SetFontSize(20)
        .SetBold()
        .SetFontColor(new DeviceRgb(0, 51, 102));

    // Generación de contenido...

    document.Close();
    return memoryStream.ToArray();
}
```

**Elementos del diseño**:

- Tabla de datos personales
- Tabla de datos académicos
- Tabla de historial de solicitudes con colores por estado
- Estilos profesionales y legibles

#### 2. Reporte de Solicitud Específica

**Endpoint**: `GET /api/reportes/solicitud/{solicitudId}`

**Contenido del reporte**:

- Datos de la solicitud (fechas, estados)
- Información del docente
- Datos académicos al momento de la solicitud
- Lista de documentos adjuntos
- Estado actual y observaciones

**Seguridad**:

- Verificación de permisos (docente solo ve sus reportes)
- Administradores pueden ver todos los reportes

#### 3. Estadísticas Generales (Solo Administradores)

**Endpoint**: `GET /api/reportes/estadisticas`

**Contenido del reporte**:

- Total de docentes en el sistema
- Distribución por niveles académicos
- Estadísticas de solicitudes por estado
- Estadísticas por nivel académico
- Fecha de generación

### Funcionalidades del Frontend

#### Visualización de Reportes

**Componente**: `DocumentVisualizationService.cs`

**Características**:

- Visualización inline en modales
- Descarga directa de archivos
- Manejo de errores y loading states
- Toasts informativos

**Ejemplo de uso**:

```csharp
public async Task<DocumentViewResult> VisualizarReporteSolicitud(Guid solicitudId)
{
    var response = await _http.GetAsync($"api/reportes/solicitud/{solicitudId}");

    if (response.IsSuccessStatusCode)
    {
        var fileBytes = await response.Content.ReadAsByteArrayAsync();
        var base64 = Convert.ToBase64String(fileBytes);
        var pdfUrl = $"data:application/pdf;base64,{base64}";

        return new DocumentViewResult
        {
            Success = true,
            PdfUrl = pdfUrl,
            FileName = fileName
        };
    }

    return new DocumentViewResult { Success = false };
}
```

#### Páginas de Reportes

1. **`/reportes`**: Página principal para docentes
2. **`/admin/reportes`**: Panel administrativo con reportes avanzados

#### Componentes de UI

**Modal de visualización**:

- Iframe para mostrar PDFs
- Botones de descarga
- Manejo de estados de carga
- Mensajes de error

**Botones de acción**:

- "Ver Reporte" (visualización)
- "Descargar Reporte" (descarga directa)
- Estados de loading con spinners

### Integración Frontend-Backend

#### Llamadas AJAX

```javascript
// En Solicitudes.razor
private async Task ViewReportInModal(Guid solicitudId)
{
    var result = await DocumentService.VisualizarReporteSolicitud(solicitudId);

    if (result.Success)
    {
        currentPdfUrl = result.PdfUrl;
        showPdfViewer = true;
    }
}
```

#### Descarga de Archivos

```javascript
// JavaScript en wwwroot
function downloadFileFromStream(fileName, fileBytes) {
  const blob = new Blob([fileBytes], { type: "application/pdf" });
  const url = window.URL.createObjectURL(blob);
  const link = document.createElement("a");
  link.href = url;
  link.download = fileName;
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
  window.URL.revokeObjectURL(url);
}
```

## 🔧 Configuración para Producción

### Variables de Entorno Requeridas

```json
{
  "EmailSettings": {
    "SmtpHost": "smtp.empresa.com",
    "SmtpPort": 587,
    "SmtpUser": "sistema@uta.edu.ec",
    "SmtpPass": "password_seguro_aqui",
    "EnableSsl": true,
    "FromName": "Sistema de Gestión de Ascensos UTA",
    "FromEmail": "sistema@uta.edu.ec"
  }
}
```

### Configuración SMTP Recomendada

#### Para Gmail (Desarrollo)

- Host: `smtp.gmail.com`
- Puerto: `587`
- SSL: `true`
- Requiere App Password (no contraseña principal)

#### Para Exchange/Outlook 365

- Host: `smtp.office365.com`
- Puerto: `587`
- SSL: `true`
- Autenticación moderna

#### Para Servidor SMTP Institucional

- Configurar según especificaciones del servidor
- Verificar certificados SSL
- Configurar firewall para puerto SMTP

### Consideraciones de Seguridad

1. **Contraseñas**: Usar App Passwords o tokens
2. **SSL/TLS**: Siempre habilitado en producción
3. **Logging**: No registrar contraseñas en logs
4. **Rate Limiting**: Implementar límites de envío
5. **Validación**: Validar direcciones de correo

## 📈 Métricas y Monitoreo

### Logging Implementado

El sistema registra:

- Envíos exitosos de correos
- Errores en envío de correos
- Generación de reportes
- Accesos a reportes
- Errores en generación de PDFs

### Ejemplos de Logs

```
[2025-07-03 10:30:15] INFO: 📧 Correo enviado exitosamente a juan.perez@uta.edu.ec con asunto: 🎉 ¡Felicitaciones! Su solicitud de ascenso ha sido aprobada
[2025-07-03 10:31:22] INFO: Hoja de vida generada exitosamente para docente: 12345-678-90, tamaño: 245632 bytes
[2025-07-03 10:32:10] ERROR: ❌ Error al enviar correo a maria.gonzalez@uta.edu.ec: SMTP server timeout
```

## 🚀 Funcionalidades Avanzadas

### Plantillas de Correo Personalizables

Las plantillas soportan:

- Variables dinámicas
- Colores institucionales
- Logos y branding
- Responsive design
- Texto alternativo

### Reportes con Estilos Profesionales

Los reportes incluyen:

- Headers institucionales
- Tablas con formato
- Colores según estado
- Información detallada
- Pie de página con fecha

### Integración con el Sistema de Gestión

- Notificaciones automáticas en cambios de estado
- Reportes asociados a solicitudes específicas
- Seguridad basada en roles
- Auditoría completa de acciones

## 📝 Conclusiones

El sistema de correos y reportes implementado en el Sistema de Gestión de Ascensos proporciona:

### Beneficios del Sistema de Correos:

1. **Comunicación automática** con docentes
2. **Plantillas profesionales** consistentes
3. **Configuración flexible** para diferentes entornos
4. **Logging detallado** para auditoría
5. **Manejo robusto de errores**

### Beneficios del Sistema de Reportes:

1. **Documentos oficiales** en formato PDF
2. **Diseño profesional** con branding institucional
3. **Información completa** y actualizada
4. **Seguridad** basada en permisos
5. **Fácil acceso** desde la interfaz web

### Tecnologías Utilizadas:

- **MailKit/MimeKit**: Para envío de correos
- **iText7**: Para generación de PDFs
- **Dependency Injection**: Para gestión de servicios
- **Logging**: Para monitoreo y auditoría
- **Blazor**: Para interfaz de usuario reactiva

El sistema está preparado para escalar y soporta configuraciones tanto de desarrollo como de producción, manteniendo altos estándares de seguridad y usabilidad.
