# Script para ejecutar API y Cliente Blazor en paralelo
# Uso: .\run-app.ps1

Write-Host "Iniciando Sistema de Gestión de Ascensos..." -ForegroundColor Green

# Función para ejecutar proyecto en nueva ventana
function Start-ProjectInNewWindow {
    param(
        [string]$ProjectPath,
        [string]$ProjectName
    )
    
    $command = "cd '$ProjectPath'; dotnet run"
    Start-Process powershell -ArgumentList "-NoExit", "-Command", $command
    Write-Host "Iniciando $ProjectName en nueva ventana..." -ForegroundColor Yellow
}

# Ruta base del proyecto
$BasePath = Split-Path -Parent $MyInvocation.MyCommand.Path

# Ejecutar API
$ApiPath = Join-Path $BasePath "SGA.Api"
Start-ProjectInNewWindow -ProjectPath $ApiPath -ProjectName "API"

# Esperar un poco para que la API inicie
Start-Sleep -Seconds 3

# Ejecutar Cliente Blazor
$ClientPath = Join-Path $BasePath "SGA.BlazorClient"
Start-ProjectInNewWindow -ProjectPath $ClientPath -ProjectName "Cliente Blazor"

Write-Host "Ambos proyectos iniciados. Revisa las ventanas de PowerShell que se abrieron." -ForegroundColor Green
Write-Host "API: https://localhost:7126" -ForegroundColor Cyan
Write-Host "Cliente: https://localhost:5173 (o el puerto que se muestre)" -ForegroundColor Cyan
