# Script para probar la carga del archivo .env
Write-Host "Probando la carga del archivo .env..." -ForegroundColor Green

$envFilePath = "c:\Users\andri\Documents\D-Proyectos\Git\ProyectoAgiles\SistemaGestionAscensos\.env"

if (Test-Path $envFilePath) {
    Write-Host "OK Archivo .env encontrado en: $envFilePath" -ForegroundColor Green
    
    # Leer contenido del archivo
    $content = Get-Content $envFilePath
    Write-Host ""
    Write-Host "Contenido del archivo .env:" -ForegroundColor Cyan
    Write-Host "================================" -ForegroundColor Cyan
    
    foreach ($line in $content) {
        if ($line.StartsWith("#") -or $line.Trim() -eq "") {
            Write-Host $line -ForegroundColor Gray
        } elseif ($line.Contains("JWT_SECRET")) {
            $parts = $line.Split("=", 2)
            Write-Host "$($parts[0])=[CONFIGURADO]" -ForegroundColor Yellow
        } else {
            Write-Host $line -ForegroundColor White
        }
    }
    
    Write-Host "================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "OK El archivo .env esta listo para usar" -ForegroundColor Green
    Write-Host "La aplicacion cargara estas variables automaticamente" -ForegroundColor Cyan
    
} else {
    Write-Host "ERROR Archivo .env no encontrado en: $envFilePath" -ForegroundColor Red
    Write-Host "Usa el script setup-env.ps1 o crea el archivo manualmente" -ForegroundColor Yellow
}
