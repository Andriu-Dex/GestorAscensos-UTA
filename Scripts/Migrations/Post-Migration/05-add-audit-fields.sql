-- PLANTILLA: Script Post-Migración
-- Archivo: 05-Add Audit Fields.sql (ej: 05-add-audit-triggers.sql)
-- Propósito: [DESCRIBIR QUE HACE ESTE SCRIPT]
-- Autor: Andriu Dex
-- Fecha: 2025-06-29

USE SGA_Main;
GO

PRINT 'Ejecutando script post-migración: Add Audit Fields';
PRINT 'Fecha: ' + CONVERT(VARCHAR, GETDATE(), 120);

-- ===========================================
-- VALIDACIONES PREVIAS
-- ===========================================

-- Verificar que existe la tabla/objeto necesario
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'NombreTabla')
BEGIN
    PRINT '❌ ERROR: Tabla requerida no existe';
    RAISERROR('Tabla NombreTabla no encontrada', 16, 1);
    RETURN;
END

-- Verificar que no existe ya el constraint/índice
IF EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_NombreConstraint')
BEGIN
    PRINT '⚠️ ADVERTENCIA: Constraint ya existe, omitiendo creación';
    RETURN;
END

-- ===========================================
-- EJECUCIÓN PRINCIPAL
-- ===========================================

BEGIN TRY
    BEGIN TRANSACTION;
    
    -- Aquí va el código principal del script
    -- Ejemplo: Agregar constraint
    ALTER TABLE NombreTabla 
    ADD CONSTRAINT CK_NombreConstraint 
    CHECK (Campo IN ('Valor1', 'Valor2', 'Valor3'));
    
    -- Ejemplo: Crear índice
    CREATE NONCLUSTERED INDEX IX_NombreTabla_Campo
    ON NombreTabla (Campo)
    WHERE CondicionFiltro = 1;
    
    -- Ejemplo: Insertar datos iniciales
    INSERT INTO TablaConfiguracion (Clave, Valor, Descripcion)
    VALUES 
        ('CONFIG_KEY', 'CONFIG_VALUE', 'Descripción de la configuración');
    
    COMMIT TRANSACTION;
    PRINT '✅ Script ejecutado exitosamente';
    
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    
    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
    DECLARE @ErrorState INT = ERROR_STATE();
    
    PRINT '❌ ERROR ejecutando script: ' + @ErrorMessage;
    
    -- Re-lanzar el error para que el framework lo capture
    RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH;

-- ===========================================
-- VALIDACIONES POST-EJECUCIÓN
-- ===========================================

-- Verificar que el cambio se aplicó correctamente
IF EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_NombreConstraint')
BEGIN
    PRINT '✅ VALIDACIÓN: Constraint creado correctamente';
END
ELSE
BEGIN
    PRINT '❌ VALIDACIÓN FALLIDA: Constraint no fue creado';
    RAISERROR('Validación post-ejecución falló', 16, 1);
END

PRINT 'Script completado: Add Audit Fields';
GO

