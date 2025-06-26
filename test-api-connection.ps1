#!/usr/bin/env pwsh

# Script para probar la conectividad con la API

Write-Host "=== Pruebas de Conectividad API ===" -ForegroundColor Green

# 1. Probar endpoint de test
Write-Host "`n1. Probando endpoint de test..." -ForegroundColor Yellow
try {
    $testResponse = Invoke-RestMethod -Uri "https://localhost:7030/api/auth/test" -Method GET -SkipCertificateCheck
    Write-Host "✓ Endpoint de test funciona:" -ForegroundColor Green
    Write-Host ($testResponse | ConvertTo-Json -Depth 2) -ForegroundColor Cyan
} catch {
    Write-Host "✗ Error en endpoint de test: $($_.Exception.Message)" -ForegroundColor Red
}

# 2. Probar validación de cédula
Write-Host "`n2. Probando validación de cédula..." -ForegroundColor Yellow
try {
    $validationBody = @{ cedula = "1805123456" } | ConvertTo-Json
    $headers = @{
        "Content-Type" = "application/json"
        "Accept" = "application/json"
    }
    
    $validationResponse = Invoke-RestMethod -Uri "https://localhost:7030/api/auth/validate-cedula" -Method POST -Body $validationBody -Headers $headers -SkipCertificateCheck
    Write-Host "✓ Validación de cédula funciona:" -ForegroundColor Green
    Write-Host ($validationResponse | ConvertTo-Json -Depth 3) -ForegroundColor Cyan
} catch {
    Write-Host "✗ Error en validación de cédula: $($_.Exception.Message)" -ForegroundColor Red
}

# 3. Probar login con datos correctos
Write-Host "`n3. Probando login..." -ForegroundColor Yellow

# Probemos con diferentes contraseñas comunes
$passwords = @("123456", "password", "admin", "sparedes", "steven", "Paredes123")

foreach ($pwd in $passwords) {
    try {
        $loginBody = @{
            email = "sparedes@uta.edu.ec"
            password = $pwd
        } | ConvertTo-Json
        
        Write-Host "Probando con contraseña: $pwd" -ForegroundColor Cyan
        $loginResponse = Invoke-RestMethod -Uri "https://localhost:7030/api/auth/login" -Method POST -Body $loginBody -Headers $headers -SkipCertificateCheck
        Write-Host "✓ Login exitoso con contraseña: $pwd" -ForegroundColor Green
        Write-Host "Token: $($loginResponse.token)" -ForegroundColor Cyan
        Write-Host "Usuario: $($loginResponse.usuario.email)" -ForegroundColor Cyan
        break
    } catch {
        Write-Host "✗ Falló con: $pwd" -ForegroundColor Red
    }
}

Write-Host "`n=== Fin de las pruebas ===" -ForegroundColor Green
