# Framework Avanzado de Gestión de Migraciones - SGA v2.0

## Problema Resuelto

Las migraciones de Entity Framework pueden ser problemáticas durante el desarrollo y producción, especialmente cuando se necesita:

- Agregar constraints personalizados y validaciones complejas
- Manejar múltiples entornos con diferentes configuraciones
- Garantizar rollbacks seguros y auditables
- Automatizar backups y restauraciones
- Aplicar scripts SQL pre y post-migración
- Mantener integridad de datos en 3FN

## Solución Implementada: Framework Extensible v2.0

### 1. Arquitectura del Framework

#### Componentes Principales

- **🎛️ Configuración Externa**: `migration-config.json` - Configuración por entornos
- **🚀 Framework Principal**: `migration-framework-v2.ps1` - Motor del framework extensible
- **📝 Generador de Scripts**: `new-migration-script.ps1` - Asistente para crear scripts
- **📋 Scripts Modulares**: Directorios organizados para cada tipo de script
- **🔄 Backups Automáticos**: Sistema inteligente de respaldo y recuperación
- **📊 Reportes HTML**: Documentación automática de cada operación

#### Estrategia Híbrida Mejorada

1. **Scripts Pre-Migración**: Preparación, validaciones y backups de datos críticos
2. **Migraciones EF Core**: Estructura básica de tablas y relaciones
3. **Scripts Post-Migración**: Constraints, índices, triggers y configuraciones avanzadas
4. **Scripts de Rollback**: Reversión controlada y segura de cambios

### 2. Estructura de Archivos

```
Scripts/
├── migration-config.json                 # ⚙️ Configuración principal
├── migration-framework-v2.ps1           # 🚀 Framework principal
├── new-migration-script.ps1              # 📝 Generador de scripts
├── manage-migrations.ps1                 # 🔧 Script simplificado (legacy)
├── Pre-Migration/                        # 📋 Scripts preparatorios
│   ├── 01-backup-critical-data.sql
│   └── 02-validate-data-integrity.sql
├── Post-Migration/                       # ⚡ Scripts post-aplicación
│   ├── 01-enum-constraints.sql
│   ├── 02-business-rules.sql
│   ├── 03-special-indexes.sql
│   └── 04-initial-data.sql
└── Rollback/                            # 🔄 Scripts de rollback
    └── 01-rollback-constraints.sql

Templates/                               # 📝 Plantillas para nuevos scripts
├── pre-migration-template.sql
├── post-migration-template.sql
└── rollback-template.sql

Backups/                                 # 💾 Backups automáticos
├── Development_SGA_Main_20250629_143022.bak
└── Development_SGA_Main_20250629_143022.json

Logs/Migrations/                         # 📊 Logs detallados
└── migration_2025-06-29.log

Reports/                                 # 📋 Reportes HTML
└── migration-report_20250629_143500.html
```

### 3. Comandos Principales del Framework v2.0

#### Inicialización del Framework

```powershell
# Crear configuración inicial
.\Scripts\migration-framework-v2.ps1 -Action init
```

#### Operaciones de Base de Datos

```powershell
# Verificación completa del estado
.\Scripts\migration-framework-v2.ps1 -Action verify

# Reset completo (solo desarrollo)
.\Scripts\migration-framework-v2.ps1 -Action reset -Force

# Actualización con migraciones pendientes
.\Scripts\migration-framework-v2.ps1 -Action update -Environment Staging

# Crear nueva migración
.\Scripts\migration-framework-v2.ps1 -Action create -MigrationName "AddAuditFields"

# Aplicar solo scripts post-migración
.\Scripts\migration-framework-v2.ps1 -Action apply-scripts
```

#### Gestión de Backups

```powershell
# Backup manual
.\Scripts\migration-framework-v2.ps1 -Action backup -BackupName "before-major-changes"

# Restaurar desde backup
.\Scripts\migration-framework-v2.ps1 -Action restore -BackupName "before-major-changes"
```

#### Herramientas de Desarrollo

```powershell
# Generar nuevo script post-migración
.\Scripts\new-migration-script.ps1 -Type post -Name "Add User Permissions"

# Generar script de rollback
.\Scripts\new-migration-script.ps1 -Type rollback -Name "Remove User Permissions"

# Dry run (ver qué haría sin ejecutar)
.\Scripts\migration-framework-v2.ps1 -Action reset -DryRun

# Generar reporte completo
.\Scripts\migration-framework-v2.ps1 -Action report
```

### 4. Configuración por Entornos

El archivo `migration-config.json` permite configurar diferentes comportamientos por entorno:

```json
{
  "environments": {
    "Development": {
      "allowReset": true,
      "autoBackup": false,
      "requireConfirmation": false
    },
    "Staging": {
      "allowReset": false,
      "autoBackup": true,
      "requireConfirmation": true
    },
    "Production": {
      "allowReset": false,
      "autoBackup": true,
      "requireConfirmation": true
    }
  }
}
```

#### Configuraciones Clave

- **allowReset**: Permite operaciones destructivas (solo desarrollo)
- **autoBackup**: Crea backups automáticos antes de cambios
- **requireConfirmation**: Requiere confirmación explícita para operaciones críticas
- **retentionDays**: Días de retención para backups y logs
- **compressionEnabled**: Compresión de backups para ahorrar espacio

### 5. Características Avanzadas del Framework

#### 🔒 Seguridad y Validaciones

- **Validaciones pre-ejecución**: Verifica herramientas requeridas y permisos
- **Confirmaciones por entorno**: Protección adicional en staging/producción
- **Rollback automático**: Restauración desde backup en caso de error
- **Logs auditables**: Registro completo de todas las operaciones

#### 🚀 Automatización Inteligente

- **Detección automática de scripts**: Ejecuta scripts en orden alfabético
- **Manejo de errores robusto**: Continúa o aborta según configuración
- **Progreso visual**: Barras de progreso y estados detallados
- **Timeouts configurables**: Evita bloqueos en operaciones largas

#### 📊 Reportes y Monitoreo

- **Reportes HTML**: Documentación automática de cada operación
- **Métricas detalladas**: Tiempo de ejecución, scripts ejecutados, errores
- **Estado de salud**: Validación completa del estado de la BD
- **Historial de migraciones**: Seguimiento de todas las migraciones aplicadas

#### 🔄 Gestión de Scripts Modular

- **Scripts versionados**: Numeración automática y orden de ejecución
- **Plantillas reutilizables**: Generación rápida de nuevos scripts
- **Validaciones integradas**: Verificación antes y después de ejecutar
- **Soporte para rollback**: Scripts específicos para revertir cambios

### 6. Flujos de Trabajo Recomendados

#### Desarrollo Local

```powershell
# 1. Cambios en entidades -> Reset completo
.\Scripts\migration-framework-v2.ps1 -Action reset -Force

# 2. Verificar estado después de cambios
.\Scripts\migration-framework-v2.ps1 -Action verify

# 3. Crear script para nuevas reglas de negocio
.\Scripts\new-migration-script.ps1 -Type post -Name "Add Business Validation"
```

#### Staging/Producción

```powershell
# 1. Backup preventivo
.\Scripts\migration-framework-v2.ps1 -Action backup -BackupName "before-release-v1.2"

# 2. Aplicar migraciones con scripts
.\Scripts\migration-framework-v2.ps1 -Action update -Environment Production

# 3. Generar reporte de la operación
.\Scripts\migration-framework-v2.ps1 -Action report
```

#### Recuperación de Errores

```powershell
# Restaurar desde backup más reciente
.\Scripts\migration-framework-v2.ps1 -Action restore -BackupName "before-release-v1.2"

# Ejecutar scripts de rollback específicos
.\Scripts\migration-framework-v2.ps1 -Action rollback
```

### 7. Beneficios de esta Estrategia

#### Para Desarrollo

- ✅ Reset completo en 1 comando
- ✅ No más conflictos de migración
- ✅ Constraints garantizados desde el inicio
- ✅ Fácil de mantener y evolucionar

#### Para Producción

- ✅ Migraciones limpias y predecibles
- ✅ Scripts SQL versionados y auditables
- ✅ Rollback controlado por separado
- ✅ Aplicación gradual de cambios

#### Para 3FN Compliance

- ✅ Constraints a nivel de BD (no solo aplicación)
- ✅ Integridad referencial garantizada
- ✅ Roles de Titular Auxiliar 1-5 únicamente
- ✅ Progresión secuencial obligatoria

### 8. Uso en Diferentes Escenarios

#### Desarrollo Activo

```powershell
# Cambié entidades -> Reset completo
.\Scripts\manage-migrations.ps1 -Action reset
```

#### Producción/Staging

```powershell
# Aplicar solo migraciones EF
dotnet ef database update --context ApplicationDbContext

# Aplicar constraints por separado (si no existen)
sqlcmd -S "server" -d "SGA_Main" -E -i "Scripts\add-constraints-simple.sql"
```

#### Verificación/Debug

```powershell
# Ver estado actual
.\Scripts\manage-migrations.ps1 -Action verify
```

### 9. Próximos Pasos Sugeridos

1. **Integración CI/CD**: Automatizar estos scripts en pipeline
2. **Scripts de Rollback**: Crear scripts para revertir constraints
3. **Validación de Datos**: Scripts para validar datos existentes antes de constraints
4. **Monitoreo**: Alertas si constraints fallan en producción

### 10. Estructura Final Garantizada

La base de datos ahora cumple con:

- ✅ **1FN**: Todos los campos atómicos
- ✅ **2FN**: Sin dependencias parciales
- ✅ **3FN**: Sin dependencias transitivas
- ✅ **Enum Constraints**: Solo valores válidos permitidos
- ✅ **Business Rules**: Lógica de negocio a nivel BD
- ✅ **Integridad Referencial**: FK y constraints coherentes

## Conclusión

Esta estrategia resuelve los problemas de migraciones complejas separando la responsabilidad:

- **EF Core**: Estructura básica y relaciones
- **SQL Scripts**: Lógica avanzada y constraints personalizados

Resultado: Sistema robusto, mantenible y compatible con 3FN.
