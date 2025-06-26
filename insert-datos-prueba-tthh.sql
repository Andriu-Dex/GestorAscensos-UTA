-- Script para insertar datos de prueba en TTHH
USE TTHH;

-- Limpiar datos existentes
DELETE FROM EmpleadosTTHH;
DELETE FROM CargosTTHH;

-- Insertar cargos
INSERT INTO CargosTTHH (NombreCargo, NivelTitular, Descripcion, EstaActivo) VALUES
('Docente Titular 1', 'Titular1', 'Docente Titular Nivel 1', 1),
('Docente Titular 2', 'Titular2', 'Docente Titular Nivel 2', 1),
('Docente Titular 3', 'Titular3', 'Docente Titular Nivel 3', 1),
('Docente Titular 4', 'Titular4', 'Docente Titular Nivel 4', 1),
('Docente Titular 5', 'Titular5', 'Docente Titular Nivel 5', 1);

-- Función para generar correo institucional
-- Formato: primera letra del nombre + apellido + @uta.edu.ec

-- Insertar empleados de prueba
INSERT INTO EmpleadosTTHH (
    Cedula, Nombres, Apellidos, Email, CorreoInstitucional, Celular,
    FechaNombramiento, CargoActual, NivelAcademico, Direccion,
    FechaNacimiento, EstadoCivil, TipoContrato, EstaActivo
) VALUES
(
    '1234567890', 
    'Juan Carlos', 
    'Pérez González', 
    'juan.perez@gmail.com',
    'jperez@uta.edu.ec',
    '0987654321',
    '2020-02-15',
    'Docente Titular 1',
    'PhD en Ingeniería',
    'Av. Los Andes 123, Ambato',
    '1985-03-20',
    'Casado',
    'Nombramiento',
    1
),
(
    '0987654321', 
    'María Elena', 
    'Rodríguez Silva', 
    'maria.rodriguez@hotmail.com',
    'mrodriguez@uta.edu.ec',
    '0998765432',
    '2019-08-10',
    'Docente Titular 2',
    'Magíster en Educación',
    'Calle Bolívar 456, Ambato',
    '1982-07-15',
    'Soltera',
    'Nombramiento',
    1
),
(
    '1122334455', 
    'Carlos Alberto', 
    'Martínez López', 
    'carlos.martinez@yahoo.com',
    'cmartinez@uta.edu.ec',
    '0976543210',
    '2018-01-20',
    'Docente Titular 3',
    'PhD en Ciencias',
    'Av. Cevallos 789, Ambato',
    '1978-11-08',
    'Casado',
    'Nombramiento',
    1
),
(
    '5544332211', 
    'Ana Sofía', 
    'García Herrera', 
    'ana.garcia@gmail.com',
    'agarcia@uta.edu.ec',
    '0965432109',
    '2021-03-05',
    'Docente Titular 1',
    'Magíster en Administración',
    'Calle Sucre 321, Ambato',
    '1990-05-12',
    'Soltera',
    'Nombramiento',
    1
),
(
    '9988776655', 
    'Pedro Luis', 
    'Vásquez Morales', 
    'pedro.vasquez@outlook.com',
    'pvasquez@uta.edu.ec',
    '0954321098',
    '2017-11-18',
    'Docente Titular 4',
    'PhD en Ingeniería Industrial',
    'Av. Atahualpa 654, Ambato',
    '1975-09-25',
    'Casado',
    'Nombramiento',
    1
);

-- Insertar acciones de personal para histórico
INSERT INTO AccionesPersonalTTHH (Cedula, TipoAccion, FechaAccion, CargoAnterior, CargoNuevo, Observaciones) VALUES
('1234567890', 'Nombramiento', '2020-02-15', '', 'Docente Titular 1', 'Nombramiento inicial'),
('0987654321', 'Nombramiento', '2019-08-10', '', 'Docente Titular 2', 'Nombramiento inicial'),
('0987654321', 'Ascenso', '2022-08-10', 'Docente Titular 1', 'Docente Titular 2', 'Ascenso por méritos académicos'),
('1122334455', 'Nombramiento', '2018-01-20', '', 'Docente Titular 1', 'Nombramiento inicial'),
('1122334455', 'Ascenso', '2020-01-20', 'Docente Titular 1', 'Docente Titular 2', 'Ascenso por méritos académicos'),
('1122334455', 'Ascenso', '2022-01-20', 'Docente Titular 2', 'Docente Titular 3', 'Ascenso por méritos académicos'),
('5544332211', 'Nombramiento', '2021-03-05', '', 'Docente Titular 1', 'Nombramiento inicial'),
('9988776655', 'Nombramiento', '2017-11-18', '', 'Docente Titular 1', 'Nombramiento inicial'),
('9988776655', 'Ascenso', '2019-11-18', 'Docente Titular 1', 'Docente Titular 2', 'Ascenso por méritos académicos'),
('9988776655', 'Ascenso', '2021-11-18', 'Docente Titular 2', 'Docente Titular 3', 'Ascenso por méritos académicos'),
('9988776655', 'Ascenso', '2023-11-18', 'Docente Titular 3', 'Docente Titular 4', 'Ascenso por méritos académicos');

SELECT 'Datos de prueba insertados exitosamente en TTHH' as Resultado;

-- Verificar los datos insertados
SELECT 
    Cedula, 
    Nombres + ' ' + Apellidos as NombreCompleto,
    CorreoInstitucional,
    CargoActual,
    CASE WHEN EstaActivo = 1 THEN 'Activo' ELSE 'Inactivo' END as Estado
FROM EmpleadosTTHH 
ORDER BY Nombres;
