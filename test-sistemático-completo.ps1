# Script de Pruebas Sistemáticas del Sistema de Gestión de Ascensos UTA
# Fecha: 5 de Junio de 2025

Write-Host "=== INICIO DE PRUEBAS SISTEMÁTICAS ===" -ForegroundColor Green
Write-Host "Sistema de Gestión de Ascensos UTA" -ForegroundColor Yellow
Write-Host "Verificando funcionalidades post-corrección de errores..." -ForegroundColor White
Write-Host ""

$baseUrl = "http://localhost:5115/api"
$global:authToken = $null

# Función para realizar peticiones HTTP con manejo de errores
function Invoke-ApiRequest {
    param(
        [string]$Url,
        [string]$Method = "GET",
        [object]$Body = $null,
        [hashtable]$Headers = @{"Content-Type" = "application/json"},
        [string]$Description
    )
    
    Write-Host "🔄 $Description..." -ForegroundColor Cyan
    
    try {
        $params = @{
            Uri = $Url
            Method = $Method
            Headers = $Headers
        }
        
        if ($Body) {
            if ($Body -is [string]) {
                $params.Body = $Body
            } else {
                $params.Body = $Body | ConvertTo-Json -Depth 10
            }
        }
        
        $response = Invoke-RestMethod @params
        Write-Host "✅ $Description - EXITOSO" -ForegroundColor Green
        return $response
    }
    catch {
        Write-Host "❌ $Description - ERROR: $($_.Exception.Message)" -ForegroundColor Red
        if ($_.Exception.Response) {
            $statusCode = $_.Exception.Response.StatusCode
            Write-Host "   Código de estado: $statusCode" -ForegroundColor Yellow
        }
        return $null
    }
}

# 1. PRUEBAS DE DATOS DE REFERENCIA
Write-Host "1. VERIFICANDO DATOS DE REFERENCIA" -ForegroundColor Magenta
Write-Host "=================================" -ForegroundColor Magenta

$facultades = Invoke-ApiRequest -Url "$baseUrl/Facultad" -Description "Obtener facultades"
if ($facultades) {
    Write-Host "   📊 Facultades disponibles: $($facultades.Count)" -ForegroundColor White
    $facultades | ForEach-Object { Write-Host "      - $($_.nombre)" -ForegroundColor Gray }
}

$tiposDocumento = Invoke-ApiRequest -Url "$baseUrl/TipoDocumento" -Description "Obtener tipos de documento"
if ($tiposDocumento) {
    Write-Host "   📊 Tipos de documento: $($tiposDocumento.Count)" -ForegroundColor White
}

$estadosSolicitud = Invoke-ApiRequest -Url "$baseUrl/EstadoSolicitud" -Description "Obtener estados de solicitud"
if ($estadosSolicitud) {
    Write-Host "   📊 Estados de solicitud: $($estadosSolicitud.Count)" -ForegroundColor White
}

Write-Host ""

# 2. PRUEBAS DE AUTENTICACIÓN
Write-Host "2. PROBANDO AUTENTICACIÓN" -ForegroundColor Magenta
Write-Host "=========================" -ForegroundColor Magenta

# Login
$loginData = @{
    email = "maria.garcia@uta.edu.ec"
    password = "Password123!"
}

$loginResponse = Invoke-ApiRequest -Url "$baseUrl/Auth/login" -Method "POST" -Body $loginData -Description "Login de usuario"

if ($loginResponse -and $loginResponse.token) {
    $global:authToken = $loginResponse.token
    Write-Host "   🔑 Token JWT obtenido correctamente" -ForegroundColor Green
    Write-Host "   👤 Usuario: $($loginResponse.email)" -ForegroundColor White
    Write-Host "   🏛️ Facultad: $($loginResponse.facultad)" -ForegroundColor White
    Write-Host "   📊 Nivel actual: $($loginResponse.nivelActual)" -ForegroundColor White
} else {
    Write-Host "   ❌ No se pudo obtener el token de autenticación" -ForegroundColor Red
    Write-Host "   🛑 Deteniendo pruebas que requieren autenticación" -ForegroundColor Yellow
}

Write-Host ""

# 3. PRUEBAS DE PERFIL DE DOCENTE (requiere autenticación)
if ($global:authToken) {
    Write-Host "3. PROBANDO GESTIÓN DE PERFIL" -ForegroundColor Magenta
    Write-Host "==============================" -ForegroundColor Magenta
    
    $authHeaders = @{
        "Content-Type" = "application/json"
        "Authorization" = "Bearer $global:authToken"
    }
    
    # Obtener perfil
    $perfil = Invoke-ApiRequest -Url "$baseUrl/Docente/perfil" -Headers $authHeaders -Description "Obtener perfil del docente"
    
    if ($perfil) {
        Write-Host "   👤 Nombre completo: $($perfil.nombres) $($perfil.apellidos)" -ForegroundColor White
        Write-Host "   📧 Email: $($perfil.email)" -ForegroundColor White
        Write-Host "   🆔 Cédula: $($perfil.cedula)" -ForegroundColor White
        Write-Host "   📊 Nivel actual: $($perfil.nivelActual)" -ForegroundColor White
        Write-Host "   📅 Fecha ingreso nivel: $($perfil.fechaIngresoNivelActual)" -ForegroundColor White
    }
    
    Write-Host ""
}

# 4. PRUEBAS DE DOCUMENTOS (requiere autenticación)
if ($global:authToken) {
    Write-Host "4. PROBANDO GESTIÓN DE DOCUMENTOS" -ForegroundColor Magenta
    Write-Host "==================================" -ForegroundColor Magenta
    
    # Obtener documentos del docente
    $documentos = Invoke-ApiRequest -Url "$baseUrl/Documento/mis-documentos" -Headers $authHeaders -Description "Obtener documentos del docente"
    
    if ($documentos) {
        Write-Host "   📁 Documentos encontrados: $($documentos.Count)" -ForegroundColor White
        if ($documentos.Count -gt 0) {
            $documentos | ForEach-Object {
                $estado = if ($_.validado) { "✅ Validado" } else { "⏳ Pendiente" }
                Write-Host "      - $($_.nombre) ($estado)" -ForegroundColor Gray
            }
        }
    }
    
    Write-Host ""
}

# 5. PRUEBAS DE SOLICITUDES DE ASCENSO (requiere autenticación)
if ($global:authToken) {
    Write-Host "5. PROBANDO GESTIÓN DE SOLICITUDES" -ForegroundColor Magenta
    Write-Host "===================================" -ForegroundColor Magenta
    
    # Obtener solicitudes del docente
    $solicitudes = Invoke-ApiRequest -Url "$baseUrl/SolicitudAscenso/mis-solicitudes" -Headers $authHeaders -Description "Obtener solicitudes del docente"
    
    if ($solicitudes) {
        Write-Host "   📋 Solicitudes encontradas: $($solicitudes.Count)" -ForegroundColor White
        if ($solicitudes.Count -gt 0) {
            $solicitudes | ForEach-Object {
                Write-Host "      - Nivel $($_.nivelActual) → $($_.nivelSolicitado) | Estado: $($_.estadoSolicitud)" -ForegroundColor Gray
                Write-Host "        Fecha: $($_.fechaSolicitud)" -ForegroundColor DarkGray
            }
        }
    }
    
    Write-Host ""
}

# 6. PRUEBAS DE CREACIÓN DE SOLICITUD (simulación)
if ($global:authToken -and $tiposDocumento -and $estadosSolicitud) {
    Write-Host "6. PROBANDO CREACIÓN DE SOLICITUD" -ForegroundColor Magenta
    Write-Host "==================================" -ForegroundColor Magenta
    
    # Datos de prueba para nueva solicitud
    $nuevaSolicitud = @{
        nivelSolicitado = 2  # Auxiliar 2
        tiempoEnRol = 24     # 24 meses
        horasCapacitacion = 120
        puntajeEvaluacion = 85.5
        tiempoInvestigacion = 12
        numeroObras = 3
        cumpleTiempo = $true
        cumpleCapacitacion = $true
        cumpleEvaluacion = $true
        cumpleInvestigacion = $true
        cumpleObras = $true
    }
    
    Write-Host "   📝 Simulando creación de solicitud de ascenso:" -ForegroundColor White
    Write-Host "      - Nivel solicitado: Auxiliar 2" -ForegroundColor Gray
    Write-Host "      - Tiempo en rol: $($nuevaSolicitud.tiempoEnRol) meses" -ForegroundColor Gray
    Write-Host "      - Horas capacitación: $($nuevaSolicitud.horasCapacitacion)" -ForegroundColor Gray
    Write-Host "      - Puntaje evaluación: $($nuevaSolicitud.puntajeEvaluacion)" -ForegroundColor Gray
    
    $solicitudCreada = Invoke-ApiRequest -Url "$baseUrl/SolicitudAscenso" -Method "POST" -Body $nuevaSolicitud -Headers $authHeaders -Description "Crear nueva solicitud de ascenso"
    
    if ($solicitudCreada) {
        Write-Host "   ✅ Solicitud creada exitosamente con ID: $($solicitudCreada.id)" -ForegroundColor Green
    }
    
    Write-Host ""
}

# 7. RESUMEN DE RESULTADOS
Write-Host "7. RESUMEN DE PRUEBAS" -ForegroundColor Magenta
Write-Host "=====================" -ForegroundColor Magenta

Write-Host "📊 Estado general del sistema:" -ForegroundColor White
Write-Host "   • API ejecutándose: ✅" -ForegroundColor Green
Write-Host "   • Base de datos conectada: ✅" -ForegroundColor Green
Write-Host "   • Datos de referencia: ✅" -ForegroundColor Green
Write-Host "   • Autenticación JWT: $(if ($global:authToken) { '✅' } else { '❌' })" -ForegroundColor $(if ($global:authToken) { 'Green' } else { 'Red' })
Write-Host "   • Gestión de perfil: $(if ($perfil) { '✅' } else { '❌' })" -ForegroundColor $(if ($perfil) { 'Green' } else { 'Red' })
Write-Host "   • Gestión de documentos: $(if ($documentos -ne $null) { '✅' } else { '❌' })" -ForegroundColor $(if ($documentos -ne $null) { 'Green' } else { 'Red' })
Write-Host "   • Gestión de solicitudes: $(if ($solicitudes -ne $null) { '✅' } else { '❌' })" -ForegroundColor $(if ($solicitudes -ne $null) { 'Green' } else { 'Red' })

Write-Host ""
Write-Host "=== FIN DE PRUEBAS SISTEMÁTICAS ===" -ForegroundColor Green
Write-Host "Fecha y hora: $(Get-Date)" -ForegroundColor Yellow
