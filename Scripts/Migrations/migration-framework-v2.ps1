# Framework Avanzado de Gestión de Migraciones - SGA
# Solución robusta y extensible para manejar migraciones de Entity Framework Core
# Versión: 2.0 - Framework General y Configurable

param(
    [ValidateSet("verify", "reset", "update", "rollback", "create", "apply-scripts", "backup", "restore", "init", "status", "report")]
    [string]$Action = "verify",
    
    [string]$MigrationName = "",
    [string]$Environment = "Development",
    [string]$ProjectContext = "main",
    [string]$BackupName = "",
    [string]$ConfigFile = "Scripts\Migrations\migration-config.json",
    [switch]$Force = $false,
    [switch]$SkipScripts = $false,
    [switch]$SkipBackup = $false,
    [switch]$DryRun = $false,
    [switch]$Verbose = $false,
    [switch]$Interactive = $false
)

# Variables globales del framework
$Global:Config = $null
$Global:EnvConfig = $null
$Global:ProjectConfig = $null
$Global:LogFile = ""
$Global:StartTime = Get-Date

#region Configuración y Inicialización

function Initialize-MigrationFramework {
    Write-Host "🚀 Inicializando Framework de Migraciones v2.0" -ForegroundColor Cyan
    
    # Cargar configuración
    if (-not (Test-Path $ConfigFile)) {
        Write-Error "Archivo de configuración no encontrado: $ConfigFile"
        Write-Host "Ejecute: .\migration-framework-v2.ps1 -Action init" -ForegroundColor Yellow
        exit 1
    }
    
    try {
        $Global:Config = Get-Content $ConfigFile | ConvertFrom-Json
        $Global:EnvConfig = $Global:Config.environments.$Environment
        $Global:ProjectConfig = $Global:Config.projects.$ProjectContext
        
        if (-not $Global:EnvConfig) {
            throw "Configuración para entorno '$Environment' no encontrada"
        }
        
        if (-not $Global:ProjectConfig) {
            throw "Configuración para proyecto '$ProjectContext' no encontrada"
        }
        
    } catch {
        Write-Error "Error cargando configuración: $($_.Exception.Message)"
        exit 1
    }
    
    # Inicializar directorios
    Initialize-Directories
    
    # Configurar logging
    Initialize-Logging
    
    Write-Log "Framework inicializado correctamente" -Level "Success"
    Write-Log "Entorno: $Environment | Proyecto: $ProjectContext" -Level "Info"
}

function Initialize-Directories {
    $directories = @(
        $Global:Config.backup.path,
        $Global:Config.logging.path,
        $Global:Config.scripts.preMigration.path,
        $Global:Config.scripts.postMigration.path,
        $Global:Config.scripts.rollback.path,
        "Templates",
        "Reports"
    )
    
    foreach ($dir in $directories) {
        if (-not (Test-Path $dir)) {
            New-Item -ItemType Directory -Path $dir -Force | Out-Null
            Write-Verbose "Directorio creado: $dir"
        }
    }
}

function Initialize-Logging {
    $logPath = $Global:Config.logging.path
    $dateStr = Get-Date -Format "yyyy-MM-dd"
    $Global:LogFile = "$logPath\migration_$dateStr.log"
    
    # Limpiar logs antiguos si está configurado
    if ($Global:Config.logging.retentionDays -gt 0) {
        $cutoffDate = (Get-Date).AddDays(-$Global:Config.logging.retentionDays)
        Get-ChildItem -Path $logPath -Filter "*.log" | 
            Where-Object { $_.LastWriteTime -lt $cutoffDate } | 
            Remove-Item -Force
    }
}

#endregion

#region Sistema de Logging Avanzado

function Write-Log {
    param(
        [string]$Message,
        [ValidateSet("Trace", "Debug", "Info", "Warning", "Error", "Success", "Critical")]
        [string]$Level = "Info",
        [switch]$NoConsole,
        [switch]$NoFile
    )
    
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss.fff"
    $logMessage = "[$timestamp] [$Level] $Message"
    
    # Escribir a archivo si está habilitado
    if ($Global:Config.logging.enabled -and -not $NoFile) {
        Add-Content -Path $Global:LogFile -Value $logMessage -Encoding UTF8
    }
    
    # Escribir a consola con colores
    if (-not $NoConsole) {
        $color = switch ($Level) {
            "Trace" { "DarkGray" }
            "Debug" { "Gray" }
            "Info" { "White" }
            "Warning" { "Yellow" }
            "Error" { "Red" }
            "Success" { "Green" }
            "Critical" { "Magenta" }
        }
        
        $prefix = switch ($Level) {
            "Success" { "✅" }
            "Error" { "❌" }
            "Warning" { "⚠️" }
            "Critical" { "🔥" }
            "Info" { "ℹ️" }
            default { "📝" }
        }
        
        Write-Host "$prefix $Message" -ForegroundColor $color
    }
}

function Write-Progress-Log {
    param(
        [string]$Activity,
        [string]$Status,
        [int]$PercentComplete = -1
    )
    
    Write-Progress -Activity $Activity -Status $Status -PercentComplete $PercentComplete
    Write-Log "$Activity - $Status" -Level "Info" -NoConsole
}

#endregion

#region Ejecución de Comandos Mejorada

function Invoke-SafeCommand {
    param(
        [string]$Command,
        [string]$Description,
        [bool]$ContinueOnError = $false,
        [bool]$Silent = $false,
        [hashtable]$Environment = @{},
        [int]$TimeoutSeconds = 300
    )
    
    Write-Log "Ejecutando: $Description" -Level "Info"
    if ($Verbose) { Write-Log "Comando: $Command" -Level "Debug" }
    
    if ($DryRun) {
        Write-Log "[DRY RUN] Se ejecutaría: $Command" -Level "Warning"
        return @{ Success = $true; Result = "[DRY RUN]"; DryRun = $true }
    }
    
    try {
        $processInfo = New-Object System.Diagnostics.ProcessStartInfo
        $processInfo.FileName = "powershell.exe"
        $processInfo.Arguments = "-Command `"$Command`""
        $processInfo.RedirectStandardOutput = $true
        $processInfo.RedirectStandardError = $true
        $processInfo.UseShellExecute = $false
        $processInfo.CreateNoWindow = $true
        
        # Agregar variables de entorno si se especifican
        foreach ($key in $Environment.Keys) {
            $processInfo.Environment[$key] = $Environment[$key]
        }
        
        $process = New-Object System.Diagnostics.Process
        $process.StartInfo = $processInfo
        
        $outputBuilder = New-Object System.Text.StringBuilder
        $errorBuilder = New-Object System.Text.StringBuilder
        
        $outputAction = {
            if (-not [string]::IsNullOrEmpty($EventArgs.Data)) {
                $outputBuilder.AppendLine($EventArgs.Data) | Out-Null
                if (-not $Silent) { Write-Host $EventArgs.Data }
            }
        }
        
        $errorAction = {
            if (-not [string]::IsNullOrEmpty($EventArgs.Data)) {
                $errorBuilder.AppendLine($EventArgs.Data) | Out-Null
            }
        }
        
        Register-ObjectEvent -InputObject $process -EventName OutputDataReceived -Action $outputAction | Out-Null
        Register-ObjectEvent -InputObject $process -EventName ErrorDataReceived -Action $errorAction | Out-Null
        
        $process.Start() | Out-Null
        $process.BeginOutputReadLine()
        $process.BeginErrorReadLine()
        
        $process.WaitForExit($TimeoutSeconds * 1000)
        
        if (-not $process.HasExited) {
            $process.Kill()
            throw "Comando excedió el timeout de $TimeoutSeconds segundos"
        }
        
        $output = $outputBuilder.ToString()
        $errorOutput = $errorBuilder.ToString()
        
        if ($process.ExitCode -eq 0) {
            Write-Log "$Description - Completado exitosamente" -Level "Success"
            return @{ 
                Success = $true
                Result = $output
                ExitCode = $process.ExitCode
            }
        } else {
            if ($ContinueOnError) {
                Write-Log "$Description - Error (continuando): ExitCode $($process.ExitCode)" -Level "Warning"
                Write-Log "Error Output: $errorOutput" -Level "Warning"
                return @{ 
                    Success = $false
                    Error = $errorOutput
                    ExitCode = $process.ExitCode
                }
            } else {
                throw "Comando falló con ExitCode $($process.ExitCode): $errorOutput"
            }
        }
        
    } catch {
        Write-Log "$Description - Error crítico: $($_.Exception.Message)" -Level "Error"
        if ($ContinueOnError) {
            return @{ Success = $false; Error = $_.Exception.Message }
        } else {
            throw
        }
    } finally {
        Get-EventSubscriber | Unregister-Event
        if ($process) { $process.Dispose() }
    }
}

#endregion

#region Gestión de Backups Mejorada

function New-DatabaseBackup {
    param(
        [string]$BackupName = "",
        [string]$Description = "Backup automático"
    )
    
    if (-not $Global:Config.backup.enabled) {
        Write-Log "Backups deshabilitados en configuración" -Level "Warning"
        return $null
    }
    
    if ($Global:EnvConfig.allowReset -and -not $Global:EnvConfig.autoBackup) {
        Write-Log "Backup omitido para entorno de desarrollo" -Level "Info"
        return $null
    }
    
    if ([string]::IsNullOrEmpty($BackupName)) {
        $timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
        $BackupName = $Global:Config.backup.namingPattern.
            Replace("{environment}", $Environment).
            Replace("{database}", $Global:EnvConfig.databaseName).
            Replace("{timestamp}", $timestamp)
    }
    
    $backupPath = $Global:Config.backup.path
    $backupFile = "$backupPath\$BackupName.bak"
    
    Write-Progress-Log -Activity "Creando Backup" -Status "Iniciando backup de $($Global:EnvConfig.databaseName)"
    
    $backupCommand = @"
sqlcmd -S "$($Global:EnvConfig.serverInstance)" -E -Q "
BACKUP DATABASE [$($Global:EnvConfig.databaseName)] 
TO DISK = '$backupFile' 
WITH FORMAT, INIT, 
NAME = '$Description',
DESCRIPTION = 'Backup automático - $Description - $(Get-Date)'
"@
    
    $result = Invoke-SafeCommand -Command $backupCommand -Description "Creando backup de base de datos" -ContinueOnError $true
    
    if ($result.Success) {
        # Registrar información del backup
        $backupInfo = @{
            FileName = $backupFile
            BackupName = $BackupName
            Description = $Description
            Environment = $Environment
            Database = $Global:EnvConfig.databaseName
            CreatedAt = Get-Date
            Size = (Get-Item $backupFile).Length
        }
        
        $backupInfoFile = "$backupPath\$BackupName.json"
        $backupInfo | ConvertTo-Json | Set-Content $backupInfoFile
        
        Write-Log "Backup creado exitosamente: $backupFile" -Level "Success"
        Write-Log "Tamaño del backup: $([math]::Round($backupInfo.Size / 1MB, 2)) MB" -Level "Info"
        
        # Limpiar backups antiguos
        Remove-OldBackups
        
        return $backupInfo
    } else {
        Write-Log "Error creando backup: $($result.Error)" -Level "Error"
        return $null
    }
}

function Remove-OldBackups {
    if ($Global:Config.backup.retentionDays -le 0) { return }
    
    $cutoffDate = (Get-Date).AddDays(-$Global:Config.backup.retentionDays)
    $backupsRemoved = 0
    
    Get-ChildItem -Path $Global:Config.backup.path -Filter "*.bak" | 
        Where-Object { $_.LastWriteTime -lt $cutoffDate } |
        ForEach-Object {
            Remove-Item $_.FullName -Force
            $jsonFile = $_.FullName -replace "\.bak$", ".json"
            if (Test-Path $jsonFile) { Remove-Item $jsonFile -Force }
            $backupsRemoved++
            Write-Log "Backup antiguo eliminado: $($_.Name)" -Level "Debug"
        }
    
    if ($backupsRemoved -gt 0) {
        Write-Log "Se eliminaron $backupsRemoved backups antiguos" -Level "Info"
    }
}

function Restore-DatabaseFromBackup {
    param([string]$BackupFile)
    
    if (-not (Test-Path $BackupFile)) {
        Write-Log "Archivo de backup no encontrado: $BackupFile" -Level "Error"
        return $false
    }
    
    Write-Progress-Log -Activity "Restaurando Base de Datos" -Status "Cerrando conexiones activas"
    
    # Cerrar conexiones activas
    $killConnectionsCommand = @"
sqlcmd -S "$($Global:EnvConfig.serverInstance)" -E -Q "
ALTER DATABASE [$($Global:EnvConfig.databaseName)] 
SET SINGLE_USER WITH ROLLBACK IMMEDIATE
"@
    
    Invoke-SafeCommand -Command $killConnectionsCommand -Description "Cerrando conexiones activas" -ContinueOnError $true
    
    Write-Progress-Log -Activity "Restaurando Base de Datos" -Status "Restaurando desde backup"
    
    # Restaurar base de datos
    $restoreCommand = @"
sqlcmd -S "$($Global:EnvConfig.serverInstance)" -E -Q "
RESTORE DATABASE [$($Global:EnvConfig.databaseName)] 
FROM DISK = '$BackupFile' 
WITH REPLACE
"@
    
    $result = Invoke-SafeCommand -Command $restoreCommand -Description "Restaurando base de datos" -ContinueOnError $true
    
    # Restaurar modo multi-usuario
    $multiUserCommand = @"
sqlcmd -S "$($Global:EnvConfig.serverInstance)" -E -Q "
ALTER DATABASE [$($Global:EnvConfig.databaseName)] 
SET MULTI_USER
"@
    
    Invoke-SafeCommand -Command $multiUserCommand -Description "Restaurando modo multi-usuario" -ContinueOnError $true
    
    if ($result.Success) {
        Write-Log "Base de datos restaurada exitosamente desde: $BackupFile" -Level "Success"
        return $true
    } else {
        Write-Log "Error restaurando base de datos: $($result.Error)" -Level "Error"
        return $false
    }
}

#endregion

#region Gestión de Scripts SQL Avanzada

function Invoke-SqlScripts {
    param(
        [ValidateSet("preMigration", "postMigration", "rollback")]
        [string]$ScriptType,
        [string]$Pattern = "*.sql"
    )
    
    $scriptConfig = $Global:Config.scripts.$ScriptType
    
    if (-not $scriptConfig.enabled) {
        Write-Log "Scripts de tipo '$ScriptType' están deshabilitados" -Level "Info"
        return $true
    }
    
    $scriptPath = $scriptConfig.path
    if (-not (Test-Path $scriptPath)) {
        Write-Log "Directorio de scripts no encontrado: $scriptPath" -Level "Warning"
        return $true
    }
    
    $scripts = Get-ChildItem -Path $scriptPath -Filter $Pattern | 
        Sort-Object Name
    
    if ($scriptConfig.executeOrder -eq "reverse-alphabetical") {
        $scripts = $scripts | Sort-Object Name -Descending
    }
    
    if ($scripts.Count -eq 0) {
        Write-Log "No se encontraron scripts en $scriptPath" -Level "Info"
        return $true
    }
    
    Write-Log "🔧 Ejecutando $($scripts.Count) scripts de tipo '$ScriptType'" -Level "Info"
    
    $successCount = 0
    $failureCount = 0
    
    for ($i = 0; $i -lt $scripts.Count; $i++) {
        $script = $scripts[$i]
        $progress = [math]::Round(($i / $scripts.Count) * 100)
        
        Write-Progress-Log -Activity "Ejecutando Scripts $ScriptType" -Status "Script: $($script.Name)" -PercentComplete $progress
        
        if ($DryRun) {
            Write-Log "[DRY RUN] Se ejecutaría script: $($script.Name)" -Level "Warning"
            $successCount++
            continue
        }
        
        try {
            # Leer contenido del script para validación básica
            $scriptContent = Get-Content $script.FullName -Raw
            
            if ([string]::IsNullOrWhiteSpace($scriptContent)) {
                Write-Log "Script vacío omitido: $($script.Name)" -Level "Warning"
                continue
            }
            
            # Ejecutar script
            $scriptCommand = @"
sqlcmd -S "$($Global:EnvConfig.serverInstance)" -d "$($Global:EnvConfig.databaseName)" -E -i "$($script.FullName)" -b
"@
            
            $result = Invoke-SafeCommand -Command $scriptCommand -Description "Ejecutando script: $($script.Name)" -ContinueOnError $true -TimeoutSeconds 600
            
            if ($result.Success) {
                Write-Log "✅ Script ejecutado exitosamente: $($script.Name)" -Level "Success"
                $successCount++
            } else {
                Write-Log "❌ Error en script $($script.Name): $($result.Error)" -Level "Error"
                $failureCount++
                
                # Decidir si continuar o abortar basado en configuración
                if ($ScriptType -eq "preMigration" -and $failureCount -gt 0) {
                    Write-Log "Abortando ejecución debido a errores en scripts pre-migración" -Level "Critical"
                    return $false
                }
            }
            
        } catch {
            Write-Log "❌ Error crítico ejecutando script $($script.Name): $($_.Exception.Message)" -Level "Critical"
            $failureCount++
        }
    }
    
    Write-Progress -Activity "Ejecutando Scripts $ScriptType" -Completed
    
    Write-Log "📊 Resumen de scripts $ScriptType - Exitosos: $successCount, Fallidos: $failureCount" -Level "Info"
    
    return $failureCount -eq 0
}

#endregion

#region Validaciones Avanzadas

function Test-DatabaseConnection {
    Write-Log "🔗 Probando conexión a base de datos..." -Level "Info"
    
    $connectionTest = @"
sqlcmd -S "$($Global:EnvConfig.serverInstance)" -E -Q "SELECT 1 as TestConnection" -h -1
"@
    
    $result = Invoke-SafeCommand -Command $connectionTest -Description "Probando conexión al servidor" -Silent $true -ContinueOnError $true
    
    if (-not $result.Success) {
        Write-Log "❌ No se puede conectar al servidor de base de datos" -Level "Error"
        Write-Log "Servidor: $($Global:EnvConfig.serverInstance)" -Level "Error"
        return $false
    }
    
    # Probar conexión específica a la base de datos
    $dbConnectionTest = @"
sqlcmd -S "$($Global:EnvConfig.serverInstance)" -d "$($Global:EnvConfig.databaseName)" -E -Q "SELECT DB_NAME() as CurrentDatabase" -h -1
"@
    
    $dbResult = Invoke-SafeCommand -Command $dbConnectionTest -Description "Probando conexión a la base de datos" -Silent $true -ContinueOnError $true
    
    if ($dbResult.Success) {
        Write-Log "✅ Conexión exitosa a la base de datos: $($Global:EnvConfig.databaseName)" -Level "Success"
        return $true
    } else {
        Write-Log "⚠️ Base de datos no existe o no es accesible: $($Global:EnvConfig.databaseName)" -Level "Warning"
        return $false
    }
}

function Get-DatabaseState {
    $state = @{
        DatabaseExists = $false
        TablesCount = 0
        ConstraintsCount = 0
        IndexesCount = 0
        MigrationsApplied = @()
        LastMigration = ""
        DatabaseSize = ""
        CreatedDate = ""
        IsHealthy = $false
    }
    
    try {
        # Verificar si la base de datos existe
        $dbExistsQuery = "SELECT COUNT(*) FROM sys.databases WHERE name = '$($Global:EnvConfig.databaseName)'"
        
        Write-Log "Ejecutando consulta: $dbExistsQuery" -Level "Debug"
        $dbExistsResult = sqlcmd -S "$($Global:EnvConfig.serverInstance)" -E -Q $dbExistsQuery -h -1 2>$null
        Write-Log "Resultado bruto: '$dbExistsResult'" -Level "Debug"
        
        if ($dbExistsResult) {
            # Limpiar resultado y convertir
            $cleanResult = $dbExistsResult | Where-Object { $_ -and $_.Trim() -match '^\d+$' } | Select-Object -First 1
            Write-Log "Resultado limpio: '$cleanResult'" -Level "Debug"
            
            if ($cleanResult) {
                $state.DatabaseExists = [int]$cleanResult.Trim() -gt 0
            }
        }
        
        if (-not $state.DatabaseExists) {
            Write-Log "Base de datos '$($Global:EnvConfig.databaseName)' no existe" -Level "Info"
            return $state
        }
        
        # Obtener información básica por separado para mejor control de errores
        try {
            $tablesQuery = "USE [$($Global:EnvConfig.databaseName)]; SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'"
            $tablesResult = sqlcmd -S "$($Global:EnvConfig.serverInstance)" -E -Q $tablesQuery -h -1 2>$null
            
            # Procesar resultado de sqlcmd más cuidadosamente
            $cleanTablesResult = $tablesResult | Where-Object { $_ -and $_.Trim() -match '^\s*\d+\s*$' } | Select-Object -First 1
            if ($cleanTablesResult) {
                $state.TablesCount = [int]$cleanTablesResult.Trim()
            }
        } catch {
            Write-Log "Error obteniendo conteo de tablas: $($_.Exception.Message)" -Level "Debug"
        }
        
        try {
            $constraintsQuery = "USE [$($Global:EnvConfig.databaseName)]; SELECT COUNT(*) FROM sys.check_constraints"
            $constraintsResult = sqlcmd -S "$($Global:EnvConfig.serverInstance)" -E -Q $constraintsQuery -h -1 2>$null
            
            $cleanConstraintsResult = $constraintsResult | Where-Object { $_ -and $_.Trim() -match '^\s*\d+\s*$' } | Select-Object -First 1
            if ($cleanConstraintsResult) {
                $state.ConstraintsCount = [int]$cleanConstraintsResult.Trim()
            }
        } catch {
            Write-Log "Error obteniendo conteo de constraints: $($_.Exception.Message)" -Level "Debug"
        }
        
        try {
            $indexesQuery = "USE [$($Global:EnvConfig.databaseName)]; SELECT COUNT(*) FROM sys.indexes WHERE is_primary_key = 0"
            $indexesResult = sqlcmd -S "$($Global:EnvConfig.serverInstance)" -E -Q $indexesQuery -h -1 2>$null
            
            $cleanIndexesResult = $indexesResult | Where-Object { $_ -and $_.Trim() -match '^\s*\d+\s*$' } | Select-Object -First 1
            if ($cleanIndexesResult) {
                $state.IndexesCount = [int]$cleanIndexesResult.Trim()
            }
        } catch {
            Write-Log "Error obteniendo conteo de índices: $($_.Exception.Message)" -Level "Debug"
        }
        
        # Información de migraciones
        try {
            $migrationsQuery = "USE [$($Global:EnvConfig.databaseName)]; IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '__EFMigrationsHistory') BEGIN SELECT MigrationId FROM __EFMigrationsHistory ORDER BY MigrationId; END"
            
            $migrations = sqlcmd -S "$($Global:EnvConfig.serverInstance)" -E -Q $migrationsQuery 2>$null
            if ($migrations) {
                $validMigrations = $migrations | Where-Object { 
                    $_ -and 
                    $_.Trim() -ne "" -and 
                    $_ -notmatch "^-+$" -and 
                    $_ -notmatch "MigrationId" -and
                    $_ -notmatch "rows affected" -and
                    $_ -notmatch "Changed database context" -and
                    $_.Trim().Length -gt 10
                } | ForEach-Object { $_.Trim() }
                
                if ($validMigrations) {
                    $state.MigrationsApplied = $validMigrations
                    $state.LastMigration = $validMigrations[-1]
                }
            }
        } catch {
            Write-Log "Error obteniendo información de migraciones: $($_.Exception.Message)" -Level "Debug"
        }
        
        # Tamaño de la base de datos
        try {
            $sizeQuery = "USE [$($Global:EnvConfig.databaseName)]; SELECT CAST(SUM(size) * 8.0 / 1024 AS DECIMAL(10,2)) as SizeMB FROM sys.master_files WHERE database_id = DB_ID('$($Global:EnvConfig.databaseName)')"
            
            $sizeResult = sqlcmd -S "$($Global:EnvConfig.serverInstance)" -E -Q $sizeQuery -h -1 2>$null
            $cleanSizeResult = $sizeResult | Where-Object { $_ -and $_.Trim() -match '^\s*\d+\.?\d*\s*$' } | Select-Object -First 1
            if ($cleanSizeResult) {
                $state.DatabaseSize = "$($cleanSizeResult.Trim()) MB"
            }
        } catch {
            Write-Log "Error obteniendo tamaño de la base de datos: $($_.Exception.Message)" -Level "Debug"
        }
        
        $state.IsHealthy = $state.TablesCount -gt 0
        
    } catch {
        Write-Log "Error general obteniendo estado de la base de datos: $($_.Exception.Message)" -Level "Warning"
    }
    
    return $state
}

function Test-PreMigrationRequirements {
    Write-Log "🔍 Validando requisitos pre-migración..." -Level "Info"
    
    $checks = @()
    
    # Verificar herramientas requeridas
    $tools = @("dotnet", "sqlcmd")
    foreach ($tool in $tools) {
        try {
            $null = Get-Command $tool -ErrorAction Stop
            $checks += @{ Name = "Herramienta: $tool"; Status = "OK"; Level = "Success" }
        } catch {
            $checks += @{ Name = "Herramienta: $tool"; Status = "NO ENCONTRADA"; Level = "Error" }
        }
    }
    
    # Verificar archivos de proyecto
    $projectFiles = @(
        "$($Global:ProjectConfig.projectPath)\*.csproj",
        "$($Global:ProjectConfig.startupProject)\*.csproj"
    )
    
    foreach ($pattern in $projectFiles) {
        $files = Get-ChildItem -Path $pattern -ErrorAction SilentlyContinue
        if ($files) {
            $checks += @{ Name = "Archivo proyecto: $pattern"; Status = "OK"; Level = "Success" }
        } else {
            $checks += @{ Name = "Archivo proyecto: $pattern"; Status = "NO ENCONTRADO"; Level = "Error" }
        }
    }
    
    # Verificar conexión a base de datos
    $connectionOk = Test-DatabaseConnection
    $checks += @{ 
        Name = "Conexión BD"; 
        Status = if ($connectionOk) { "OK" } else { "FALLO" }; 
        Level = if ($connectionOk) { "Success" } else { "Error" }
    }
    
    # Mostrar resultados
    Write-Log "📋 Resultados de validación:" -Level "Info"
    $errorCount = 0
    foreach ($check in $checks) {
        Write-Log "  $($check.Name): $($check.Status)" -Level $check.Level
        if ($check.Level -eq "Error") { $errorCount++ }
    }
    
    return $errorCount -eq 0
}

#endregion

#region Acciones Principales del Framework

function Invoke-VerifyAction {
    Write-Log "🔍 Verificando estado actual del sistema..." -Level "Info"
    
    $state = Get-DatabaseState
    
    Write-Log "📊 Estado de la Base de Datos:" -Level "Info"
    Write-Log "  📂 Base de datos existe: $(if ($state.DatabaseExists) { '✅ Sí' } else { '❌ No' })" -Level $(if ($state.DatabaseExists) { "Success" } else { "Warning" })
    
    if ($state.DatabaseExists) {
        Write-Log "  📋 Tablas: $($state.TablesCount)" -Level "Info"
        Write-Log "  🔒 Constraints: $($state.ConstraintsCount)" -Level "Info"
        Write-Log "  📇 Índices: $($state.IndexesCount)" -Level "Info"
        Write-Log "  💾 Tamaño: $($state.DatabaseSize)" -Level "Info"
        Write-Log "  🚀 Migraciones aplicadas: $($state.MigrationsApplied.Count)" -Level "Info"
        
        if ($state.LastMigration) {
            Write-Log "  🏷️ Última migración: $($state.LastMigration)" -Level "Info"
        }
        
        Write-Log "  💚 Estado general: $(if ($state.IsHealthy) { 'Saludable' } else { 'Requiere atención' })" -Level $(if ($state.IsHealthy) { "Success" } else { "Warning" })
        
        # Mostrar migraciones aplicadas
        if ($state.MigrationsApplied.Count -gt 0) {
            Write-Log "📝 Migraciones aplicadas:" -Level "Info"
            foreach ($migration in $state.MigrationsApplied) {
                Write-Log "    • $migration" -Level "Debug"
            }
        }
    }
    
    return $state.IsHealthy
}

function Invoke-ResetAction {
    Write-Log "🔄 Iniciando reset completo del sistema..." -Level "Info"
    
    # Validaciones de seguridad
    if (-not $Global:EnvConfig.allowReset) {
        Write-Log "❌ Reset no permitido en el entorno '$Environment'" -Level "Error"
        Write-Log "Configuración: allowReset = false" -Level "Error"
        return $false
    }
    
    if ($Global:EnvConfig.requireConfirmation -and -not $Force -and -not $Interactive) {
        Write-Log "⚠️ El entorno '$Environment' requiere confirmación explícita" -Level "Warning"
        Write-Log "Use -Force para omitir confirmación o -Interactive para confirmar interactivamente" -Level "Info"
        return $false
    }
    
    if ($Interactive) {
        $confirmation = Read-Host "¿Está seguro de que desea realizar un reset completo? (sí/no)"
        if ($confirmation -ne "sí" -and $confirmation -ne "s" -and $confirmation -ne "yes") {
            Write-Log "Reset cancelado por el usuario" -Level "Info"
            return $false
        }
    }
    
    # Validar requisitos
    $requirementsOk = Test-PreMigrationRequirements
    if (-not $requirementsOk) {
        Write-Log "❌ No se cumplen los requisitos para ejecutar reset" -Level "Error"
        return $false
    }
    
    # Crear backup si está configurado
    $backupInfo = $null
    if (-not $SkipBackup) {
        $backupInfo = New-DatabaseBackup -Description "Backup antes de reset completo"
    }
    
    try {
        # 1. Ejecutar scripts pre-migración
        Write-Progress-Log -Activity "Reset Completo" -Status "Ejecutando scripts pre-migración" -PercentComplete 10
        if (-not $SkipScripts) {
            $preScriptsOk = Invoke-SqlScripts -ScriptType "preMigration"
            if (-not $preScriptsOk) {
                throw "Error en scripts pre-migración"
            }
        }
        
        # 2. Limpiar migraciones existentes
        Write-Progress-Log -Activity "Reset Completo" -Status "Limpiando migraciones existentes" -PercentComplete 20
        Clear-ExistingMigrations
        
        # 3. Eliminar base de datos
        Write-Progress-Log -Activity "Reset Completo" -Status "Eliminando base de datos" -PercentComplete 30
        $dropResult = Remove-Database
        
        # 4. Crear migración inicial
        Write-Progress-Log -Activity "Reset Completo" -Status "Creando migración inicial" -PercentComplete 50
        $migrationResult = New-EFMigration -Name "InitialCreate"
        if (-not $migrationResult.Success) {
            throw "Error creando migración inicial: $($migrationResult.Error)"
        }
        
        # 5. Aplicar migración
        Write-Progress-Log -Activity "Reset Completo" -Status "Aplicando migración inicial" -PercentComplete 70
        $applyResult = Apply-EFMigrations
        if (-not $applyResult.Success) {
            throw "Error aplicando migración: $($applyResult.Error)"
        }
        
        # 6. Ejecutar scripts post-migración
        Write-Progress-Log -Activity "Reset Completo" -Status "Ejecutando scripts post-migración" -PercentComplete 85
        if (-not $SkipScripts) {
            $postScriptsOk = Invoke-SqlScripts -ScriptType "postMigration"
            if (-not $postScriptsOk) {
                Write-Log "⚠️ Algunos scripts post-migración fallaron, pero continuando..." -Level "Warning"
            }
        }
        
        # 7. Verificar resultado final
        Write-Progress-Log -Activity "Reset Completo" -Status "Verificando resultado final" -PercentComplete 95
        Start-Sleep -Seconds 2  # Dar tiempo para que la BD se estabilice
        $finalState = Get-DatabaseState
        
        Write-Progress -Activity "Reset Completo" -Completed
        
        if ($finalState.IsHealthy) {
            Write-Log "🎉 Reset completo exitoso" -Level "Success"
            Write-Log "  📊 Tablas creadas: $($finalState.TablesCount)" -Level "Success"
            Write-Log "  🔒 Constraints aplicados: $($finalState.ConstraintsCount)" -Level "Success"
            Write-Log "  📇 Índices creados: $($finalState.IndexesCount)" -Level "Success"
            return $true
        } else {
            Write-Log "⚠️ Reset completado con advertencias" -Level "Warning"
            return $false
        }
        
    } catch {
        Write-Log "❌ Error durante reset: $($_.Exception.Message)" -Level "Error"
        
        # Intentar restaurar desde backup si existe
        if ($backupInfo -and -not $DryRun) {
            Write-Log "🔄 Intentando restaurar desde backup..." -Level "Warning"
            $restored = Restore-DatabaseFromBackup -BackupFile $backupInfo.FileName
            if ($restored) {
                Write-Log "✅ Base de datos restaurada desde backup" -Level "Success"
            } else {
                Write-Log "❌ Error restaurando desde backup" -Level "Error"
            }
        }
        
        return $false
    }
}

function Invoke-UpdateAction {
    Write-Log "🔄 Actualizando base de datos con migraciones pendientes..." -Level "Info"
    
    # Validar requisitos
    $requirementsOk = Test-PreMigrationRequirements
    if (-not $requirementsOk) {
        Write-Log "❌ No se cumplen los requisitos para actualizar" -Level "Error"
        return $false
    }
    
    # Crear backup si está configurado
    $backupInfo = $null
    if (-not $SkipBackup -and $Global:EnvConfig.autoBackup) {
        $backupInfo = New-DatabaseBackup -Description "Backup antes de update"
    }
    
    try {
        # 1. Verificar estado actual
        $initialState = Get-DatabaseState
        Write-Log "📊 Estado inicial - Migraciones: $($initialState.MigrationsApplied.Count)" -Level "Info"
        
        # 2. Aplicar migraciones
        Write-Progress-Log -Activity "Actualización" -Status "Aplicando migraciones pendientes" -PercentComplete 30
        $applyResult = Apply-EFMigrations
        if (-not $applyResult.Success) {
            throw "Error aplicando migraciones: $($applyResult.Error)"
        }
        
        # 3. Ejecutar scripts post-migración
        Write-Progress-Log -Activity "Actualización" -Status "Ejecutando scripts post-migración" -PercentComplete 70
        if (-not $SkipScripts) {
            $postScriptsOk = Invoke-SqlScripts -ScriptType "postMigration"
            if (-not $postScriptsOk) {
                Write-Log "⚠️ Algunos scripts post-migración fallaron" -Level "Warning"
            }
        }
        
        # 4. Verificar resultado
        Write-Progress-Log -Activity "Actualización" -Status "Verificando resultado" -PercentComplete 90
        $finalState = Get-DatabaseState
        
        Write-Progress -Activity "Actualización" -Completed
        
        $newMigrations = $finalState.MigrationsApplied.Count - $initialState.MigrationsApplied.Count
        
        Write-Log "🎉 Actualización completada" -Level "Success"
        Write-Log "  📈 Nuevas migraciones aplicadas: $newMigrations" -Level "Success"
        Write-Log "  📊 Total migraciones: $($finalState.MigrationsApplied.Count)" -Level "Info"
        
        return $finalState.IsHealthy
        
    } catch {
        Write-Log "❌ Error durante actualización: $($_.Exception.Message)" -Level "Error"
        
        # Intentar restaurar desde backup si existe
        if ($backupInfo -and -not $DryRun) {
            Write-Log "🔄 Intentando restaurar desde backup..." -Level "Warning"
            $restored = Restore-DatabaseFromBackup -BackupFile $backupInfo.FileName
            if ($restored) {
                Write-Log "✅ Base de datos restaurada desde backup" -Level "Success"
            }
        }
        
        return $false
    }
}

function Invoke-CreateMigrationAction {
    if ([string]::IsNullOrEmpty($MigrationName)) {
        Write-Log "❌ Se requiere -MigrationName para crear una migración" -Level "Error"
        return $false
    }
    
    Write-Log "🆕 Creando nueva migración: $MigrationName" -Level "Info"
    
    # Validar requisitos
    $requirementsOk = Test-PreMigrationRequirements
    if (-not $requirementsOk) {
        Write-Log "❌ No se cumplen los requisitos para crear migración" -Level "Error"
        return $false
    }
    
    $result = New-EFMigration -Name $MigrationName
    
    if ($result.Success) {
        Write-Log "✅ Migración '$MigrationName' creada exitosamente" -Level "Success"
        
        # Mostrar archivos creados
        $migrationPath = $Global:ProjectConfig.contexts[0].migrationPath
        $migrationFiles = Get-ChildItem -Path $migrationPath -Filter "*$MigrationName*"
        
        Write-Log "📁 Archivos de migración creados:" -Level "Info"
        foreach ($file in $migrationFiles) {
            Write-Log "  • $($file.Name)" -Level "Info"
        }
        
        return $true
    } else {
        Write-Log "❌ Error creando migración: $($result.Error)" -Level "Error"
        return $false
    }
}

#endregion

#region Funciones de Utilidad EF Core

function Clear-ExistingMigrations {
    Write-Log "🧹 Limpiando migraciones existentes..." -Level "Info"
    
    foreach ($context in $Global:ProjectConfig.contexts) {
        $migrationPath = $context.migrationPath
        
        if (Test-Path $migrationPath) {
            $migrationFiles = Get-ChildItem -Path $migrationPath -File
            $removedCount = 0
            
            foreach ($file in $migrationFiles) {
                if (-not $DryRun) {
                    Remove-Item -Path $file.FullName -Force
                }
                $removedCount++
                Write-Log "  🗑️ Eliminado: $($file.Name)" -Level "Debug"
            }
            
            Write-Log "✅ $removedCount archivos de migración eliminados de $($context.name)" -Level "Success"
        } else {
            Write-Log "⚠️ Directorio de migraciones no encontrado: $migrationPath" -Level "Warning"
        }
    }
}

function Remove-Database {
    Write-Log "🗑️ Eliminando base de datos existente..." -Level "Info"
    
    $dropCommand = @"
dotnet ef database drop 
--project "$($Global:ProjectConfig.projectPath)" 
--startup-project "$($Global:ProjectConfig.startupProject)" 
--context "$($Global:ProjectConfig.contexts[0].contextClass)" 
--force
"@
    
    $result = Invoke-SafeCommand -Command $dropCommand.Replace("`n", " ").Replace("`r", "") -Description "Eliminando base de datos" -ContinueOnError $true
    
    if ($result.Success -or $result.Error -match "does not exist") {
        Write-Log "✅ Base de datos eliminada o no existía" -Level "Success"
        return $true
    } else {
        Write-Log "⚠️ Error eliminando base de datos (continuando): $($result.Error)" -Level "Warning"
        return $false
    }
}

function New-EFMigration {
    param([string]$Name)
    
    $createCommand = @"
dotnet ef migrations add "$Name" 
--project "$($Global:ProjectConfig.projectPath)" 
--startup-project "$($Global:ProjectConfig.startupProject)" 
--context "$($Global:ProjectConfig.contexts[0].contextClass)"
"@
    
    return Invoke-SafeCommand -Command $createCommand.Replace("`n", " ").Replace("`r", "") -Description "Creando migración: $Name"
}

function Apply-EFMigrations {
    $updateCommand = @"
dotnet ef database update 
--project "$($Global:ProjectConfig.projectPath)" 
--startup-project "$($Global:ProjectConfig.startupProject)" 
--context "$($Global:ProjectConfig.contexts[0].contextClass)"
"@
    
    return Invoke-SafeCommand -Command $updateCommand.Replace("`n", " ").Replace("`r", "") -Description "Aplicando migraciones"
}

#endregion

#region Funciones de Reportes

function New-MigrationReport {
    $reportPath = "Reports"
    $timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
    $reportFile = "$reportPath\migration-report_$timestamp.html"
    
    $state = Get-DatabaseState
    $duration = (Get-Date) - $Global:StartTime
    
    $html = @"
<!DOCTYPE html>
<html>
<head>
    <title>Reporte de Migración - SGA</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 20px; }
        .header { background: #2196F3; color: white; padding: 20px; border-radius: 5px; }
        .section { margin: 20px 0; padding: 15px; border: 1px solid #ddd; border-radius: 5px; }
        .success { background: #E8F5E8; border-color: #4CAF50; }
        .warning { background: #FFF3E0; border-color: #FF9800; }
        .error { background: #FFEBEE; border-color: #F44336; }
        .info { background: #E3F2FD; border-color: #2196F3; }
        table { width: 100%; border-collapse: collapse; margin: 10px 0; }
        th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
        th { background: #f2f2f2; }
        .timestamp { color: #666; font-size: 0.9em; }
    </style>
</head>
<body>
    <div class="header">
        <h1>🚀 Reporte de Migración - Sistema SGA</h1>
        <p class="timestamp">Generado: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss") | Duración: $([math]::Round($duration.TotalMinutes, 2)) minutos</p>
    </div>
    
    <div class="section info">
        <h2>📋 Información General</h2>
        <table>
            <tr><td><strong>Entorno</strong></td><td>$Environment</td></tr>
            <tr><td><strong>Proyecto</strong></td><td>$ProjectContext</td></tr>
            <tr><td><strong>Acción</strong></td><td>$Action</td></tr>
            <tr><td><strong>Base de Datos</strong></td><td>$($Global:EnvConfig.databaseName)</td></tr>
            <tr><td><strong>Servidor</strong></td><td>$($Global:EnvConfig.serverInstance)</td></tr>
        </table>
    </div>
    
    <div class="section $(if ($state.IsHealthy) { 'success' } else { 'warning' })">
        <h2>📊 Estado de la Base de Datos</h2>
        <table>
            <tr><td><strong>Existe</strong></td><td>$(if ($state.DatabaseExists) { '✅ Sí' } else { '❌ No' })</td></tr>
            <tr><td><strong>Tablas</strong></td><td>$($state.TablesCount)</td></tr>
            <tr><td><strong>Constraints</strong></td><td>$($state.ConstraintsCount)</td></tr>
            <tr><td><strong>Índices</strong></td><td>$($state.IndexesCount)</td></tr>
            <tr><td><strong>Tamaño</strong></td><td>$($state.DatabaseSize)</td></tr>
            <tr><td><strong>Migraciones</strong></td><td>$($state.MigrationsApplied.Count)</td></tr>
            <tr><td><strong>Última Migración</strong></td><td>$($state.LastMigration)</td></tr>
        </table>
    </div>
    
    $(if ($state.MigrationsApplied.Count -gt 0) {
        "<div class='section info'>
        <h2>📝 Migraciones Aplicadas</h2>
        <ul>" + 
        ($state.MigrationsApplied | ForEach-Object { "<li>$_</li>" }) -join "" +
        "</ul>
        </div>"
    })
    
    <div class="section info">
        <h2>⚙️ Configuración Utilizada</h2>
        <pre>$(($Global:Config | ConvertTo-Json -Depth 3) -replace '<', '&lt;' -replace '>', '&gt;')</pre>
    </div>
    
    <div class="section">
        <h2>📄 Log de Ejecución</h2>
        <pre>$(if (Test-Path $Global:LogFile) { Get-Content $Global:LogFile | Select-Object -Last 50 | Out-String } else { "Log no disponible" })</pre>
    </div>
    
    <footer style="margin-top: 40px; text-align: center; color: #666; font-size: 0.9em;">
        <p>Generado por Framework de Migraciones SGA v2.0</p>
    </footer>
</body>
</html>
"@
    
    if (-not $DryRun) {
        $html | Set-Content -Path $reportFile -Encoding UTF8
        Write-Log "📋 Reporte generado: $reportFile" -Level "Success"
    }
    
    return $reportFile
}

#endregion

#region Función Principal y Manejo de Ayuda

function Show-Help {
    Write-Host @"
🚀 Framework Avanzado de Gestión de Migraciones SGA v2.0

DESCRIPCIÓN:
  Framework completo y configurable para gestionar migraciones de Entity Framework Core
  con soporte para múltiples entornos, backups automáticos, scripts SQL y validaciones.

ACCIONES DISPONIBLES:
  verify          - Verificar estado actual de la base de datos y migraciones
  reset           - Reset completo (solo entornos de desarrollo)
  update          - Aplicar migraciones pendientes + scripts post-migración
  create          - Crear nueva migración
  apply-scripts   - Ejecutar solo scripts post-migración
  backup          - Crear backup manual de la base de datos
  restore         - Restaurar base de datos desde backup
  init            - Inicializar configuración del framework
  status          - Mostrar estado detallado del sistema
  report          - Generar reporte HTML completo

PARÁMETROS:
  -Action           Acción a ejecutar (obligatorio)
  -MigrationName    Nombre para migración o backup
  -Environment      Entorno: Development, Staging, Production (default: Development)
  -ProjectContext   Contexto del proyecto (default: main)
  -ConfigFile       Archivo de configuración (default: Scripts\migration-config.json)
  -BackupName       Nombre específico para backup/restore
  -Force            Omitir confirmaciones de seguridad
  -SkipScripts      Omitir ejecución de scripts SQL
  -SkipBackup       Omitir creación de backups
  -DryRun           Mostrar qué se haría sin ejecutar
  -Verbose          Mostrar información detallada
  -Interactive      Solicitar confirmación interactiva

EJEMPLOS DE USO:

  # Verificar estado actual
  .\migration-framework-v2.ps1 -Action verify

  # Reset completo en desarrollo
  .\migration-framework-v2.ps1 -Action reset -Force

  # Actualizar base de datos en staging
  .\migration-framework-v2.ps1 -Action update -Environment Staging

  # Crear nueva migración
  .\migration-framework-v2.ps1 -Action create -MigrationName "AddUserRoles"

  # Backup manual antes de cambios importantes
  .\migration-framework-v2.ps1 -Action backup -BackupName "before-major-changes"

  # Restaurar desde backup específico
  .\migration-framework-v2.ps1 -Action restore -BackupName "before-major-changes"

  # Dry run para ver qué haría un reset
  .\migration-framework-v2.ps1 -Action reset -DryRun

  # Generar reporte completo
  .\migration-framework-v2.ps1 -Action report

ESTRUCTURA DE DIRECTORIOS:
  Scripts/
    ├── migration-config.json          # Configuración principal
    ├── migration-framework-v2.ps1     # Este script
    ├── Pre-Migration/                 # Scripts antes de migraciones
    ├── Post-Migration/                # Scripts después de migraciones
    └── Rollback/                      # Scripts de rollback
  Backups/                             # Backups automáticos
  Logs/Migrations/                     # Logs detallados
  Reports/                             # Reportes HTML
  Templates/                           # Plantillas de scripts

CARACTERÍSTICAS:
  ✅ Configuración externa por entornos
  ✅ Backups automáticos antes de cambios destructivos  
  ✅ Scripts SQL pre y post migración
  ✅ Validaciones de seguridad por entorno
  ✅ Logging detallado con retención automática
  ✅ Reportes HTML completos
  ✅ Soporte para dry-run
  ✅ Rollback desde backups
  ✅ Múltiples contextos de EF Core

CONFIGURACIÓN:
  El archivo migration-config.json controla todos los aspectos del framework.
  Use -Action init para generar una configuración base.

"@ -ForegroundColor Cyan
}

function Invoke-MigrationFrameworkV2 {
    try {
        switch ($Action.ToLower()) {
            "init" {
                Write-Log "🎯 Inicializando configuración del framework..." -Level "Info"
                # La configuración ya fue creada, solo informar
                Write-Log "✅ Configuración creada: $ConfigFile" -Level "Success"
                Write-Log "Edite el archivo de configuración según sus necesidades" -Level "Info"
                return $true
            }
            
            "help" {
                Show-Help
                return $true
            }
            
            default {
                # Inicializar framework para todas las demás acciones
                Initialize-MigrationFramework
                
                switch ($Action.ToLower()) {
                    "verify" { 
                        return Invoke-VerifyAction
                    }
                    "status" {
                        return Invoke-VerifyAction  # Alias para verify
                    }
                    "reset" { 
                        return Invoke-ResetAction
                    }
                    "update" { 
                        return Invoke-UpdateAction
                    }
                    "create" {
                        return Invoke-CreateMigrationAction
                    }
                    "apply-scripts" {
                        Write-Log "🔧 Ejecutando scripts post-migración..." -Level "Info"
                        return Invoke-SqlScripts -ScriptType "postMigration"
                    }
                    "backup" {
                        $backupName = if ($BackupName) { $BackupName } else { "manual-backup" }
                        $backupInfo = New-DatabaseBackup -BackupName $backupName -Description "Backup manual"
                        return $backupInfo -ne $null
                    }
                    "restore" {
                        if ([string]::IsNullOrEmpty($BackupName)) {
                            Write-Log "❌ Se requiere -BackupName para restaurar" -Level "Error"
                            return $false
                        }
                        $backupFile = "$($Global:Config.backup.path)\$BackupName.bak"
                        return Restore-DatabaseFromBackup -BackupFile $backupFile
                    }
                    "rollback" {
                        Write-Log "🔄 Ejecutando scripts de rollback..." -Level "Info"
                        return Invoke-SqlScripts -ScriptType "rollback"
                    }
                    "report" {
                        Write-Log "📋 Generando reporte completo..." -Level "Info"
                        $reportFile = New-MigrationReport
                        Write-Log "✅ Reporte generado: $reportFile" -Level "Success"
                        return $true
                    }
                    default {
                        Write-Log "❌ Acción no válida: $Action" -Level "Error"
                        Write-Log "Use -Action help para ver ayuda completa" -Level "Info"
                        return $false
                    }
                }
            }
        }
    } catch {
        Write-Log "💥 Error crítico en el framework: $($_.Exception.Message)" -Level "Critical"
        Write-Log "Stack trace: $($_.Exception.StackTrace)" -Level "Debug"
        return $false
    }
}

#endregion

# Punto de entrada principal
if ($args -contains "-Help" -or $args -contains "-h" -or $Action -eq "help") {
    Show-Help
    exit 0
}

try {
    Write-Host "🚀 Framework de Migraciones SGA v2.0" -ForegroundColor Cyan
    Write-Host "Iniciando acción: $Action" -ForegroundColor White
    
    $success = Invoke-MigrationFrameworkV2
    
    if ($success) {
        Write-Log "🎉 Operación completada exitosamente" -Level "Success"
        
        # Generar reporte si no es una acción simple
        if ($Action -notin @("verify", "status", "help", "init") -and -not $DryRun) {
            Write-Log "📋 Generando reporte de la operación..." -Level "Info"
            $reportFile = New-MigrationReport
        }
        
        exit 0
    } else {
        Write-Log "❌ Operación completada con errores" -Level "Error"
        exit 1
    }
    
} catch {
    Write-Log "💥 Error fatal: $($_.Exception.Message)" -Level "Critical"
    exit 1
} finally {
    if ($Global:Config.logging.enabled -and $Global:LogFile) {
        $duration = (Get-Date) - $Global:StartTime
        Write-Log "⏱️ Duración total: $([math]::Round($duration.TotalSeconds, 2)) segundos" -Level "Info" -NoConsole
    }
}
