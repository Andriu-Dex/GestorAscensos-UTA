# Implementación de Reportes Administrativos - SGA

## Descripción General

Este documento detalla la implementación completa del sistema de reportes administrativos para el Sistema de Gestión de Ascensos (SGA). La implementación sigue principios de Clean Architecture, separación de responsabilidades y mejores prácticas de desarrollo.

## Arquitectura y Estructura

### 1. Capas Implementadas

```
SGA.Web (Presentación)
├── Pages/ReportesAdmin.razor
├── Models/ReportesModels.cs
└── Services/ApiService.cs (métodos extendidos)

SGA.Api (Controladores)
└── Controllers/ReportesAdminController.cs

SGA.Application (Lógica de Negocio)
├── DTOs/ReportesAdminDTOs.cs
├── Interfaces/IReporteAdminService.cs
├── Services/ReporteAdminService.cs
└── DependencyInjection.cs (registro de servicios)

SGA.Domain (Entidades)
├── Entities/Facultad.cs
├── Entities/Departamento.cs
└── Entities/Docente.cs (relaciones actualizadas)

SGA.Infrastructure (Datos)
├── Data/ApplicationDbContext.cs (configuración de entidades)
└── Migrations/ (migraciones de BD)
```

## Funcionalidades Implementadas

### 1. Tipos de Reportes

#### a) Reporte de Procesos por Estado

- **Descripción**: Analiza el estado de las solicitudes de ascenso
- **Datos**: Estados (Pendiente, En Proceso, Aprobada, Rechazada) con porcentajes
- **Agrupación**: Por facultad y departamento
- **Filtros**: Rango de fechas, facultad específica

#### b) Reporte de Ascensos por Facultad

- **Descripción**: Distribución de ascensos por facultad
- **Datos**: Total de ascensos por facultad con desglose por departamento
- **Filtros**: Rango de fechas

#### c) Reporte de Tiempo de Resolución

- **Descripción**: Análisis de tiempos de procesamiento de solicitudes
- **Datos**: Tiempo promedio en días por facultad
- **Cálculo**: Diferencia entre fecha de solicitud y fecha de aprobación/rechazo

#### d) Reporte de Distribución de Docentes

- **Descripción**: Distribución actual de docentes por nivel y facultad
- **Datos**: Cantidad de docentes por nivel titular (1-5)
- **Agrupación**: Por facultad y departamento

#### e) Reporte de Actividad por Período

- **Descripción**: Actividad mensual del sistema
- **Datos**: Solicitudes nuevas, resueltas, aprobadas y rechazadas por mes
- **Visualización**: Datos tabulares y por facultad

#### f) Reporte Consolidado

- **Descripción**: Resumen general de toda la actividad
- **Datos**: Métricas generales, actividad mensual y por facultad
- **Uso**: Dashboard ejecutivo

### 2. Formatos de Salida

#### PDF

- Generación automática con HTML to PDF
- Estilo corporativo con color principal #8a1538
- Encabezados con logo e información institucional
- Tablas formateadas y gráficos estadísticos

#### HTML (Vista Previa)

- Vista en modal para revisión rápida
- Misma estructura que el PDF
- Optimizada para pantalla

## Implementación Técnica

### 1. DTOs (Data Transfer Objects)

```csharp
// Archivo: SGA.Application/DTOs/ReportesAdminDTOs.cs
public class FiltroReporteAdminDTO
{
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string? Estado { get; set; }
    public Guid? FacultadId { get; set; }
    public string? Periodo { get; set; }
}
```

### 2. Servicio de Reportes

```csharp
// Archivo: SGA.Application/Services/ReporteAdminService.cs
public class ReporteAdminService : IReporteAdminService
{
    // Métodos principales:
    // - ObtenerReporteProcesosPorEstadoAsync()
    // - ObtenerReporteAscensosPorFacultadAsync()
    // - ObtenerReporteTiempoResolucionAsync()
    // - ObtenerReporteDistribucionDocentesAsync()
    // - ObtenerReporteActividadPeriodoAsync()
    // - ObtenerReporteConsolidadoAsync()

    // Métodos de generación:
    // - GenerarPdfAsync()
    // - GenerarHtmlAsync()
}
```

### 3. Controlador API

```csharp
// Archivo: SGA.Api/Controllers/ReportesAdminController.cs
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador,Admin")]
public class ReportesAdminController : ControllerBase
{
    // Endpoints para cada tipo de reporte
    // Soporte para PDF y HTML
    // Autorización solo para administradores
}
```

### 4. Interfaz de Usuario

```razor
<!-- Archivo: SGA.Web/Pages/ReportesAdmin.razor -->
@page "/reportes-admin"
@attribute [Authorize(Roles = "Administrador,Admin")]

<!-- Filtros dinámicos -->
<!-- Botones de generación -->
<!-- Modales de vista previa -->
<!-- Sistema de notificaciones con Toast -->
```

## Base de Datos

### 1. Nuevas Entidades

#### Facultad

```sql
CREATE TABLE Facultades (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Nombre NVARCHAR(200) NOT NULL,
    Codigo NVARCHAR(10) NOT NULL,
    Descripcion NVARCHAR(500),
    Color NVARCHAR(7),
    EsActiva BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME2 NOT NULL,
    FechaModificacion DATETIME2 NOT NULL
)
```

#### Departamento

```sql
CREATE TABLE Departamentos (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Nombre NVARCHAR(200) NOT NULL,
    Codigo NVARCHAR(10) NOT NULL,
    Descripcion NVARCHAR(500),
    FacultadId UNIQUEIDENTIFIER NOT NULL,
    EsActivo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME2 NOT NULL,
    FechaModificacion DATETIME2 NOT NULL,
    FOREIGN KEY (FacultadId) REFERENCES Facultades(Id)
)
```

### 2. Relaciones

- **Docente** → **Departamento** (Muchos a Uno)
- **Departamento** → **Facultad** (Muchos a Uno)
- **SolicitudAscenso** → **Docente** → **Departamento** → **Facultad**

### 3. Datos Semilla

```sql
-- Facultades incluidas:
-- FISEI: Facultad de Ingeniería en Sistemas, Electrónica e Industrial
-- FCHE: Facultad de Ciencias Humanas y de la Educación
-- FCA: Facultad de Contabilidad y Auditoría
-- FCAG: Facultad de Ciencias Agropecuarias
-- FCS: Facultad de Ciencias de la Salud

-- Departamentos por facultad (18 total)
```

## Seguridad

### 1. Autorización

- Solo usuarios con rol "Administrador" o "Admin"
- Validación tanto en frontend como backend
- Atributos de autorización en controladores y páginas

### 2. Validación de Datos

- Validación de filtros de entrada
- Sanitización de parámetros
- Manejo de errores robusto

## Características Técnicas

### 1. Patrón Repository/Service

- Separación clara entre lógica de datos y negocio
- Interfaces para facilitar testing
- Inyección de dependencias

### 2. Async/Await

- Operaciones asíncronas para mejorar performance
- No bloqueo de UI durante generación de reportes

### 3. Entity Framework Core

- Consultas optimizadas con Include/ThenInclude
- Lazy loading deshabilitado para mejor control
- Manejo de null references

### 4. Manejo de Errores

- Try-catch en todos los métodos críticos
- Logging de errores (preparado para implementar)
- Mensajes de usuario amigables

## Configuración y Despliegue

### 1. Prerequisitos

- .NET 9.0
- SQL Server (o compatible)
- Entity Framework Core Tools

### 2. Instalación

1. **Restaurar paquetes:**

   ```bash
   dotnet restore
   ```

2. **Aplicar migraciones:**

   ```bash
   cd SGA.Infrastructure
   dotnet ef database update
   ```

3. **Insertar datos semilla:**

   ```sql
   -- Ejecutar script: Scripts/Seed/facultades-departamentos-seed.sql
   ```

4. **Compilar solución:**
   ```bash
   dotnet build
   ```

### 3. Configuración

#### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=SGA;..."
  }
}
```

## Testing

### 1. Casos de Prueba Sugeridos

- Generación de reportes con datos vacíos
- Filtrado por facultad específica
- Rangos de fechas válidos e inválidos
- Autorización de usuarios
- Performance con grandes volúmenes de datos

### 2. Datos de Prueba

- Crear solicitudes de ascenso de ejemplo
- Asignar docentes a diferentes facultades
- Variar estados y fechas para pruebas completas

## Mantenimiento

### 1. Logs

- Implementar logging detallado en producción
- Monitorear performance de consultas complejas
- Alertas por errores de generación de reportes

### 2. Performance

- Índices en campos de consulta frecuente
- Paginación para reportes grandes
- Cache de datos estáticos (facultades/departamentos)

### 3. Escalabilidad

- Generación asíncrona para reportes pesados
- Cola de trabajos para múltiples solicitudes
- Almacenamiento de PDFs generados

## Futuras Mejoras

### 1. Funcionalidades

- Reportes personalizables por usuario
- Exportación a Excel
- Gráficos interactivos con Chart.js
- Programación automática de reportes
- Notificaciones por email

### 2. Técnicas

- Implementar AutoMapper para DTOs
- Cache distribuido con Redis
- Background services para reportes pesados
- API versioning
- Documentación con Swagger

## Conclusión

La implementación de reportes administrativos sigue las mejores prácticas de desarrollo, mantiene la separación de responsabilidades y proporciona una base sólida para futuras expansiones. El sistema es escalable, mantenible y cumple con todos los requisitos funcionales especificados.

## Archivos Principales

### Backend

- `SGA.Application/DTOs/ReportesAdminDTOs.cs`
- `SGA.Application/Interfaces/IReporteAdminService.cs`
- `SGA.Application/Services/ReporteAdminService.cs`
- `SGA.Api/Controllers/ReportesAdminController.cs`
- `SGA.Domain/Entities/Facultad.cs`
- `SGA.Domain/Entities/Departamento.cs`

### Frontend

- `SGA.Web/Pages/ReportesAdmin.razor`
- `SGA.Web/Models/ReportesModels.cs`
- `SGA.Web/Services/ApiService.cs`

### Base de Datos

- `SGA.Infrastructure/Data/ApplicationDbContext.cs`
- `Scripts/Seed/facultades-departamentos-seed.sql`

### Navegación

- `SGA.Web/Layout/NavMenu.razor`

---

**Fecha de Implementación:** Julio 2025  
**Versión:** 1.0  
**Desarrollador:** Sistema de Gestión de Ascensos - SGA
