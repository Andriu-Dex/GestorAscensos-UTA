# Test simple de Obras Académicas
Write-Host "=== PRUEBA SIMPLE DE OBRAS ACADÉMICAS ===" -ForegroundColor Cyan

$baseUrl = "https://localhost:7030/api"

# Función para hacer login
function Login-User {
    param($email, $password)
    
    $loginData = @{
        email = $email
        password = $password
    } | ConvertTo-Json

    try {
        $response = Invoke-RestMethod -Uri "$baseUrl/auth/login" -Method POST -Body $loginData -ContentType "application/json" -SkipCertificateCheck
        return $response.token
    }
    catch {
        Write-Host "Error en login: $($_.Exception.Message)" -ForegroundColor Red
        return $null
    }
}

# Función para solicitar nuevas obras académicas
function Request-NewWorks {
    param($token)
    
    $headers = @{
        "Authorization" = "Bearer $token"
    }

    $obraData = @{
        Obras = @(
            @{
                Titulo = "Test de desarrollo con .NET 8"
                TipoObra = "Artículo"
                FechaPublicacion = "2024-06-01T00:00:00Z"
                Editorial = ""
                Revista = "Revista de Pruebas"
                ISBN_ISSN = "1234-5678"
                DOI = "10.1234/test.2024.001"
                EsIndexada = $true
                IndiceIndexacion = "Test"
                Autores = "Test Author"
                Descripcion = "Artículo de prueba"
                ArchivoNombre = ""
                ArchivoContenido = ""
                ArchivoTipo = ""
            }
        )
        Comentarios = "Solicitud de prueba"
    } | ConvertTo-Json -Depth 5

    try {
        Write-Host "Enviando solicitud de obra académica..." -ForegroundColor Green
        $response = Invoke-RestMethod -Uri "$baseUrl/obraacademicas/solicitar-nuevas" -Method POST -Body $obraData -ContentType "application/json" -Headers $headers -SkipCertificateCheck
        return $response
    }
    catch {
        Write-Host "Error al solicitar obra: $($_.Exception.Message)" -ForegroundColor Red
        if ($_.Exception.Response) {
            $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
            $responseBody = $reader.ReadToEnd()
            Write-Host "Detalles del error: $responseBody" -ForegroundColor Red
        }
        return $null
    }
}

# Paso 1: Login
Write-Host "1. Login con Andriu Dex..." -ForegroundColor Yellow
$token = Login-User -email "adex@uta.edu.ec" -password "@Andriu3Dex@"

if (-not $token) {
    Write-Host "✗ No se pudo hacer login. Abortando prueba." -ForegroundColor Red
    exit 1
}

Write-Host "✓ Login exitoso" -ForegroundColor Green

# Paso 2: Solicitar nueva obra
Write-Host "2. Solicitando nueva obra académica..." -ForegroundColor Yellow
$resultado = Request-NewWorks -token $token

if ($resultado) {
    if ($resultado.Exitoso) {
        Write-Host "✓ Solicitud enviada exitosamente" -ForegroundColor Green
        Write-Host "  - Mensaje: $($resultado.Mensaje)" -ForegroundColor White
        Write-Host "  - Obras solicitadas: $($resultado.TotalObras)" -ForegroundColor White
    }
    else {
        Write-Host "✗ Error en la solicitud: $($resultado.Mensaje)" -ForegroundColor Red
    }
}
else {
    Write-Host "✗ No se pudo completar la solicitud" -ForegroundColor Red
}

Write-Host ""
Write-Host "Prueba completada." -ForegroundColor White
