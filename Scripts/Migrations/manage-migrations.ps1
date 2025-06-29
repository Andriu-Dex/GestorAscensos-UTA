# Script simplificado para gestión de migraciones
param(
    [string]$Action = "verify"
)

Write-Host "=== Gestor de Migraciones SGA ===" -ForegroundColor Green

function Verify-Database {
    Write-Host "Verificando estructura de base de datos..." -ForegroundColor Yellow
    
    try {
        # Verificar tablas
        $tables = sqlcmd -S ".\SQLEXPRESS" -d "SGA_Main" -E -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'" -h -1
        Write-Host "Tablas encontradas: $($tables.Trim())" -ForegroundColor Green
        
        # Verificar constraints
        $constraints = sqlcmd -S ".\SQLEXPRESS" -d "SGA_Main" -E -Q "SELECT COUNT(*) FROM sys.check_constraints" -h -1
        Write-Host "Constraints encontrados: $($constraints.Trim())" -ForegroundColor Green
        
        # Mostrar constraints específicos
        Write-Host "Constraints de enum:" -ForegroundColor Cyan
        sqlcmd -S ".\SQLEXPRESS" -d "SGA_Main" -E -Q "SELECT name as ConstraintName FROM sys.check_constraints WHERE name LIKE 'CK_%'"
        
    } catch {
        Write-Host "Error verificando base de datos: $($_.Exception.Message)" -ForegroundColor Red
    }
}

function Reset-Complete {
    Write-Host "Iniciando reset completo..." -ForegroundColor Yellow
    
    # Limpiar migraciones
    if (Test-Path "SGA.Infrastructure\Migrations") {
        Remove-Item -Path "SGA.Infrastructure\Migrations\*" -Recurse -Force -ErrorAction SilentlyContinue
        Write-Host "Migraciones limpiadas" -ForegroundColor Green
    }
    
    # Eliminar BD
    try {
        dotnet ef database drop --project SGA.Infrastructure --startup-project SGA.Api --context ApplicationDbContext --force
        Write-Host "Base de datos eliminada" -ForegroundColor Green
    } catch {
        Write-Host "BD no existía o error eliminando" -ForegroundColor Yellow
    }
    
    # Crear migración
    dotnet ef migrations add InitialCreate --project SGA.Infrastructure --startup-project SGA.Api --context ApplicationDbContext
    Write-Host "Migración creada" -ForegroundColor Green
    
    # Aplicar migración
    dotnet ef database update --project SGA.Infrastructure --startup-project SGA.Api --context ApplicationDbContext
    Write-Host "Migración aplicada" -ForegroundColor Green
    
    # Agregar constraints
    sqlcmd -S ".\SQLEXPRESS" -d "SGA_Main" -E -i "Scripts\Migrations\Post-Migration\01-enum-constraints.sql"
    Write-Host "Constraints agregados" -ForegroundColor Green
    
    # Crear índice único
    sqlcmd -S ".\SQLEXPRESS" -d "SGA_Main" -E -i "Scripts\Migrations\Post-Migration\03-special-indexes.sql"
    Write-Host "Índice único creado" -ForegroundColor Green
}

switch ($Action) {
    "verify" { Verify-Database }
    "reset" { Reset-Complete; Verify-Database }
    default { 
        Write-Host "Opciones: verify, reset" -ForegroundColor Red 
        Write-Host "Uso: .\Scripts\manage-migrations.ps1 -Action verify" -ForegroundColor Gray
    }
}

Write-Host "=== Completado ===" -ForegroundColor Green
