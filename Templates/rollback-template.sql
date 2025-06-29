-- PLANTILLA: Script de Rollback
-- Archivo: [NUMERO]-rollback-[DESCRIPCION].sql (ej: 05-rollback-audit-triggers.sql)
-- Propósito: [DESCRIBIR QUE REVIERTE ESTE SCRIPT]
-- Autor: [NOMBRE]
-- Fecha: [FECHA]
-- REVIERTE: [NOMBRE DEL SCRIPT ORIGINAL QUE REVIERTE]

USE [DATABASE_NAME];
GO

PRINT 'Ejecutando script de rollback: [DESCRIPCION]';
PRINT 'Fecha: ' + CONVERT(VARCHAR, GETDATE(), 120);

-- ===========================================
-- VALIDACIONES PREVIAS DE ROLLBACK
-- ===========================================

-- Verificar que existe lo que queremos revertir
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_NombreConstraint')
BEGIN
    PRINT '⚠️ ADVERTENCIA: Constraint a eliminar no existe';
    PRINT '✅ Rollback ya aplicado o no necesario';
    RETURN;
END

-- Verificar que es seguro hacer rollback
-- (ej: no hay datos que dependan del constraint)
IF EXISTS (SELECT 1 FROM NombreTabla WHERE Campo NOT IN ('Valor1', 'Valor2', 'Valor3'))
BEGIN
    PRINT '❌ ERROR: Existen datos que violan el rollback anterior';
    PRINT 'Se requiere limpieza manual de datos antes del rollback';
    RAISERROR('Rollback no seguro - datos inconsistentes', 16, 1);
    RETURN;
END

-- ===========================================
-- EJECUCIÓN DEL ROLLBACK
-- ===========================================

BEGIN TRY
    BEGIN TRANSACTION;
    
    -- Paso 1: Eliminar constraint agregado
    IF EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_NombreConstraint')
    BEGIN
        ALTER TABLE NombreTabla DROP CONSTRAINT CK_NombreConstraint;
        PRINT '✅ Constraint eliminado: CK_NombreConstraint';
    END
    
    -- Paso 2: Eliminar índice creado
    IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_NombreTabla_Campo')
    BEGIN
        DROP INDEX IX_NombreTabla_Campo ON NombreTabla;
        PRINT '✅ Índice eliminado: IX_NombreTabla_Campo';
    END
    
    -- Paso 3: Eliminar datos insertados (si es apropiado)
    DELETE FROM TablaConfiguracion 
    WHERE Clave = 'CONFIG_KEY' 
    AND Valor = 'CONFIG_VALUE';
    
    IF @@ROWCOUNT > 0
    BEGIN
        PRINT '✅ Datos de configuración eliminados';
    END
    
    -- Paso 4: Restaurar estado anterior si es necesario
    -- Ejemplo: Re-habilitar constraint anterior
    IF EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_OldConstraint' AND is_disabled = 1)
    BEGIN
        ALTER TABLE NombreTabla CHECK CONSTRAINT CK_OldConstraint;
        PRINT '✅ Constraint anterior re-habilitado';
    END
    
    COMMIT TRANSACTION;
    PRINT '✅ Rollback ejecutado exitosamente';
    
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    
    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
    DECLARE @ErrorState INT = ERROR_STATE();
    
    PRINT '❌ ERROR ejecutando rollback: ' + @ErrorMessage;
    
    -- Re-lanzar el error para que el framework lo capture
    RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH;

-- ===========================================
-- VALIDACIONES POST-ROLLBACK
-- ===========================================

-- Verificar que el rollback se aplicó correctamente
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_NombreConstraint')
BEGIN
    PRINT '✅ VALIDACIÓN: Constraint eliminado correctamente';
END
ELSE
BEGIN
    PRINT '❌ VALIDACIÓN FALLIDA: Constraint aún existe';
    RAISERROR('Validación post-rollback falló', 16, 1);
END

-- Verificar integridad general después del rollback
DBCC CHECKCONSTRAINTS WITH ALL_CONSTRAINTS;

-- Log de rollback para auditoría
INSERT INTO LogRollbacks (ScriptName, ExecutedDate, ExecutedBy, Notes)
VALUES ('[DESCRIPCION]', GETDATE(), SYSTEM_USER, 'Rollback automático ejecutado');

PRINT 'Rollback completado: [DESCRIPCION]';
GO
