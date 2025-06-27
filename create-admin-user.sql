-- Script para crear usuario administrador
-- Usuario: Admin Global
-- Email: admin@uta.edu.ec
-- Contraseña: Admin12345
-- Cédula: 999999999

USE [SGA_Main]
GO

-- Primero verificar si ya existe el usuario
IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Email = 'admin@uta.edu.ec')
BEGIN
    -- Crear usuario administrador
    -- Password hash para "Admin12345" usando BCrypt
    INSERT INTO Usuarios (Id, Email, PasswordHash, Rol, Activo, FechaCreacion, FechaModificacion)
    VALUES (
        NEWID(),
        'admin@uta.edu.ec',
        '$2a$11$29SLx0jJy8aqMcln6NB61eIKVR6AiI7GCdMsZuoF0Y8END3lrEnFO', -- Hash real de Admin12345
        'Administrador',
        1,
        GETUTCDATE(),
        NULL
    );

    DECLARE @AdminUserId UNIQUEIDENTIFIER = (SELECT Id FROM Usuarios WHERE Email = 'admin@uta.edu.ec');

    -- Crear docente asociado al usuario administrador
    INSERT INTO Docentes (Id, UsuarioId, Cedula, Nombres, Apellidos, Email, NivelActual, FechaIngresoUTA, FechaInicioNivelActual, PromedioEvaluaciones, NumeroObrasAcademicas, HorasCapacitacion, MesesInvestigacion, CumpleRequisitos, FechaCreacion, FechaModificacion)
    VALUES (
        NEWID(),
        @AdminUserId,
        '999999999',
        'Admin',
        'Global',
        'admin@uta.edu.ec',
        'Titular5', -- Nivel más alto
        '2015-01-01', -- Fecha de ingreso antigua
        '2020-01-01', -- Fecha inicio nivel actual
        95.0, -- Excelente promedio
        10, -- Muchas obras académicas
        500, -- Muchas horas de capacitación
        60, -- Muchos meses de investigación
        1, -- Cumple todos los requisitos
        GETUTCDATE(),
        NULL
    );

    PRINT 'Usuario administrador creado exitosamente';
    PRINT 'Email: admin@uta.edu.ec';
    PRINT 'Contraseña: Admin12345';
    PRINT 'Cédula: 999999999';
END
ELSE
BEGIN
    PRINT 'El usuario administrador ya existe';
END
GO
