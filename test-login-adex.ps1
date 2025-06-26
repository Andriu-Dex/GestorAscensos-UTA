# Script de Prueba Login Andriu Dex
# Sistema de Gestión de Ascensos UTA

Write-Host "=== PRUEBA LOGIN ANDRIU DEX ===" -ForegroundColor Green
Write-Host "Email: adex@uta.edu.ec" -ForegroundColor Yellow
Write-Host "Password: @Andriu3Dex@" -ForegroundColor Yellow
Write-Host ""

$baseUrl = "http://localhost:5115"

# 1. Verificar que la API está funcionando
Write-Host "1. VERIFICANDO API" -ForegroundColor Magenta
try {
    $testResponse = Invoke-RestMethod -Uri "$baseUrl/api/auth/test" -Method GET
    Write-Host "✅ API funcionando: $($testResponse.message)" -ForegroundColor Green
} catch {
    Write-Host "❌ API no responde: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Iniciando API..." -ForegroundColor Yellow
    exit
}

# 2. Probar login con Andriu Dex
Write-Host ""
Write-Host "2. PRUEBA DE LOGIN" -ForegroundColor Magenta
Write-Host "==================" -ForegroundColor Magenta
try {
    $loginBody = @{ 
        email = "adex@uta.edu.ec"
        password = "@Andriu3Dex@" 
    } | ConvertTo-Json
    
    $headers = @{ "Content-Type" = "application/json" }
    Write-Host "Enviando request login..." -ForegroundColor Cyan
    $loginResponse = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method POST -Headers $headers -Body $loginBody
    
    Write-Host ""
    Write-Host "🎉 LOGIN EXITOSO!" -ForegroundColor Green
    Write-Host "   👤 Usuario: $($loginResponse.usuario)" -ForegroundColor White
    Write-Host "   📧 Email: $($loginResponse.email)" -ForegroundColor White
    Write-Host "   🎭 Rol: $($loginResponse.rol)" -ForegroundColor White
    if ($loginResponse.facultad) {
        Write-Host "   🏛️ Facultad: $($loginResponse.facultad)" -ForegroundColor White
    }
    Write-Host "   🔑 Token: $($loginResponse.token.Substring(0,50))..." -ForegroundColor Gray
    Write-Host "   ⏰ Expires: $($loginResponse.expires)" -ForegroundColor Gray
    
    # Guardar el token para pruebas adicionales
    $global:authToken = $loginResponse.token
    Write-Host ""
    Write-Host "✅ Token guardado para pruebas adicionales" -ForegroundColor Green
    
} catch {
    Write-Host "❌ LOGIN FALLÓ: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        Write-Host "   Código de estado: $($_.Exception.Response.StatusCode)" -ForegroundColor Red
    }
}

# 3. Si el login fue exitoso, probar un endpoint autenticado
if ($global:authToken) {
    Write-Host ""
    Write-Host "3. PRUEBA ENDPOINT AUTENTICADO" -ForegroundColor Magenta
    Write-Host "==============================" -ForegroundColor Magenta
    try {
        $authHeaders = @{ 
            "Content-Type" = "application/json"
            "Authorization" = "Bearer $global:authToken"
        }
        
        # Probar obtener perfil (si existe este endpoint)
        Write-Host "Probando endpoint de perfil..." -ForegroundColor Cyan
        $perfilResponse = Invoke-RestMethod -Uri "$baseUrl/api/auth/profile" -Method GET -Headers $authHeaders
        Write-Host "✅ Perfil obtenido correctamente" -ForegroundColor Green
        
    } catch {
        Write-Host "⚠️  Endpoint de perfil no disponible: $($_.Exception.Response.StatusCode)" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "=== FIN DE PRUEBAS ===" -ForegroundColor Green
