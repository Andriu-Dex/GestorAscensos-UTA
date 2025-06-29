-- Script 03: Índices Especiales
-- Índices únicos y filtrados para optimización y constraints complejos

USE SGA_Main;
GO

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

PRINT 'Aplicando índices especiales...';

-- Índice único filtrado para solicitudes activas (un docente = una solicitud activa)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SolicitudesAscenso_DocenteId_SolicitudActiva' AND object_id = OBJECT_ID('SolicitudesAscenso'))
BEGIN
    CREATE UNIQUE INDEX IX_SolicitudesAscenso_DocenteId_SolicitudActiva 
    ON SolicitudesAscenso (DocenteId) 
    WHERE Estado IN ('Pendiente', 'EnProceso');
    PRINT '✓ Índice único para solicitudes activas creado';
END
ELSE
BEGIN
    PRINT '⚠ Índice único para solicitudes activas ya existe';
END

-- Índice compuesto para búsquedas por nivel y estado de docentes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Docentes_NivelActual_EstaActivo' AND object_id = OBJECT_ID('Docentes'))
BEGIN
    CREATE INDEX IX_Docentes_NivelActual_EstaActivo 
    ON Docentes (NivelActual, EstaActivo)
    INCLUDE (Nombres, Apellidos, Email);
    PRINT '✓ Índice compuesto Docentes_NivelActual_EstaActivo creado';
END
ELSE
BEGIN
    PRINT '⚠ Índice compuesto Docentes_NivelActual_EstaActivo ya existe';
END

-- Índice para búsquedas por estado y fecha de solicitudes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SolicitudesAscenso_Estado_FechaSolicitud' AND object_id = OBJECT_ID('SolicitudesAscenso'))
BEGIN
    CREATE INDEX IX_SolicitudesAscenso_Estado_FechaSolicitud 
    ON SolicitudesAscenso (Estado, FechaSolicitud DESC)
    INCLUDE (DocenteId, NivelActual, NivelSolicitado);
    PRINT '✓ Índice compuesto SolicitudesAscenso_Estado_FechaSolicitud creado';
END
ELSE
BEGIN
    PRINT '⚠ Índice compuesto SolicitudesAscenso_Estado_FechaSolicitud ya existe';
END

-- Índice para búsquedas de documentos por tipo
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Documentos_TipoDocumento_SolicitudId' AND object_id = OBJECT_ID('Documentos'))
BEGIN
    CREATE INDEX IX_Documentos_TipoDocumento_SolicitudId 
    ON Documentos (TipoDocumento, SolicitudAscensoId)
    INCLUDE (NombreArchivo, TamanoArchivo, FechaCreacion);
    PRINT '✓ Índice compuesto Documentos_TipoDocumento_SolicitudId creado';
END
ELSE
BEGIN
    PRINT '⚠ Índice compuesto Documentos_TipoDocumento_SolicitudId ya existe';
END

-- Índice para auditoría por fecha y usuario
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_LogsAuditoria_FechaAccion_UsuarioEmail' AND object_id = OBJECT_ID('LogsAuditoria'))
BEGIN
    CREATE INDEX IX_LogsAuditoria_FechaAccion_UsuarioEmail 
    ON LogsAuditoria (FechaAccion DESC, UsuarioEmail)
    INCLUDE (Accion, EntidadAfectada);
    PRINT '✓ Índice compuesto LogsAuditoria_FechaAccion_UsuarioEmail creado';
END
ELSE
BEGIN
    PRINT '⚠ Índice compuesto LogsAuditoria_FechaAccion_UsuarioEmail ya existe';
END

-- Índice para búsquedas de cédula de docente (usado frecuentemente)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Docentes_Cedula_Covering' AND object_id = OBJECT_ID('Docentes'))
BEGIN
    CREATE INDEX IX_Docentes_Cedula_Covering 
    ON Docentes (Cedula)
    INCLUDE (Id, Nombres, Apellidos, NivelActual, EstaActivo)
    WHERE EstaActivo = 1;
    PRINT '✓ Índice filtrado Docentes_Cedula_Covering creado';
END
ELSE
BEGIN
    PRINT '⚠ Índice filtrado Docentes_Cedula_Covering ya existe';
END

PRINT 'Índices especiales aplicados correctamente.';
GO
