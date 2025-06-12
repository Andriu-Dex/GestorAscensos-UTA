#!/usr/bin/env pwsh
# SGA - Improved Application Launcher
# Este script ejecuta API y Cliente Blazor con una interfaz unificada y profesional

# Configuración de colores
$colorTitle = "Cyan"
$colorAPI = "Green"
$colorClient = "Blue"
$colorError = "Red"
$colorSuccess = "Yellow"
$colorInfo = "White"

# Limpiar la pantalla y mostrar banner
Clear-Host
Write-Host "╔═══════════════════════════════════════════════════════════╗" -ForegroundColor $colorTitle
Write-Host "║  SISTEMA DE GESTIÓN Y CONTROL DE ASCENSOS - LAUNCHER      ║" -ForegroundColor $colorTitle
Write-Host "╚═══════════════════════════════════════════════════════════╝" -ForegroundColor $colorTitle
Write-Host ""

# Obtener la ruta base del proyecto
$basePath = Split-Path -Parent $MyInvocation.MyCommand.Path
$apiPath = Join-Path $basePath "SGA.Api"
$serverPath = Join-Path $basePath "SGA.BlazorServer"

# Función para detener procesos al salir
function Cleanup {
    param(
        [System.Diagnostics.Process]$apiProcess,
        [System.Diagnostics.Process]$clientProcess
    )
    
    Write-Host "`n`nDeteniendo servicios..." -ForegroundColor $colorInfo
    
    if ($apiProcess -ne $null -and -not $apiProcess.HasExited) {
        Write-Host "Deteniendo API..." -ForegroundColor $colorAPI
        Stop-Process -Id $apiProcess.Id -Force -ErrorAction SilentlyContinue
    }
      if ($clientProcess -ne $null -and -not $clientProcess.HasExited) {
        Write-Host "Deteniendo Servidor Blazor..." -ForegroundColor $colorClient
        Stop-Process -Id $clientProcess.Id -Force -ErrorAction SilentlyContinue
    }
    
    Write-Host "Servicios detenidos. ¡Hasta pronto!" -ForegroundColor $colorSuccess
}

# Detectar si se presiona Ctrl+C para detener todo ordenadamente
[console]::TreatControlCAsInput = $true

try {
    # Mostrar mensaje de inicio
    Write-Host "Iniciando servicios del sistema..." -ForegroundColor $colorInfo
    Write-Host "Presiona [Ctrl+C] en cualquier momento para detener todos los servicios" -ForegroundColor $colorInfo
    Write-Host ""
    
    # Iniciar API con output redirigido
    Write-Host "[$(Get-Date -Format 'HH:mm:ss')] " -NoNewline
    Write-Host "Iniciando API..." -ForegroundColor $colorAPI
    
    $apiProcess = Start-Process -FilePath "dotnet" -ArgumentList "run", "--project", "$apiPath", "--launch-profile", "https" -PassThru -NoNewWindow -RedirectStandardOutput "$env:TEMP\sga-api-output.log" -RedirectStandardError "$env:TEMP\sga-api-error.log"
    
    # Esperar un poco antes de iniciar el cliente
    Start-Sleep -Seconds 3
    
    # Iniciar Servidor Blazor con output redirigido
    Write-Host "[$(Get-Date -Format 'HH:mm:ss')] " -NoNewline
    Write-Host "Iniciando Servidor Blazor..." -ForegroundColor $colorClient
    
    $clientProcess = Start-Process -FilePath "dotnet" -ArgumentList "run", "--project", "$serverPath", "--launch-profile", "https" -PassThru -NoNewWindow -RedirectStandardOutput "$env:TEMP\sga-client-output.log" -RedirectStandardError "$env:TEMP\sga-client-error.log"
    
    # Función para leer el final de un archivo de log
    function Get-LogTail {
        param (
            [string]$logFile,
            [int]$lines = 5
        )
        
        if (Test-Path $logFile) {
            Get-Content $logFile -Tail $lines
        }
    }
    
    # Mostrar estado inicial
    Write-Host ""
    Write-Host "Estado de los servicios:" -ForegroundColor $colorInfo
    Write-Host "  API     : " -NoNewline
    if ($apiProcess -ne $null -and -not $apiProcess.HasExited) {
        Write-Host "EN EJECUCIÓN (PID: $($apiProcess.Id))" -ForegroundColor $colorSuccess
    } else {
        Write-Host "ERROR" -ForegroundColor $colorError
    }
    
    Write-Host "  Servidor: " -NoNewline
    if ($clientProcess -ne $null -and -not $clientProcess.HasExited) {
        Write-Host "EN EJECUCIÓN (PID: $($clientProcess.Id))" -ForegroundColor $colorSuccess
    } else {
        Write-Host "ERROR" -ForegroundColor $colorError
    }
    
    # Monitorear logs y detectar Ctrl+C
    $apiRunning = $true
    $clientRunning = $true
    $lastApiLineCount = 0
    $lastClientLineCount = 0
    $urlsShown = $false
    
    Write-Host ""
    Write-Host "Mostrando logs en tiempo real..." -ForegroundColor $colorInfo
    Write-Host "════════════════════════════════════════════════════════════" -ForegroundColor $colorInfo
    
    while ($apiRunning -or $clientRunning) {
        # Verificar si API sigue ejecutándose
        if (-not $apiProcess.HasExited) {
            # Mostrar nuevas líneas de log de API
            if (Test-Path "$env:TEMP\sga-api-output.log") {
                $apiLines = Get-Content "$env:TEMP\sga-api-output.log"
                if ($apiLines.Count -gt $lastApiLineCount) {
                    $newLines = $apiLines[$lastApiLineCount..($apiLines.Count-1)]
                    foreach ($line in $newLines) {
                        # Buscar URLs en el log
                        if ($line -match "Now listening on: (https?://[^\s]+)" -and -not $urlsShown) {
                            Write-Host "[API] " -NoNewline -ForegroundColor $colorAPI
                            Write-Host "$line" -ForegroundColor $colorSuccess
                        } 
                        # Detectar cuando la API está lista
                        elseif ($line -match "Application started." -or $line -match "Content root path") {
                            Write-Host "[API] " -NoNewline -ForegroundColor $colorAPI
                            Write-Host "$line" -ForegroundColor $colorSuccess
                        }
                        # Mostrar errores resaltados
                        elseif ($line -match "fail|error|exception|warn" -and $line -notmatch "Microsoft.Hosting.Lifetime") {
                            Write-Host "[API] " -NoNewline -ForegroundColor $colorAPI
                            Write-Host "$line" -ForegroundColor $colorError
                        }
                    }
                    $lastApiLineCount = $apiLines.Count
                }
            }
        } else {
            if ($apiRunning) {
                Write-Host "[$(Get-Date -Format 'HH:mm:ss')] " -NoNewline
                Write-Host "La API se ha detenido." -ForegroundColor $colorError
                $apiRunning = $false
                
                # Mostrar últimas líneas de error si existen
                if (Test-Path "$env:TEMP\sga-api-error.log") {
                    $errorLines = Get-LogTail -logFile "$env:TEMP\sga-api-error.log"
                    if ($errorLines) {
                        Write-Host "Últimos errores de API:" -ForegroundColor $colorError
                        foreach ($line in $errorLines) {
                            Write-Host "  $line" -ForegroundColor $colorError
                        }
                    }
                }
            }
        }
        
    # Verificar si Servidor Blazor sigue ejecutándose
        if (-not $clientProcess.HasExited) {
            # Mostrar nuevas líneas de log del Servidor
            if (Test-Path "$env:TEMP\sga-client-output.log") {
                $clientLines = Get-Content "$env:TEMP\sga-client-output.log"
                if ($clientLines.Count -gt $lastClientLineCount) {
                    $newLines = $clientLines[$lastClientLineCount..($clientLines.Count-1)]
                    foreach ($line in $newLines) {
                        # Buscar URLs en el log
                        if ($line -match "Now listening on: (https?://[^\s]+)") {
                            Write-Host "[Servidor] " -NoNewline -ForegroundColor $colorClient
                            Write-Host "Disponible en: $($Matches[1])" -ForegroundColor $colorSuccess
                            $urlsShown = $true
                        }
                        # Detectar cuando el servidor está listo
                        elseif ($line -match "Application started." -or $line -match "Content root path") {
                            Write-Host "[Servidor] " -NoNewline -ForegroundColor $colorClient
                            Write-Host "$line" -ForegroundColor $colorSuccess
                        }
                        # Mostrar errores resaltados
                        elseif ($line -match "fail|error|exception|warn" -and $line -notmatch "Microsoft.Hosting.Lifetime") {
                            Write-Host "[Servidor] " -NoNewline -ForegroundColor $colorClient
                            Write-Host "$line" -ForegroundColor $colorError
                        }
                    }
                    $lastClientLineCount = $clientLines.Count
                }
            }
        } else {
            if ($clientRunning) {
                Write-Host "[$(Get-Date -Format 'HH:mm:ss')] " -NoNewline
                Write-Host "El Servidor Blazor se ha detenido." -ForegroundColor $colorError
                $clientRunning = $false
                  # Mostrar últimas líneas de error si existen
                if (Test-Path "$env:TEMP\sga-client-error.log") {
                    $errorLines = Get-LogTail -logFile "$env:TEMP\sga-client-error.log"
                    if ($errorLines) {
                        Write-Host "Últimos errores del Servidor Blazor:" -ForegroundColor $colorError
                        foreach ($line in $errorLines) {
                            Write-Host "  $line" -ForegroundColor $colorError
                        }
                    }
                }
            }
        }
        
        # Verificar si se ha presionado Ctrl+C
        if ([console]::KeyAvailable) {
            $key = [console]::ReadKey($true)
            if (($key.Modifiers -band [ConsoleModifiers]::Control) -and ($key.Key -eq 'C')) {
                Write-Host "`n`nSe detectó [Ctrl+C]. Deteniendo servicios..." -ForegroundColor $colorInfo
                break
            }
        }
        
        # Dormir un poco para no consumir demasiada CPU
        Start-Sleep -Milliseconds 500
        
        # Si ambos procesos terminaron, salir del bucle
        if ($apiProcess.HasExited -and $clientProcess.HasExited) {
            break
        }
    }
} 
finally {
    # Restaurar comportamiento normal de Ctrl+C
    [console]::TreatControlCAsInput = $false
    
    # Limpiar procesos al salir
    Cleanup -apiProcess $apiProcess -clientProcess $clientProcess
    
    # Eliminar archivos temporales de log
    Remove-Item -Path "$env:TEMP\sga-api-output.log" -ErrorAction SilentlyContinue
    Remove-Item -Path "$env:TEMP\sga-api-error.log" -ErrorAction SilentlyContinue
    Remove-Item -Path "$env:TEMP\sga-client-output.log" -ErrorAction SilentlyContinue
    Remove-Item -Path "$env:TEMP\sga-client-error.log" -ErrorAction SilentlyContinue
}
