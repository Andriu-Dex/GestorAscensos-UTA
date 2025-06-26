# Test de login con las credenciales correctas
$loginRequest = @{
    Email = "sparedes@uta.edu.ec"
    Password = "123456"
} | ConvertTo-Json

Write-Host "Probando login con email: sparedes@uta.edu.ec" -ForegroundColor Yellow
Write-Host "Enviando request..." -ForegroundColor Gray

try {
    $response = Invoke-RestMethod -Uri "http://localhost:5000/api/auth/login" -Method POST -Body $loginRequest -ContentType "application/json" -ErrorAction Stop
    
    Write-Host "✅ LOGIN EXITOSO!" -ForegroundColor Green
    Write-Host "Token: $($response.token.Substring(0, 50))..." -ForegroundColor Cyan
    Write-Host "Email: $($response.email)" -ForegroundColor Cyan
    Write-Host "Nombre: $($response.nombreCompleto)" -ForegroundColor Cyan
    Write-Host "Rol: $($response.rol)" -ForegroundColor Cyan
    
    if ($response.datosEmpleado) {
        Write-Host "Datos TTHH encontrados:" -ForegroundColor Green
        Write-Host "  Cédula: $($response.datosEmpleado.cedula)" -ForegroundColor White
        Write-Host "  Cargo: $($response.datosEmpleado.cargoActual)" -ForegroundColor White
        Write-Host "  Facultad: $($response.datosEmpleado.facultad)" -ForegroundColor White
    }
}
catch {
    Write-Host "❌ ERROR EN LOGIN:" -ForegroundColor Red
    Write-Host "Status: $($_.Exception.Response.StatusCode)" -ForegroundColor Red
    Write-Host "Mensaje: $($_.Exception.Message)" -ForegroundColor Red
    
    if ($_.Exception.Response) {
        $errorContent = $_.Exception.Response.GetResponseStream()
        if ($errorContent) {
            $reader = New-Object System.IO.StreamReader($errorContent)
            $errorText = $reader.ReadToEnd()
            Write-Host "Detalle: $errorText" -ForegroundColor Red
        }
    }
}
