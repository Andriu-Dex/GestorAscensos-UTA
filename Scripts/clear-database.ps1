# Script para limpiar todos los datos de las tablas
# Mantiene la estructura de la base de datos pero elimina todos los registros
# Incluye limpieza de bases de datos externas

Write-Host "🗑️ Limpiando todas las bases de datos..." -ForegroundColor Yellow

# Función para ejecutar comando SQL
function Execute-SqlCommand {
    param(
        [string]$ConnectionString,
        [string]$Command,
        [string]$DatabaseName
    )
    
    try {
        Write-Host "  📋 Ejecutando en $DatabaseName..." -ForegroundColor Gray
        sqlcmd -S ".\SQLEXPRESS" -E -Q $Command
        if ($LASTEXITCODE -eq 0) {
            Write-Host "  ✅ $DatabaseName limpiada" -ForegroundColor Green
        } else {
            Write-Host "  ⚠️ $DatabaseName no encontrada o sin datos" -ForegroundColor Yellow
        }
    }
    catch {
        Write-Host "  ❌ Error en $DatabaseName`: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Cambiar al directorio de la API
Set-Location "SGA.Api"

try {
    Write-Host ""
    Write-Host "🏗️ LIMPIANDO BASE DE DATOS PRINCIPAL..." -ForegroundColor Cyan
    
    # Eliminar y recrear la base de datos principal
    Write-Host "📋 Eliminando base de datos principal SGA_Main..." -ForegroundColor Cyan
    dotnet ef database drop --force --context ApplicationDbContext
    
    # Recrear la base de datos principal con las migraciones
    Write-Host "🔄 Recreando base de datos principal..." -ForegroundColor Cyan
    dotnet ef database update --context ApplicationDbContext
    
    Write-Host "✅ Base de datos principal limpiada exitosamente!" -ForegroundColor Green
    
    Write-Host ""
    Write-Host "🌐 LIMPIANDO BASES DE DATOS EXTERNAS..." -ForegroundColor Cyan
    
    # Limpiar base de datos TTHH
    Write-Host "🏢 Limpiando base de datos TTHH (Talento Humano)..." -ForegroundColor Cyan
    $tthh_commands = @(
        "IF DB_ID('TTHH') IS NOT NULL BEGIN USE TTHH; DELETE FROM Empleados; DELETE FROM AccionesPersonal; DELETE FROM Cargos; DELETE FROM HistorialPromociones; END"
    )
    foreach ($cmd in $tthh_commands) {
        Execute-SqlCommand -ConnectionString "" -Command $cmd -DatabaseName "TTHH"
    }
    
    # Limpiar base de datos DAC
    Write-Host "📊 Limpiando base de datos DAC (Evaluación Docente)..." -ForegroundColor Cyan
    $dac_commands = @(
        "IF DB_ID('DAC') IS NOT NULL BEGIN USE DAC; DELETE FROM EvaluacionesDocentes; DELETE FROM PeriodosAcademicos; DELETE FROM CriteriosEvaluacion; END"
    )
    foreach ($cmd in $dac_commands) {
        Execute-SqlCommand -ConnectionString "" -Command $cmd -DatabaseName "DAC"
    }
    
    # Limpiar base de datos DITIC
    Write-Host "🎓 Limpiando base de datos DITIC (Capacitación)..." -ForegroundColor Cyan
    $ditic_commands = @(
        "IF DB_ID('DITIC') IS NOT NULL BEGIN USE DITIC; DELETE FROM ParticipacionCursos; DELETE FROM CursosDisponibles; DELETE FROM Certificaciones; END"
    )
    foreach ($cmd in $ditic_commands) {
        Execute-SqlCommand -ConnectionString "" -Command $cmd -DatabaseName "DITIC"
    }
    
    # Limpiar base de datos DIRINV
    Write-Host "🔬 Limpiando base de datos DIRINV (Investigación)..." -ForegroundColor Cyan
    $dirinv_commands = @(
        "IF DB_ID('DIRINV') IS NOT NULL BEGIN USE DIRINV; DELETE FROM ObrasAcademicas; DELETE FROM ProyectosInvestigacion; DELETE FROM ParticipacionProyectos; END"
    )
    foreach ($cmd in $dirinv_commands) {
        Execute-SqlCommand -ConnectionString "" -Command $cmd -DatabaseName "DIRINV"
    }
    
    Write-Host ""
    Write-Host "✅ TODAS LAS BASES DE DATOS LIMPIADAS EXITOSAMENTE!" -ForegroundColor Green
    Write-Host ""
    Write-Host "🔑 Usuario administrador creado:" -ForegroundColor Blue
    Write-Host "   Email: admin@uta.edu.ec" -ForegroundColor White
    Write-Host "   Password: Admin12345" -ForegroundColor White
    Write-Host ""
    Write-Host "📋 Bases de datos afectadas:" -ForegroundColor Blue
    Write-Host "   • SGA_Main (Principal) - Recreada ✅" -ForegroundColor White
    Write-Host "   • TTHH (Talento Humano) - Limpiada ✅" -ForegroundColor White
    Write-Host "   • DAC (Evaluación Docente) - Limpiada ✅" -ForegroundColor White
    Write-Host "   • DITIC (Capacitación) - Limpiada ✅" -ForegroundColor White
    Write-Host "   • DIRINV (Investigación) - Limpiada ✅" -ForegroundColor White
    
} catch {
    Write-Host "❌ Error al limpiar las bases de datos: $($_.Exception.Message)" -ForegroundColor Red
}

# Volver al directorio raíz
Set-Location ".."
