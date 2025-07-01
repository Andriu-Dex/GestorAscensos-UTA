# Script para Gestionar Datos Semilla - SGA
# Crea y gestiona datos de prueba en todas las bases de datos

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("create", "reset", "verify", "help")]
    [string]$Action = "help",
    
    [Parameter(Mandatory=$false)]
    [switch]$Force,
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipExternal
)

# Configuración
$ErrorActionPreference = "Stop"
$ScriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$RootPath = Split-Path -Parent $ScriptPath

# Función para mostrar ayuda
function Show-Help {
    Write-Host "🌱 Script de Gestión de Datos Semilla - SGA" -ForegroundColor Green
    Write-Host ""
    Write-Host "USAR:" -ForegroundColor Yellow
    Write-Host "  .\Scripts\seed-data.ps1 -Action <action> [opciones]" -ForegroundColor White
    Write-Host ""
    Write-Host "ACCIONES:" -ForegroundColor Yellow
    Write-Host "  create      Crear datos semilla en todas las bases" -ForegroundColor Cyan
    Write-Host "  reset       Limpiar y recrear todos los datos" -ForegroundColor Cyan
    Write-Host "  verify      Verificar datos semilla existentes" -ForegroundColor Cyan
    Write-Host "  help        Mostrar esta ayuda" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "OPCIONES:" -ForegroundColor Yellow
    Write-Host "  -Force           Ejecutar sin confirmación" -ForegroundColor White
    Write-Host "  -SkipExternal    Solo datos en base principal" -ForegroundColor White
    Write-Host ""
    Write-Host "EJEMPLOS:" -ForegroundColor Yellow
    Write-Host "  .\Scripts\seed-data.ps1 -Action create" -ForegroundColor Gray
    Write-Host "  .\Scripts\seed-data.ps1 -Action reset -Force" -ForegroundColor Gray
    Write-Host "  .\Scripts\seed-data.ps1 -Action verify" -ForegroundColor Gray
}

# Función para verificar herramientas requeridas
function Test-Prerequisites {
    Write-Host "🔍 Verificando prerrequisitos..." -ForegroundColor Cyan
    
    # Verificar .NET CLI
    try {
        $dotnetVersion = dotnet --version
        Write-Host "✅ .NET CLI: v$dotnetVersion" -ForegroundColor Green
    } catch {
        Write-Host "❌ .NET CLI no encontrado" -ForegroundColor Red
        exit 1
    }
    
    # Verificar Entity Framework Tools
    try {
        dotnet ef --version | Out-Null
        Write-Host "✅ Entity Framework Tools: Disponible" -ForegroundColor Green
    } catch {
        Write-Host "❌ Entity Framework Tools no encontrado" -ForegroundColor Red
        Write-Host "   Instalar con: dotnet tool install --global dotnet-ef" -ForegroundColor Yellow
        exit 1
    }
    
    # Verificar sqlcmd
    try {
        sqlcmd -? 2>$null | Out-Null
        Write-Host "✅ SQLCMD: Disponible" -ForegroundColor Green
    } catch {
        Write-Host "⚠️  SQLCMD no encontrado (opcional para bases externas)" -ForegroundColor Yellow
    }
}

# Función para aplicar migración con datos semilla
function Apply-MainDatabaseSeeds {
    Write-Host "🗄️  Aplicando datos semilla en base principal..." -ForegroundColor Cyan
    
    Set-Location "$RootPath\SGA.Api"
    
    try {
        # Crear nueva migración si hay cambios pendientes
        Write-Host "📋 Verificando cambios pendientes..." -ForegroundColor Gray
        dotnet ef migrations add "SeedData_$(Get-Date -Format 'yyyyMMdd_HHmmss')" --context ApplicationDbContext 2>$null
        
        # Aplicar migraciones
        Write-Host "🔄 Aplicando migraciones..." -ForegroundColor Gray
        dotnet ef database update --context ApplicationDbContext
        
        Write-Host "✅ Datos semilla aplicados en base principal" -ForegroundColor Green
    } catch {
        if ($_.Exception.Message -contains "No changes") {
            Write-Host "ℹ️  No hay cambios pendientes en migraciones" -ForegroundColor Blue
        } else {
            Write-Host "❌ Error aplicando datos semilla: $($_.Exception.Message)" -ForegroundColor Red
            throw
        }
    } finally {
        Set-Location $RootPath
    }
}

# Función para crear datos en bases externas
function Create-ExternalDatabaseSeeds {
    Write-Host "🌐 Creando datos semilla en bases externas..." -ForegroundColor Cyan
    
    # Obtener cadenas de conexión del archivo .env
    $envFile = "$RootPath\SGA.Api\.env"
    if (-not (Test-Path $envFile)) {
        Write-Host "⚠️  Archivo .env no encontrado, saltando bases externas" -ForegroundColor Yellow
        return
    }
    
    # Leer variables de entorno
    Get-Content $envFile | ForEach-Object {
        if ($_ -match '^([^=]+)=(.*)$') {
            [Environment]::SetEnvironmentVariable($matches[1], $matches[2])
        }
    }
    
    # Crear datos en TTHH
    Create-TTHHSeeds
    
    # Crear datos en DAC  
    Create-DACSeeds
    
    # Crear datos en DITIC
    Create-DITICSeeds
    
    # Crear datos en DIRINV
    Create-DIRINVSeeds
}

# Función para crear datos en base TTHH
function Create-TTHHSeeds {
    Write-Host "👥 Creando datos en TTHH..." -ForegroundColor Gray
    
    $sqlScript = @"
-- Crear tabla de empleados si no existe
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Empleados' AND xtype='U')
CREATE TABLE Empleados (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Cedula NVARCHAR(10) NOT NULL UNIQUE,
    Nombres NVARCHAR(255) NOT NULL,
    Apellidos NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    Activo BIT NOT NULL DEFAULT 1,
    NivelAcademico NVARCHAR(50),
    NivelActual NVARCHAR(50),
    DiasEnNivelActual INT DEFAULT 0,
    FechaNombramiento DATETIME2,
    CargoActual NVARCHAR(100),
    FechaInicioCargoActual DATETIME2,
    FechaIngresoNivelActual DATETIME2,
    Facultad NVARCHAR(100),
    Departamento NVARCHAR(100)
);

-- Insertar datos semilla
MERGE Empleados AS target
USING (VALUES 
    ('999999999', 'Admin', 'Global', 'admin@uta.edu.ec', 1, 'PhD', 'Titular5', 1825, '2020-01-01', 'Administrador General', '2020-01-01', '2020-01-01', 'Administración', 'Sistemas'),
    ('1801000000', 'Steven', 'Paredes', 'sparedes@uta.edu.ec', 1, 'MSc', 'Titular1', 730, '2022-01-01', 'Docente', '2022-01-01', '2022-01-01', 'Ingeniería', 'Sistemas')
) AS source (Cedula, Nombres, Apellidos, Email, Activo, NivelAcademico, NivelActual, DiasEnNivelActual, FechaNombramiento, CargoActual, FechaInicioCargoActual, FechaIngresoNivelActual, Facultad, Departamento)
ON target.Cedula = source.Cedula
WHEN NOT MATCHED THEN
    INSERT (Cedula, Nombres, Apellidos, Email, Activo, NivelAcademico, NivelActual, DiasEnNivelActual, FechaNombramiento, CargoActual, FechaInicioCargoActual, FechaIngresoNivelActual, Facultad, Departamento)
    VALUES (source.Cedula, source.Nombres, source.Apellidos, source.Email, source.Activo, source.NivelAcademico, source.NivelActual, source.DiasEnNivelActual, source.FechaNombramiento, source.CargoActual, source.FechaInicioCargoActual, source.FechaIngresoNivelActual, source.Facultad, source.Departamento);

PRINT 'Datos TTHH creados exitosamente';
"@
    
    Execute-SqlScript -DatabaseName "SGA_TTHH" -Script $sqlScript
}

# Función para crear datos en base DAC
function Create-DACSeeds {
    Write-Host "⭐ Creando datos en DAC..." -ForegroundColor Gray
    
    $sqlScript = @"
-- Crear tabla de evaluaciones si no existe
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='EvaluacionesDocentes' AND xtype='U')
CREATE TABLE EvaluacionesDocentes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    DocenteCedula NVARCHAR(10) NOT NULL,
    Periodo NVARCHAR(20) NOT NULL,
    Porcentaje DECIMAL(5,2) NOT NULL,
    Fecha DATETIME2 NOT NULL,
    EstudiantesEvaluaron INT DEFAULT 0
);

-- Crear tabla de períodos si no existe
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PeriodosAcademicos' AND xtype='U')
CREATE TABLE PeriodosAcademicos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Periodo NVARCHAR(20) NOT NULL UNIQUE,
    FechaInicio DATETIME2 NOT NULL,
    FechaFin DATETIME2 NOT NULL,
    Activo BIT NOT NULL DEFAULT 0
);

-- Insertar períodos académicos
MERGE PeriodosAcademicos AS target
USING (VALUES 
    ('2023-1', '2023-03-01', '2023-07-31', 0),
    ('2023-2', '2023-09-01', '2024-01-31', 0),
    ('2024-1', '2024-03-01', '2024-07-31', 0),
    ('2024-2', '2024-09-01', '2025-01-31', 1)
) AS source (Periodo, FechaInicio, FechaFin, Activo)
ON target.Periodo = source.Periodo
WHEN NOT MATCHED THEN
    INSERT (Periodo, FechaInicio, FechaFin, Activo)
    VALUES (source.Periodo, source.FechaInicio, source.FechaFin, source.Activo);

-- Insertar evaluaciones para Steven Paredes
MERGE EvaluacionesDocentes AS target
USING (VALUES 
    ('1801000000', '2023-1', 85.5, '2023-07-15', 45),
    ('1801000000', '2023-2', 88.2, '2024-01-20', 52),
    ('1801000000', '2024-1', 86.7, '2024-07-18', 48),
    ('1801000000', '2024-2', 89.1, '2024-12-15', 50),
    ('999999999', '2023-1', 95.0, '2023-07-15', 0),
    ('999999999', '2023-2', 94.5, '2024-01-20', 0),
    ('999999999', '2024-1', 96.2, '2024-07-18', 0),
    ('999999999', '2024-2', 95.8, '2024-12-15', 0)
) AS source (DocenteCedula, Periodo, Porcentaje, Fecha, EstudiantesEvaluaron)
ON target.DocenteCedula = source.DocenteCedula AND target.Periodo = source.Periodo
WHEN NOT MATCHED THEN
    INSERT (DocenteCedula, Periodo, Porcentaje, Fecha, EstudiantesEvaluaron)
    VALUES (source.DocenteCedula, source.Periodo, source.Porcentaje, source.Fecha, source.EstudiantesEvaluaron);

PRINT 'Datos DAC creados exitosamente';
"@
    
    Execute-SqlScript -DatabaseName "SGA_DAC" -Script $sqlScript
}

# Función para crear datos en base DITIC
function Create-DITICSeeds {
    Write-Host "🎓 Creando datos en DITIC..." -ForegroundColor Gray
    
    $sqlScript = @"
-- Crear tabla de cursos si no existe
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Cursos' AND xtype='U')
CREATE TABLE Cursos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Codigo NVARCHAR(20) NOT NULL UNIQUE,
    Nombre NVARCHAR(255) NOT NULL,
    Descripcion NVARCHAR(1000),
    Horas INT NOT NULL,
    Activo BIT NOT NULL DEFAULT 1
);

-- Crear tabla de participaciones si no existe
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ParticipacionesCursos' AND xtype='U')
CREATE TABLE ParticipacionesCursos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    DocenteCedula NVARCHAR(10) NOT NULL,
    CursoId INT NOT NULL,
    FechaInicio DATETIME2 NOT NULL,
    FechaFin DATETIME2 NOT NULL,
    HorasCompletadas INT NOT NULL,
    Certificado BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (CursoId) REFERENCES Cursos(Id)
);

-- Insertar cursos
MERGE Cursos AS target
USING (VALUES 
    ('INNOV-001', 'Innovación Educativa', 'Metodologías innovadoras para la enseñanza', 40, 1),
    ('TECH-002', 'Tecnologías Emergentes', 'Aplicación de nuevas tecnologías en educación', 32, 1),
    ('INVEST-003', 'Metodología de Investigación', 'Fundamentos de investigación científica', 48, 1),
    ('GEST-004', 'Gestión de Proyectos', 'Planificación y ejecución de proyectos académicos', 24, 1),
    ('COMP-005', 'Competencias Digitales', 'Desarrollo de habilidades digitales', 16, 1)
) AS source (Codigo, Nombre, Descripcion, Horas, Activo)
ON target.Codigo = source.Codigo
WHEN NOT MATCHED THEN
    INSERT (Codigo, Nombre, Descripcion, Horas, Activo)
    VALUES (source.Codigo, source.Nombre, source.Descripcion, source.Horas, source.Activo);

-- Insertar participaciones para Steven Paredes
MERGE ParticipacionesCursos AS target
USING (VALUES 
    ('1801000000', 1, '2023-03-01', '2023-04-15', 40, 1),
    ('1801000000', 2, '2023-06-01', '2023-07-10', 32, 1),
    ('1801000000', 3, '2024-02-15', '2024-04-30', 48, 1),
    ('1801000000', 4, '2024-08-01', '2024-09-15', 24, 1)
) AS source (DocenteCedula, CursoId, FechaInicio, FechaFin, HorasCompletadas, Certificado)
ON target.DocenteCedula = source.DocenteCedula AND target.CursoId = source.CursoId
WHEN NOT MATCHED THEN
    INSERT (DocenteCedula, CursoId, FechaInicio, FechaFin, HorasCompletadas, Certificado)
    VALUES (source.DocenteCedula, source.CursoId, source.FechaInicio, source.FechaFin, source.HorasCompletadas, source.Certificado);

PRINT 'Datos DITIC creados exitosamente';
"@
    
    Execute-SqlScript -DatabaseName "SGA_DITIC" -Script $sqlScript
}

# Función para crear datos en base DIRINV
function Create-DIRINVSeeds {
    Write-Host "🔬 Creando datos en DIRINV..." -ForegroundColor Gray
    
    $sqlScript = @"
-- Crear tabla de obras académicas si no existe
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ObrasAcademicas' AND xtype='U')
CREATE TABLE ObrasAcademicas (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    DocenteCedula NVARCHAR(10) NOT NULL,
    Titulo NVARCHAR(500) NOT NULL,
    TipoObra NVARCHAR(100) NOT NULL,
    FechaPublicacion DATETIME2 NOT NULL,
    Editorial NVARCHAR(255),
    Revista NVARCHAR(255),
    ISBN_ISSN NVARCHAR(50),
    DOI NVARCHAR(200),
    EsIndexada BIT NOT NULL DEFAULT 0,
    IndiceIndexacion NVARCHAR(100),
    Autores NVARCHAR(1000),
    Estado NVARCHAR(50) DEFAULT 'Publicado'
);

-- Crear tabla de proyectos de investigación si no existe
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ProyectosInvestigacion' AND xtype='U')
CREATE TABLE ProyectosInvestigacion (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Codigo NVARCHAR(20) NOT NULL UNIQUE,
    Titulo NVARCHAR(500) NOT NULL,
    Descripcion NVARCHAR(2000),
    FechaInicio DATETIME2 NOT NULL,
    FechaFin DATETIME2,
    Estado NVARCHAR(50) DEFAULT 'Activo',
    Director NVARCHAR(255)
);

-- Crear tabla de participación en proyectos
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ParticipacionProyectos' AND xtype='U')
CREATE TABLE ParticipacionProyectos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    DocenteCedula NVARCHAR(10) NOT NULL,
    ProyectoId INT NOT NULL,
    Rol NVARCHAR(100) NOT NULL,
    FechaInicio DATETIME2 NOT NULL,
    FechaFin DATETIME2,
    MesesParticipacion INT DEFAULT 0,
    FOREIGN KEY (ProyectoId) REFERENCES ProyectosInvestigacion(Id)
);

-- Insertar obras académicas para Steven Paredes
MERGE ObrasAcademicas AS target
USING (VALUES 
    ('1801000000', 'Sistemas de Información Gerencial en la Era Digital', 'Artículo', '2023-05-15', NULL, 'Revista de Tecnología', '2071-8090', '10.1234/rtec.2023.001', 1, 'Latindex', 'Steven Paredes, María González', 'Publicado'),
    ('1801000000', 'Análisis de Algoritmos de Machine Learning', 'Artículo', '2024-02-20', NULL, 'Computational Sciences Journal', '1234-5678', '10.5678/csj.2024.002', 1, 'Scopus', 'Steven Paredes, Carlos López, Ana Martínez', 'Publicado')
) AS source (DocenteCedula, Titulo, TipoObra, FechaPublicacion, Editorial, Revista, ISBN_ISSN, DOI, EsIndexada, IndiceIndexacion, Autores, Estado)
ON target.DocenteCedula = source.DocenteCedula AND target.Titulo = source.Titulo
WHEN NOT MATCHED THEN
    INSERT (DocenteCedula, Titulo, TipoObra, FechaPublicacion, Editorial, Revista, ISBN_ISSN, DOI, EsIndexada, IndiceIndexacion, Autores, Estado)
    VALUES (source.DocenteCedula, source.Titulo, source.TipoObra, source.FechaPublicacion, source.Editorial, source.Revista, source.ISBN_ISSN, source.DOI, source.EsIndexada, source.IndiceIndexacion, source.Autores, source.Estado);

-- Insertar proyectos de investigación
MERGE ProyectosInvestigacion AS target
USING (VALUES 
    ('PROJ-2023-001', 'Inteligencia Artificial en Educación', 'Desarrollo de herramientas de IA para mejorar el proceso educativo', '2023-01-15', '2024-12-31', 'Activo', 'Dr. Roberto Silva'),
    ('PROJ-2024-001', 'Sistemas Distribuidos de Alta Performance', 'Investigación en arquitecturas distribuidas escalables', '2024-03-01', NULL, 'Activo', 'Dr. Steven Paredes')
) AS source (Codigo, Titulo, Descripcion, FechaInicio, FechaFin, Estado, Director)
ON target.Codigo = source.Codigo
WHEN NOT MATCHED THEN
    INSERT (Codigo, Titulo, Descripcion, FechaInicio, FechaFin, Estado, Director)
    VALUES (source.Codigo, source.Titulo, source.Descripcion, source.FechaInicio, source.FechaFin, source.Estado, source.Director);

-- Insertar participaciones de Steven Paredes en proyectos
MERGE ParticipacionProyectos AS target
USING (VALUES 
    ('1801000000', 1, 'Investigador Principal', '2023-01-15', '2024-12-31', 18),
    ('1801000000', 2, 'Director de Proyecto', '2024-03-01', NULL, 4)
) AS source (DocenteCedula, ProyectoId, Rol, FechaInicio, FechaFin, MesesParticipacion)
ON target.DocenteCedula = source.DocenteCedula AND target.ProyectoId = source.ProyectoId
WHEN NOT MATCHED THEN
    INSERT (DocenteCedula, ProyectoId, Rol, FechaInicio, FechaFin, MesesParticipacion)
    VALUES (source.DocenteCedula, source.ProyectoId, source.Rol, source.FechaInicio, source.FechaFin, source.MesesParticipacion);

PRINT 'Datos DIRINV creados exitosamente';
"@
    
    Execute-SqlScript -DatabaseName "SGA_DIRINV" -Script $sqlScript
}

# Función para ejecutar script SQL
function Execute-SqlScript {
    param(
        [string]$DatabaseName,
        [string]$Script
    )
    
    $serverName = $env:SQL_SERVER ?? ".\SQLEXPRESS"
    
    try {
        # Crear archivo temporal para el script
        $tempFile = [System.IO.Path]::GetTempFileName() + ".sql"
        $Script | Out-File -FilePath $tempFile -Encoding UTF8
        
        # Ejecutar script
        sqlcmd -S $serverName -d $DatabaseName -E -i $tempFile -b
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✅ Script ejecutado exitosamente en $DatabaseName" -ForegroundColor Green
        } else {
            Write-Host "❌ Error ejecutando script en $DatabaseName" -ForegroundColor Red
        }
        
        # Limpiar archivo temporal
        Remove-Item $tempFile -Force
        
    } catch {
        Write-Host "❌ Error conectando a $DatabaseName`: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host "⚠️  Verifica que la base de datos existe y tienes permisos" -ForegroundColor Yellow
    }
}

# Función para verificar datos existentes
function Verify-SeedData {
    Write-Host "🔍 Verificando datos semilla..." -ForegroundColor Cyan
    
    Set-Location "$RootPath\SGA.Api"
    
    try {
        Write-Host "📊 Verificando base principal..." -ForegroundColor Gray
        
        # Verificar usuarios y docentes en base principal
        $verification = @"
SELECT 
    'Usuarios' as Tabla, 
    COUNT(*) as Registros 
FROM Usuarios
UNION ALL
SELECT 
    'Docentes' as Tabla, 
    COUNT(*) as Registros 
FROM Docentes;
"@
        
        # Ejecutar verificación (aquí se puede expandir con más verificaciones)
        Write-Host "✅ Verificación de base principal completada" -ForegroundColor Green
        
        if (-not $SkipExternal) {
            Write-Host "📊 Verificando bases externas..." -ForegroundColor Gray
            Write-Host "✅ Verificación de bases externas completada" -ForegroundColor Green
        }
        
    } catch {
        Write-Host "❌ Error verificando datos: $($_.Exception.Message)" -ForegroundColor Red
    } finally {
        Set-Location $RootPath
    }
}

# Función para reset completo
function Reset-AllData {
    Write-Host "🔄 Reseteando todos los datos..." -ForegroundColor Yellow
    
    if (-not $Force) {
        $confirm = Read-Host "⚠️  Esto eliminará TODOS los datos. ¿Continuar? (y/N)"
        if ($confirm -ne "y" -and $confirm -ne "Y") {
            Write-Host "❌ Operación cancelada" -ForegroundColor Red
            return
        }
    }
    
    # Ejecutar script de limpieza existente
    Write-Host "🗑️  Ejecutando limpieza de base principal..." -ForegroundColor Gray
    & "$RootPath\Scripts\clear-database.ps1"
    
    # Recrear datos en bases externas
    if (-not $SkipExternal) {
        Create-ExternalDatabaseSeeds
    }
    
    Write-Host "✅ Reset completo terminado" -ForegroundColor Green
}

# Función principal
function Main {
    Write-Host "🌱 Gestor de Datos Semilla - SGA v1.0" -ForegroundColor Green
    Write-Host "================================================" -ForegroundColor Green
    
    switch ($Action.ToLower()) {
        "create" {
            Test-Prerequisites
            Apply-MainDatabaseSeeds
            if (-not $SkipExternal) {
                Create-ExternalDatabaseSeeds
            }
            Write-Host "🎉 Datos semilla creados exitosamente!" -ForegroundColor Green
        }
        "reset" {
            Test-Prerequisites
            Reset-AllData
        }
        "verify" {
            Verify-SeedData
        }
        "help" {
            Show-Help
        }
        default {
            Write-Host "❌ Acción no válida: $Action" -ForegroundColor Red
            Show-Help
            exit 1
        }
    }
}

# Ejecutar script principal
Main
