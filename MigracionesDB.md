# Guía de Migración de Base de Datos para SGA

Este documento describe las herramientas y procesos disponibles para gestionar las migraciones de base de datos en el Sistema de Gestión y Control de Ascensos para Profesores.

## Herramientas disponibles

El sistema ofrece varias formas de gestionar las migraciones de base de datos:

1. **Migración automática al iniciar la API**: Las migraciones pendientes se aplican automáticamente cuando se inicia la aplicación API.
2. **Herramienta de línea de comandos (`db-migrations.ps1`)**: Un script de PowerShell para crear, listar y aplicar migraciones.
3. **Aplicación de consola (`SGA.MigrationTool`)**: Una herramienta independiente para aplicar migraciones a la base de datos.

## Uso del script PowerShell

El script `db-migrations.ps1` proporciona una interfaz sencilla para gestionar migraciones desde la línea de comandos.

### Crear una nueva migración

```powershell
.\db-migrations.ps1 add NombreDeLaMigracion
```

Esto creará una nueva migración con el nombre especificado en la carpeta `SGA.Infrastructure/Migrations`.

### Aplicar migraciones pendientes

```powershell
.\db-migrations.ps1 apply
```

Este comando aplicará todas las migraciones pendientes a la base de datos configurada en `appsettings.json`.

### Listar todas las migraciones

```powershell
.\db-migrations.ps1 list
```

Muestra todas las migraciones disponibles y su estado (aplicada o pendiente).

### Eliminar la última migración

```powershell
.\db-migrations.ps1 remove
```

Elimina la última migración (solo si aún no se ha aplicado a la base de datos).

### Ayuda

```powershell
.\db-migrations.ps1 help
```

Muestra información de ayuda sobre el uso del script.

## Uso de la herramienta de consola (SGA.MigrationTool)

La herramienta de consola es útil para aplicar migraciones en entornos donde no se puede ejecutar PowerShell o donde se necesita una aplicación independiente.

1. Compilar la herramienta:

   ```
   dotnet build SGA.MigrationTool
   ```

2. Ejecutar la herramienta:
   ```
   dotnet run --project SGA.MigrationTool
   ```

La herramienta verificará si hay migraciones pendientes y preguntará si desea aplicarlas.

## Migración programática

El sistema también incluye la clase `DatabaseMigrationManager` en la capa de infraestructura, que puede utilizarse para aplicar migraciones programáticamente desde cualquier parte del código:

```csharp
// Obtener el proveedor de servicios
var serviceProvider = /* ... */;

// Aplicar migraciones
await DatabaseMigrationManager.MigrateAsync(serviceProvider, logger);

// Verificar si se necesitan migraciones
bool needsMigration = await DatabaseMigrationManager.NeedsMigrationAsync(serviceProvider);

// Asegurar que la base de datos existe
await DatabaseMigrationManager.EnsureDatabaseCreatedAsync(serviceProvider, logger);
```

## Consideraciones importantes

- Las migraciones se ejecutan dentro de un `TransactionScope` para garantizar operaciones atómicas.
- Se recomienda hacer una copia de seguridad de la base de datos antes de aplicar migraciones en producción.
- La cadena de conexión se configura en el archivo `appsettings.json` de cada proyecto.
- Las migraciones generan un registro completo (logs) de todas las operaciones realizadas.

## Solución de problemas comunes

### Error al crear una migración

Si aparece un error al crear una migración, asegúrese de que:

- La solución compila correctamente
- Tiene instaladas las herramientas de Entity Framework Core
- Está ejecutando el comando desde la carpeta raíz del proyecto

### Error al aplicar una migración

Si aparece un error al aplicar una migración, verifique que:

- La cadena de conexión es correcta
- El servidor SQL está en funcionamiento
- Tiene permisos para crear/modificar tablas en la base de datos
- No hay conflictos con migraciones anteriores
