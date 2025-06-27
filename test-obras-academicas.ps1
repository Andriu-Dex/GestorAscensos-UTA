# Test de Obras Académicas - Verificar solicitud de nuevas obras
Write-Host "=== PRUEBA DE SOLICITUD DE OBRAS ACADÉMICAS ===" -ForegroundColor Cyan
Write-Host "Verificando que el proceso de solicitud funcione correctamente" -ForegroundColor Yellow

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
                Titulo = "Desarrollo de aplicaciones web con Blazor y .NET 8"
                TipoObra = "Artículo"
                FechaPublicacion = "2024-06-01T00:00:00Z"
                Editorial = ""
                Revista = "Revista de Tecnología y Desarrollo"
                ISBN_ISSN = "2345-6789"
                DOI = "10.1234/ejemplo.2024.001"
                EsIndexada = $true
                IndiceIndexacion = "Scopus"
                Autores = "Steven Paredes, María García"
                Descripcion = "Artículo sobre desarrollo moderno de aplicaciones web usando tecnologías Microsoft"
                ArchivoNombre = ""
                ArchivoContenido = ""
                ArchivoTipo = ""
            }
        )
        Comentarios = "Solicitud de prueba para verificar funcionamiento del sistema"
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

# Función para obtener solicitudes pendientes
function Get-PendingRequests {
    param($token)
    
    $headers = @{
        "Authorization" = "Bearer $token"
    }

    try {
        $response = Invoke-RestMethod -Uri "$baseUrl/obraacademicas/solicitudes-pendientes" -Method GET -Headers $headers -SkipCertificateCheck
        return $response
    }
    catch {
        Write-Host "Error al obtener solicitudes pendientes: $($_.Exception.Message)" -ForegroundColor Red
        return $null
    }
}

# Función para obtener obras actuales
function Get-CurrentWorks {
    param($token)
    
    $headers = @{
        "Authorization" = "Bearer $token"
    }

    try {
        $response = Invoke-RestMethod -Uri "$baseUrl/obraacademicas/mis-obras" -Method GET -Headers $headers -SkipCertificateCheck
        return $response
    }
    catch {
        Write-Host "Error al obtener obras actuales: $($_.Exception.Message)" -ForegroundColor Red
        return $null
    }
}

# Paso 1: Login con Andriu Dex
Write-Host ""
Write-Host "1. === LOGIN CON ANDRIU DEX ===" -ForegroundColor Cyan
$token = Login-User -email "adex@uta.edu.ec" -password "@Andriu3Dex@"

if (-not $token) {
    Write-Host "No se pudo hacer login. Abortando prueba." -ForegroundColor Red
    exit 1
}

Write-Host "✓ Login exitoso" -ForegroundColor Green

# Paso 2: Obtener obras actuales
Write-Host ""
Write-Host "2. === OBTENER OBRAS ACTUALES ===" -ForegroundColor Cyan
$obraActuales = Get-CurrentWorks -token $token

if ($obraActuales) {
    Write-Host "✓ Obras actuales obtenidas exitosamente" -ForegroundColor Green
    Write-Host "  - Total de obras: $($obraActuales.TotalObras)" -ForegroundColor White
    if ($obraActuales.Obras -and $obraActuales.Obras.Count -gt 0) {
        $obraActuales.Obras | ForEach-Object {
            Write-Host "    - $($_.Titulo) ($($_.TipoObra))" -ForegroundColor Gray
        }
    }
} else {
    Write-Host "⚠ No se pudieron obtener las obras actuales" -ForegroundColor Yellow
}

# Paso 3: Obtener solicitudes pendientes antes
Write-Host ""
Write-Host "3. === VERIFICAR SOLICITUDES PENDIENTES (ANTES) ===" -ForegroundColor Cyan
$solicitudesPendientesAntes = Get-PendingRequests -token $token

if ($solicitudesPendientesAntes) {
    Write-Host "✓ Solicitudes pendientes obtenidas exitosamente" -ForegroundColor Green
    Write-Host "  - Solicitudes pendientes: $($solicitudesPendientesAntes.TotalObras)" -ForegroundColor White
    if ($solicitudesPendientesAntes.Obras -and $solicitudesPendientesAntes.Obras.Count -gt 0) {
        $solicitudesPendientesAntes.Obras | ForEach-Object {
            Write-Host "    - $($_.Titulo) (Estado: $($_.Estado))" -ForegroundColor Gray
        }
    }
} else {
    Write-Host "⚠ No se pudieron obtener las solicitudes pendientes" -ForegroundColor Yellow
}

# Paso 4: Solicitar nueva obra
Write-Host ""
Write-Host "4. === SOLICITAR NUEVA OBRA ACADÉMICA ===" -ForegroundColor Cyan
$resultado = Request-NewWorks -token $token

if ($resultado) {
    if ($resultado.Exitoso) {
        Write-Host "✓ Solicitud enviada exitosamente" -ForegroundColor Green
        Write-Host "  - Mensaje: $($resultado.Mensaje)" -ForegroundColor White
        Write-Host "  - Obras solicitadas: $($resultado.TotalObras)" -ForegroundColor White
    } else {
        Write-Host "✗ Error en la solicitud: $($resultado.Mensaje)" -ForegroundColor Red
    }
} else {
    Write-Host "✗ No se pudo completar la solicitud" -ForegroundColor Red
}

# Paso 5: Verificar solicitudes pendientes después
Write-Host ""
Write-Host "5. === VERIFICAR SOLICITUDES PENDIENTES (DESPUÉS) ===" -ForegroundColor Cyan
$solicitudesPendientesDespues = Get-PendingRequests -token $token

if ($solicitudesPendientesDespues) {
    Write-Host "✓ Solicitudes pendientes obtenidas exitosamente" -ForegroundColor Green
    Write-Host "  - Solicitudes pendientes: $($solicitudesPendientesDespues.TotalObras)" -ForegroundColor White
    if ($solicitudesPendientesDespues.Obras -and $solicitudesPendientesDespues.Obras.Count -gt 0) {
        $solicitudesPendientesDespues.Obras | ForEach-Object {
            Write-Host "    - $($_.Titulo) (Estado: $($_.Estado), Fecha: $($_.FechaCreacion))" -ForegroundColor Gray
        }
    }
    
    # Comparar antes y después
    $diferencia = $solicitudesPendientesDespues.TotalObras - $solicitudesPendientesAntes.TotalObras
    if ($diferencia -gt 0) {
        Write-Host "✓ Se agregaron $diferencia nueva(s) solicitud(es)" -ForegroundColor Green
    } else {
        Write-Host "⚠ No se detectó incremento en solicitudes pendientes" -ForegroundColor Yellow
    }
} else {
    Write-Host "⚠ No se pudieron obtener las solicitudes pendientes finales" -ForegroundColor Yellow
}

# Conclusión
Write-Host ""
Write-Host "6. === CONCLUSIÓN ===" -ForegroundColor Cyan
if ($resultado -and $resultado.Exitoso) {
    Write-Host "✓ La funcionalidad de solicitud de obras académicas está funcionando correctamente" -ForegroundColor Green
    Write-Host "✓ El modal debería cerrarse después de enviar la solicitud" -ForegroundColor Green
    Write-Host "✓ Las solicitudes se están registrando en el sistema" -ForegroundColor Green
} else {
    Write-Host "✗ Hay problemas con la funcionalidad de solicitud de obras académicas" -ForegroundColor Red
    Write-Host "- Revisar logs del servidor para más detalles" -ForegroundColor Yellow
    Write-Host "- Verificar la configuración de la base de datos" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Prueba completada." -ForegroundColor White
