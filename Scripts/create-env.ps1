# Script para crear archivo .env desde .env.example
Write-Host "Configurando entorno de desarrollo desde .env.example..." -ForegroundColor Green

$exampleFile = ".env.example"
$envFile = ".env"

if (-not (Test-Path $exampleFile)) {
    Write-Host "ERROR: Archivo .env.example no encontrado" -ForegroundColor Red
    exit 1
}

if (Test-Path $envFile) {
    Write-Host "AVISO: El archivo .env ya existe" -ForegroundColor Yellow
    $response = Read-Host "¿Deseas sobreescribirlo? (s/N)"
    if ($response -ne "s" -and $response -ne "S") {
        Write-Host "Operacion cancelada" -ForegroundColor Yellow
        exit 0
    }
}

# Copiar archivo ejemplo
Copy-Item $exampleFile $envFile
Write-Host "OK Archivo .env creado desde .env.example" -ForegroundColor Green

Write-Host ""
Write-Host "IMPORTANTE: Debes editar el archivo .env con tus datos reales:" -ForegroundColor Yellow
Write-Host "============================================================" -ForegroundColor Yellow
Write-Host "1. Reemplaza 'TU_SERVIDOR' con tu instancia de SQL Server" -ForegroundColor White
Write-Host "   Ejemplo: .\SQLEXPRESS (para SQL Server Express local)" -ForegroundColor Gray
Write-Host ""
Write-Host "2. Reemplaza 'TU_CLAVE_SECRETA_JWT...' con una clave segura" -ForegroundColor White
Write-Host "   Debe tener al menos 64 caracteres" -ForegroundColor Gray
Write-Host ""
Write-Host "3. Ajusta las URLs de CORS si usas puertos diferentes" -ForegroundColor White
Write-Host ""
Write-Host "Para editar el archivo:" -ForegroundColor Cyan
Write-Host "code .env" -ForegroundColor Gray
Write-Host "# o"
Write-Host "notepad .env" -ForegroundColor Gray
Write-Host ""

# Verificar si podemos sugerir valores por defecto
$hasSSMS = Get-Process | Where-Object { $_.ProcessName -like "*ssms*" -or $_.ProcessName -like "*sql*" }
if ($hasSSMS) {
    Write-Host "Detectamos SQL Server ejecutandose. Puedes probar con:" -ForegroundColor Green
    Write-Host "Server=.\SQLEXPRESS" -ForegroundColor Gray
} else {
    Write-Host "Asegurate de que SQL Server este instalado y ejecutandose" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Una vez configurado, ejecuta:" -ForegroundColor Cyan
Write-Host ".\scripts\verify-env.ps1    # Para verificar la configuracion" -ForegroundColor Gray
Write-Host "dotnet run          # Para ejecutar la aplicacion" -ForegroundColor Gray
