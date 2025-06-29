-- PLANTILLA: Script Pre-Migración
-- Archivo: [NUMERO]-[DESCRIPCION].sql (ej: 01-backup-critical-data.sql)
-- Propósito: [DESCRIBIR QUE HACE ESTE SCRIPT - PREPARACIÓN ANTES DE MIGRACIÓN]
-- Autor: [NOMBRE]
-- Fecha: [FECHA]

USE [DATABASE_NAME];
GO

PRINT 'Ejecutando script pre-migración: [DESCRIPCION]';
PRINT 'Fecha: ' + CONVERT(VARCHAR, GETDATE(), 120);

-- ===========================================
-- VALIDACIONES PREVIAS
-- ===========================================

-- Verificar estado de la base de datos
IF EXISTS (SELECT 1 FROM sys.dm_exec_requests WHERE blocking_session_id > 0)
BEGIN
    PRINT '⚠️ ADVERTENCIA: Existen procesos bloqueados';
    -- Opcional: RAISERROR para abortar si es crítico
END

-- Verificar espacio disponible
DECLARE @SpaceAvailableMB INT;
SELECT @SpaceAvailableMB = 
    (size - FILEPROPERTY(name, 'SpaceUsed')) * 8 / 1024
FROM sys.database_files 
WHERE type = 0; -- Data file

IF @SpaceAvailableMB < 100 -- Requerir al menos 100MB
BEGIN
    PRINT '❌ ERROR: Espacio insuficiente en disco';
    RAISERROR('Se requieren al menos 100MB de espacio libre', 16, 1);
    RETURN;
END

-- ===========================================
-- PREPARACIÓN PRE-MIGRACIÓN
-- ===========================================

BEGIN TRY
    BEGIN TRANSACTION;
    
    -- Ejemplo 1: Crear tabla temporal para backup de datos críticos
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TempBackup_CriticalData')
    BEGIN
        CREATE TABLE TempBackup_CriticalData (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            OriginalId INT,
            OriginalData NVARCHAR(MAX),
            BackupDate DATETIME2 DEFAULT GETDATE()
        );
        
        -- Copiar datos críticos que podrían perderse
        INSERT INTO TempBackup_CriticalData (OriginalId, OriginalData)
        SELECT Id, SomeImportantField 
        FROM CriticalTable 
        WHERE SomeCondition = 1;
        
        PRINT '✅ Backup de datos críticos creado';
    END
    
    -- Ejemplo 2: Deshabilitar triggers que podrían interferir
    IF EXISTS (SELECT 1 FROM sys.triggers WHERE name = 'TR_AuditTrigger')
    BEGIN
        DISABLE TRIGGER TR_AuditTrigger ON SomeTable;
        PRINT '✅ Triggers deshabilitados';
    END
    
    -- Ejemplo 3: Limpiar datos obsoletos antes de migración
    DELETE FROM LogTable 
    WHERE CreatedDate < DATEADD(MONTH, -6, GETDATE());
    
    PRINT '✅ Limpieza de datos obsoletos completada';
    
    -- Ejemplo 4: Verificar integridad referencial
    DBCC CHECKCONSTRAINTS WITH ALL_CONSTRAINTS;
    
    COMMIT TRANSACTION;
    PRINT '✅ Script pre-migración ejecutado exitosamente';
    
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    
    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
    DECLARE @ErrorState INT = ERROR_STATE();
    
    PRINT '❌ ERROR en script pre-migración: ' + @ErrorMessage;
    
    -- Re-habilitar triggers si fueron deshabilitados
    IF EXISTS (SELECT 1 FROM sys.triggers WHERE name = 'TR_AuditTrigger' AND is_disabled = 1)
    BEGIN
        ENABLE TRIGGER TR_AuditTrigger ON SomeTable;
        PRINT '🔄 Triggers re-habilitados debido a error';
    END
    
    -- Re-lanzar el error para que el framework lo capture
    RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH;

-- ===========================================
-- VALIDACIONES POST-EJECUCIÓN
-- ===========================================

-- Verificar que el backup se creó correctamente
DECLARE @BackupCount INT;
SELECT @BackupCount = COUNT(*) FROM TempBackup_CriticalData;

IF @BackupCount > 0
BEGIN
    PRINT '✅ VALIDACIÓN: Backup contiene ' + CAST(@BackupCount AS VARCHAR) + ' registros';
END
ELSE
BEGIN
    PRINT '⚠️ VALIDACIÓN: Backup está vacío (puede ser normal)';
END

PRINT 'Script pre-migración completado: [DESCRIPCION]';
GO
