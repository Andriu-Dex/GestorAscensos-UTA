-- Actualizar contraseña de Steven Paredes con hash correcto
USE SGA_Main;

-- Nuevo hash para la contraseña "123456"
UPDATE Usuarios 
SET PasswordHash = '$2a$11$uQrF6pby7knqbHE/k3ZjG.GxuwPCWFEhW/G48yUgCCYjsKPrnu2A6'
WHERE Email = 'sparedes@uta.edu.ec';

-- Verificar la actualización
SELECT 
    Email,
    LEN(PasswordHash) as HashLength,
    PasswordHash
FROM Usuarios 
WHERE Email = 'sparedes@uta.edu.ec';

PRINT 'Contraseña actualizada correctamente para 123456';
