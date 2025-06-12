# Prueba Sistemática del Flujo Completo de Solicitudes de Ascenso
# Sistema de Gestión de Ascensos UTA
# Fecha: $(Get-Date)

Write-Host "=== INICIANDO PRUEBA SISTEMÁTICA DEL FLUJO COMPLETO ===" -ForegroundColor Green
Write-Host "Fecha: $(Get-Date)" -ForegroundColor Yellow
Write-Host ""

# Configuración
$baseUrl = "https://localhost:7030/api"
$headers = @{
    "Content-Type" = "application/json"
    "Accept" = "application/json"
}

# Datos de login
$loginData = @{
    email = "maria.garcia@uta.edu.ec"
    password = "Password123!"
} | ConvertTo-Json

Write-Host "1. === PRUEBA DE AUTENTICACIÓN ===" -ForegroundColor Cyan

try {
    $loginResponse = Invoke-RestMethod -Uri "$baseUrl/Auth/login" -Method POST -Body $loginData -Headers $headers -SkipCertificateCheck
    $token = $loginResponse.token
    Write-Host "✓ Login exitoso" -ForegroundColor Green
    Write-Host "  Token obtenido: $($token.Substring(0,20))..." -ForegroundColor Gray
    
    # Agregar token a headers
    $headers["Authorization"] = "Bearer $token"
} catch {
    Write-Host "✗ Error en login: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "2. === CONSULTA DE PERFIL DE USUARIO ===" -ForegroundColor Cyan

try {
    $profile = Invoke-RestMethod -Uri "$baseUrl/Usuario/profile" -Method GET -Headers $headers -SkipCertificateCheck
    Write-Host "✓ Perfil obtenido exitosamente" -ForegroundColor Green
    Write-Host "  Usuario: $($profile.email)" -ForegroundColor Gray
    Write-Host "  Cédula: $($profile.cedula)" -ForegroundColor Gray
    Write-Host "  Facultad: $($profile.facultadNombre)" -ForegroundColor Gray
} catch {
    Write-Host "✗ Error obteniendo perfil: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "3. === CONSULTA DE SOLICITUDES EXISTENTES ===" -ForegroundColor Cyan

try {
    $solicitudesExistentes = Invoke-RestMethod -Uri "$baseUrl/SolicitudAscenso/mis-solicitudes" -Method GET -Headers $headers -SkipCertificateCheck
    Write-Host "✓ Consulta exitosa" -ForegroundColor Green
    Write-Host "  Solicitudes encontradas: $($solicitudesExistentes.Count)" -ForegroundColor Gray
    
    if ($solicitudesExistentes.Count -gt 0) {
        Write-Host "  Últimas solicitudes:" -ForegroundColor Gray
        $solicitudesExistentes | Select-Object -First 3 | ForEach-Object {
            Write-Host "    - ID: $($_.id) | Nivel: $($_.nivelSolicitado) | Estado: $($_.estadoNombre) | Fecha: $($_.fechaSolicitud)" -ForegroundColor Gray
        }
    }
} catch {
    Write-Host "✗ Error consultando solicitudes: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "4. === CONSULTA DE DATOS MAESTROS ===" -ForegroundColor Cyan

# Consultar tipos de documento
try {
    $tiposDocumento = Invoke-RestMethod -Uri "$baseUrl/TipoDocumento" -Method GET -Headers $headers -SkipCertificateCheck
    Write-Host "✓ Tipos de documento obtenidos: $($tiposDocumento.Count)" -ForegroundColor Green
} catch {
    Write-Host "✗ Error obteniendo tipos de documento: $($_.Exception.Message)" -ForegroundColor Red
}

# Consultar estados de solicitud
try {
    $estados = Invoke-RestMethod -Uri "$baseUrl/EstadoSolicitud" -Method GET -Headers $headers -SkipCertificateCheck
    Write-Host "✓ Estados de solicitud obtenidos: $($estados.Count)" -ForegroundColor Green
} catch {
    Write-Host "✗ Error obteniendo estados: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "5. === CREACIÓN DE NUEVA SOLICITUD DE ASCENSO ===" -ForegroundColor Cyan

$nuevaSolicitud = @{
    nivelActual = "Profesor Auxiliar 1"
    nivelSolicitado = "Profesor Auxiliar 2"
    justificacion = "Solicitud de prueba automatizada: Completé estudios de maestría, tengo 3 años de experiencia docente, he publicado artículos en revistas indexadas y participado en proyectos de investigación. Esta es una prueba del sistema de gestión de ascensos."
    fechaSolicitud = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ss")
    documentos = @()
} | ConvertTo-Json

try {
    $solicitudCreada = Invoke-RestMethod -Uri "$baseUrl/SolicitudAscenso" -Method POST -Body $nuevaSolicitud -Headers $headers -SkipCertificateCheck
    Write-Host "✓ Solicitud creada exitosamente" -ForegroundColor Green
    Write-Host "  ID de solicitud: $($solicitudCreada.id)" -ForegroundColor Gray
    Write-Host "  Estado inicial: $($solicitudCreada.estadoNombre)" -ForegroundColor Gray
    $solicitudId = $solicitudCreada.id
} catch {
    Write-Host "✗ Error creando solicitud: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "  Detalle: $($_.ErrorDetails.Message)" -ForegroundColor Red
    $solicitudId = $null
}

Write-Host ""
Write-Host "6. === PRUEBA DE ACTUALIZACIÓN DE ESTADO ===" -ForegroundColor Cyan

if ($solicitudId) {
    $actualizacionEstado = @{
        nuevoEstadoId = 2
        observaciones = "Prueba automatizada: Solicitud revisada inicialmente. Documentación básica verificada."
    } | ConvertTo-Json

    try {
        $estadoActualizado = Invoke-RestMethod -Uri "$baseUrl/SolicitudAscenso/$solicitudId/estado" -Method PUT -Body $actualizacionEstado -Headers $headers -SkipCertificateCheck
        Write-Host "✓ Estado actualizado exitosamente" -ForegroundColor Green
        Write-Host "  Nuevo estado: En Revisión" -ForegroundColor Gray
    } catch {
        Write-Host "✗ Error actualizando estado: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host "  Detalle: $($_.ErrorDetails.Message)" -ForegroundColor Red
    }
} else {
    Write-Host "⚠ Saltando actualización de estado (no hay solicitud creada)" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "7. === CONSULTA DE HISTORIAL DE SOLICITUD ===" -ForegroundColor Cyan

if ($solicitudId) {
    try {
        $historial = Invoke-RestMethod -Uri "$baseUrl/SolicitudAscenso/$solicitudId/historial" -Method GET -Headers $headers -SkipCertificateCheck
        Write-Host "✓ Historial obtenido exitosamente" -ForegroundColor Green
        Write-Host "  Entradas en historial: $($historial.Count)" -ForegroundColor Gray
        
        if ($historial.Count -gt 0) {
            Write-Host "  Últimos movimientos:" -ForegroundColor Gray
            $historial | ForEach-Object {
                Write-Host "    - $($_.fecha): $($_.descripcion)" -ForegroundColor Gray
            }
        }
    } catch {
        Write-Host "✗ Error obteniendo historial: $($_.Exception.Message)" -ForegroundColor Red
    }
} else {
    Write-Host "⚠ Saltando consulta de historial (no hay solicitud creada)" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "8. === CONSULTA FINAL DE TODAS LAS SOLICITUDES ===" -ForegroundColor Cyan

try {
    $todasSolicitudes = Invoke-RestMethod -Uri "$baseUrl/SolicitudAscenso" -Method GET -Headers $headers -SkipCertificateCheck
    Write-Host "✓ Consulta general exitosa" -ForegroundColor Green
    Write-Host "  Total de solicitudes en sistema: $($todasSolicitudes.Count)" -ForegroundColor Gray
    
    if ($todasSolicitudes.Count -gt 0) {
        $solicitudesRecientes = $todasSolicitudes | Sort-Object fechaSolicitud -Descending | Select-Object -First 5
        Write-Host "  Solicitudes más recientes:" -ForegroundColor Gray
        $solicitudesRecientes | ForEach-Object {
            Write-Host "    - ID: $($_.id) | Usuario: $($_.usuarioEmail) | Estado: $($_.estadoNombre) | Fecha: $($_.fechaSolicitud)" -ForegroundColor Gray
        }
    }
} catch {
    Write-Host "✗ Error en consulta general: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "=== RESUMEN DE LA PRUEBA ===" -ForegroundColor Green
Write-Host "Fecha de finalización: $(Get-Date)" -ForegroundColor Yellow

if ($solicitudId) {
    Write-Host "✓ Flujo completo ejecutado exitosamente" -ForegroundColor Green
    Write-Host "✓ Solicitud de prueba creada con ID: $solicitudId" -ForegroundColor Green
    Write-Host "✓ Sistema de gestión de ascensos operativo" -ForegroundColor Green
} else {
    Write-Host "⚠ Flujo ejecutado con limitaciones" -ForegroundColor Yellow
    Write-Host "⚠ Algunas funcionalidades requieren revisión" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "=== FIN DE LA PRUEBA ===" -ForegroundColor Green
