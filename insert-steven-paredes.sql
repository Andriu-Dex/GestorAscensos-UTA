-- Script para insertar Steven Paredes en TTHH
USE TTHH;

-- Insertar Steven Paredes
INSERT INTO EmpleadosTTHH (
    Cedula, 
    Nombres, 
    Apellidos, 
    Email, 
    CorreoInstitucional, 
    Celular, 
    FechaNombramiento, 
    CargoActual, 
    NivelAcademico, 
    Direccion, 
    FechaNacimiento, 
    EstadoCivil, 
    TipoContrato, 
    EstaActivo
) VALUES (
    '1805123456',
    'Steven Alexander',
    'Paredes',
    'steven.paredes@gmail.com',
    'sparedes@uta.edu.ec',
    '0987654321',
    '2019-06-25 08:00:00',
    'Docente Titular 2',
    'Magíster',
    'Av. Los Shyris 123 y Amazonas, Ambato',
    '1985-03-15 00:00:00',
    'Soltero',
    'Contrato Indefinido',
    1
);

-- Insertar algunos empleados adicionales para pruebas
INSERT INTO EmpleadosTTHH (
    Cedula, 
    Nombres, 
    Apellidos, 
    Email, 
    CorreoInstitucional, 
    Celular, 
    FechaNombramiento, 
    CargoActual, 
    NivelAcademico, 
    Direccion, 
    FechaNacimiento, 
    EstadoCivil, 
    TipoContrato, 
    EstaActivo
) VALUES 
('1800000001', 'María Elena', 'Rodríguez', 'maria.rodriguez@gmail.com', 'mrodriguez@uta.edu.ec', '0987123456', '2020-01-15 08:00:00', 'Docente Titular 1', 'PhD', 'Calle 123 #45, Ambato', '1980-05-20 00:00:00', 'Casada', 'Nombramiento', 1),
('1800000002', 'Carlos Alberto', 'Morales', 'carlos.morales@gmail.com', 'cmorales@uta.edu.ec', '0987234567', '2018-08-10 08:00:00', 'Docente Titular 3', 'Magíster', 'Av. Cevallos 67, Ambato', '1975-11-12 00:00:00', 'Casado', 'Contrato Indefinido', 1),
('1800000003', 'Ana Cristina', 'Vásquez', 'ana.vasquez@gmail.com', 'avasquez@uta.edu.ec', '0987345678', '2021-02-28 08:00:00', 'Docente Titular 1', 'Licenciada', 'Calle Bolívar 89, Ambato', '1988-07-03 00:00:00', 'Soltera', 'Contrato a Término Fijo', 1),
('1800000004', 'Luis Fernando', 'Herrera', 'luis.herrera@gmail.com', 'lherrera@uta.edu.ec', '0987456789', '2017-09-05 08:00:00', 'Docente Titular 4', 'PhD', 'Av. Atahualpa 234, Ambato', '1970-12-18 00:00:00', 'Casado', 'Nombramiento', 1);

-- Insertar algunos cargos
INSERT INTO CargosTTHH (NombreCargo, NivelTitular, Descripcion, EstaActivo) VALUES
('Docente Titular 1', 'Titular1', 'Nivel inicial de carrera docente', 1),
('Docente Titular 2', 'Titular2', 'Segundo nivel de carrera docente', 1),
('Docente Titular 3', 'Titular3', 'Tercer nivel de carrera docente', 1),
('Docente Titular 4', 'Titular4', 'Cuarto nivel de carrera docente', 1),
('Docente Titular 5', 'Titular5', 'Nivel máximo de carrera docente', 1);

PRINT 'Steven Paredes y datos de prueba insertados exitosamente en TTHH';
