# Script para configurar variables de entorno del Sistema de Gestion de Ascensos
# Ejecutar como Administrador

Write-Host "Configurando variables de entorno para SGA..." -ForegroundColor Green

# Funcion para establecer variable de entorno
function Set-EnvVariable {
    param(
        [string]$Name,
        [string]$Value,
        [string]$Description
    )
    
    Write-Host "Configurando $Name..." -ForegroundColor Yellow
    [Environment]::SetEnvironmentVariable($Name, $Value, [EnvironmentVariableTarget]::User)
    Write-Host "OK $Description" -ForegroundColor Green
}

# Configuracion de Base de Datos
$dbMain = "Server=.\SQLEXPRESS;Database=SGA_Main;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
$dbTTHH = "Server=.\SQLEXPRESS;Database=TTHH;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
$dbDAC = "Server=.\SQLEXPRESS;Database=DAC;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
$dbDITIC = "Server=.\SQLEXPRESS;Database=DITIC;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
$dbDIRINV = "Server=.\SQLEXPRESS;Database=DIRINV;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"

Set-EnvVariable -Name "SGA_DB_CONNECTION" -Value $dbMain -Description "Conexion principal SGA"
Set-EnvVariable -Name "SGA_TTHH_CONNECTION" -Value $dbTTHH -Description "Conexion TTHH"
Set-EnvVariable -Name "SGA_DAC_CONNECTION" -Value $dbDAC -Description "Conexion DAC"
Set-EnvVariable -Name "SGA_DITIC_CONNECTION" -Value $dbDITIC -Description "Conexion DITIC"
Set-EnvVariable -Name "SGA_DIRINV_CONNECTION" -Value $dbDIRINV -Description "Conexion DIRINV"

# Configuracion JWT
$jwtSecret = "SGA_SecretKey_2024_Sistema_Gestion_Ascensos_UTA_123456789012345678901234567890"
Set-EnvVariable -Name "SGA_JWT_SECRET_KEY" -Value $jwtSecret -Description "Clave secreta JWT"

# Configuracion CORS
Set-EnvVariable -Name "SGA_CORS_ORIGINS" -Value "https://localhost:7149,http://localhost:5039" -Description "Origenes permitidos CORS"

# Configuracion de Logging
Set-EnvVariable -Name "SGA_LOG_LEVEL" -Value "Information" -Description "Nivel de logging"

Write-Host ""
Write-Host "Configuracion completada!" -ForegroundColor Green
Write-Host "Las variables de entorno han sido configuradas para el usuario actual." -ForegroundColor Cyan
Write-Host "Reinicia tu IDE o terminal para que las variables surtan efecto." -ForegroundColor Yellow

# Mostrar variables configuradas
Write-Host ""
Write-Host "Variables configuradas:" -ForegroundColor Cyan
Write-Host "SGA_DB_CONNECTION: CONFIGURADO"
Write-Host "SGA_JWT_SECRET_KEY: CONFIGURADO"
Write-Host "SGA_CORS_ORIGINS: CONFIGURADO"
Write-Host "SGA_LOG_LEVEL: CONFIGURADO"
