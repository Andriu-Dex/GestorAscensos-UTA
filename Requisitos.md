# Instrucciones para Sistema de Gestión de Ascensos Docentes - Versión Base

## 1. Objetivo del Sistema

Desarrollar un sistema web **completamente funcional** para la gestión de ascensos docentes que permita:

- Registro e inicio de sesión de docentes
- Importación **real** de datos desde bases de datos externas
- Validación automática de requisitos de ascenso
- Creación y gestión de solicitudes de ascenso
- Aprobación/rechazo por parte de administradores
- Generación de reportes

## 2. Arquitectura del Sistema

### Tecnologías a Utilizar

- **Frontend**: Blazor WebAssembly
- **Backend**: ASP.NET Core 9 Web API
- **Base de Datos**: SQL Server Express
- **Autenticación**: JWT (JSON Web Tokens)
- **Arquitectura**: Onion Architecture (4 capas)

### Estructura de Capas

1. **Capa de Dominio**: Entidades principales (Docente, SolicitudAscenso, Documento)
2. **Capa de Aplicación**: Casos de uso y lógica de negocio
3. **Capa de Infraestructura**: Acceso a datos y servicios externos
4. **Capa de Presentación**: API REST y cliente Blazor

### Stack Tecnológico Optimizado para Desarrollo Acelerado

**Frontend (Blazor WebAssembly)**

- **MudBlazor**: Componentes Material Design completos (DataGrid, FileUpload, Charts)
- **Blazored.LocalStorage**: Gestión de tokens JWT en cliente
- **Microsoft.AspNetCore.Components.WebAssembly.Authentication**: Autenticación integrada

**Backend (ASP.NET Core 9)**

- **MediatR**: Patrón CQRS para separar comandos y consultas
- **AutoMapper**: Mapeo automático entre DTOs y entidades
- **FluentValidation**: Validaciones fluidas y expresivas
- **Entity Framework Core**: ORM para base de datos principal
- **Dapper**: Micro-ORM para consultas rápidas a bases externas
- **Serilog**: Logging estructurado y configurable

**Gestión de Archivos y Reportes**

- **iTextSharp**: Generación de PDFs para reportes
- **ImageSharp**: Compresión y optimización de archivos
- **Swashbuckle**: Documentación automática de APIs

**Autenticación y Seguridad**

- **Microsoft.AspNetCore.Authentication.JwtBearer**: Manejo de JWT
- **BCrypt.Net**: Hash seguro de contraseñas
- **Microsoft.AspNetCore.Identity**: Gestión de usuarios y roles

**Testing y Calidad**

- **xUnit**: Framework de testing
- **FluentAssertions**: Assertions más legibles
- **Moq**: Mocking para unit tests
- **Microsoft.AspNetCore.Mvc.Testing**: Integration tests

**DevOps y Productividad**

- **Docker**: Containerización para deployment
- **GitHub Actions**: CI/CD automatizado
- **SQL Server LocalDB**: Base de datos de desarrollo

## 3. Base de Datos Completa

### Base de Datos Principal del Sistema

- **Usuarios**: Información de login y roles
- **Docentes**: Datos personales y nivel actual
- **SolicitudesAscenso**: Solicitudes con estado y documentos
- **Documentos**: Archivos PDF adjuntos
- **LogAuditoria**: Registro de operaciones importantes

### Bases de Datos Externas (Reales y Separadas)

Crear bases de datos independientes que representen los sistemas universitarios externos:

**Base de Datos TTHH (Talento Humano)**

- Tabla de empleados con datos personales y laborales
- Tabla de acciones de personal con historial de cambios
- Tabla de cargos y niveles académicos
- Poblar con al menos 50 docentes con datos realistas

**Base de Datos DAC (Evaluación Docente)**

- Tabla de evaluaciones docentes por período
- Tabla de períodos académicos
- Tabla de criterios de evaluación
- Poblar con evaluaciones de múltiples períodos para cada docente

**Base de Datos DITIC (Capacitación)**

- Tabla de cursos disponibles
- Tabla de participación en cursos por docente
- Tabla de certificaciones obtenidas
- Poblar con variedad de cursos y participaciones realistas

**Base de Datos DIR INV (Investigación)**

- Tabla de obras académicas (publicaciones, artículos, libros)
- Tabla de proyectos de investigación
- Tabla de participación en proyectos por docente
- Poblar con producción académica variada por docente

## 4. Funcionalidades Completamente Operativas

### Autenticación y Usuarios

- **Registro de docentes**
- **Login con JWT**: Tokens seguros con expiración configurable
- **Dos roles**: Docente (ve solo sus datos) y Administrador (ve todo)
- **Seguridad**: Bloqueo temporal después de 3 intentos fallidos

### Importación Real de Datos

- **Conexión a TTHH**: Obtener tiempo real en cargo actual y fecha de nombramiento
- **Conexión a DAC**: Calcular promedio real de evaluaciones de últimos 4 períodos
- **Conexión a DITIC**: Sumar horas reales de capacitación de últimos 3 años
- **Conexión a DIR INV**: Contar obras académicas y calcular tiempo en investigación
- **Botones funcionales**: Cada botón ejecuta consultas reales a bases de datos externas

### Validación Automática de Requisitos

El sistema debe validar automáticamente usando datos reales importados:

- **Titular Auxiliar 1 → 2**: 4 años en rol, 1 obra, 75% evaluación, 96h capacitación
- **Titular Auxiliar 2 → 3**: 4 años en rol, 2 obras, 75% evaluación, 96h capacitación, 12 meses investigación
- **Titular Auxiliar 3 → 4**: 4 años en rol, 3 obras, 75% evaluación, 128h capacitación, 24 meses investigación
- **Titular Auxiliar 4 → 5**: 4 años en rol, 5 obras, 75% evaluación, 160h capacitación, 24 meses investigación

### Gestión de Solicitudes de Ascenso

- **Creación**: Solo permitir si datos reales muestran cumplimiento de requisitos
- **Documentos**: Subida real de archivos PDF con compresión automática
- **Estados**: Pendiente, En Proceso, Aprobada, Rechazada
- **Workflow**: Administradores pueden revisar, aprobar o rechazar con motivos
- **Restricción**: Un docente solo puede tener una solicitud activa

### Reinicio de Contadores

- **Al aprobar ascenso**: Actualizar nivel del docente y fecha de promoción
- **Reinicio automático**: Los contadores de obras, horas, etc. se reinician automáticamente
- **Base para siguiente ascenso**: Nuevos cálculos desde fecha de último ascenso

## 5. APIs REST Principales

### Autenticación

- Endpoints para login, registro y renovación de tokens
- Validación de credenciales contra base de datos principal

### Docentes

- Obtener datos del docente autenticado
- Importar datos desde cada sistema externo (TTHH, DAC, DITIC, DIR INV)
- Validar requisitos para ascenso a nivel específico

### Solicitudes

- Crear nueva solicitud con documentos
- Listar solicitudes según rol del usuario
- Aprobar o rechazar solicitudes (solo administradores)

### Documentos

- Subir archivos PDF con validación y compresión
- Descargar documentos asociados a solicitudes

### Reportes

- Generar hoja de vida en PDF
- Generar reporte de proceso de ascenso

## 6. Cliente Blazor WebAssembly

### Páginas Principales

- **Login**: Autenticación de usuarios
- **Dashboard**: Estado actual del docente con resumen de requisitos
- **Mi Perfil**: Datos personales y nivel académico actual
- **Importar Datos**: Botones para traer datos de sistemas externos
- **Solicitar Ascenso**: Formulario para crear solicitud (solo si cumple requisitos)
- **Mis Solicitudes**: Historial de solicitudes del docente
- **Administración**: Panel para gestionar todas las solicitudes (solo admin)

### Funcionalidades de Interface

- **Indicadores visuales**: Mostrar claramente qué requisitos cumple y cuáles no
- **Progreso en tiempo real**: Actualizar estado después de importar datos
- **Validación de archivos**: Solo permitir PDFs con tamaño máximo
- **Notificaciones**: Alertas sobre cambios de estado en solicitudes

## 7. Validaciones y Reglas de Negocio

### Validaciones de Datos

- **Archivos PDF**: Verificar formato y tamaño máximo
- **Requisitos de ascenso**: Validación automática con datos reales
- **Ascenso secuencial**: Solo permitir ascenso al siguiente nivel

### Reglas de Negocio

- **Un docente, una solicitud**: No permitir solicitudes múltiples simultáneas
- **Reinicio de contadores**: Al ascender, reiniciar todos los indicadores
- **Fechas de corte**: Cálculos basados en fecha de inicio en nivel actual
- **Estados de solicitud**: Workflow claro y auditable

## 8. Manejo de Archivos

### Gestión de PDFs

- **Validación estricta**: Solo archivos PDF válidos
- **Compresión automática**: Reducir tamaño para optimizar almacenamiento
- **Almacenamiento seguro**: En base de datos con metadata completa
- **Descarga controlada**: Solo usuarios autorizados

### Generación de Reportes

- **Hoja de vida**: PDF con todos los datos del docente
- **Reporte de proceso**: Estado detallado de solicitud de ascenso
- **Formato profesional**: Documentos bien estructurados y presentables

## 9. Datos de Prueba Realistas

### Poblar Bases de Datos Externas

**TTHH**: Crear al menos 50 docentes con fechas de nombramiento variadas, algunos que cumplan 4 años en su nivel y otros que no.

**DAC**: Generar evaluaciones para múltiples períodos académicos, con puntajes variados donde algunos docentes superen 75% y otros no.

**DITIC**: Crear catálogo de cursos realistas y asignar participaciones variadas, algunos docentes con suficientes horas y otros sin completar.

**DIR INV**: Asignar obras académicas y proyectos de investigación de manera realista, variando la productividad entre docentes.

### Casos de Prueba Diversos

- **Docentes que cumplen**: Todos los requisitos para ascender
- **Docentes que no cumplen**: Faltan uno o más requisitos
- **Casos límite**: Docentes que están muy cerca de cumplir requisitos
- **Diferentes niveles**: Docentes en cada nivel de Titular Auxiliar 1 a Titular Auxiliar 4

## 10. Configuración y Seguridad

### Variables de Configuración

- **Cadenas de conexión**: Para base principal y las 4 bases externas
- **Configuración JWT**: Clave secreta, emisor, audiencia, tiempo de expiración
- **Configuración de archivos**: Tamaño máximo, tipos permitidos, ruta de almacenamiento
- **Configuración de requisitos**: Parámetros de ascenso por nivel

### Medidas de Seguridad

- **Autenticación JWT**: Tokens seguros con validación en cada request
- **Autorización por roles**: Control estricto de acceso según perfil
- **Validación de entrada**: Sanitización de todos los inputs del usuario
- **Auditoría**: Log de operaciones críticas (login, solicitudes, aprobaciones)

## 11. Entregables del Proyecto

### Sistema Funcional Completo

- **Aplicación web**: Blazor WebAssembly completamente operativa
- **API REST**: Todos los endpoints funcionando correctamente
- **Bases de datos**: Sistema principal + 4 bases externas con datos realistas
- **Autenticación**: Sistema de login funcional con roles
- **Importación**: Conexiones reales a bases de datos externas
- **Workflow**: Proceso completo de solicitud y aprobación de ascensos

### Documentación Técnica

- **Diagrama de arquitectura**: Mostrando las 4 capas de Onion Architecture
- **Modelo de datos**: Estructura de las 5 bases de datos
- **Documentación de APIs**: Descripción de todos los endpoints
- **Manual de instalación**: Pasos para configurar el sistema
- **Manual de usuario**: Guía para docentes y administradores

El sistema debe ser **completamente funcional** y demostrar todos los aspectos del proceso de ascensos docentes de manera real y operativa.

## 12. Estrategia de Desarrollo Acelerado

### Cronograma Optimizado (6-8 semanas)

**Semana 1-2: Fundación y Scaffolding**

- Setup inicial con templates predefinidos
- Configuración de MudBlazor y arquitectura base
- Entity Framework Code First con migraciones automáticas
- Scaffolding de controladores CRUD básicos
- Autenticación JWT con Identity

**Semana 3-4: Integración de Datos**

- Conexiones a bases de datos externas con Dapper
- Servicios de importación automatizados
- Poblado de datos de prueba con scripts automatizados
- Implementación de MediatR para casos de uso

**Semana 5-6: Lógica de Negocio Completa**

- Validación de requisitos con FluentValidation
- Workflow de solicitudes con estados
- File upload y gestión de PDFs
- Generación de reportes con iTextSharp

**Semana 7-8: Frontend y Pulimiento**

- Páginas principales con componentes MudBlazor
- Dashboard con indicadores visuales
- Testing automatizado
- Deployment y documentación

### Herramientas de Productividad

**Generadores de Código**

- Visual Studio Scaffolding para controladores
- T4 Templates para DTOs automáticos
- Entity Framework Power Tools para reverse engineering
- Swagger para documentación automática de APIs

**Templates y Snippets**

- Plantillas de componentes Blazor reutilizables
- Snippets de código para patrones comunes
- Templates de testing automatizado
- Scripts de deployment automatizado

### Paquetes NuGet Esenciales

```xml
<!-- Frontend (Blazor WebAssembly) -->
<PackageReference Include="MudBlazor" Version="6.11.0" />
<PackageReference Include="Blazored.LocalStorage" Version="4.4.0" />
<PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly.Authentication" Version="9.0.0" />

<!-- Backend (API) -->
<PackageReference Include="MediatR" Version="12.2.0" />
<PackageReference Include="AutoMapper" Version="12.0.1" />
<PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.0.1" />
<PackageReference Include="FluentValidation" Version="11.8.0" />
<PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="11.8.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.0" />
<PackageReference Include="Dapper" Version="2.1.24" />
<PackageReference Include="Serilog.AspNetCore" Version="8.0.0" />
<PackageReference Include="iTextSharp" Version="5.5.13.3" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="9.0.0" />
<PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />

<!-- Testing -->
<PackageReference Include="xunit" Version="2.6.2" />
<PackageReference Include="FluentAssertions" Version="6.12.0" />
<PackageReference Include="Moq" Version="4.20.69" />
<PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="9.0.0" />
```

### Optimizaciones de Desarrollo

**Automatización de Tareas Repetitivas**

- Scripts PowerShell para creación de entidades
- Generación automática de DTOs y mappers
- Seeding automático de bases de datos
- Deployment con un comando

**Reutilización de Componentes**

- Componentes Blazor genéricos (DataGrid, Forms, FileUpload)
- Servicios base reutilizables
- Validators genéricos con FluentValidation
- Repositories con patrones genéricos

**Testing Automatizado**

- Unit tests con datos de prueba automáticos
- Integration tests para APIs
- End-to-end tests para workflows críticos
- Performance tests para operaciones de importación
