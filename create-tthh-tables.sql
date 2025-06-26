-- Script para crear tablas en TTHH
USE TTHH;

-- Tabla EmpleadosTTHH
CREATE TABLE EmpleadosTTHH (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Cedula NVARCHAR(10) NOT NULL UNIQUE,
    Nombres NVARCHAR(255) NOT NULL,
    Apellidos NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    CorreoInstitucional NVARCHAR(255) NOT NULL UNIQUE,
    Celular NVARCHAR(15) NOT NULL,
    FechaNombramiento DATETIME2 NOT NULL,
    CargoActual NVARCHAR(255) NOT NULL,
    NivelAcademico NVARCHAR(100) NOT NULL,
    Direccion NVARCHAR(500) NOT NULL,
    FechaNacimiento DATETIME2 NOT NULL,
    EstadoCivil NVARCHAR(50) NOT NULL,
    TipoContrato NVARCHAR(100) NOT NULL,
    EstaActivo BIT NOT NULL DEFAULT 1
);

-- Tabla AccionesPersonalTTHH
CREATE TABLE AccionesPersonalTTHH (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Cedula NVARCHAR(10) NOT NULL,
    TipoAccion NVARCHAR(100) NOT NULL,
    FechaAccion DATETIME2 NOT NULL,
    CargoAnterior NVARCHAR(255) NOT NULL,
    CargoNuevo NVARCHAR(255) NOT NULL,
    Observaciones NVARCHAR(MAX) NOT NULL
);

-- Tabla CargosTTHH
CREATE TABLE CargosTTHH (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    NombreCargo NVARCHAR(255) NOT NULL,
    NivelTitular NVARCHAR(50) NOT NULL,
    Descripcion NVARCHAR(MAX) NOT NULL,
    EstaActivo BIT NOT NULL DEFAULT 1
);

-- Índices para mejorar rendimiento
CREATE INDEX IX_EmpleadosTTHH_Cedula ON EmpleadosTTHH(Cedula);
CREATE INDEX IX_EmpleadosTTHH_CorreoInstitucional ON EmpleadosTTHH(CorreoInstitucional);
CREATE INDEX IX_AccionesPersonalTTHH_Cedula ON AccionesPersonalTTHH(Cedula);
CREATE INDEX IX_AccionesPersonalTTHH_FechaAccion ON AccionesPersonalTTHH(FechaAccion);

PRINT 'Tablas creadas exitosamente en TTHH';
