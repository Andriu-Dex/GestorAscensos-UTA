-- Script 01: Constraints de Enum
-- Se ejecuta automáticamente después de aplicar migraciones EF
-- Asegura que solo se permitan valores válidos en campos enum

USE SGA_Main;
GO

PRINT 'Aplicando constraints de enum...';

-- Constraint para Rol en tabla Usuarios
BEGIN TRY
    ALTER TABLE Usuarios 
    ADD CONSTRAINT CK_Usuarios_Rol 
    CHECK (Rol IN ('Docente', 'Administrador'));
    PRINT '✓ Constraint CK_Usuarios_Rol agregado';
END TRY
BEGIN CATCH
    PRINT '⚠ Constraint CK_Usuarios_Rol ya existe o error: ' + ERROR_MESSAGE();
END CATCH;

-- Constraint para NivelActual en tabla Docentes
BEGIN TRY
    ALTER TABLE Docentes 
    ADD CONSTRAINT CK_Docentes_NivelActual 
    CHECK (NivelActual IN ('Titular1', 'Titular2', 'Titular3', 'Titular4', 'Titular5'));
    PRINT '✓ Constraint CK_Docentes_NivelActual agregado';
END TRY
BEGIN CATCH
    PRINT '⚠ Constraint CK_Docentes_NivelActual ya existe o error: ' + ERROR_MESSAGE();
END CATCH;

-- Constraints para tabla SolicitudesAscenso
BEGIN TRY
    ALTER TABLE SolicitudesAscenso 
    ADD CONSTRAINT CK_SolicitudesAscenso_NivelActual 
    CHECK (NivelActual IN ('Titular1', 'Titular2', 'Titular3', 'Titular4', 'Titular5'));
    PRINT '✓ Constraint CK_SolicitudesAscenso_NivelActual agregado';
END TRY
BEGIN CATCH
    PRINT '⚠ Constraint CK_SolicitudesAscenso_NivelActual ya existe o error: ' + ERROR_MESSAGE();
END CATCH;

BEGIN TRY
    ALTER TABLE SolicitudesAscenso 
    ADD CONSTRAINT CK_SolicitudesAscenso_NivelSolicitado 
    CHECK (NivelSolicitado IN ('Titular1', 'Titular2', 'Titular3', 'Titular4', 'Titular5'));
    PRINT '✓ Constraint CK_SolicitudesAscenso_NivelSolicitado agregado';
END TRY
BEGIN CATCH
    PRINT '⚠ Constraint CK_SolicitudesAscenso_NivelSolicitado ya existe o error: ' + ERROR_MESSAGE();
END CATCH;

BEGIN TRY
    ALTER TABLE SolicitudesAscenso 
    ADD CONSTRAINT CK_SolicitudesAscenso_Estado 
    CHECK (Estado IN ('Pendiente', 'EnProceso', 'Aprobada', 'Rechazada'));
    PRINT '✓ Constraint CK_SolicitudesAscenso_Estado agregado';
END TRY
BEGIN CATCH
    PRINT '⚠ Constraint CK_SolicitudesAscenso_Estado ya existe o error: ' + ERROR_MESSAGE();
END CATCH;

-- Constraint para TipoDocumento en tabla Documentos
BEGIN TRY
    ALTER TABLE Documentos 
    ADD CONSTRAINT CK_Documentos_TipoDocumento 
    CHECK (TipoDocumento IN ('CertificadoTrabajo', 'EvaluacionesDocentes', 'CertificadosCapacitacion', 'ObrasAcademicas', 'CertificadoInvestigacion', 'Otro'));
    PRINT '✓ Constraint CK_Documentos_TipoDocumento agregado';
END TRY
BEGIN CATCH
    PRINT '⚠ Constraint CK_Documentos_TipoDocumento ya existe o error: ' + ERROR_MESSAGE();
END CATCH;

PRINT 'Constraints de enum aplicados correctamente.';
GO
