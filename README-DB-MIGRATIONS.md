# Instrucciones para Gestionar Migraciones de Base de Datos

Este documento proporciona instrucciones para utilizar las herramientas de migración de base de datos en el Sistema de Gestión y Control de Ascensos.

## Herramientas incluidas

En este proyecto se incluyen varias herramientas para gestionar las migraciones de base de datos:

1. **Menu interactivo**: `database-tools.bat` o `database-tools.ps1`
2. **Script de PowerShell**: `db-migrations.ps1`
3. **Aplicación de consola**: `SGA.MigrationTool`
4. **Migración automática**: Integrada en la API

## Forma más sencilla: Menú interactivo

Para la mayoría de usuarios, la forma más sencilla de gestionar las migraciones es mediante el menú interactivo:

1. Haga doble clic en `database-tools.bat` o ejecute `.\database-tools.ps1` en PowerShell
2. Seleccione la opción deseada en el menú

## Creación de migraciones desde la línea de comandos

Para crear una nueva migración que refleje los cambios en el modelo de datos:

```powershell
.\db-migrations.ps1 add NombreDeLaMigracion
```

Por ejemplo:

```powershell
.\db-migrations.ps1 add AgregarCampoTelefono
```

## Aplicación de migraciones desde la línea de comandos

Para aplicar todas las migraciones pendientes a la base de datos:

```powershell
.\db-migrations.ps1 apply
```

## Listar migraciones desde la línea de comandos

Para ver todas las migraciones disponibles y su estado:

```powershell
.\db-migrations.ps1 list
```

## Eliminar la última migración

Para eliminar la última migración (solo si no se ha aplicado a la base de datos):

```powershell
.\db-migrations.ps1 remove
```

## Ejecutar la aplicación con migración automática

La API está configurada para aplicar automáticamente las migraciones pendientes al iniciar. Para iniciar la API:

```powershell
dotnet run --project SGA.Api/SGA.Api.csproj
```

## Ejecutar la herramienta de migración

También puede ejecutar la herramienta de consola dedicada para migraciones:

```powershell
dotnet run --project SGA.MigrationTool/SGA.MigrationTool.csproj
```

## Información adicional

Para obtener información más detallada sobre las migraciones, consulte el archivo `MigracionesDB.md`.
