# Script de Prueba Login Steven Paredes
# Sistema de Gestión de Ascensos UTA

Write-Host "=== PRUEBA LOGIN STEVEN PAREDES ===" -ForegroundColor Green
Write-Host "Email: sparedes@uta.edu.ec" -ForegroundColor Yellow
Write-Host "Password: 123456" -ForegroundColor Yellow
Write-Host ""

$baseUrl = "http://localhost:5115"

# 1. Verificar que la API está funcionando
Write-Host "1. VERIFICANDO API" -ForegroundColor Magenta
Write-Host "=================" -ForegroundColor Magenta
try {
    $testResponse = Invoke-RestMethod -Uri "$baseUrl/api/auth/test" -Method GET
    Write-Host "✅ API funcionando: $($testResponse.message)" -ForegroundColor Green
} catch {
    Write-Host "❌ API no responde: $($_.Exception.Message)" -ForegroundColor Red
    exit
}

# 2. Probar validación de cédula
Write-Host ""
Write-Host "2. VALIDACIÓN DE CÉDULA" -ForegroundColor Magenta
Write-Host "=======================" -ForegroundColor Magenta
try {
    $validationBody = @{ Cedula = "1805123456" } | ConvertTo-Json
    $headers = @{ "Content-Type" = "application/json" }
    $validationResponse = Invoke-RestMethod -Uri "$baseUrl/api/auth/validate-cedula" -Method POST -Headers $headers -Body $validationBody
    
    if ($validationResponse.valid) {
        Write-Host "✅ Empleado encontrado: $($validationResponse.empleado.nombres) $($validationResponse.empleado.apellidos)" -ForegroundColor Green
        Write-Host "   🏛️ Facultad: $($validationResponse.empleado.facultad)" -ForegroundColor White
        Write-Host "   💼 Cargo: $($validationResponse.empleado.cargoActual)" -ForegroundColor White
        Write-Host "   📧 Email: $($validationResponse.empleado.correoInstitucional)" -ForegroundColor White
    }
} catch {
    Write-Host "❌ Error validando cédula: $($_.Exception.Message)" -ForegroundColor Red
}

# 3. Probar login
Write-Host ""
Write-Host "3. PRUEBA DE LOGIN" -ForegroundColor Magenta
Write-Host "==================" -ForegroundColor Magenta
try {
    $loginBody = @{ 
        email = "sparedes@uta.edu.ec"
        password = "123456" 
    } | ConvertTo-Json
    
    $headers = @{ "Content-Type" = "application/json" }
    $loginResponse = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method POST -Headers $headers -Body $loginBody
    
    Write-Host ""
    Write-Host "🎉 LOGIN EXITOSO!" -ForegroundColor Green
    Write-Host "   👤 Usuario: $($loginResponse.usuario)" -ForegroundColor White
    Write-Host "   📧 Email: $($loginResponse.email)" -ForegroundColor White
    Write-Host "   🏛️ Facultad: $($loginResponse.facultad)" -ForegroundColor White
    Write-Host "   🔑 Token: $($loginResponse.token.Substring(0,50))..." -ForegroundColor Gray
    Write-Host "   ⏰ Expires: $($loginResponse.expires)" -ForegroundColor Gray
} catch {
    Write-Host "❌ LOGIN FALLÓ: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        Write-Host "   Código de estado: $($_.Exception.Response.StatusCode)" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "=== FIN DE PRUEBAS ===" -ForegroundColor Green
