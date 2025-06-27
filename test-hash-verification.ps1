# Test de verificación de hash de contraseña
Write-Host "=== VERIFICACIÓN DE HASH DE CONTRASEÑA ===" -ForegroundColor Cyan

$baseUrl = "https://localhost:7030/api"

# Probar diferentes combinaciones de contraseñas
$passwords = @(
    "@Andriu3Dex@",
    "123456",
    "admin123",
    "password"
)

foreach ($password in $passwords) {
    Write-Host ""
    Write-Host "Probando contraseña: '$password'" -ForegroundColor Yellow
    
    $loginData = @{
        email = "adex@uta.edu.ec"
        password = $password
    } | ConvertTo-Json

    try {
        $response = Invoke-RestMethod -Uri "$baseUrl/auth/login" -Method POST -Body $loginData -ContentType "application/json" -SkipCertificateCheck
        Write-Host "✓ LOGIN EXITOSO con '$password'" -ForegroundColor Green
        Write-Host "Token: $($response.token)" -ForegroundColor White
        break
    }
    catch {
        Write-Host "✗ Login falló con '$password': $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "Verificación completada." -ForegroundColor White
