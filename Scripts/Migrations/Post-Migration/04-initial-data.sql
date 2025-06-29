-- Script 04: Datos Iniciales y Configuración
-- Datos semilla adicionales y configuraciones del sistema

USE SGA_Main;
GO

PRINT 'Aplicando datos iniciales y configuración...';

-- Verificar si ya existen datos semilla adicionales
IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Email = 'docente.test@uta.edu.ec')
BEGIN
    PRINT 'Agregando usuario docente de prueba...';
    
    DECLARE @DocenteTestUserId UNIQUEIDENTIFIER = NEWID();
    DECLARE @DocenteTestId UNIQUEIDENTIFIER = NEWID();
    
    -- Usuario docente de prueba
    INSERT INTO Usuarios (Id, Email, PasswordHash, Rol, EstaActivo, IntentosLogin, UltimoLogin, FechaCreacion)
    VALUES (
        @DocenteTestUserId,
        'docente.test@uta.edu.ec',
        '$2a$11$Htd5IHWrNNNE9zlTolsnZ.BCk3CAHaoEVr8jH6MFZ1cuLvZecjypC', -- Password: Test123!
        'Docente',
        1,
        0,
        GETDATE(),
        GETDATE()
    );
    
    -- Docente de prueba
    INSERT INTO Docentes (Id, Cedula, Nombres, Apellidos, Email, NivelActual, FechaInicioNivelActual, EstaActivo, UsuarioId, FechaCreacion)
    VALUES (
        @DocenteTestId,
        '1234567890',
        'Juan Carlos',
        'Pérez González',
        'docente.test@uta.edu.ec',
        'Titular1',
        DATEADD(YEAR, -2, GETDATE()), -- 2 años en el nivel actual
        1,
        @DocenteTestUserId,
        GETDATE()
    );
    
    PRINT '✓ Usuario docente de prueba creado';
END
ELSE
BEGIN
    PRINT '⚠ Usuario docente de prueba ya existe';
END

-- Crear datos de configuración del sistema si no existen
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ConfiguracionSistema')
BEGIN
    PRINT 'Creando tabla de configuración del sistema...';
    
    CREATE TABLE ConfiguracionSistema (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        Clave NVARCHAR(100) NOT NULL UNIQUE,
        Valor NVARCHAR(500) NOT NULL,
        Descripcion NVARCHAR(1000) NULL,
        TipoDato NVARCHAR(50) NOT NULL DEFAULT 'String', -- String, Number, Boolean, Date
        EsEditable BIT NOT NULL DEFAULT 1,
        FechaCreacion DATETIME2 NOT NULL DEFAULT GETDATE(),
        FechaModificacion DATETIME2 NULL
    );
    
    -- Insertar configuraciones por defecto
    INSERT INTO ConfiguracionSistema (Clave, Valor, Descripcion, TipoDato, EsEditable) VALUES
    ('RequisitosTiempoMinimo_Anos', '4', 'Años mínimos requeridos en nivel actual para ascender', 'Number', 1),
    ('RequisitosTitular2_Obras', '1', 'Número mínimo de obras académicas para ascender a Titular 2', 'Number', 1),
    ('RequisitosTitular2_Evaluacion', '75', 'Promedio mínimo de evaluaciones para ascender a Titular 2', 'Number', 1),
    ('RequisitosTitular2_Capacitacion', '96', 'Horas mínimas de capacitación para ascender a Titular 2', 'Number', 1),
    ('RequisitosTitular3_Obras', '2', 'Número mínimo de obras académicas para ascender a Titular 3', 'Number', 1),
    ('RequisitosTitular3_Evaluacion', '75', 'Promedio mínimo de evaluaciones para ascender a Titular 3', 'Number', 1),
    ('RequisitosTitular3_Capacitacion', '96', 'Horas mínimas de capacitación para ascender a Titular 3', 'Number', 1),
    ('RequisitosTitular3_Investigacion', '12', 'Meses mínimos de investigación para ascender a Titular 3', 'Number', 1),
    ('RequisitosTitular4_Obras', '3', 'Número mínimo de obras académicas para ascender a Titular 4', 'Number', 1),
    ('RequisitosTitular4_Evaluacion', '75', 'Promedio mínimo de evaluaciones para ascender a Titular 4', 'Number', 1),
    ('RequisitosTitular4_Capacitacion', '128', 'Horas mínimas de capacitación para ascender a Titular 4', 'Number', 1),
    ('RequisitosTitular4_Investigacion', '24', 'Meses mínimos de investigación para ascender a Titular 4', 'Number', 1),
    ('RequisitosTitular5_Obras', '5', 'Número mínimo de obras académicas para ascender a Titular 5', 'Number', 1),
    ('RequisitosTitular5_Evaluacion', '75', 'Promedio mínimo de evaluaciones para ascender a Titular 5', 'Number', 1),
    ('RequisitosTitular5_Capacitacion', '160', 'Horas mínimas de capacitación para ascender a Titular 5', 'Number', 1),
    ('RequisitosTitular5_Investigacion', '24', 'Meses mínimos de investigación para ascender a Titular 5', 'Number', 1),
    ('SistemaVersionActual', '1.0.0', 'Versión actual del sistema', 'String', 0),
    ('SistemaFechaUltimaActualizacion', CONVERT(NVARCHAR, GETDATE(), 23), 'Fecha de última actualización del sistema', 'Date', 0),
    ('MaxSolicitudesActivasPorDocente', '1', 'Máximo número de solicitudes activas por docente', 'Number', 1),
    ('TiempoExpiracionTokenJWT_Horas', '24', 'Tiempo de expiración de tokens JWT en horas', 'Number', 1),
    ('MaxTamanoArchivoMB', '10', 'Tamaño máximo de archivo en MB', 'Number', 1),
    ('TiposArchivoPermitidos', 'pdf,doc,docx', 'Tipos de archivo permitidos para documentos', 'String', 1),
    ('EmailNotificacionesActivo', 'false', 'Indica si las notificaciones por email están activas', 'Boolean', 1),
    ('MantenimientoProgramado', 'false', 'Indica si el sistema está en mantenimiento programado', 'Boolean', 1);
    
    PRINT '✓ Tabla de configuración del sistema y datos por defecto creados';
END
ELSE
BEGIN
    PRINT '⚠ Tabla de configuración del sistema ya existe';
END

-- Crear vista para consultas frecuentes de docentes con requisitos
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_NAME = 'VW_DocentesConRequisitos')
BEGIN
    PRINT 'Creando vista VW_DocentesConRequisitos...';
    
    EXEC('CREATE VIEW VW_DocentesConRequisitos AS
    SELECT 
        d.Id,
        d.Cedula,
        d.Nombres,
        d.Apellidos,
        d.Email,
        d.NivelActual,
        d.FechaInicioNivelActual,
        d.EstaActivo,
        
        -- Tiempo en nivel actual
        DATEDIFF(DAY, d.FechaInicioNivelActual, GETDATE()) AS DiasEnNivelActual,
        DATEDIFF(YEAR, d.FechaInicioNivelActual, GETDATE()) AS AnosEnNivelActual,
        
        -- Datos actuales
        ISNULL(d.PromedioEvaluaciones, 0) AS PromedioEvaluaciones,
        ISNULL(d.HorasCapacitacion, 0) AS HorasCapacitacion,
        ISNULL(d.NumeroObrasAcademicas, 0) AS NumeroObrasAcademicas,
        ISNULL(d.MesesInvestigacion, 0) AS MesesInvestigacion,
        
        -- Estado de solicitudes
        (SELECT COUNT(*) FROM SolicitudesAscenso sa WHERE sa.DocenteId = d.Id AND sa.Estado IN (''Pendiente'', ''EnProceso'')) AS SolicitudesActivas,
        
        -- Siguiente nivel posible
        CASE d.NivelActual
            WHEN ''Titular1'' THEN ''Titular2''
            WHEN ''Titular2'' THEN ''Titular3''
            WHEN ''Titular3'' THEN ''Titular4''
            WHEN ''Titular4'' THEN ''Titular5''
            ELSE NULL
        END AS SiguienteNivel,
        
        -- Puede ascender (lógica básica)
        CASE 
            WHEN d.NivelActual = ''Titular5'' THEN 0 -- Ya está en el máximo nivel
            WHEN DATEDIFF(YEAR, d.FechaInicioNivelActual, GETDATE()) < 4 THEN 0 -- No cumple tiempo mínimo
            WHEN (SELECT COUNT(*) FROM SolicitudesAscenso sa WHERE sa.DocenteId = d.Id AND sa.Estado IN (''Pendiente'', ''EnProceso'')) > 0 THEN 0 -- Tiene solicitud activa
            ELSE 1
        END AS PuedeCrearSolicitud
        
    FROM Docentes d
    WHERE d.EstaActivo = 1');
    
    PRINT '✓ Vista VW_DocentesConRequisitos creada';
END
ELSE
BEGIN
    PRINT '⚠ Vista VW_DocentesConRequisitos ya existe';
END

-- Crear función para calcular si un docente cumple requisitos para un nivel específico
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE type = 'FN' AND name = 'FN_CumpleRequisitosParaNivel')
BEGIN
    PRINT 'Creando función FN_CumpleRequisitosParaNivel...';
    
    EXEC('CREATE FUNCTION FN_CumpleRequisitosParaNivel(
        @DocenteId UNIQUEIDENTIFIER,
        @NivelObjetivo NVARCHAR(50)
    )
    RETURNS BIT
    AS
    BEGIN
        DECLARE @Cumple BIT = 0;
        DECLARE @AnosEnNivel INT;
        DECLARE @Obras INT;
        DECLARE @Evaluacion DECIMAL(5,2);
        DECLARE @Capacitacion INT;
        DECLARE @Investigacion INT;
        
        -- Obtener datos del docente
        SELECT 
            @AnosEnNivel = DATEDIFF(YEAR, FechaInicioNivelActual, GETDATE()),
            @Obras = ISNULL(NumeroObrasAcademicas, 0),
            @Evaluacion = ISNULL(PromedioEvaluaciones, 0),
            @Capacitacion = ISNULL(HorasCapacitacion, 0),
            @Investigacion = ISNULL(MesesInvestigacion, 0)
        FROM Docentes 
        WHERE Id = @DocenteId;
        
        -- Validar requisitos según el nivel objetivo
        IF @NivelObjetivo = ''Titular2''
        BEGIN
            IF @AnosEnNivel >= 4 AND @Obras >= 1 AND @Evaluacion >= 75 AND @Capacitacion >= 96
                SET @Cumple = 1;
        END
        ELSE IF @NivelObjetivo = ''Titular3''
        BEGIN
            IF @AnosEnNivel >= 4 AND @Obras >= 2 AND @Evaluacion >= 75 AND @Capacitacion >= 96 AND @Investigacion >= 12
                SET @Cumple = 1;
        END
        ELSE IF @NivelObjetivo = ''Titular4''
        BEGIN
            IF @AnosEnNivel >= 4 AND @Obras >= 3 AND @Evaluacion >= 75 AND @Capacitacion >= 128 AND @Investigacion >= 24
                SET @Cumple = 1;
        END
        ELSE IF @NivelObjetivo = ''Titular5''
        BEGIN
            IF @AnosEnNivel >= 4 AND @Obras >= 5 AND @Evaluacion >= 75 AND @Capacitacion >= 160 AND @Investigacion >= 24
                SET @Cumple = 1;
        END
        
        RETURN @Cumple;
    END');
    
    PRINT '✓ Función FN_CumpleRequisitosParaNivel creada';
END
ELSE
BEGIN
    PRINT '⚠ Función FN_CumpleRequisitosParaNivel ya existe';
END

PRINT 'Datos iniciales y configuración aplicados correctamente.';
GO
