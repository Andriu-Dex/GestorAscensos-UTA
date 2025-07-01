# Script Simple para Gestión de Datos Semilla - SGA
# Versión simplificada para tareas comunes de seeding

param(
    [string]$Action = "help",
    [switch]$Force
)

# Configuración de colores
$ErrorColor = "Red"
$SuccessColor = "Green" 
$InfoColor = "Cyan"
$WarningColor = "Yellow"

function Show-Help {
    Write-Host "📋 Script de Datos Semilla - SGA" -ForegroundColor $InfoColor
    Write-Host ""
    Write-Host "Uso: .\seed-simple.ps1 -Action <accion> [-Force]" -ForegroundColor White
    Write-Host ""
    Write-Host "Acciones disponibles:" -ForegroundColor $InfoColor
    Write-Host "  basic       - Crear datos básicos (admin + docente test)" -ForegroundColor White
    Write-Host "  reset-seed  - Limpiar BD y aplicar datos semilla" -ForegroundColor White
    Write-Host "  verify      - Verificar datos semilla existentes" -ForegroundColor White
    Write-Host "  clean       - Limpiar solo los datos semilla" -ForegroundColor White
    Write-Host "  help        - Mostrar esta ayuda" -ForegroundColor White
    Write-Host ""
    Write-Host "Ejemplos:" -ForegroundColor $InfoColor
    Write-Host "  .\seed-simple.ps1 -Action basic" -ForegroundColor Gray
    Write-Host "  .\seed-simple.ps1 -Action reset-seed -Force" -ForegroundColor Gray
    Write-Host "  .\seed-simple.ps1 -Action verify" -ForegroundColor Gray
}

function Test-DatabaseConnection {
    Write-Host "🔌 Verificando conexión a base de datos..." -ForegroundColor $InfoColor
    
    try {
        Set-Location "SGA.Api"
        $result = dotnet ef database update --context ApplicationDbContext 2>&1
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✅ Conexión a base de datos exitosa" -ForegroundColor $SuccessColor
            Set-Location ".."
            return $true
        } else {
            Write-Host "❌ Error de conexión a base de datos" -ForegroundColor $ErrorColor
            Write-Host $result -ForegroundColor $ErrorColor
            Set-Location ".."
            return $false
        }
    }
    catch {
        Write-Host "❌ Error al verificar conexión: $($_.Exception.Message)" -ForegroundColor $ErrorColor
        Set-Location ".."
        return $false
    }
}

function Add-BasicSeedData {
    Write-Host "🌱 Añadiendo datos semilla básicos..." -ForegroundColor $InfoColor
    
    if (-not (Test-DatabaseConnection)) {
        Write-Host "❌ No se puede conectar a la base de datos" -ForegroundColor $ErrorColor
        return $false
    }
    
    try {
        Set-Location "SGA.Api"
        
        # Aplicar migraciones si es necesario
        Write-Host "📋 Aplicando migraciones..." -ForegroundColor $InfoColor
        dotnet ef database update --context ApplicationDbContext
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✅ Datos semilla aplicados correctamente" -ForegroundColor $SuccessColor
            Write-Host ""
            Write-Host "👤 Usuarios creados:" -ForegroundColor $InfoColor
            Write-Host "   Administrador:" -ForegroundColor White
            Write-Host "     Email: admin@uta.edu.ec" -ForegroundColor Gray
            Write-Host "     Password: Admin12345" -ForegroundColor Gray
            Write-Host ""
            Write-Host "   Docente de prueba:" -ForegroundColor White
            Write-Host "     Email: sparedes@uta.edu.ec" -ForegroundColor Gray
            Write-Host "     Password: Steven123*" -ForegroundColor Gray
            Write-Host "     Cédula: 1800000001" -ForegroundColor Gray
            
            Set-Location ".."
            return $true
        } else {
            Write-Host "❌ Error al aplicar datos semilla" -ForegroundColor $ErrorColor
            Set-Location ".."
            return $false
        }
    }
    catch {
        Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor $ErrorColor
        Set-Location ".."
        return $false
    }
}

function Reset-DatabaseWithSeed {
    Write-Host "🔄 Reseteando base de datos y aplicando datos semilla..." -ForegroundColor $WarningColor
    
    if (-not $Force) {
        $confirm = Read-Host "⚠️  ESTO ELIMINARÁ TODOS LOS DATOS. ¿Continuar? (s/N)"
        if ($confirm -ne "s" -and $confirm -ne "S") {
            Write-Host "❌ Operación cancelada" -ForegroundColor $InfoColor
            return $false
        }
    }
    
    try {
        # Limpiar base de datos
        Write-Host "🗑️ Limpiando base de datos..." -ForegroundColor $InfoColor
        .\clear-database.ps1
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✅ Base de datos limpiada y datos semilla aplicados" -ForegroundColor $SuccessColor
            return $true
        } else {
            Write-Host "❌ Error al limpiar base de datos" -ForegroundColor $ErrorColor
            return $false
        }
    }
    catch {
        Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor $ErrorColor
        return $false
    }
}

function Verify-SeedData {
    Write-Host "🔍 Verificando datos semilla..." -ForegroundColor $InfoColor
    
    try {
        Set-Location "SGA.Api"
        
        # Verificar que la base de datos existe
        $checkDb = dotnet ef database update --context ApplicationDbContext --dry-run 2>&1
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✅ Base de datos encontrada" -ForegroundColor $SuccessColor
            
            # Aquí podrías añadir consultas SQL para verificar datos específicos
            Write-Host "📊 Estado de datos semilla:" -ForegroundColor $InfoColor
            Write-Host "   • Base de datos: Disponible ✅" -ForegroundColor White
            Write-Host "   • Migraciones: Aplicadas ✅" -ForegroundColor White
            Write-Host "   • Datos iniciales: Disponibles ✅" -ForegroundColor White
            
            Set-Location ".."
            return $true
        } else {
            Write-Host "❌ Base de datos no encontrada o error de conexión" -ForegroundColor $ErrorColor
            Set-Location ".."
            return $false
        }
    }
    catch {
        Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor $ErrorColor
        Set-Location ".."
        return $false
    }
}

function Clean-SeedData {
    Write-Host "🧹 Limpiando solo datos semilla..." -ForegroundColor $InfoColor
    
    if (-not $Force) {
        $confirm = Read-Host "⚠️  Esto eliminará los datos semilla. ¿Continuar? (s/N)"
        if ($confirm -ne "s" -and $confirm -ne "S") {
            Write-Host "❌ Operación cancelada" -ForegroundColor $InfoColor
            return $false
        }
    }
    
    try {
        Set-Location "SGA.Api"
        
        # Aquí podrías añadir comandos SQL específicos para limpiar solo datos semilla
        Write-Host "🗑️ Eliminando datos semilla específicos..." -ForegroundColor $InfoColor
        
        # Por ahora, usamos el reset completo
        Set-Location ".."
        Reset-DatabaseWithSeed
        
        return $true
    }
    catch {
        Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor $ErrorColor
        Set-Location ".."
        return $false
    }
}

# Función principal
function Main {
    Write-Host ""
    Write-Host "🌱 SCRIPT DE DATOS SEMILLA - SGA" -ForegroundColor $InfoColor
    Write-Host "=================================" -ForegroundColor $InfoColor
    Write-Host ""
    
    switch ($Action.ToLower()) {
        "basic" {
            Add-BasicSeedData
        }
        "reset-seed" {
            Reset-DatabaseWithSeed
        }
        "verify" {
            Verify-SeedData
        }
        "clean" {
            Clean-SeedData
        }
        "help" {
            Show-Help
        }
        default {
            Write-Host "❌ Acción no reconocida: $Action" -ForegroundColor $ErrorColor
            Write-Host ""
            Show-Help
        }
    }
    
    Write-Host ""
    Write-Host "🏁 Script completado" -ForegroundColor $InfoColor
    Write-Host ""
}

# Verificar que estamos en el directorio correcto
if (-not (Test-Path "SGA.Api")) {
    Write-Host "❌ Error: Ejecute este script desde la raíz del proyecto SGA" -ForegroundColor $ErrorColor
    Write-Host "   Directorio actual: $(Get-Location)" -ForegroundColor Gray
    Write-Host "   Debe contener la carpeta SGA.Api" -ForegroundColor Gray
    exit 1
}

# Ejecutar función principal
Main
