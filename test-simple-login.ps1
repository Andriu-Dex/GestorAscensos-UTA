# Script simple de prueba de login
Write-Host "Probando login de Steven Paredes..." -ForegroundColor Green

$baseUrl = "http://localhost:5115"

# Test API
try {
    $test = Invoke-RestMethod -Uri "$baseUrl/api/auth/test" -Method GET
    Write-Host "API OK: $($test.message)" -ForegroundColor Green
} catch {
    Write-Host "Error API: $($_.Exception.Message)" -ForegroundColor Red
    exit
}

# Validar cedula
try {
    $validationBody = '{"Cedula": "1805123456"}'
    $headers = @{ "Content-Type" = "application/json" }
    $validation = Invoke-RestMethod -Uri "$baseUrl/api/auth/validate-cedula" -Method POST -Headers $headers -Body $validationBody
    
    if ($validation.valid) {
        Write-Host "Empleado encontrado: $($validation.empleado.nombres) $($validation.empleado.apellidos)" -ForegroundColor Green
        Write-Host "Facultad: $($validation.empleado.facultad)" -ForegroundColor Yellow
        Write-Host "Email: $($validation.empleado.correoInstitucional)" -ForegroundColor Yellow
    }
} catch {
    Write-Host "Error validacion: $($_.Exception.Message)" -ForegroundColor Red
}

# Login
try {
    $loginBody = '{"email": "sparedes@uta.edu.ec", "password": "123456"}'
    $headers = @{ "Content-Type" = "application/json" }
    $login = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method POST -Headers $headers -Body $loginBody
    
    Write-Host "LOGIN EXITOSO!" -ForegroundColor Green
    Write-Host "Usuario: $($login.usuario)" -ForegroundColor Yellow
    Write-Host "Email: $($login.email)" -ForegroundColor Yellow
    Write-Host "Facultad: $($login.facultad)" -ForegroundColor Yellow
    Write-Host "Token generado correctamente" -ForegroundColor Green
} catch {
    Write-Host "LOGIN FALLO: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        Write-Host "Codigo: $($_.Exception.Response.StatusCode)" -ForegroundColor Red
    }
}
