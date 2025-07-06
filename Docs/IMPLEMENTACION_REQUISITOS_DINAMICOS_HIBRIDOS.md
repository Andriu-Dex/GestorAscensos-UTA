# Implementación de Requisitos Dinámicos Híbridos

**Fecha de Implementación:** Julio 5, 2025  
**Versión:** 1.0  
**Autor:** Implementación automatizada

## 📋 Resumen Ejecutivo

Se implementó un sistema híbrido de configuración de requisitos de ascenso que permite coexistir niveles predefinidos (enum) con títulos académicos dinámicos, proporcionando flexibilidad total sin romper la compatibilidad con el sistema existente.

## 🎯 Objetivo

Hacer que la configuración de requisitos en la vista de Admin sea totalmente dinámica y flexible, permitiendo crear y editar títulos/niveles de ascenso personalizados sin violar la 3FN en la base de datos, manteniendo buenas prácticas de POO, modularidad y compatibilidad.

## 🏗️ Arquitectura de la Solución

### Enfoque Híbrido Implementado

La solución permite dos tipos de configuraciones:

1. **Niveles Predefinidos:** Usando el enum `NivelTitular` existente
2. **Títulos Dinámicos:** Usando la nueva entidad `TituloAcademico`

### Principios de Diseño

- ✅ **Compatibilidad:** El sistema existente continúa funcionando sin cambios
- ✅ **Integridad:** Constraints de base de datos previenen estados inválidos
- ✅ **Flexibilidad:** Nuevos títulos se pueden crear sin modificar código
- ✅ **Performance:** Indexación optimizada para consultas eficientes
- ✅ **Modularidad:** Componentes independientes y bien separados

## 🔧 Componentes Implementados

### 1. Entidades de Dominio

#### Nueva Entidad: `TituloAcademico`

```csharp
// Ubicación: SGA.Domain/Entities/TituloAcademico.cs
public class TituloAcademico : BaseEntity
{
    public string Nombre { get; set; }               // Nombre del título
    public string? Descripcion { get; set; }        // Descripción opcional
    public int OrdenJerarquico { get; set; }        // Para jerarquía de ascensos
    public string Codigo { get; set; }              // Código único
    public bool EstaActivo { get; set; }            // Estado activo/inactivo
    public string? ModificadoPor { get; set; }      // Auditoría
    public bool EsTituloSistema { get; set; }       // Títulos del sistema vs. personalizados
    public NivelTitular? NivelEquivalente { get; set; }  // Equivalencia con enum
    public string? ColorHex { get; set; }           // Color para UI

    // Navegación
    public virtual ICollection<ConfiguracionRequisito> ConfiguracionesComoActual { get; set; }
    public virtual ICollection<ConfiguracionRequisito> ConfiguracionesComoSolicitado { get; set; }
}
```

#### Entidad Modificada: `ConfiguracionRequisito`

```csharp
// Cambios principales:
- NivelActual?: NivelTitular? (ahora nullable)
- NivelSolicitado?: NivelTitular? (ahora nullable)
+ TituloActualId?: Guid? (nuevo campo)
+ TituloSolicitadoId?: Guid? (nuevo campo)
+ TituloActual: TituloAcademico? (navegación)
+ TituloSolicitado: TituloAcademico? (navegación)
```

### 2. Capa de Datos

#### Repository Pattern

```csharp
// Nueva interfaz: ITituloAcademicoRepository
public interface ITituloAcademicoRepository : IRepository<TituloAcademico>
{
    Task<List<TituloAcademico>> GetActivosAsync();
    Task<List<TituloAcademico>> GetByOrdenJerarquicoRangeAsync(int min, int max);
    Task<TituloAcademico?> GetByCodigoAsync(string codigo);
    Task<bool> ExisteCodigoAsync(string codigo, Guid? excludeId = null);
    Task<List<TituloAcademico>> GetPosiblesAscensosAsync(Guid tituloActualId);
    Task<bool> PuedeSerEliminadoAsync(Guid id);
}
```

#### Configuración de Base de Datos

```csharp
// ApplicationDbContext - Configuración híbrida
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Configuración de TituloAcademico
    modelBuilder.Entity<TituloAcademico>(entity =>
    {
        entity.HasIndex(e => e.Nombre).IsUnique();
        entity.HasIndex(e => e.Codigo).IsUnique();
        entity.HasIndex(e => e.OrdenJerarquico).IsUnique();
        entity.Property(e => e.EstaActivo).HasDefaultValue(true);
        entity.Property(e => e.EsTituloSistema).HasDefaultValue(false);
    });

    // Relaciones híbridas en ConfiguracionRequisito
    modelBuilder.Entity<ConfiguracionRequisito>(entity =>
    {
        entity.HasOne(e => e.TituloActual)
              .WithMany(t => t.ConfiguracionesComoActual)
              .HasForeignKey(e => e.TituloActualId)
              .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.TituloSolicitado)
              .WithMany(t => t.ConfiguracionesComoSolicitado)
              .HasForeignKey(e => e.TituloSolicitadoId)
              .OnDelete(DeleteBehavior.Restrict);

        // Constraint híbrido para integridad
        entity.HasCheckConstraint("CK_ConfiguracionRequisito_TipoNivel",
            "(NivelActual IS NOT NULL AND TituloActualId IS NULL AND NivelSolicitado IS NOT NULL AND TituloSolicitadoId IS NULL) OR " +
            "(NivelActual IS NULL AND TituloActualId IS NOT NULL AND NivelSolicitado IS NULL AND TituloSolicitadoId IS NOT NULL)");
    });
}
```

### 3. Capa de Aplicación

#### Servicios de Negocio

```csharp
// TituloAcademicoService - Lógica de negocio
public class TituloAcademicoService : ITituloAcademicoService
{
    // CRUD completo con validaciones
    public async Task<TituloAcademicoDto> CrearAsync(CrearTituloAcademicoDto dto, string usuarioEmail)
    public async Task<TituloAcademicoDto> ActualizarAsync(Guid id, ActualizarTituloAcademicoDto dto, string usuarioEmail)
    public async Task<bool> EliminarAsync(Guid id, string usuarioEmail)

    // Lógica específica de títulos
    public async Task<List<TituloAcademicoOpcionDto>> GetOpcionesAsync()
    public async Task<List<TituloAcademicoOpcionDto>> GetPosiblesAscensosAsync(Guid tituloActualId)
    public async Task<bool> PuedeSerEliminadoAsync(Guid id)
}
```

#### DTOs Híbridos

```csharp
// ConfiguracionRequisitoDto actualizado para soporte híbrido
public class ConfiguracionRequisitoDto
{
    // Soporte híbrido para niveles enum
    public NivelTitular? NivelActual { get; set; }
    public NivelTitular? NivelSolicitado { get; set; }

    // Soporte híbrido para títulos dinámicos
    public Guid? TituloActualId { get; set; }
    public Guid? TituloSolicitadoId { get; set; }
    public string? TituloActualNombre { get; set; }
    public string? TituloSolicitadoNombre { get; set; }

    // Propiedades de conveniencia
    public bool EsNivelEnum => NivelActual.HasValue && NivelSolicitado.HasValue;
    public bool EsTituloDinamico => TituloActualId.HasValue && TituloSolicitadoId.HasValue;
}
```

### 4. Capa de API

#### Controlador de Títulos Académicos

```csharp
// TitulosAcademicosController - Endpoints RESTful
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class TitulosAcademicosController : ControllerBase
{
    [HttpGet("opciones")]               // GET /api/TitulosAcademicos/opciones
    [HttpGet("{id}/posibles-ascensos")] // GET /api/TitulosAcademicos/{id}/posibles-ascensos
    [HttpGet("{id}/puede-ser-eliminado")] // GET /api/TitulosAcademicos/{id}/puede-ser-eliminado
    [HttpPost]                          // POST /api/TitulosAcademicos
    [HttpPut("{id}")]                   // PUT /api/TitulosAcademicos/{id}
    [HttpDelete("{id}")]                // DELETE /api/TitulosAcademicos/{id}
}
```

### 5. Capa de Presentación (Frontend)

#### Servicio Web

```csharp
// TitulosAcademicosService - Comunicación con API
public class TitulosAcademicosService
{
    public async Task<List<TituloAcademicoOpcionDto>> GetOpcionesAsync()
    public async Task<List<TituloAcademicoOpcionDto>> GetPosiblesAscensosAsync(Guid tituloActualId)
    public async Task<bool> PuedeSerEliminadoAsync(Guid id)
}
```

#### Interfaz Híbrida

```html
<!-- ConfiguracionRequisitos.razor - UI híbrida -->
<div class="mb-4">
  <label class="form-label fw-bold">Tipo de Configuración</label>
  <div class="d-flex gap-3">
    <div class="form-check">
      <input type="radio" name="tipoNivel" value="enum" checked="@(tipoNivel ==
      "enum")" @onclick="@(() => OnTipoNivelChanged("enum"))" />
      <label class="form-check-label">
        <i class="fas fa-layer-group text-primary me-1"></i>
        Niveles Predefinidos
      </label>
    </div>
    <div class="form-check">
      <input type="radio" name="tipoNivel" value="titulo" checked="@(tipoNivel
      == "titulo")" @onclick="@(() => OnTipoNivelChanged("titulo"))" />
      <label class="form-check-label">
        <i class="fas fa-graduation-cap text-success me-1"></i>
        Títulos Académicos Dinámicos
      </label>
    </div>
  </div>
</div>
```

## 📊 Base de Datos

### Migración Aplicada: `20250706012427_AgregarTitulosAcademicos`

#### Cambios en la Base de Datos

```sql
-- Nueva tabla TitulosAcademicos
CREATE TABLE TitulosAcademicos (
    Id uniqueidentifier NOT NULL PRIMARY KEY,
    Nombre nvarchar(100) NOT NULL,
    Descripcion nvarchar(500) NULL,
    OrdenJerarquico int NOT NULL,
    Codigo nvarchar(20) NOT NULL,
    EstaActivo bit NOT NULL DEFAULT 1,
    ModificadoPor nvarchar(255) NULL,
    EsTituloSistema bit NOT NULL DEFAULT 0,
    NivelEquivalente int NULL,
    ColorHex nvarchar(7) NULL,
    FechaCreacion datetime2 NOT NULL,
    FechaModificacion datetime2 NULL
);

-- Modificaciones en ConfiguracionesRequisitos
ALTER TABLE ConfiguracionesRequisitos
    ALTER COLUMN NivelActual nvarchar(450) NULL;

ALTER TABLE ConfiguracionesRequisitos
    ALTER COLUMN NivelSolicitado nvarchar(450) NULL;

ALTER TABLE ConfiguracionesRequisitos
    ADD TituloActualId uniqueidentifier NULL;

ALTER TABLE ConfiguracionesRequisitos
    ADD TituloSolicitadoId uniqueidentifier NULL;

-- Índices para performance
CREATE UNIQUE INDEX IX_TitulosAcademicos_Codigo ON TitulosAcademicos (Codigo);
CREATE UNIQUE INDEX IX_TitulosAcademicos_Nombre ON TitulosAcademicos (Nombre);
CREATE UNIQUE INDEX IX_TitulosAcademicos_OrdenJerarquico ON TitulosAcademicos (OrdenJerarquico);

-- Foreign Keys
ALTER TABLE ConfiguracionesRequisitos
    ADD CONSTRAINT FK_ConfiguracionesRequisitos_TitulosAcademicos_TituloActualId
    FOREIGN KEY (TituloActualId) REFERENCES TitulosAcademicos (Id);

-- Check Constraint para integridad híbrida
ALTER TABLE ConfiguracionesRequisitos
    ADD CONSTRAINT CK_ConfiguracionRequisito_TipoNivel
    CHECK (
        (NivelActual IS NOT NULL AND TituloActualId IS NULL AND
         NivelSolicitado IS NOT NULL AND TituloSolicitadoId IS NULL)
        OR
        (NivelActual IS NULL AND TituloActualId IS NOT NULL AND
         NivelSolicitado IS NULL AND TituloSolicitadoId IS NOT NULL)
    );
```

### Integridad de Datos

- **Constraint Híbrido:** Previene configuraciones mixtas inválidas
- **Índices Únicos:** Garantizan unicidad en nombre, código y orden jerárquico
- **Foreign Keys:** Mantienen integridad referencial
- **Cascade Restrict:** Previene eliminación accidental de títulos en uso

## 🔄 Flujo de Trabajo

### Para Administradores

1. **Acceso al Sistema**

   - Navegar a `/admin/configuracion-requisitos`
   - Autenticación requerida con rol Administrador

2. **Crear Nueva Configuración**

   - Hacer clic en "Nueva Configuración"
   - Seleccionar tipo: "Niveles Predefinidos" o "Títulos Académicos Dinámicos"

3. **Configuración de Niveles Predefinidos**

   - Seleccionar nivel actual del dropdown enum
   - Seleccionar nivel solicitado del dropdown enum
   - Configurar requisitos (tiempo, obras, evaluación, etc.)

4. **Configuración de Títulos Dinámicos**

   - Seleccionar título actual de la lista de títulos activos
   - Sistema carga automáticamente títulos posibles para ascenso
   - Seleccionar título solicitado de opciones válidas
   - Configurar requisitos específicos

5. **Guardar y Activar**
   - Revisar configuración
   - Guardar con estado activo/inactivo según necesidad

### Para el Sistema

1. **Validación Automática**

   - Check constraints en BD previenen configuraciones inválidas
   - Validaciones de negocio en servicios
   - Validación de UI en tiempo real

2. **Carga Dinámica**

   - Títulos se cargan dinámicamente según selección
   - Filtrado automático de opciones válidas
   - Estados de carga para mejor UX

3. **Persistencia Híbrida**
   - Configuraciones enum se guardan en campos tradicionales
   - Configuraciones dinámicas usan nuevas FK
   - Ambos tipos coexisten sin conflictos

## 📈 Beneficios Logrados

### 1. Flexibilidad Total

- ✅ **Nuevos Títulos:** Se pueden crear sin modificar código
- ✅ **Jerarquías Personalizadas:** Orden jerárquico configurable
- ✅ **Colores y Descriptions:** Personalización visual completa

### 2. Compatibilidad Completa

- ✅ **Sistema Existente:** Funciona sin cambios
- ✅ **Datos Existentes:** No se pierden configuraciones previas
- ✅ **APIs Existentes:** Continúan funcionando normalmente

### 3. Integridad de Datos

- ✅ **Constraints:** Previenen estados inválidos
- ✅ **Validaciones:** Múltiples capas de validación
- ✅ **Auditoría:** Tracking completo de cambios

### 4. Performance Optimizada

- ✅ **Índices:** Consultas eficientes
- ✅ **Lazy Loading:** Carga bajo demanda
- ✅ **Caching:** Títulos se cachean en frontend

### 5. Experiencia de Usuario

- ✅ **UI Intuitiva:** Radio buttons claros para selección de tipo
- ✅ **Carga Dinámica:** Opciones se actualizan automáticamente
- ✅ **Feedback Visual:** Estados de carga y validación en tiempo real
- ✅ **Distinción Visual:** Badges de colores distinguen tipos

## 🔍 Casos de Uso

### Caso 1: Configuración Tradicional (Enum)

```
Nivel Actual: Titular1 (enum)
Nivel Solicitado: Titular2 (enum)
Requisitos: 48 meses, 1 obra, 75% evaluación, 96h capacitación
Estado: Activo
```

### Caso 2: Configuración Dinámica (Títulos Personalizados)

```
Título Actual: "Profesor Asistente de Investigación" (dinámico)
Título Solicitado: "Profesor Asociado de Investigación" (dinámico)
Requisitos: 60 meses, 3 obras, 80% evaluación, 120h capacitación
Estado: Activo
```

### Caso 3: Coexistencia

- El sistema puede tener ambos tipos de configuraciones activas
- Los reportes muestran ambos tipos correctamente
- Las validaciones de ascenso funcionan para ambos tipos

## 🛠️ Herramientas y Tecnologías

### Backend

- **.NET 9.0:** Framework principal
- **Entity Framework Core:** ORM con migraciones automáticas
- **AutoMapper:** Mapeo entre entidades y DTOs
- **FluentValidation:** Validaciones robustas
- **ASP.NET Core API:** Endpoints RESTful

### Frontend

- **Blazor WebAssembly:** SPA framework
- **Bootstrap 5:** Framework CSS
- **Font Awesome:** Iconografía
- **JavaScript interop:** Funcionalidades nativas del browser

### Base de Datos

- **SQL Server:** Base de datos principal
- **Índices optimizados:** Performance de consultas
- **Constraints:** Integridad de datos
- **Foreign Keys:** Relaciones consistentes

## 📋 Validaciones Implementadas

### Backend Validations

1. **Títulos Únicos:** Nombre y código deben ser únicos
2. **Orden Jerárquico:** Debe ser único y positivo
3. **Configuración Híbrida:** Solo un tipo por configuración
4. **Ascensos Válidos:** Solo se permiten ascensos lógicos
5. **Eliminación Segura:** No se pueden eliminar títulos en uso

### Frontend Validations

1. **Campos Requeridos:** Validación en tiempo real
2. **Selección Coherente:** Opciones filtradas dinámicamente
3. **Feedback Visual:** Mensajes de error claros
4. **Estados de Carga:** Indicadores durante operaciones async

### Database Constraints

1. **Check Constraint Híbrido:** Previene configuraciones mixtas
2. **Unique Indexes:** Garantizan unicidad
3. **Foreign Key Constraints:** Integridad referencial
4. **Not Null Constraints:** Campos obligatorios

## 🔧 Configuración y Deployment

### Requisitos Previos

- .NET 9.0 SDK
- SQL Server 2019+
- Visual Studio 2022 o VS Code

### Pasos de Deployment

1. **Migración de BD:** `dotnet ef database update`
2. **Build del Proyecto:** `dotnet build`
3. **Configuración de Environment:** Variables de entorno para conexión BD
4. **Deploy de API y Web:** Según infraestructura target

### Variables de Entorno

```
ConnectionStrings__DefaultConnection=Server=...;Database=...;
```

## 📊 Métricas y Monitoreo

### KPIs Sugeridos

- **Adopción:** % de configuraciones que usan títulos dinámicos vs enum
- **Performance:** Tiempo de respuesta de endpoints de títulos
- **Errores:** Rate de errores en validaciones híbridas
- **Uso:** Frecuencia de creación de nuevos títulos

### Logging Implementado

- **Auditoría:** Todas las operaciones CRUD se registran
- **Performance:** Tiempos de consulta para optimización
- **Errores:** Stack traces completos para debugging
- **Seguridad:** Intentos de acceso no autorizado

## 🚀 Próximos Pasos (Opcionales)

### Mejoras Futuras Sugeridas

1. **UI de Gestión de Títulos**

   - Página dedicada para CRUD de títulos académicos
   - Bulk import/export de títulos
   - Gestión de jerarquías visuales

2. **Reportes Avanzados**

   - Dashboard de uso híbrido
   - Métricas de adopción
   - Análisis de rutas de ascenso más comunes

3. **Integraciones**

   - API para sistemas externos
   - Sincronización con sistemas académicos
   - Import desde planillas Excel

4. **Optimizaciones**
   - Caching distribuido para títulos
   - Índices adicionales según patrones de uso
   - Compresión de respuestas API

## 📝 Conclusiones

La implementación híbrida de requisitos dinámicos logra exitosamente:

- ✅ **Objetivo Principal:** Sistema totalmente flexible y dinámico
- ✅ **Compatibilidad:** 100% backward compatible
- ✅ **Integridad:** Database constraints robustos
- ✅ **Performance:** Optimizado para escala
- ✅ **UX:** Interfaz intuitiva y responsive
- ✅ **Mantenibilidad:** Código modular y bien documentado

El sistema está **listo para producción** y proporciona la base para futuras extensiones del modelo de ascensos académicos.

---

**📄 Documentación relacionada:**

- [Guía de Usuario](../README.md)
- [Documentación de API](../FRAMEWORK_MIGRACIONES_ARCHIVOS.md)
- [Estrategia de Migraciones](../ESTRATEGIA_MIGRACIONES.md)

**🔧 Mantenido por:** Equipo de Desarrollo SGA  
**📅 Última actualización:** Julio 5, 2025
