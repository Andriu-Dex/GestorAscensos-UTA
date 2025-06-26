-- Script para resetear la contraseña de Steven Paredes o crear el usuario si no existe
USE SGA_Main;

-- Verificar si el usuario existe
DECLARE @UsuarioId UNIQUEIDENTIFIER
SELECT @UsuarioId = Id FROM Usuarios WHERE Email = 'sparedes@uta.edu.ec'

IF @UsuarioId IS NOT NULL
BEGIN
    -- El usuario existe, actualizar solo la contraseña
    -- Contraseña "123456" hasheada con BCrypt
    UPDATE Usuarios 
    SET PasswordHash = '$2a$11$6EV8uv7NhfBLeWHkRZ5LJuOPBtJD8iJZfyE8.WqBBBzWgNKjE1fke',
        IntentosLogin = 0,
        UltimoBloqueado = NULL,
        FechaModificacion = GETUTCDATE()
    WHERE Id = @UsuarioId

    PRINT 'Contraseña actualizada para Steven Paredes. Nueva contraseña: 123456'
END
ELSE
BEGIN
    -- El usuario no existe, crearlo
    DECLARE @NuevoUsuarioId UNIQUEIDENTIFIER = NEWID()
    
    -- Insertar usuario
    INSERT INTO Usuarios (Id, Email, PasswordHash, Rol, EstaActivo, IntentosLogin, UltimoLogin, FechaCreacion)
    VALUES (
        @NuevoUsuarioId,
        'sparedes@uta.edu.ec',
        '$2a$11$6EV8uv7NhfBLeWHkRZ5LJuOPBtJD8iJZfyE8.WqBBBzWgNKjE1fke', -- 123456
        'Docente',
        1,
        0,
        GETUTCDATE(),
        GETUTCDATE()
    )
    
    -- Insertar docente
    INSERT INTO Docentes (Id, Cedula, Nombres, Apellidos, Email, NivelActual, FechaInicioNivelActual, UsuarioId, EstaActivo, FechaCreacion)
    VALUES (
        NEWID(),
        '1805123456',
        'Steven Alexander',
        'Paredes',
        'sparedes@uta.edu.ec',
        'Titular2',
        '2019-06-25',
        @NuevoUsuarioId,
        1,
        GETUTCDATE()
    )
    
    PRINT 'Usuario Steven Paredes creado exitosamente. Email: sparedes@uta.edu.ec, Contraseña: 123456'
END

-- Verificar el resultado
SELECT 
    u.Email,
    u.Rol,
    u.EstaActivo,
    d.Nombres + ' ' + d.Apellidos as NombreCompleto,
    d.Cedula,
    d.NivelActual
FROM Usuarios u
INNER JOIN Docentes d ON u.Id = d.UsuarioId
WHERE u.Email = 'sparedes@uta.edu.ec'
