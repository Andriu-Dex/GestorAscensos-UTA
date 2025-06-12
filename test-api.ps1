# Script de Pruebas Automatizadas para el Sistema de Gestión de Ascensos UTA
# Ejecutar con: .\test-api.ps1

param(
    [string]$BaseUrl = "http://localhost:5115/api",
    [switch]$Verbose
)

Write-Host "🚀 Iniciando pruebas del Sistema de Gestión de Ascensos UTA" -ForegroundColor Cyan
Write-Host "Base URL: $BaseUrl" -ForegroundColor Yellow

# Variables globales
$Global:Token = ""
$Global:DocenteId = 0
$Global:DocumentoId = 0
$Global:SolicitudId = 0

# Función para hacer peticiones HTTP
function Invoke-ApiRequest {
    param(
        [string]$Method,
        [string]$Endpoint,
        [hashtable]$Body = $null,
        [bool]$RequireAuth = $false,
        [string]$ContentType = "application/json"
    )
    
    $uri = "$BaseUrl$Endpoint"
    $headers = @{
        "Content-Type" = $ContentType
    }
    
    if ($RequireAuth -and $Global:Token) {
        $headers["Authorization"] = "Bearer $Global:Token"
    }
    
    try {
        $params = @{
            Uri = $uri
            Method = $Method
            Headers = $headers
        }
        
        if ($Body) {
            if ($ContentType -eq "application/json") {
                $params.Body = $Body | ConvertTo-Json -Depth 10
            } else {
                $params.Body = $Body
            }
        }
        
        if ($Verbose) {
            Write-Host "🔍 $Method $uri" -ForegroundColor Gray
        }
        
        $response = Invoke-RestMethod @params
        Write-Host "✅ $Method $Endpoint - SUCCESS" -ForegroundColor Green
        return $response
    }
    catch {
        Write-Host "❌ $Method $Endpoint - ERROR: $($_.Exception.Message)" -ForegroundColor Red
        if ($_.Exception.Response) {
            $errorDetail = $_.Exception.Response | ConvertFrom-Json -ErrorAction SilentlyContinue
            if ($errorDetail) {
                Write-Host "   Detalle: $($errorDetail.message)" -ForegroundColor Red
            }
        }
        return $null
    }
}

# Test 1: Verificar que la API esté funcionando
Write-Host "`n📋 Test 1: Verificando que la API esté funcionando..." -ForegroundColor Cyan
$facultades = Invoke-ApiRequest -Method "GET" -Endpoint "/Facultad"
if ($facultades) {
    Write-Host "   ✅ API respondiendo correctamente. Facultades encontradas: $($facultades.Count)" -ForegroundColor Green
} else {
    Write-Host "   ❌ La API no está respondiendo. Verifica que esté ejecutándose en $BaseUrl" -ForegroundColor Red
    exit 1
}

# Test 2: Crear datos de referencia
Write-Host "`n📋 Test 2: Creando datos de referencia..." -ForegroundColor Cyan

# Crear facultad de prueba
$nuevaFacultad = @{
    codigo = "FIEI_TEST"
    nombre = "Facultad de Pruebas Técnicas"
    descripcion = "Facultad creada para pruebas automatizadas"
    color = "#FF5733"
    esActiva = $true
}
$facultadCreada = Invoke-ApiRequest -Method "POST" -Endpoint "/Facultad" -Body $nuevaFacultad

# Crear tipo de documento de prueba
$nuevoTipoDoc = @{
    codigo = "TEST_DOC"
    nombre = "Documento de Prueba"
    descripcion = "Tipo de documento para pruebas automatizadas"
    requiereValidacion = $true
    formatoEsperado = "PDF"
    tamanoMaximoMB = 5
    esActivo = $true
}
$tipoDocCreado = Invoke-ApiRequest -Method "POST" -Endpoint "/TipoDocumento" -Body $nuevoTipoDoc

# Test 3: Registro de usuario
Write-Host "`n📋 Test 3: Registrando usuario de prueba..." -ForegroundColor Cyan
$nuevoDocente = @{
    cedula = "9999999999"
    nombres = "Usuario"
    apellidos = "De Prueba"
    email = "prueba@uta.edu.ec"
    telefonoContacto = "0999999999"
    facultadId = 1
    nombreUsuario = "usuarioprueba"
    password = "Prueba123!"
}
$docenteRegistrado = Invoke-ApiRequest -Method "POST" -Endpoint "/Auth/register" -Body $nuevoDocente

# Test 4: Login
Write-Host "`n📋 Test 4: Iniciando sesión..." -ForegroundColor Cyan
$loginData = @{
    nombreUsuario = "usuarioprueba"
    password = "Prueba123!"
}
$loginResponse = Invoke-ApiRequest -Method "POST" -Endpoint "/Auth/login" -Body $loginData
if ($loginResponse -and $loginResponse.token) {
    $Global:Token = $loginResponse.token
    Write-Host "   ✅ Token obtenido exitosamente" -ForegroundColor Green
} else {
    Write-Host "   ❌ No se pudo obtener el token de autenticación" -ForegroundColor Red
}

# Test 5: Obtener perfil
Write-Host "`n📋 Test 5: Obteniendo perfil del usuario..." -ForegroundColor Cyan
$perfil = Invoke-ApiRequest -Method "GET" -Endpoint "/Docente/perfil" -RequireAuth $true
if ($perfil) {
    $Global:DocenteId = $perfil.id
    Write-Host "   ✅ Perfil obtenido. Docente ID: $($Global:DocenteId)" -ForegroundColor Green
}

# Test 6: Actualizar indicadores
Write-Host "`n📋 Test 6: Actualizando indicadores del docente..." -ForegroundColor Cyan
$indicadores = @{
    tiempoRol = 5
    numeroObras = 3
    puntajeEvaluacion = 85.5
    horasCapacitacion = 150
    tiempoInvestigacion = 24
}
$indicadoresActualizados = Invoke-ApiRequest -Method "PUT" -Endpoint "/Docente/indicadores" -Body $indicadores -RequireAuth $true

# Test 7: Validar requisitos
Write-Host "`n📋 Test 7: Validando requisitos para ascenso..." -ForegroundColor Cyan
$requisitos = Invoke-ApiRequest -Method "GET" -Endpoint "/Docente/validar-requisitos" -RequireAuth $true
if ($requisitos) {
    Write-Host "   ✅ Requisitos validados:" -ForegroundColor Green
    $requisitos.PSObject.Properties | ForEach-Object {
        $status = if ($_.Value) { "✅" } else { "❌" }
        Write-Host "      $status $($_.Name): $($_.Value)" -ForegroundColor $(if ($_.Value) { "Green" } else { "Red" })
    }
}

# Test 8: Obtener documentos
Write-Host "`n📋 Test 8: Obteniendo documentos del docente..." -ForegroundColor Cyan
$documentos = Invoke-ApiRequest -Method "GET" -Endpoint "/Documento/docente" -RequireAuth $true

# Test 9: Crear solicitud de ascenso (si hay documentos)
Write-Host "`n📋 Test 9: Creando solicitud de ascenso..." -ForegroundColor Cyan
$solicitudData = @{
    documentosIds = @() # Array vacío por ahora, ya que no hemos subido documentos
}
$solicitudCreada = Invoke-ApiRequest -Method "POST" -Endpoint "/SolicitudAscenso" -Body $solicitudData -RequireAuth $true

# Test 10: Obtener solicitudes del docente
Write-Host "`n📋 Test 10: Obteniendo solicitudes del docente..." -ForegroundColor Cyan
$solicitudes = Invoke-ApiRequest -Method "GET" -Endpoint "/SolicitudAscenso/docente" -RequireAuth $true

# Test 11: Pruebas de validación (casos de error esperados)
Write-Host "`n📋 Test 11: Probando validaciones (errores esperados)..." -ForegroundColor Cyan

# Login con credenciales incorrectas
Write-Host "   🔍 Probando login con credenciales incorrectas..." -ForegroundColor Gray
$loginIncorrecto = @{
    nombreUsuario = "usuario_inexistente"
    password = "password_incorrecto"
}
$loginError = Invoke-ApiRequest -Method "POST" -Endpoint "/Auth/login" -Body $loginIncorrecto

# Acceso sin autorización
Write-Host "   🔍 Probando acceso sin token de autorización..." -ForegroundColor Gray
$perfilSinAuth = Invoke-ApiRequest -Method "GET" -Endpoint "/Docente/perfil"

# Resumen final
Write-Host "`n📊 RESUMEN DE PRUEBAS" -ForegroundColor Cyan
Write-Host "===================" -ForegroundColor Cyan
Write-Host "✅ API Funcionando: $(if ($facultades) { 'SÍ' } else { 'NO' })" -ForegroundColor $(if ($facultades) { "Green" } else { "Red" })
Write-Host "✅ Registro de Usuario: $(if ($docenteRegistrado) { 'SÍ' } else { 'NO' })" -ForegroundColor $(if ($docenteRegistrado) { "Green" } else { "Red" })
Write-Host "✅ Autenticación: $(if ($Global:Token) { 'SÍ' } else { 'NO' })" -ForegroundColor $(if ($Global:Token) { "Green" } else { "Red" })
Write-Host "✅ Gestión de Perfil: $(if ($perfil) { 'SÍ' } else { 'NO' })" -ForegroundColor $(if ($perfil) { "Green" } else { "Red" })
Write-Host "✅ Validación de Requisitos: $(if ($requisitos) { 'SÍ' } else { 'NO' })" -ForegroundColor $(if ($requisitos) { "Green" } else { "Red" })

Write-Host "`n🎉 Pruebas completadas!" -ForegroundColor Cyan
Write-Host "💡 Para pruebas más detalladas, usa Swagger UI en: http://localhost:5115/swagger" -ForegroundColor Yellow
Write-Host "💡 Para subir documentos, usa Postman o Insomnia con FormData" -ForegroundColor Yellow
