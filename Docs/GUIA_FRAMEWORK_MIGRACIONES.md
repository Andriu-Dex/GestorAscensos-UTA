# Guía Completa del Framework de Migraciones SGA v2.0

## 📋 Resumen

Este framework proporciona una solución **completa, robusta y extensible** para gestionar migraciones de Entity Framework Core en cualquier proyecto .NET, no solo el actual SGA. Está diseñado para ser reutilizable y configurable para diferentes proyectos y entornos.

## 🚀 Características Principales

### ✅ **Configuración Externa**

- Archivo JSON configurable por entornos
- Soporte para múltiples proyectos y contextos
- Configuraciones de seguridad por entorno

### ✅ **Automatización Completa**

- Scripts pre-migración, post-migración y rollback
- Backups automáticos antes de operaciones destructivas
- Validaciones previas y post-ejecución

### ✅ **Seguridad y Auditoría**

- Protecciones por entorno (desarrollo/staging/producción)
- Logging detallado de todas las operaciones
- Reportes HTML automáticos

### ✅ **Facilidad de Uso**

- Comandos simples para operaciones complejas
- Generador automático de scripts SQL
- Plantillas reutilizables

## 📁 Estructura de Archivos

```
Scripts/
├── migration-config.json                 # ⚙️ Configuración principal
├── migration-framework-v2.ps1           # 🚀 Framework principal
├── new-migration-script.ps1              # 📝 Generador de scripts
├── manage-migrations.ps1                 # 🔧 Script simplificado (legacy)
├── Pre-Migration/                        # 📋 Scripts preparatorios
│   └── [ejemplo: 01-backup-data.sql]
├── Post-Migration/                       # ⚡ Scripts post-aplicación
│   ├── 01-enum-constraints.sql          # ✅ Constraints de enum
│   ├── 02-business-rules.sql            # ✅ Reglas de negocio
│   ├── 03-special-indexes.sql           # ✅ Índices especiales
│   ├── 04-initial-data.sql              # ✅ Datos iniciales
│   └── 05-add-audit-fields.sql          # ✅ Script generado
└── Rollback/                            # 🔄 Scripts de rollback

Templates/                               # 📝 Plantillas para nuevos scripts
├── pre-migration-template.sql
├── post-migration-template.sql
└── rollback-template.sql

Backups/                                 # 💾 Backups automáticos
└── [archivos .bak y .json]

Logs/Migrations/                         # 📊 Logs detallados
└── migration_2025-06-29.log

Reports/                                 # 📋 Reportes HTML
└── migration-report_20250629_170633.html
```

## 🎯 Comandos Principales

### **Inicialización (Solo primera vez)**

```powershell
# Crear configuración inicial del framework
.\Scripts\migration-framework-v2.ps1 -Action init
```

### **Operaciones de Base de Datos**

```powershell
# Verificar estado actual
.\Scripts\migration-framework-v2.ps1 -Action verify

# Reset completo (solo desarrollo)
.\Scripts\migration-framework-v2.ps1 -Action reset -Force

# Actualizar con migraciones pendientes
.\Scripts\migration-framework-v2.ps1 -Action update -Environment Staging

# Crear nueva migración EF
.\Scripts\migration-framework-v2.ps1 -Action create -MigrationName "AddAuditFields"

# Aplicar solo scripts post-migración
.\Scripts\migration-framework-v2.ps1 -Action apply-scripts
```

### **Gestión de Backups**

```powershell
# Backup manual antes de cambios importantes
.\Scripts\migration-framework-v2.ps1 -Action backup -BackupName "before-major-changes" -Environment Staging

# Restaurar desde backup específico
.\Scripts\migration-framework-v2.ps1 -Action restore -BackupName "before-major-changes"
```

### **Herramientas de Desarrollo**

```powershell
# Generar nuevo script post-migración
.\Scripts\new-migration-script.ps1 -Type post -Name "Add User Permissions"

# Generar script de rollback
.\Scripts\new-migration-script.ps1 -Type rollback -Name "Remove User Permissions"

# Ver qué haría sin ejecutar (dry run)
.\Scripts\migration-framework-v2.ps1 -Action reset -DryRun

# Generar reporte completo
.\Scripts\migration-framework-v2.ps1 -Action report
```

## ⚙️ Configuración por Entornos

El archivo `migration-config.json` permite diferentes comportamientos:

```json
{
  "environments": {
    "Development": {
      "allowReset": true, // ✅ Permite reset completo
      "autoBackup": false, // ❌ Sin backups automáticos
      "requireConfirmation": false // ❌ Sin confirmaciones
    },
    "Staging": {
      "allowReset": false, // ❌ No permite reset
      "autoBackup": true, // ✅ Backups automáticos
      "requireConfirmation": true // ✅ Requiere confirmación
    },
    "Production": {
      "allowReset": false, // ❌ No permite reset
      "autoBackup": true, // ✅ Backups automáticos
      "requireConfirmation": true // ✅ Requiere confirmación
    }
  }
}
```

## 🔄 Flujos de Trabajo Recomendados

### **Desarrollo Local**

```powershell
# 1. Cambios en entidades -> Reset completo
.\Scripts\migration-framework-v2.ps1 -Action reset -Force

# 2. Verificar que todo esté correcto
.\Scripts\migration-framework-v2.ps1 -Action verify

# 3. Crear script para nuevas reglas de negocio
.\Scripts\new-migration-script.ps1 -Type post -Name "Add Business Validation"
```

### **Staging/Producción**

```powershell
# 1. Backup preventivo
.\Scripts\migration-framework-v2.ps1 -Action backup -BackupName "before-release-v1.2" -Environment Production

# 2. Aplicar migraciones con scripts
.\Scripts\migration-framework-v2.ps1 -Action update -Environment Production

# 3. Generar reporte de la operación
.\Scripts\migration-framework-v2.ps1 -Action report -Environment Production
```

### **Recuperación de Errores**

```powershell
# Restaurar desde backup más reciente
.\Scripts\migration-framework-v2.ps1 -Action restore -BackupName "before-release-v1.2" -Environment Production

# O ejecutar scripts de rollback específicos
.\Scripts\migration-framework-v2.ps1 -Action rollback -Environment Production
```

## 🎨 Creación de Scripts Personalizados

### **Scripts Post-Migración**

Úsalos para:

- Constraints de validación
- Índices especiales
- Triggers de auditoría
- Datos de configuración

```powershell
.\Scripts\new-migration-script.ps1 -Type post -Name "Add Audit Triggers"
```

### **Scripts Pre-Migración**

Úsalos para:

- Backup de datos críticos
- Validaciones previas
- Limpieza de datos obsoletos

```powershell
.\Scripts\new-migration-script.ps1 -Type pre -Name "Backup Critical Data"
```

### **Scripts de Rollback**

Úsalos para:

- Revertir constraints
- Eliminar índices agregados
- Restaurar configuraciones

```powershell
.\Scripts\new-migration-script.ps1 -Type rollback -Name "Remove Audit Triggers"
```

## 📊 Reportes y Monitoreo

El framework genera automáticamente:

- **Logs detallados** de todas las operaciones
- **Reportes HTML** con el estado completo del sistema
- **Información de backups** con metadatos JSON
- **Métricas de tiempo** de ejecución

Los reportes incluyen:

- Estado de la base de datos
- Migraciones aplicadas
- Scripts ejecutados
- Errores y advertencias
- Configuración utilizada

## 🔧 Casos de Uso Avanzados

### **Integración con CI/CD**

```powershell
# En pipeline de despliegue
.\Scripts\migration-framework-v2.ps1 -Action update -Environment Production -Force
```

### **Validación Previa a Despliegue**

```powershell
# Verificar sin aplicar cambios
.\Scripts\migration-framework-v2.ps1 -Action verify -Environment Production -DryRun
```

### **Rollback Rápido**

```powershell
# Restaurar estado anterior
.\Scripts\migration-framework-v2.ps1 -Action restore -BackupName "before-release-v1.1" -Environment Production
```

## 🚧 Extensibilidad

Este framework está diseñado para ser **completamente extensible**:

1. **Nuevos Entornos**: Agregar configuraciones en `migration-config.json`
2. **Múltiples Proyectos**: Configurar diferentes contextos de EF
3. **Scripts Personalizados**: Usar las plantillas para crear nuevos scripts
4. **Integraciones**: Hooks para sistemas externos (CI/CD, monitoreo)

## ✅ Estado Actual SGA

**Sistema completamente funcional** con:

- ✅ **7 tablas** en la base de datos
- ✅ **7 constraints** de validación (enum + business rules)
- ✅ **199 índices** (incluyendo únicos e índices filtrados)
- ✅ **1 migración aplicada** correctamente
- ✅ **Base de datos saludable** y cumpliendo 3FN

## 🎉 Ventajas del Framework

### **Para el Desarrollo**

- **Reset en 1 comando** - Sin más conflictos de migración
- **Scripts versionados** - Fácil seguimiento de cambios
- **Validaciones automáticas** - Menos errores manuales

### **Para Producción**

- **Backups automáticos** - Rollback seguro siempre disponible
- **Confirmaciones por entorno** - Protección contra errores humanos
- **Auditoría completa** - Logs de todas las operaciones

### **Para el Equipo**

- **Reutilizable** - Usar en cualquier proyecto .NET
- **Plantillas** - Creación rápida de scripts consistentes
- **Documentación automática** - Reportes HTML detallados

## 📞 Soporte y Mantenimiento

Este framework es **completamente autónomo** y **fácil de mantener**:

- Configuración en archivos JSON claros
- Scripts SQL modulares y versionados
- Logging detallado para troubleshooting
- Plantillas para consistencia

**¡El framework está listo para manejar migraciones futuras de manera profesional y segura!**
