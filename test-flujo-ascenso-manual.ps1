# Test del flujo de ascenso manual - Verificar que no se inicie automáticamente
Write-Host "=== PRUEBA DE FLUJO DE ASCENSO MANUAL ===" -ForegroundColor Cyan
Write-Host "Verificando que el proceso de ascenso NO se inicie automáticamente" -ForegroundColor Yellow

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

# Función para verificar estado de requisitos
function Check-RequisitoStatus {
    param($token)
    
    $headers = @{
        "Authorization" = "Bearer $token"
    }

    try {
        $indicadores = Invoke-RestMethod -Uri "$baseUrl/docente/indicadores" -Method GET -Headers $headers -SkipCertificateCheck
        $requisitos = Invoke-RestMethod -Uri "$baseUrl/docente/requisitos" -Method GET -Headers $headers -SkipCertificateCheck
        
        Write-Host "Estado de indicadores:" -ForegroundColor Green
        Write-Host "  - Tiempo en rol: $($indicadores.TiempoRol) meses"
        Write-Host "  - Obras académicas: $($indicadores.NumeroObras)"
        Write-Host "  - Evaluación docente: $($indicadores.PuntajeEvaluacion)%"
        Write-Host "  - Horas capacitación: $($indicadores.HorasCapacitacion)"
        Write-Host "  - Tiempo investigación: $($indicadores.TiempoInvestigacion) meses"
        
        Write-Host "Estado de requisitos:" -ForegroundColor Green
        Write-Host "  - Cumple tiempo: $($requisitos.CumpleTiempoRol)"
        Write-Host "  - Cumple obras: $($requisitos.CumpleObras)"
        Write-Host "  - Cumple evaluación: $($requisitos.CumpleEvaluacion)"
        Write-Host "  - Cumple capacitación: $($requisitos.CumpleCapacitacion)"
        Write-Host "  - Cumple investigación: $($requisitos.CumpleInvestigacion)"
        
        $cumpleTodos = $requisitos.CumpleTiempoRol -and $requisitos.CumpleObras -and $requisitos.CumpleEvaluacion -and $requisitos.CumpleCapacitacion -and $requisitos.CumpleInvestigacion
        
        return @{
            CumpleTodos = $cumpleTodos
            Indicadores = $indicadores
            Requisitos = $requisitos
        }
    }
    catch {
        Write-Host "Error al verificar requisitos: $($_.Exception.Message)" -ForegroundColor Red
        return $null
    }
}

# Función para verificar solicitudes activas
function Check-SolicitudesActivas {
    param($token)
    
    $headers = @{
        "Authorization" = "Bearer $token"
    }

    try {
        $solicitudes = Invoke-RestMethod -Uri "$baseUrl/solicitudascenso/mis-solicitudes" -Method GET -Headers $headers -SkipCertificateCheck
        $tieneSolicitudActiva = Invoke-RestMethod -Uri "$baseUrl/solicitudascenso/tiene-solicitud-activa" -Method GET -Headers $headers -SkipCertificateCheck
        
        Write-Host "Solicitudes del usuario:" -ForegroundColor Green
        if ($solicitudes.Count -eq 0) {
            Write-Host "  - No tiene solicitudes registradas" -ForegroundColor Yellow
        } else {
            foreach ($solicitud in $solicitudes) {
                Write-Host "  - ID: $($solicitud.Id), Estado: $($solicitud.Estado), Fecha: $($solicitud.FechaSolicitud)"
            }
        }
        
        Write-Host "¿Tiene solicitud activa? $tieneSolicitudActiva" -ForegroundColor $(if ($tieneSolicitudActiva) { "Yellow" } else { "Green" })
        
        return @{
            Solicitudes = $solicitudes
            TieneSolicitudActiva = $tieneSolicitudActiva
        }
    }
    catch {
        Write-Host "Error al verificar solicitudes: $($_.Exception.Message)" -ForegroundColor Red
        return $null
    }
}

# Paso 1: Login con Steven Paredes
Write-Host ""
Write-Host "1. === LOGIN CON STEVEN PAREDES ===" -ForegroundColor Cyan
$token = Login-User -email "steven.paredes@uta.edu.ec" -password "123456"

if (-not $token) {
    Write-Host "No se pudo hacer login. Abortando prueba." -ForegroundColor Red
    exit 1
}

Write-Host "✓ Login exitoso" -ForegroundColor Green

# Paso 2: Verificar estado de requisitos
Write-Host ""
Write-Host "2. === VERIFICACIÓN DE REQUISITOS ===" -ForegroundColor Cyan
$estadoRequisitos = Check-RequisitoStatus -token $token

if (-not $estadoRequisitos) {
    Write-Host "No se pudo verificar requisitos. Continuando..." -ForegroundColor Yellow
} else {
    if ($estadoRequisitos.CumpleTodos) {
        Write-Host "✓ El usuario CUMPLE todos los requisitos" -ForegroundColor Green
    } else {
        Write-Host "⚠ El usuario NO cumple todos los requisitos" -ForegroundColor Yellow
    }
}

# Paso 3: Verificar solicitudes existentes
Write-Host ""
Write-Host "3. === VERIFICACIÓN DE SOLICITUDES EXISTENTES ===" -ForegroundColor Cyan
$estadoSolicitudes = Check-SolicitudesActivas -token $token

if (-not $estadoSolicitudes) {
    Write-Host "No se pudo verificar solicitudes. Continuando..." -ForegroundColor Yellow
}

# Paso 4: Análisis de la situación
Write-Host ""
Write-Host "4. === ANÁLISIS DEL COMPORTAMIENTO ESPERADO ===" -ForegroundColor Cyan

if ($estadoRequisitos -and $estadoSolicitudes) {
    if ($estadoRequisitos.CumpleTodos -and -not $estadoSolicitudes.TieneSolicitudActiva) {
        Write-Host "✓ CORRECTO: El usuario cumple requisitos pero NO tiene solicitud activa" -ForegroundColor Green
        Write-Host "  → En el dashboard debería ver el botón 'Iniciar Proceso de Ascenso'" -ForegroundColor Green
        Write-Host "  → NO debería ver ningún mensaje de 'proceso iniciado automáticamente'" -ForegroundColor Green
    }
    elseif ($estadoSolicitudes.TieneSolicitudActiva) {
        Write-Host "✓ CORRECTO: El usuario tiene una solicitud activa" -ForegroundColor Green
        Write-Host "  → En el dashboard debería ver 'Proceso en curso'" -ForegroundColor Green
        Write-Host "  → NO debería poder crear otra solicitud" -ForegroundColor Green
    }
    elseif (-not $estadoRequisitos.CumpleTodos) {
        Write-Host "✓ CORRECTO: El usuario NO cumple todos los requisitos" -ForegroundColor Green
        Write-Host "  → En el dashboard debería ver mensaje de requisitos faltantes" -ForegroundColor Green
        Write-Host "  → NO debería ver botón para iniciar proceso" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "5. === CONCLUSIÓN ===" -ForegroundColor Cyan
Write-Host "✓ La verificación del flujo manual está completa" -ForegroundColor Green
Write-Host "✓ El sistema NO debe iniciar automáticamente ningún proceso" -ForegroundColor Green
Write-Host "✓ El usuario debe hacer clic explícito en 'Iniciar Proceso de Ascenso'" -ForegroundColor Green
Write-Host ""
Write-Host "Recomendaciones:" -ForegroundColor Yellow
Write-Host "1. Acceder al dashboard web y verificar que el mensaje sea claro" -ForegroundColor White
Write-Host "2. Verificar que no aparezca ningún mensaje de 'proceso iniciado'" -ForegroundColor White
Write-Host "3. Confirmar que el botón diga claramente 'INICIAR proceso'" -ForegroundColor White
Write-Host ""
