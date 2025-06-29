-- Script 02: Business Rules Constraints
-- Reglas de negocio específicas del dominio SGA

USE SGA_Main;
GO

PRINT 'Aplicando constraints de reglas de negocio...';

-- Constraint para progresión secuencial de niveles
BEGIN TRY
    ALTER TABLE SolicitudesAscenso 
    ADD CONSTRAINT CK_SolicitudesAscenso_NivelProgresion 
    CHECK (
        (NivelActual = 'Titular1' AND NivelSolicitado = 'Titular2') OR
        (NivelActual = 'Titular2' AND NivelSolicitado = 'Titular3') OR
        (NivelActual = 'Titular3' AND NivelSolicitado = 'Titular4') OR
        (NivelActual = 'Titular4' AND NivelSolicitado = 'Titular5')
    );
    PRINT '✓ Constraint CK_SolicitudesAscenso_NivelProgresion agregado';
END TRY
BEGIN CATCH
    PRINT '⚠ Constraint CK_SolicitudesAscenso_NivelProgresion ya existe o error: ' + ERROR_MESSAGE();
END CATCH;

-- Constraint para validar que la fecha de solicitud no sea futura
BEGIN TRY
    ALTER TABLE SolicitudesAscenso 
    ADD CONSTRAINT CK_SolicitudesAscenso_FechaSolicitud 
    CHECK (FechaSolicitud <= GETDATE());
    PRINT '✓ Constraint CK_SolicitudesAscenso_FechaSolicitud agregado';
END TRY
BEGIN CATCH
    PRINT '⚠ Constraint CK_SolicitudesAscenso_FechaSolicitud ya existe o error: ' + ERROR_MESSAGE();
END CATCH;

-- Constraint para validar que las evaluaciones estén en rango válido (0-100)
BEGIN TRY
    ALTER TABLE SolicitudesAscenso 
    ADD CONSTRAINT CK_SolicitudesAscenso_PromedioEvaluaciones 
    CHECK (PromedioEvaluaciones >= 0 AND PromedioEvaluaciones <= 100);
    PRINT '✓ Constraint CK_SolicitudesAscenso_PromedioEvaluaciones agregado';
END TRY
BEGIN CATCH
    PRINT '⚠ Constraint CK_SolicitudesAscenso_PromedioEvaluaciones ya existe o error: ' + ERROR_MESSAGE();
END CATCH;

-- Constraint similar para tabla Docentes
BEGIN TRY
    ALTER TABLE Docentes 
    ADD CONSTRAINT CK_Docentes_PromedioEvaluaciones 
    CHECK (PromedioEvaluaciones IS NULL OR (PromedioEvaluaciones >= 0 AND PromedioEvaluaciones <= 100));
    PRINT '✓ Constraint CK_Docentes_PromedioEvaluaciones agregado';
END TRY
BEGIN CATCH
    PRINT '⚠ Constraint CK_Docentes_PromedioEvaluaciones ya existe o error: ' + ERROR_MESSAGE();
END CATCH;

-- Constraint para validar que las horas de capacitación sean positivas
BEGIN TRY
    ALTER TABLE SolicitudesAscenso 
    ADD CONSTRAINT CK_SolicitudesAscenso_HorasCapacitacion 
    CHECK (HorasCapacitacion >= 0);
    PRINT '✓ Constraint CK_SolicitudesAscenso_HorasCapacitacion agregado';
END TRY
BEGIN CATCH
    PRINT '⚠ Constraint CK_SolicitudesAscenso_HorasCapacitacion ya existe o error: ' + ERROR_MESSAGE();
END CATCH;

BEGIN TRY
    ALTER TABLE Docentes 
    ADD CONSTRAINT CK_Docentes_HorasCapacitacion 
    CHECK (HorasCapacitacion IS NULL OR HorasCapacitacion >= 0);
    PRINT '✓ Constraint CK_Docentes_HorasCapacitacion agregado';
END TRY
BEGIN CATCH
    PRINT '⚠ Constraint CK_Docentes_HorasCapacitacion ya existe o error: ' + ERROR_MESSAGE();
END CATCH;

-- Constraint para validar que el número de obras académicas sea positivo
BEGIN TRY
    ALTER TABLE SolicitudesAscenso 
    ADD CONSTRAINT CK_SolicitudesAscenso_NumeroObrasAcademicas 
    CHECK (NumeroObrasAcademicas >= 0);
    PRINT '✓ Constraint CK_SolicitudesAscenso_NumeroObrasAcademicas agregado';
END TRY
BEGIN CATCH
    PRINT '⚠ Constraint CK_SolicitudesAscenso_NumeroObrasAcademicas ya existe o error: ' + ERROR_MESSAGE();
END CATCH;

BEGIN TRY
    ALTER TABLE Docentes 
    ADD CONSTRAINT CK_Docentes_NumeroObrasAcademicas 
    CHECK (NumeroObrasAcademicas IS NULL OR NumeroObrasAcademicas >= 0);
    PRINT '✓ Constraint CK_Docentes_NumeroObrasAcademicas agregado';
END TRY
BEGIN CATCH
    PRINT '⚠ Constraint CK_Docentes_NumeroObrasAcademicas ya existe o error: ' + ERROR_MESSAGE();
END CATCH;

-- Constraint para validar que los meses de investigación sean positivos
BEGIN TRY
    ALTER TABLE SolicitudesAscenso 
    ADD CONSTRAINT CK_SolicitudesAscenso_MesesInvestigacion 
    CHECK (MesesInvestigacion >= 0);
    PRINT '✓ Constraint CK_SolicitudesAscenso_MesesInvestigacion agregado';
END TRY
BEGIN CATCH
    PRINT '⚠ Constraint CK_SolicitudesAscenso_MesesInvestigacion ya existe o error: ' + ERROR_MESSAGE();
END CATCH;

BEGIN TRY
    ALTER TABLE Docentes 
    ADD CONSTRAINT CK_Docentes_MesesInvestigacion 
    CHECK (MesesInvestigacion IS NULL OR MesesInvestigacion >= 0);
    PRINT '✓ Constraint CK_Docentes_MesesInvestigacion agregado';
END TRY
BEGIN CATCH
    PRINT '⚠ Constraint CK_Docentes_MesesInvestigacion ya existe o error: ' + ERROR_MESSAGE();
END CATCH;

PRINT 'Constraints de reglas de negocio aplicados correctamente.';
GO
