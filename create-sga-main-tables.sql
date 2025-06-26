-- Script para crear tablas en SGA_Main
USE SGA_Main;

-- Tabla Usuarios
CREATE TABLE Usuarios (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    Rol NVARCHAR(50) NOT NULL,
    EstaActivo BIT NOT NULL DEFAULT 1,
    IntentosLogin INT NOT NULL DEFAULT 0,
    UltimoBloqueado DATETIME2 NULL,
    UltimoLogin DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FechaCreacion DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FechaModificacion DATETIME2 NULL
);

-- Tabla Docentes
CREATE TABLE Docentes (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Cedula NVARCHAR(10) NOT NULL UNIQUE,
    Nombres NVARCHAR(255) NOT NULL,
    Apellidos NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    NivelActual NVARCHAR(50) NOT NULL DEFAULT 'Titular1',
    FechaInicioNivelActual DATETIME2 NOT NULL,
    FechaUltimoAscenso DATETIME2 NULL,
    EstaActivo BIT NOT NULL DEFAULT 1,
    FechaNombramiento DATETIME2 NULL,
    PromedioEvaluaciones DECIMAL(5,2) NULL,
    HorasCapacitacion INT NULL,
    NumeroObrasAcademicas INT NULL,
    MesesInvestigacion INT NULL,
    FechaUltimaImportacion DATETIME2 NULL,
    UsuarioId UNIQUEIDENTIFIER NOT NULL UNIQUE,
    FechaCreacion DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FechaModificacion DATETIME2 NULL,
    FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id)
);

-- Tabla SolicitudesAscenso
CREATE TABLE SolicitudesAscenso (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    DocenteId UNIQUEIDENTIFIER NOT NULL,
    NivelActual NVARCHAR(50) NOT NULL,
    NivelSolicitado NVARCHAR(50) NOT NULL,
    Estado NVARCHAR(50) NOT NULL DEFAULT 'Pendiente',
    MotivoRechazo NVARCHAR(MAX) NULL,
    FechaSolicitud DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FechaAprobacion DATETIME2 NULL,
    AprobadoPorId UNIQUEIDENTIFIER NULL,
    PromedioEvaluaciones DECIMAL(5,2) NOT NULL,
    HorasCapacitacion INT NOT NULL,
    NumeroObrasAcademicas INT NOT NULL,
    MesesInvestigacion INT NOT NULL,
    TiempoEnNivelDias INT NOT NULL,
    FechaCreacion DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FechaModificacion DATETIME2 NULL,
    FOREIGN KEY (DocenteId) REFERENCES Docentes(Id),
    FOREIGN KEY (AprobadoPorId) REFERENCES Usuarios(Id)
);

-- Tabla Documentos
CREATE TABLE Documentos (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    NombreArchivo NVARCHAR(255) NOT NULL,
    RutaArchivo NVARCHAR(500) NOT NULL,
    TamanoArchivo BIGINT NOT NULL,
    TipoDocumento NVARCHAR(50) NOT NULL,
    ContenidoArchivo VARBINARY(MAX) NOT NULL,
    ContentType NVARCHAR(255) NOT NULL,
    SolicitudAscensoId UNIQUEIDENTIFIER NOT NULL,
    FechaCreacion DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FechaModificacion DATETIME2 NULL,
    FOREIGN KEY (SolicitudAscensoId) REFERENCES SolicitudesAscenso(Id)
);

-- Tabla LogsAuditoria
CREATE TABLE LogsAuditoria (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Accion NVARCHAR(255) NOT NULL,
    UsuarioId NVARCHAR(255) NULL,
    UsuarioEmail NVARCHAR(255) NULL,
    EntidadAfectada NVARCHAR(255) NULL,
    ValoresAnteriores NVARCHAR(MAX) NULL,
    ValoresNuevos NVARCHAR(MAX) NULL,
    DireccionIP NVARCHAR(45) NULL,
    FechaAccion DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FechaCreacion DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FechaModificacion DATETIME2 NULL
);

PRINT 'Tablas creadas exitosamente en SGA_Main';
