#!/usr/bin/env pwsh

# Script para verificar y configurar herramientas de Entity Framework Core

# Colores para la salida
$colorInfo = "Cyan"
$colorSuccess = "Green"
$colorError = "Red"
$colorWarning = "Yellow"

Write-Host "Verificando herramientas de Entity Framework Core..." -ForegroundColor $colorInfo

# Verificar si las herramientas de EF Core están instaladas globalmente
$efToolsInstalled = $false
try {
    $efVersion = dotnet ef --version
    if ($efVersion) {
        $efToolsInstalled = $true
        Write-Host "Herramientas de Entity Framework Core instaladas: $efVersion" -ForegroundColor $colorSuccess
    }
}
catch {
    Write-Host "No se encontraron las herramientas de Entity Framework Core." -ForegroundColor $colorWarning
}

# Si las herramientas no están instaladas, preguntar si desea instalarlas
if (-not $efToolsInstalled) {
    Write-Host "Las herramientas de Entity Framework Core son necesarias para gestionar migraciones." -ForegroundColor $colorInfo
    $response = Read-Host "¿Desea instalarlas ahora? (s/n)"
    
    if ($response.ToLower() -eq "s") {
        Write-Host "Instalando herramientas de Entity Framework Core..." -ForegroundColor $colorInfo
        
        try {
            dotnet tool install --global dotnet-ef
            
            if ($LASTEXITCODE -eq 0) {
                Write-Host "Herramientas de Entity Framework Core instaladas correctamente." -ForegroundColor $colorSuccess
            }
            else {
                Write-Host "Error al instalar las herramientas de Entity Framework Core." -ForegroundColor $colorError
                Write-Host "Por favor, intente instalarlas manualmente con: dotnet tool install --global dotnet-ef" -ForegroundColor $colorInfo
            }
        }
        catch {
            Write-Host "Error al instalar las herramientas de Entity Framework Core: $_" -ForegroundColor $colorError
            Write-Host "Por favor, intente instalarlas manualmente con: dotnet tool install --global dotnet-ef" -ForegroundColor $colorInfo
        }
    }
    else {
        Write-Host "Las herramientas no serán instaladas. Algunas funcionalidades de migración podrían no estar disponibles." -ForegroundColor $colorWarning
    }
}

# Verificar el paquete de diseño de EF Core en el proyecto de infraestructura
Write-Host "`nVerificando paquetes de Entity Framework Core en el proyecto de infraestructura..." -ForegroundColor $colorInfo

$rootDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$infraProjectFile = Join-Path $rootDir "SGA.Infrastructure\SGA.Infrastructure.csproj"

if (Test-Path $infraProjectFile) {
    $projectContent = Get-Content $infraProjectFile -Raw
    
    # Verificar si los paquetes necesarios están instalados
    $efDesignInstalled = $projectContent -match "Microsoft.EntityFrameworkCore.Design"
    $efSqlServerInstalled = $projectContent -match "Microsoft.EntityFrameworkCore.SqlServer"
    
    if ($efDesignInstalled) {
        Write-Host "El paquete Microsoft.EntityFrameworkCore.Design está instalado." -ForegroundColor $colorSuccess
    }
    else {
        Write-Host "El paquete Microsoft.EntityFrameworkCore.Design no está instalado en el proyecto de infraestructura." -ForegroundColor $colorWarning
        $response = Read-Host "¿Desea instalarlo ahora? (s/n)"
        
        if ($response.ToLower() -eq "s") {
            Write-Host "Instalando Microsoft.EntityFrameworkCore.Design..." -ForegroundColor $colorInfo
            
            try {
                dotnet add $infraProjectFile package Microsoft.EntityFrameworkCore.Design
                
                if ($LASTEXITCODE -eq 0) {
                    Write-Host "Microsoft.EntityFrameworkCore.Design instalado correctamente." -ForegroundColor $colorSuccess
                }
                else {
                    Write-Host "Error al instalar Microsoft.EntityFrameworkCore.Design." -ForegroundColor $colorError
                }
            }
            catch {
                Write-Host "Error al instalar Microsoft.EntityFrameworkCore.Design: $_" -ForegroundColor $colorError
            }
        }
    }
    
    if ($efSqlServerInstalled) {
        Write-Host "El paquete Microsoft.EntityFrameworkCore.SqlServer está instalado." -ForegroundColor $colorSuccess
    }
    else {
        Write-Host "El paquete Microsoft.EntityFrameworkCore.SqlServer no está instalado en el proyecto de infraestructura." -ForegroundColor $colorWarning
        $response = Read-Host "¿Desea instalarlo ahora? (s/n)"
        
        if ($response.ToLower() -eq "s") {
            Write-Host "Instalando Microsoft.EntityFrameworkCore.SqlServer..." -ForegroundColor $colorInfo
            
            try {
                dotnet add $infraProjectFile package Microsoft.EntityFrameworkCore.SqlServer
                
                if ($LASTEXITCODE -eq 0) {
                    Write-Host "Microsoft.EntityFrameworkCore.SqlServer instalado correctamente." -ForegroundColor $colorSuccess
                }
                else {
                    Write-Host "Error al instalar Microsoft.EntityFrameworkCore.SqlServer." -ForegroundColor $colorError
                }
            }
            catch {
                Write-Host "Error al instalar Microsoft.EntityFrameworkCore.SqlServer: $_" -ForegroundColor $colorError
            }
        }
    }
}
else {
    Write-Host "No se encontró el archivo de proyecto de infraestructura en la ruta: $infraProjectFile" -ForegroundColor $colorError
}

Write-Host "`nVerificación completada." -ForegroundColor $colorInfo
Write-Host "Presione cualquier tecla para salir..." -ForegroundColor $colorInfo
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
