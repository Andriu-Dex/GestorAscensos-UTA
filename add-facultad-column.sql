-- Script para agregar la columna Facultad a la tabla EmpleadosTTHH
USE TTHH;

-- Agregar la columna Facultad a la tabla EmpleadosTTHH
ALTER TABLE EmpleadosTTHH 
ADD Facultad NVARCHAR(200) NOT NULL DEFAULT '';

-- Actualizar Steven Paredes con una facultad de ejemplo
UPDATE EmpleadosTTHH 
SET Facultad = 'Facultad de Ingeniería en Sistemas, Electrónica e Industrial'
WHERE Cedula = '1803154278';

-- Verificar la actualización
SELECT 
    Cedula,
    Nombres,
    Apellidos,
    Email,
    CargoActual,
    Facultad
FROM EmpleadosTTHH 
WHERE Cedula = '1803154278';

PRINT 'Columna Facultad agregada correctamente a la tabla EmpleadosTTHH';
