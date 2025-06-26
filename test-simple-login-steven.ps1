# Script simple de prueba login Steven Paredes
Write-Host "Probando login de Steven Paredes..." -ForegroundColor Green

try {
    # Test API
    $test = Invoke-RestMethod -Uri "http://localhost:5115/api/auth/test" -Method GET
    Write-Host "API OK: $($test.message)" -ForegroundColor Green
    
    # Login
    $loginData = @{
        email = "sparedes@uta.edu.ec"
        password = "123456"
    } | ConvertTo-Json
    
    $headers = @{ "Content-Type" = "application/json" }
    $response = Invoke-RestMethod -Uri "http://localhost:5115/api/auth/login" -Method POST -Headers $headers -Body $loginData
    
    Write-Host "LOGIN EXITOSO!" -ForegroundColor Green
    Write-Host "Usuario: $($response.usuario.email)" -ForegroundColor Cyan
    Write-Host "Rol: $($response.usuario.rol)" -ForegroundColor Yellow
    if ($response.facultad) {
        Write-Host "Facultad: $($response.facultad)" -ForegroundColor White
    }
    Write-Host "Token generado correctamente" -ForegroundColor Gray
    
} catch {
    Write-Host "LOGIN FALLO: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Codigo: $($_.Exception.Response.StatusCode)" -ForegroundColor Red
}
