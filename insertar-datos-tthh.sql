-- Insertar datos de prueba en la tabla DatosTTHH
INSERT INTO DatosTTHH (Cedula, Nombres, Apellidos, FacultadId, EmailPersonal, EmailInstitucional, FechaIngreso, FechaRegistro, FechaActualizacion, Activo)
VALUES 
('1804567890', 'Daniel Eduardo', 'Jerez Mayorga', 1, 'daniel.jerez@gmail.com', '', '2020-01-01', GETDATE(), GETDATE(), 1),
('1712345678', 'María Elena', 'García López', 2, 'maria.garcia@hotmail.com', '', '2022-01-01', GETDATE(), GETDATE(), 1),
('1823456789', 'Carlos Antonio', 'Rodriguez Vaca', 1, 'c.rodriguez@yahoo.com', '', '2018-01-01', GETDATE(), GETDATE(), 1),
('1934567890', 'Ana Cristina', 'Morales Silva', 3, 'ana.morales@outlook.com', '', '2023-01-01', GETDATE(), GETDATE(), 1),
('1045678901', 'José Luis', 'Hernández Torres', 4, 'jose.hernandez@gmail.com', '', '2021-01-01', GETDATE(), GETDATE(), 1);

-- Verificar los datos insertados
SELECT Cedula, Nombres, Apellidos, EmailPersonal, EmailInstitucional FROM DatosTTHH;
