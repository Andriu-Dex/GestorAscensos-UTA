-- Script para actualizar la contraseña de Steven Paredes
USE SGA_Main;

-- Actualizar el hash de la contraseña
UPDATE Usuarios 
SET PasswordHash = '$2a$11$Ky2XR0gSQAYPag27Gfdg4O3h5aNnrBwU4JfVgLudVR/9U88NyEf4mu'
WHERE Email = 'sparedes@uta.edu.ec';

-- Verificar la actualización
SELECT 
    Email,
    LEN(PasswordHash) as HashLength,
    LEFT(PasswordHash, 20) + '...' as HashPreview
FROM Usuarios 
WHERE Email = 'sparedes@uta.edu.ec';

PRINT 'Contraseña actualizada correctamente';
