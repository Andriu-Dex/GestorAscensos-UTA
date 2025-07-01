# Datos Semilla para Bases de Datos Externas

## Base de Datos TTHH (Talento Humano)

Para que los docentes puedan ser encontrados durante el registro y la importación de datos, necesitamos agregar datos en las bases de datos externas simuladas.

### Docente Steven Paredes

Insertar en la base de datos TTHH los siguientes datos:

```sql
-- Base de datos TTHH_DB
USE TTHH_DB;

-- Insertar Steven Paredes en Talento Humano
INSERT INTO Empleados (
    Cedula,
    Nombres,
    Apellidos,
    Email,
    Activo,
    NivelAcademico,
    NivelActual,
    DiasEnNivelActual,
    FechaNombramiento,
    CargoActual,
    FechaInicioCargoActual,
    FechaIngresoNivelActual,
    Facultad,
    Departamento
) VALUES (
    '1800123456',
    'Steven',
    'Paredes',
    'sparedes@uta.edu.ec',
    1,
    'Titular Auxiliar',
    'Titular1',
    730, -- Aproximadamente 2 años (730 días)
    '2021-06-01 08:00:00',
    'Docente Titular Auxiliar 1',
    '2023-06-29 08:00:00',
    '2023-06-29 08:00:00',
    'Facultad de Ingeniería',
    'Departamento de Sistemas'
);

-- Verificar inserción
SELECT * FROM Empleados WHERE Cedula = '1800123456';
```

## Base de Datos DAC (Evaluación Docente)

```sql
-- Base de datos DAC_DB
USE DAC_DB;

-- Insertar evaluaciones para Steven Paredes
INSERT INTO EvaluacionesDocentes (
    CedulaDocente,
    Periodo,
    Porcentaje,
    Fecha,
    EstudiantesEvaluaron
) VALUES
('1800123456', '2023-1', 78.5, '2023-07-15', 45),
('1800123456', '2023-2', 82.3, '2023-12-20', 52),
('1800123456', '2024-1', 79.8, '2024-07-15', 48),
('1800123456', '2024-2', 81.2, '2024-12-20', 50);

-- Verificar inserción
SELECT * FROM EvaluacionesDocentes WHERE CedulaDocente = '1800123456' ORDER BY Fecha;
```

## Base de Datos DITIC (Capacitación)

```sql
-- Base de datos DITIC_DB
USE DITIC_DB;

-- Insertar participaciones en cursos para Steven Paredes
INSERT INTO ParticipacionCursos (
    CedulaDocente,
    CodigoCurso,
    NombreCurso,
    FechaInicio,
    FechaFin,
    HorasDuracion,
    Completado
) VALUES
('1800123456', 'CAP001', 'Metodologías de Enseñanza Activa', '2023-08-01', '2023-08-15', 40, 1),
('1800123456', 'CAP002', 'Tecnologías Educativas Digitales', '2023-11-10', '2023-11-25', 32, 1),
('1800123456', 'CAP003', 'Evaluación por Competencias', '2024-03-15', '2024-03-30', 35, 1),
('1800123456', 'CAP004', 'Investigación Pedagógica', '2024-09-05', '2024-09-20', 28, 1);

-- Verificar inserción
SELECT * FROM ParticipacionCursos WHERE CedulaDocente = '1800123456' ORDER BY FechaInicio;
```

## Base de Datos DIR INV (Investigación)

```sql
-- Base de datos DIRINV_DB
USE DIRINV_DB;

-- Insertar obras académicas para Steven Paredes
INSERT INTO ObrasAcademicas (
    CedulaDocente,
    Titulo,
    TipoObra,
    FechaPublicacion,
    Editorial,
    Revista,
    ISBN_ISSN,
    DOI,
    EsIndexada,
    IndiceIndexacion,
    Autores,
    Descripcion
) VALUES
('1800123456', 'Innovaciones en Metodologías de Enseñanza para Ingeniería', 'Artículo', '2024-03-15',
 NULL, 'Revista de Educación en Ingeniería', '2345-6789', '10.1234/rei.2024.001', 1, 'Latindex',
 'Steven Paredes, María González', 'Estudio sobre nuevas metodologías aplicadas en enseñanza de ingeniería');

-- Insertar proyectos de investigación
INSERT INTO ProyectosInvestigacion (
    CedulaDocente,
    CodigoProyecto,
    Titulo,
    FechaInicio,
    FechaFin,
    Estado,
    RolDocente,
    MesesParticipacion
) VALUES
('1800123456', 'INV-2024-001', 'Metodologías Activas en Educación Superior', '2024-01-01', '2024-12-31',
 'Activo', 'Investigador Principal', 12);

-- Verificar inserción
SELECT * FROM ObrasAcademicas WHERE CedulaDocente = '1800123456';
SELECT * FROM ProyectosInvestigacion WHERE CedulaDocente = '1800123456';
```

## Resumen de Datos Creados

### 👤 Usuarios en Sistema Principal (SGA)

1. **Administrador**

   - Cédula: `999999999`
   - Email: `admin@uta.edu.ec`
   - Password: `Admin12345`
   - Rol: Administrador
   - Nivel: Titular 5

2. **Steven Paredes**
   - Cédula: `1800123456`
   - Email: `sparedes@uta.edu.ec`
   - Password: `Steven123!`
   - Rol: Docente
   - Nivel: Titular 1

### 📊 Datos Externos para Steven Paredes

- **TTHH**: Empleado activo con 2 años en nivel actual
- **DAC**: 4 evaluaciones con promedio ~80.5%
- **DITIC**: 4 cursos completados, 135 horas totales
- **DIR INV**: 1 artículo publicado, 1 proyecto activo (12 meses)

### 🎯 Uso en Registro

Ahora cuando un usuario intente registrarse con:

- **Email**: `sparedes@uta.edu.ec`
- **Cédula**: `1800123456`

El sistema podrá:

1. ✅ Encontrar los datos en TTHH
2. ✅ Validar que existe como empleado activo
3. ✅ Crear la cuenta automáticamente
4. ✅ Permitir la importación de datos reales

## 🚀 Próximos Pasos

1. Ejecutar los scripts SQL en las bases de datos externas correspondientes
2. Verificar la conectividad entre bases
3. Probar el registro con las credenciales de Steven Paredes
4. Validar la importación de datos desde cada sistema externo

## 🛠️ Scripts de Datos Semilla

Para facilitar la gestión de datos semilla, se han creado scripts automatizados que simplifican el proceso de configuración inicial de la base de datos.

### 📁 Scripts Disponibles

Los scripts se encuentran en la carpeta `Scripts\Seed\`:

#### **Script Principal: `seed-simple.ps1`**

Script simplificado para tareas comunes de datos semilla con las siguientes funcionalidades:

##### 🎯 **Comandos Disponibles:**

- **`basic`** - Crear datos básicos (admin + docente test)
- **`reset-seed`** - Limpiar BD y aplicar datos semilla
- **`verify`** - Verificar datos semilla existentes
- **`clean`** - Limpiar solo los datos semilla
- **`help`** - Mostrar ayuda completa

##### 🚀 **Ejemplos de Uso:**

```powershell
# Aplicar datos semilla básicos
.\Scripts\Seed\seed-simple.ps1 -Action basic

# Reset completo con datos semilla (con confirmación)
.\Scripts\Seed\seed-simple.ps1 -Action reset-seed

# Reset completo sin confirmación
.\Scripts\Seed\seed-simple.ps1 -Action reset-seed -Force

# Verificar estado de datos semilla
.\Scripts\Seed\seed-simple.ps1 -Action verify

# Ver ayuda completa
.\Scripts\Seed\seed-simple.ps1 -Action help
```

##### ✅ **Características del Script:**

- **Validación automática** de conexión a base de datos
- **Confirmación de seguridad** para operaciones destructivas
- **Colores informativos** para mejor experiencia de usuario
- **Manejo robusto de errores** con mensajes claros
- **Verificación de directorio** correcto de ejecución
- **Aplicación automática** de migraciones si es necesario

### 📋 **Flujo de Trabajo Recomendado**

#### **Para Configuración Inicial:**

```powershell
# 1. Verificar estado actual del sistema
.\Scripts\Seed\seed-simple.ps1 -Action verify

# 2. Si la base de datos ya existe, aplicar datos básicos
.\Scripts\Seed\seed-simple.ps1 -Action basic

# 3. Si necesitas empezar desde cero
.\Scripts\Seed\seed-simple.ps1 -Action reset-seed -Force
```

#### **Para Desarrollo Continuo:**

```powershell
# Limpiar y resetear cuando sea necesario
.\Scripts\Seed\seed-simple.ps1 -Action reset-seed

# Verificar que todo esté en orden
.\Scripts\Seed\seed-simple.ps1 -Action verify
```

### 🎯 **Datos Aplicados Automáticamente**

El script aplica automáticamente los siguientes datos semilla:

#### **👤 Usuario Administrador:**

- **Email:** `admin@uta.edu.ec`
- **Password:** `Admin12345`
- **Cédula:** `999999999`
- **Nombre:** Admin Global
- **Rol:** Administrador
- **Nivel:** Titular 5

#### **👨‍🏫 Docente de Prueba:**

- **Email:** `sparedes@uta.edu.ec`
- **Password:** `Steven123*`
- **Cédula:** `1800000001`
- **Nombre:** Steven Paredes
- **Rol:** Docente
- **Nivel:** Titular 1

### 🔧 **Ventajas de Usar los Scripts**

- ✅ **Automatización completa** del proceso de seeding
- ✅ **Consistencia** en datos de prueba entre entornos
- ✅ **Facilidad de uso** con comandos simples
- ✅ **Seguridad** con confirmaciones para operaciones destructivas
- ✅ **Feedback visual** con colores y mensajes informativos
- ✅ **Manejo de errores** robusto y comprensible

### 💡 **Notas Importantes**

- Los scripts deben ejecutarse desde la **raíz del proyecto** (donde está la carpeta `SGA.Api`)
- El comando `reset-seed` eliminará **todos los datos existentes** en la base de datos
- Use la bandera `-Force` solo cuando esté completamente seguro
- Siempre verifique el estado con `verify` después de aplicar cambios
