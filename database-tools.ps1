#!/usr/bin/env pwsh

# Función para mostrar mensajes con color
function Write-ColorMessage {
    param (
        [string]$Message,
        [string]$Color = "White"
    )
    
    Write-Host $Message -ForegroundColor $Color
}

# Menú principal
function Show-Menu {
    Clear-Host
    Write-ColorMessage "=== Sistema de Gestión y Control de Ascensos ===" "Cyan"
    Write-ColorMessage "Utilidades de Base de Datos" "Cyan"
    Write-ColorMessage "==============================" "Cyan"
    Write-ColorMessage ""
    Write-ColorMessage "1. Crear nueva migración" "Yellow"
    Write-ColorMessage "2. Aplicar migraciones pendientes" "Yellow"
    Write-ColorMessage "3. Listar migraciones" "Yellow"
    Write-ColorMessage "4. Eliminar última migración" "Yellow"
    Write-ColorMessage "5. Iniciar aplicación con migración automática" "Yellow"
    Write-ColorMessage "6. Ejecutar herramienta de migración" "Yellow"
    Write-ColorMessage "7. Actualizar paquetes NuGet" "Yellow"
    Write-ColorMessage "8. Salir" "Yellow"
    Write-ColorMessage ""
    Write-ColorMessage "Seleccione una opción: " "Green" -NoNewline
}

# Ruta raíz del proyecto
$rootPath = Split-Path -Parent $MyInvocation.MyCommand.Path

# Bucle principal
do {
    Show-Menu
    $option = Read-Host
    
    switch ($option) {
        "1" {
            Write-ColorMessage "`nCrear nueva migración" "Cyan"
            Write-ColorMessage "Ingrese el nombre de la migración: " "Green" -NoNewline
            $migrationName = Read-Host
            
            if (-not [string]::IsNullOrWhiteSpace($migrationName)) {
                & "$rootPath\db-migrations.ps1" add $migrationName
            } else {
                Write-ColorMessage "El nombre de la migración no puede estar vacío." "Red"
            }
            
            Write-ColorMessage "`nPresione cualquier tecla para continuar..." "Gray"
            $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
        }
        
        "2" {
            Write-ColorMessage "`nAplicar migraciones pendientes" "Cyan"
            Write-ColorMessage "¿Está seguro de que desea aplicar las migraciones pendientes? (s/n): " "Yellow" -NoNewline
            $confirm = Read-Host
            
            if ($confirm -eq "s") {
                & "$rootPath\db-migrations.ps1" apply
            } else {
                Write-ColorMessage "Operación cancelada." "Red"
            }
            
            Write-ColorMessage "`nPresione cualquier tecla para continuar..." "Gray"
            $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
        }
        
        "3" {
            Write-ColorMessage "`nListar migraciones" "Cyan"
            & "$rootPath\db-migrations.ps1" list
            
            Write-ColorMessage "`nPresione cualquier tecla para continuar..." "Gray"
            $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
        }
        
        "4" {
            Write-ColorMessage "`nEliminar última migración" "Cyan"
            Write-ColorMessage "¿Está seguro de que desea eliminar la última migración? (s/n): " "Yellow" -NoNewline
            $confirm = Read-Host
            
            if ($confirm -eq "s") {
                & "$rootPath\db-migrations.ps1" remove
            } else {
                Write-ColorMessage "Operación cancelada." "Red"
            }
            
            Write-ColorMessage "`nPresione cualquier tecla para continuar..." "Gray"
            $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
        }
        
        "5" {
            Write-ColorMessage "`nIniciar aplicación con migración automática" "Cyan"
            Start-Process -FilePath "dotnet" -ArgumentList "run --project $rootPath\SGA.Api\SGA.Api.csproj" -NoNewWindow
            
            Write-ColorMessage "`nLa aplicación se está ejecutando en una nueva ventana." "Green"
            Write-ColorMessage "Presione cualquier tecla para continuar..." "Gray"
            $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
        }
        
        "6" {
            Write-ColorMessage "`nEjecutar herramienta de migración" "Cyan"
            Start-Process -FilePath "dotnet" -ArgumentList "run --project $rootPath\SGA.MigrationTool\SGA.MigrationTool.csproj" -NoNewWindow -Wait
            
            Write-ColorMessage "`nPresione cualquier tecla para continuar..." "Gray"
            $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
        }
        
        "7" {
            Write-ColorMessage "`nActualizar paquetes NuGet" "Cyan"
            
            $projects = @(
                "$rootPath\SGA.Api\SGA.Api.csproj",
                "$rootPath\SGA.Application\SGA.Application.csproj",
                "$rootPath\SGA.Domain\SGA.Domain.csproj",
                "$rootPath\SGA.Infrastructure\SGA.Infrastructure.csproj",
                "$rootPath\SGA.BlazorClient\SGA.BlazorApp.Client.csproj",
                "$rootPath\SGA.MigrationTool\SGA.MigrationTool.csproj"
            )
            
            foreach ($project in $projects) {
                if (Test-Path $project) {
                    Write-ColorMessage "Actualizando paquetes para $project..." "Yellow"
                    dotnet restore $project
                }
            }
            
            Write-ColorMessage "`nPresione cualquier tecla para continuar..." "Gray"
            $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
        }
        
        "8" {
            Write-ColorMessage "`nSaliendo..." "Cyan"
            return
        }
        
        default {
            Write-ColorMessage "`nOpción no válida. Por favor, intente de nuevo." "Red"
            Start-Sleep -Seconds 2
        }
    }
} while ($true)
