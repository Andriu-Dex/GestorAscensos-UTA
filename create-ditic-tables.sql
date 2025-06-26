-- Script para crear tablas en DITIC
USE DITIC;

-- Tabla CursosDITIC
CREATE TABLE CursosDITIC (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(255) NOT NULL,
    Descripcion NVARCHAR(MAX) NOT NULL,
    HorasDuracion INT NOT NULL,
    Modalidad NVARCHAR(50) NOT NULL, -- Presencial, Virtual, Mixta
    EstaActivo BIT NOT NULL DEFAULT 1
);

-- Tabla ParticipacionesCursoDITIC
CREATE TABLE ParticipacionesCursoDITIC (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Cedula NVARCHAR(10) NOT NULL,
    CursoId INT NOT NULL,
    FechaInicio DATETIME2 NOT NULL,
    FechaFin DATETIME2 NOT NULL,
    Aprobado BIT NOT NULL DEFAULT 0,
    NotaFinal DECIMAL(5,2) NULL,
    CertificadoEmitido BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (CursoId) REFERENCES CursosDITIC(Id)
);

-- Tabla CertificacionesDITIC
CREATE TABLE CertificacionesDITIC (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Cedula NVARCHAR(10) NOT NULL,
    NombreCertificacion NVARCHAR(255) NOT NULL,
    Institucion NVARCHAR(255) NOT NULL,
    FechaEmision DATETIME2 NOT NULL,
    FechaVencimiento DATETIME2 NULL,
    HorasEquivalentes INT NOT NULL
);

-- Índices para mejorar rendimiento
CREATE INDEX IX_ParticipacionesCursoDITIC_Cedula ON ParticipacionesCursoDITIC(Cedula);
CREATE INDEX IX_ParticipacionesCursoDITIC_CursoId ON ParticipacionesCursoDITIC(CursoId);
CREATE INDEX IX_ParticipacionesCursoDITIC_FechaInicio ON ParticipacionesCursoDITIC(FechaInicio);
CREATE INDEX IX_CertificacionesDITIC_Cedula ON CertificacionesDITIC(Cedula);
CREATE INDEX IX_CertificacionesDITIC_FechaEmision ON CertificacionesDITIC(FechaEmision);

PRINT 'Tablas creadas exitosamente en DITIC';
