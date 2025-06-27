# Script para probar la funcionalidad completa del sistema
# Verifica que los datos reales se estén mostrando correctamente

$apiUrl = "http://localhost:5115"
$webUrl = "http://localhost:5039"

Write-Host "=== PRUEBA DE FUNCIONALIDAD COMPLETA DEL SISTEMA ===" -ForegroundColor Green
Write-Host ""

# 1. Login
Write-Host "1. Probando login con Steven Paredes..." -ForegroundColor Yellow
$loginData = @{
    email = "sparedes@uta.edu.ec"
    password = "123456"
} | ConvertTo-Json

try {
    $loginResponse = Invoke-RestMethod -Uri "$apiUrl/api/auth/login" -Method POST -Body $loginData -ContentType "application/json"
    $token = $loginResponse.token
    Write-Host "✓ Login exitoso" -ForegroundColor Green
    Write-Host "Token: $($token.Substring(0, 50))..." -ForegroundColor Cyan
} catch {
    Write-Host "✗ Error en login: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Headers para las siguientes requests
$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}

# 2. Obtener información del usuario
Write-Host ""
Write-Host "2. Obteniendo información del usuario autenticado..." -ForegroundColor Yellow
try {
    $userInfo = Invoke-RestMethod -Uri "$apiUrl/api/auth/me" -Method GET -Headers $headers
    Write-Host "✓ Usuario obtenido: $($userInfo.email)" -ForegroundColor Green
    Write-Host "Nombre: $($userInfo.nombres) $($userInfo.apellidos)" -ForegroundColor Cyan
} catch {
    Write-Host "✗ Error obteniendo usuario: $($_.Exception.Message)" -ForegroundColor Red
}

# 3. Importar datos desde TTHH
Write-Host ""
Write-Host "3. Importando datos desde TTHH..." -ForegroundColor Yellow
$importData = @{
    cedula = "1805123456"
} | ConvertTo-Json

try {
    $importResponse = Invoke-RestMethod -Uri "$apiUrl/api/docentes/importar-datos-tthh" -Method POST -Body $importData -Headers $headers
    Write-Host "✓ Datos importados desde TTHH" -ForegroundColor Green
} catch {
    Write-Host "✗ Error importando desde TTHH: $($_.Exception.Message)" -ForegroundColor Red
}

# 4. Obtener perfil del docente
Write-Host ""
Write-Host "4. Obteniendo perfil del docente..." -ForegroundColor Yellow
try {
    $perfil = Invoke-RestMethod -Uri "$apiUrl/api/docentes/perfil" -Method GET -Headers $headers
    Write-Host "✓ Perfil obtenido" -ForegroundColor Green
    Write-Host "Cargo: $($perfil.cargoActual)" -ForegroundColor Cyan
    Write-Host "Nivel Académico: $($perfil.nivelAcademico)" -ForegroundColor Cyan
    Write-Host "Fecha Nombramiento: $($perfil.fechaNombramiento)" -ForegroundColor Cyan
} catch {
    Write-Host "✗ Error obteniendo perfil: $($_.Exception.Message)" -ForegroundColor Red
}

# 5. Obtener indicadores
Write-Host ""
Write-Host "5. Obteniendo indicadores del docente..." -ForegroundColor Yellow
try {
    $indicadores = Invoke-RestMethod -Uri "$apiUrl/api/docentes/indicadores" -Method GET -Headers $headers
    Write-Host "✓ Indicadores obtenidos" -ForegroundColor Green
    Write-Host "Años experiencia: $($indicadores.aniosExperiencia)" -ForegroundColor Cyan
    Write-Host "Publicaciones ISI: $($indicadores.publicacionesISI)" -ForegroundColor Cyan
    Write-Host "Publicaciones Latindex: $($indicadores.publicacionesLatindex)" -ForegroundColor Cyan
    Write-Host "Evaluación docente: $($indicadores.evaluacionesDocentes)" -ForegroundColor Cyan
} catch {
    Write-Host "✗ Error obteniendo indicadores: $($_.Exception.Message)" -ForegroundColor Red
}

# 6. Obtener requisitos de ascenso
Write-Host ""
Write-Host "6. Obteniendo requisitos de ascenso..." -ForegroundColor Yellow
try {
    $requisitos = Invoke-RestMethod -Uri "$apiUrl/api/docentes/requisitos" -Method GET -Headers $headers
    Write-Host "✓ Requisitos obtenidos" -ForegroundColor Green
    Write-Host "Total requisitos: $($requisitos.requisitos.Count)" -ForegroundColor Cyan
    Write-Host "Cumplidos: $($requisitos.requisitos | Where-Object { $_.cumplido } | Measure-Object | Select-Object -ExpandProperty Count)" -ForegroundColor Cyan
    Write-Host "Porcentaje cumplimiento: $($requisitos.porcentajeCumplimiento)%" -ForegroundColor Cyan
} catch {
    Write-Host "✗ Error obteniendo requisitos: $($_.Exception.Message)" -ForegroundColor Red
}

# 7. Validar requisitos con datos externos
Write-Host ""
Write-Host "7. Validando requisitos con datos externos..." -ForegroundColor Yellow
try {
    $validacion = Invoke-RestMethod -Uri "$apiUrl/api/docentes/validar-requisitos" -Method POST -Headers $headers
    Write-Host "✓ Validación completada" -ForegroundColor Green
    Write-Host "Resultado: $($validacion.resultado)" -ForegroundColor Cyan
} catch {
    Write-Host "✗ Error en validación: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "=== RESUMEN DE PRUEBAS ===" -ForegroundColor Green
Write-Host "API ejecutándose en: $apiUrl" -ForegroundColor Cyan
Write-Host "Web ejecutándose en: $webUrl" -ForegroundColor Cyan
Write-Host ""
Write-Host "Ahora puedes abrir $webUrl en tu navegador para probar la interfaz web" -ForegroundColor Yellow
Write-Host "Credenciales de prueba:" -ForegroundColor Yellow
Write-Host "Email: sparedes@uta.edu.ec" -ForegroundColor Cyan
Write-Host "Password: 123456" -ForegroundColor Cyan
