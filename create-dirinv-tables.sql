-- Script para crear tablas en DIRINV
USE DIRINV;

-- Tabla ProyectosInvestigacionDIRINV
CREATE TABLE ProyectosInvestigacionDIRINV (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Titulo NVARCHAR(255) NOT NULL,
    Descripcion NVARCHAR(MAX) NOT NULL,
    FechaInicio DATETIME2 NOT NULL,
    FechaFin DATETIME2 NULL,
    Estado NVARCHAR(50) NOT NULL, -- En curso, Finalizado, Suspendido
    PresupuestoTotal DECIMAL(15,2) NULL,
    FuenteFinanciamiento NVARCHAR(255) NULL
);

-- Tabla ObrasAcademicasDIRINV
CREATE TABLE ObrasAcademicasDIRINV (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Cedula NVARCHAR(10) NOT NULL,
    Titulo NVARCHAR(255) NOT NULL,
    TipoObra NVARCHAR(100) NOT NULL, -- Artículo, Libro, Capítulo, etc.
    FechaPublicacion DATETIME2 NOT NULL,
    Editorial NVARCHAR(255) NULL,
    Revista NVARCHAR(255) NULL,
    ISBN_ISSN NVARCHAR(50) NULL,
    EsIndexada BIT NOT NULL DEFAULT 0,
    IndiceIndexacion NVARCHAR(100) NULL
);

-- Tabla ParticipacionesProyectoDIRINV
CREATE TABLE ParticipacionesProyectoDIRINV (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Cedula NVARCHAR(10) NOT NULL,
    ProyectoId INT NOT NULL,
    RolEnProyecto NVARCHAR(100) NOT NULL, -- Director, Investigador, Colaborador
    FechaInicio DATETIME2 NOT NULL,
    FechaFin DATETIME2 NULL,
    HorasSemanales INT NOT NULL,
    FOREIGN KEY (ProyectoId) REFERENCES ProyectosInvestigacionDIRINV(Id)
);

-- Índices para mejorar rendimiento
CREATE INDEX IX_ObrasAcademicasDIRINV_Cedula ON ObrasAcademicasDIRINV(Cedula);
CREATE INDEX IX_ObrasAcademicasDIRINV_FechaPublicacion ON ObrasAcademicasDIRINV(FechaPublicacion);
CREATE INDEX IX_ObrasAcademicasDIRINV_TipoObra ON ObrasAcademicasDIRINV(TipoObra);
CREATE INDEX IX_ParticipacionesProyectoDIRINV_Cedula ON ParticipacionesProyectoDIRINV(Cedula);
CREATE INDEX IX_ParticipacionesProyectoDIRINV_ProyectoId ON ParticipacionesProyectoDIRINV(ProyectoId);

PRINT 'Tablas creadas exitosamente en DIRINV';
