-- Script para crear tablas en DAC
USE DAC;

-- Tabla PeriodosAcademicosDAC
CREATE TABLE PeriodosAcademicosDAC (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(255) NOT NULL,
    FechaInicio DATETIME2 NOT NULL,
    FechaFin DATETIME2 NOT NULL,
    EstaActivo BIT NOT NULL DEFAULT 1
);

-- Tabla EvaluacionesDocenteDAC
CREATE TABLE EvaluacionesDocenteDAC (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Cedula NVARCHAR(10) NOT NULL,
    PeriodoId INT NOT NULL,
    PuntajeTotal DECIMAL(10,2) NOT NULL,
    PuntajeMaximo DECIMAL(10,2) NOT NULL,
    Porcentaje DECIMAL(5,2) NOT NULL,
    FechaEvaluacion DATETIME2 NOT NULL,
    Observaciones NVARCHAR(MAX) NULL,
    FOREIGN KEY (PeriodoId) REFERENCES PeriodosAcademicosDAC(Id)
);

-- Tabla CriteriosEvaluacionDAC
CREATE TABLE CriteriosEvaluacionDAC (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(255) NOT NULL,
    Descripcion NVARCHAR(MAX) NOT NULL,
    PesoMaximo DECIMAL(5,2) NOT NULL,
    EstaActivo BIT NOT NULL DEFAULT 1
);

-- Índices para mejorar rendimiento
CREATE INDEX IX_EvaluacionesDocenteDAC_Cedula ON EvaluacionesDocenteDAC(Cedula);
CREATE INDEX IX_EvaluacionesDocenteDAC_PeriodoId ON EvaluacionesDocenteDAC(PeriodoId);
CREATE INDEX IX_EvaluacionesDocenteDAC_FechaEvaluacion ON EvaluacionesDocenteDAC(FechaEvaluacion);

PRINT 'Tablas creadas exitosamente en DAC';
