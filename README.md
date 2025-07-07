# 🎓 Sistema de Gestión de Ascensos Docentes (SGA)

## 📋 Descripción General

El **Sistema de Gestión de Ascensos Docentes (SGA)** es una aplicación web integral desarrollada para la Universidad Técnica de Ambato que automatiza y gestiona el proceso completo de ascensos académicos de docentes. El sistema integra múltiples fuentes de datos institucionales y proporciona una plataforma moderna para la gestión administrativa y docente.

### 🎯 Objetivo Principal

Modernizar y automatizar el proceso de ascenso docente mediante:

- **Gestión centralizada** de solicitudes de ascenso
- **Integración automática** con sistemas institucionales (TTHH, DAC, DITIC, DIRINV)
- **Validación dinámica** de requisitos según niveles académicos
- **Seguimiento en tiempo real** del estado de solicitudes
- **Reportes administrativos** completos y estadísticas avanzadas

---

## 🏗️ Arquitectura del Sistema

### 📊 Diagrama de Arquitectura

```
┌─────────────────────────────────────────────────────────────┐
│                    SGA.Web (Frontend)                      │
│         Blazor WebAssembly + MudBlazor UI                  │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────┐ │
│  │ Páginas Docente │  │ Admin Dashboard │  │ Autenticación│ │
│  │ - Solicitudes   │  │ - Gestión       │  │ - JWT Auth  │ │
│  │ - Documentos    │  │ - Reportes      │  │ - LocalStorage│ │
│  │ - Perfil        │  │ - Estadísticas  │  │ - SignalR   │ │
│  └─────────────────┘  └─────────────────┘  └─────────────┘ │
└─────────────────────────────────────────────────────────────┘
                               │ HTTPS/REST API
                               ▼
┌─────────────────────────────────────────────────────────────┐
│                    SGA.Api (Backend)                       │
│              ASP.NET Core 9.0 Web API                      │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────┐ │
│  │ Controllers     │  │ SignalR Hub     │  │ Middleware  │ │
│  │ - Auth          │  │ - Notificaciones│  │ - CORS      │ │
│  │ - Solicitudes   │  │ - Tiempo Real   │  │ - JWT       │ │
│  │ - Reportes      │  │                 │  │ - Logging   │ │
│  └─────────────────┘  └─────────────────┘  └─────────────┘ │
└─────────────────────────────────────────────────────────────┘
                               │
                               ▼
┌─────────────────────────────────────────────────────────────┐
│               SGA.Application (Lógica de Negocio)          │
│                  CQRS + MediatR + AutoMapper               │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────┐ │
│  │ Services        │  │ DTOs            │  │ Features    │ │
│  │ - Notificaciones│  │ - Mappings      │  │ - Commands  │ │
│  │ - Integraciones │  │ - Validations   │  │ - Queries   │ │
│  │ - Reportes      │  │                 │  │ - Handlers  │ │
│  └─────────────────┘  └─────────────────┘  └─────────────┘ │
└─────────────────────────────────────────────────────────────┘
                               │
                               ▼
┌─────────────────────────────────────────────────────────────┐
│              SGA.Infrastructure (Datos)                    │
│                Entity Framework Core 9.0                   │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────┐ │
│  │ ApplicationDb   │  │ Repositories    │  │ External DBs│ │
│  │ - SGA_Main      │  │ - Generic       │  │ - TTHH      │ │
│  │ - Migrations    │  │ - Específicos   │  │ - DAC       │ │
│  │ - Seeding       │  │                 │  │ - DITIC     │ │
│  │                 │  │                 │  │ - DIRINV    │ │
│  └─────────────────┘  └─────────────────┘  └─────────────┘ │
└─────────────────────────────────────────────────────────────┘
                               │
                               ▼
┌─────────────────────────────────────────────────────────────┐
│                   SGA.Domain (Dominio)                     │
│                    Domain-Driven Design                    │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────┐ │
│  │ Entities        │  │ Enums           │  │ Extensions  │ │
│  │ - Docente       │  │ - NivelTitular  │  │ - Helpers   │ │
│  │ - SolicitudAscenso│ │ - EstadoSol   │  │ - Validations│ │
│  │ - Documento     │  │ - TipoNotif     │  │ - Constants │ │
│  └─────────────────┘  └─────────────────┘  └─────────────┘ │
└─────────────────────────────────────────────────────────────┘
```

### 🔧 Stack Tecnológico

#### **Frontend**

- **Blazor WebAssembly**: Framework principal de UI
- **MudBlazor**: Biblioteca de componentes Material Design
- **Blazored LocalStorage**: Almacenamiento local para autenticación
- **Blazored Toast**: Notificaciones y mensajes
- **Microsoft.AspNetCore.SignalR.Client**: Cliente para notificaciones en tiempo real

#### **Backend**

- **ASP.NET Core 9.0**: Framework web principal
- **Entity Framework Core 9.0**: ORM para acceso a datos
- **MediatR**: Implementación de patrón CQRS
- **AutoMapper**: Mapeo de objetos DTOs
- **FluentValidation**: Validaciones de modelos
- **SignalR**: Comunicación en tiempo real
- **Serilog**: Logging estructurado
- **BCrypt.Net**: Hashing de contraseñas

#### **Base de Datos**

- **SQL Server Express**: Base de datos principal (SGA_Main)
- **Múltiples bases de datos externas**: TTHH, DAC, DITIC, DIRINV
- **Entity Framework Migrations**: Control de versiones de esquema

#### **Herramientas Adicionales**

- **iText7**: Generación de PDFs
- **JWT**: Autenticación stateless
- **CORS**: Control de acceso cross-origin
- **DotNetEnv**: Gestión de variables de entorno

---

## 🚀 Funcionalidades Principales

### 👨‍🎓 **Para Docentes**

#### 📝 Gestión de Solicitudes

- **Crear solicitudes** de ascenso automáticamente validadas
- **Subir documentos** requeridos (PDFs con compresión automática)
- **Seguimiento en tiempo real** del estado de solicitudes
- **Historial completo** de todas las solicitudes realizadas
- **Reenvío de solicitudes** rechazadas con correcciones

#### 📊 Dashboard Personal

- **Perfil completo** con foto e información académica
- **Resumen de datos** importados de sistemas externos
- **Métricas personales** (evaluaciones, capacitaciones, obras, investigación)
- **Calendario de ascensos** elegibles
- **Notificaciones en tiempo real** de cambios de estado

#### 📄 Gestión Documental

- **Subida masiva** de documentos
- **Previsualización** de PDFs en el navegador
- **Control de duplicados** automático
- **Categorización** por tipo de documento
- **Descarga** de documentos propios

### 👨‍💼 **Para Administradores**

#### 🎛️ Panel de Control Administrativo

- **Dashboard principal** con métricas en tiempo real
- **Estadísticas avanzadas** por facultad, departamento y nivel
- **Gráficos interactivos** de tendencias y patrones
- **Alertas automáticas** de solicitudes pendientes

#### ✅ Gestión de Solicitudes

- **Revisión masiva** de solicitudes pendientes
- **Aprobación/rechazo** con comentarios detallados
- **Validación automática** de requisitos dinámicos
- **Flujo de trabajo** configurable por tipo de solicitud
- **Asignación** de revisores especializados

#### 📈 Reportes y Análisis

- **Reportes administrativos** completos en PDF/Excel
- **Análisis de tendencias** de ascensos por período
- **Indicadores de desempeño** institucional
- **Exportación de datos** para análisis externo
- **Reportes personalizados** por filtros múltiples

#### ⚙️ Configuración del Sistema

- **Requisitos dinámicos** por nivel de ascenso
- **Gestión de facultades** y departamentos
- **Configuración de notificaciones** automáticas
- **Importación automática** desde sistemas externos
- **Gestión de usuarios** y permisos

### 🔔 **Sistema de Notificaciones**

#### 📱 Notificaciones en Tiempo Real

- **SignalR Hub** para comunicación bidireccional
- **Toasts automáticos** con diseño moderno
- **Notificaciones push** para eventos críticos
- **Historial navegable** con paginación
- **Marcado como leída** individual y masivo

#### 📧 Integración con Email

- **Envío automático** de notificaciones por correo
- **Plantillas personalizadas** por tipo de evento
- **Configuración SMTP** flexible
- **Fallback automático** si SignalR no está disponible

### 🔗 **Integraciones Externas**

#### 🏢 Sistema TTHH (Talento Humano)

- **Datos personales** y organizacionales
- **Historial de nombramientos** y promociones
- **Información de departamentos** y facultades
- **Datos de contacto** actualizados

#### 📊 Sistema DAC (Evaluación Docente)

- **Evaluaciones académicas** por período
- **Promedios ponderados** históricos
- **Criterios de evaluación** detallados
- **Tendencias de desempeño** por docente

#### 🎓 Sistema DITIC (Capacitación)

- **Cursos completados** y certificaciones
- **Horas de capacitación** acumuladas
- **Participación en eventos** académicos
- **Certificados digitales** emitidos

#### 🔬 Sistema DIRINV (Investigación)

- **Proyectos de investigación** activos y finalizados
- **Obras académicas** publicadas
- **Participación en investigación** por rol
- **Métricas de productividad** científica

---

## 📁 Estructura del Proyecto

```
SistemaGestionAscensos/
│
├── 📁 SGA.Api/                          # API REST Principal
│   ├── Controllers/                     # Controladores REST
│   │   ├── AuthController.cs           # Autenticación JWT
│   │   ├── SolicitudesController.cs    # Gestión solicitudes
│   │   ├── DocentesController.cs       # Gestión docentes
│   │   ├── ReportesController.cs       # Reportes usuarios
│   │   ├── ReportesAdminController.cs  # Reportes administrativos
│   │   ├── EstadisticasController.cs   # Estadísticas avanzadas
│   │   └── NotificacionesController.cs # Sistema notificaciones
│   ├── Hubs/                           # SignalR Hubs
│   │   └── NotificacionesHub.cs        # Hub notificaciones tiempo real
│   ├── Middleware/                     # Middleware personalizado
│   ├── Configuration/                  # Configuraciones
│   └── Program.cs                      # Punto de entrada
│
├── 📁 SGA.Web/                         # Frontend Blazor WebAssembly
│   ├── Pages/                          # Páginas principales
│   │   ├── Index.razor                 # Dashboard principal
│   │   ├── Login.razor                 # Autenticación
│   │   ├── Perfil.razor                # Perfil docente
│   │   ├── Solicitudes.razor           # Gestión solicitudes
│   │   ├── SolicitudNueva.razor        # Nueva solicitud
│   │   ├── Documentos.razor            # Gestión documentos
│   │   ├── Reportes.razor              # Reportes docente
│   │   └── Admin/                      # Páginas administrativas
│   │       ├── Dashboard.razor         # Dashboard admin
│   │       ├── SolicitudesAdmin.razor  # Gestión admin solicitudes
│   │       ├── ReportesAdmin.razor     # Reportes administrativos
│   │       └── Configuracion.razor     # Configuración sistema
│   ├── Components/                     # Componentes reutilizables
│   ├── Layout/                         # Layouts de aplicación
│   ├── Services/                       # Servicios cliente
│   ├── Shared/                         # Componentes compartidos
│   └── wwwroot/                        # Archivos estáticos
│
├── 📁 SGA.Application/                 # Lógica de Negocio
│   ├── DTOs/                           # Data Transfer Objects
│   │   ├── Auth/                       # DTOs autenticación
│   │   ├── Admin/                      # DTOs administrativos
│   │   ├── Docente/                    # DTOs docente
│   │   └── ExternalData/               # DTOs sistemas externos
│   ├── Features/                       # Commands/Queries CQRS
│   ├── Services/                       # Servicios aplicación
│   │   ├── NotificacionTiempoRealService.cs
│   │   ├── IntegracionExternaService.cs
│   │   ├── ReporteAdminService.cs
│   │   └── ConfiguracionRequisitoService.cs
│   ├── Interfaces/                     # Interfaces servicios
│   ├── Mappings/                       # Mapeos AutoMapper
│   └── Validators/                     # Validaciones FluentValidation
│
├── 📁 SGA.Domain/                      # Dominio de Aplicación
│   ├── Entities/                       # Entidades principales
│   │   ├── Usuario.cs                  # Entidad usuario
│   │   ├── Docente.cs                  # Entidad docente
│   │   ├── SolicitudAscenso.cs         # Entidad solicitud
│   │   ├── Documento.cs                # Entidad documento
│   │   ├── Notificacion.cs             # Entidad notificación
│   │   ├── Facultad.cs                 # Entidad facultad
│   │   ├── Departamento.cs             # Entidad departamento
│   │   └── External/                   # Entidades sistemas externos
│   ├── Enums/                          # Enumeraciones
│   ├── Common/                         # Clases base comunes
│   ├── Extensions/                     # Métodos de extensión
│   └── Constants/                      # Constantes del dominio
│
├── 📁 SGA.Infrastructure/              # Infraestructura y Datos
│   ├── Data/                           # Contextos Entity Framework
│   │   ├── ApplicationDbContext.cs     # Contexto principal
│   │   └── External/                   # Contextos sistemas externos
│   │       ├── TTHHDbContext.cs        # BD Talento Humano
│   │       ├── DACDbContext.cs         # BD Evaluación Docente
│   │       ├── DITICDbContext.cs       # BD Capacitación
│   │       └── DIRINVDbContext.cs      # BD Investigación
│   ├── Repositories/                   # Implementación repositorios
│   ├── Services/                       # Servicios infraestructura
│   ├── Migrations/                     # Migraciones EF Core
│   └── DependencyInjection.cs         # Registro dependencias
│
├── 📁 Docs/                            # Documentación técnica
│   ├── IMPLEMENTACION_*.md             # Guías implementación
│   ├── SISTEMA_NOTIFICACIONES_*.md     # Documentación SignalR
│   ├── IMPLEMENTACION_REPORTES_*.md    # Sistema reportes
│   └── perfil_foto.md                  # Funcionalidad fotos
│
└── 📄 README.md                        # Este archivo
```

---

## 🗄️ Base de Datos

### 📊 Esquema Principal (SGA_Main)

El sistema utiliza una base de datos principal con las siguientes entidades principales:

#### 👥 **Entidades de Usuario**

- **Usuarios**: Credenciales y autenticación
- **Docentes**: Información académica y personal
- **Notificaciones**: Sistema de alertas en tiempo real

#### 📋 **Entidades de Solicitudes**

- **SolicitudesAscenso**: Solicitudes principales de ascenso
- **SolicitudesCertificadoCapacitacion**: Certificados de capacitación
- **SolicitudesObraAcademica**: Obras académicas
- **SolicitudesEvidenciaInvestigacion**: Evidencias de investigación

#### 📄 **Entidades de Documentos**

- **Documentos**: Archivos PDF con compresión
- **TitulosAcademicos**: Títulos y certificaciones
- **ObrasAcademicas**: Publicaciones científicas

#### 🏢 **Entidades Organizacionales**

- **Facultades**: Facultades universitarias
- **Departamentos**: Departamentos académicos
- **ConfiguracionRequisitos**: Requisitos dinámicos de ascenso

#### 📊 **Entidades de Auditoría**

- **LogsAuditoria**: Seguimiento de cambios
- **ArchivosImportados**: Control de importaciones

### 🔗 **Bases de Datos Externas**

#### 🏢 **TTHH (Talento Humano)**

```sql
EmpleadosTTHH          -- Datos personales y organizacionales
AccionesPersonalTTHH   -- Historial de promociones
CargosTTHH             -- Catálogo de cargos académicos
```

#### 📊 **DAC (Evaluación Docente)**

```sql
EvaluacionesDAC        -- Evaluaciones por período
PeriodosAcademicosDAC  -- Períodos de evaluación
CriteriosEvaluacionDAC -- Criterios de evaluación
```

#### 🎓 **DITIC (Capacitación)**

```sql
CursosDITIC            -- Catálogo de cursos
ParticipacionCursosDITIC -- Participación docente
CertificacionesDITIC   -- Certificados emitidos
```

#### 🔬 **DIRINV (Investigación)**

```sql
ProyectosInvestigacionDIRINV  -- Proyectos de investigación
ObraAcademicaDIRINV          -- Obras académicas publicadas
ParticipacionProyectoDIRINV  -- Participación en proyectos
```

---

## 🔧 Instalación y Configuración

### ⚡ **Instalación Rápida desde RAR**

Si tienes el archivo RAR del proyecto y los archivos .bak de las bases de datos:

#### 1. **Extracción del Proyecto**

```powershell
# Extraer el archivo SistemaGestionAscensos.rar a una carpeta local
# Ejemplo: C:\Proyectos\SistemaGestionAscensos\
```

#### 2. **Restauración de Bases de Datos**

Restaura las 5 bases de datos desde los archivos .bak:

```sql
-- En SQL Server Management Studio:

-- Base de datos principal
RESTORE DATABASE [SGA_Main]
FROM DISK = 'C:\ruta\SGA_Main.bak'
WITH REPLACE,
MOVE 'SGA_Main' TO 'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\SGA_Main.mdf',
MOVE 'SGA_Main_Log' TO 'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\SGA_Main_Log.ldf'

-- Bases de datos externas
RESTORE DATABASE [TTHH] FROM DISK = 'C:\ruta\TTHH.bak' WITH REPLACE
RESTORE DATABASE [DAC] FROM DISK = 'C:\ruta\DAC.bak' WITH REPLACE
RESTORE DATABASE [DITIC] FROM DISK = 'C:\ruta\DITIC.bak' WITH REPLACE
RESTORE DATABASE [DIRINV] FROM DISK = 'C:\ruta\DIRINV.bak' WITH REPLACE
```

#### 3. **Configuración de Conexiones**

Edita el archivo `SGA.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=SGA_Main;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true",
    "TTHHConnection": "Server=.\\SQLEXPRESS;Database=TTHH;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true",
    "DACConnection": "Server=.\\SQLEXPRESS;Database=DAC;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true",
    "DITICConnection": "Server=.\\SQLEXPRESS;Database=DITIC;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true",
    "DIRINVConnection": "Server=.\\SQLEXPRESS;Database=DIRINV;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
  }
}
```

#### 4. **Ejecución del Sistema**

```powershell
# Navegar a la carpeta del proyecto
cd C:\ruta\SistemaGestionAscensos

# Restaurar paquetes NuGet
dotnet restore

# Compilar la solución
dotnet build

# Ejecutar la API (en una terminal)
cd SGA.Api
dotnet run --launch-profile https

# Ejecutar el Frontend (en otra terminal)
cd SGA.Web
dotnet run --launch-profile https
```

#### 5. **Acceso al Sistema**

- **API**: https://localhost:7030 (o el puerto mostrado en consola)
- **Frontend**: https://localhost:7149 (o el puerto mostrado en consola)

### 🔨 **Instalación desde Código Fuente**

Si necesitas configurar el proyecto desde cero:

#### **Prerequisitos**

1. **.NET 9.0 SDK**

   ```powershell
   # Descargar desde: https://dotnet.microsoft.com/download/dotnet/9.0
   dotnet --version  # Verificar instalación
   ```

2. **SQL Server Express** (o superior)

   ```powershell
   # Descargar desde: https://www.microsoft.com/sql-server/sql-server-downloads
   ```

3. **Visual Studio 2022** o **VS Code** (recomendado)

#### **Pasos de Instalación**

1. **Clonación del Repositorio**

   ```powershell
   git clone <url-repositorio>
   cd SistemaGestionAscensos
   ```

2. **Restauración de Paquetes**

   ```powershell
   dotnet restore
   ```

3. **Configuración de Base de Datos**

   ```powershell
   # Editar cadenas de conexión en appsettings.json
   # Ejecutar migraciones
   cd SGA.Infrastructure
   dotnet ef database update --startup-project ../SGA.Api
   ```

4. **Configuración de Variables de Entorno** (Opcional)

   Crear archivo `.env` en la raíz del proyecto:

   ```env
   SGA_JWT_SECRET=SGA_SecretKey_2024_Sistema_Gestion_Ascensos_UTA_123456789012345678901234567890
   SGA_EMAIL_HOST=smtp.gmail.com
   SGA_EMAIL_PORT=587
   SGA_EMAIL_USER=tu-email@gmail.com
   SGA_EMAIL_PASS=tu-app-password
   ```

5. **Datos de Prueba** (Opcional)
   ```sql
   -- Ejecutar scripts de seed en las bases de datos
   -- Los datos de prueba incluyen usuarios, docentes y configuraciones básicas
   ```

#### **Estructura de URLs del Sistema**

- **API Principal**: `https://localhost:7030`

  - Swagger UI: `https://localhost:7030/swagger`
  - Health Check: `https://localhost:7030/api/health`
  - SignalR Hub: `https://localhost:7030/notificacionesHub`

- **Frontend**: `https://localhost:7149`
  - Login: `https://localhost:7149/login`
  - Dashboard: `https://localhost:7149/`
  - Admin: `https://localhost:7149/admin`

#### **Usuarios de Prueba**

Si restauraste desde los archivos .bak, estos usuarios están disponibles:

```
🔑 Administrador:
   Email: admin@uta.edu.ec
   Password: Admin12345

👨‍🎓 Docente de Prueba:
   Email: docente@uta.edu.ec
   Password: Docente123!
```

---

## 🚀 **Ejecutar el Sistema**

### **Método 1: Visual Studio 2022**

1. Abrir `SistemaGestionAscensos.sln`
2. Configurar proyectos de inicio múltiples:
   - `SGA.Api`
   - `SGA.Web`
3. Presionar `F5` o `Ctrl+F5`

### **Método 2: Terminal/PowerShell**

```powershell
# Terminal 1 - API Backend
cd SGA.Api
dotnet run --urls="https://localhost:7030"

# Terminal 2 - Frontend
cd SGA.Web
dotnet run --urls="https://localhost:7149"
```

---

## 🔐 **Configuración de Seguridad**

### **Autenticación JWT**

El sistema utiliza JWT (JSON Web Tokens) para autenticación stateless:

```json
{
  "JWT": {
    "SecretKey": "SGA_SecretKey_2024_Sistema_Gestion_Ascensos_UTA_123456789012345678901234567890",
    "Issuer": "SGA.Api",
    "Audience": "SGA.Client",
    "ExpirationHours": 8
  }
}
```

### **CORS Configuration**

```json
{
  "CORS": {
    "AllowedOrigins": ["https://localhost:7149", "http://localhost:5039"]
  }
}
```

### **Configuración de Email**

```json
{
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUser": "tu-email@gmail.com",
    "SmtpPass": "tu-app-password",
    "EnableSsl": true,
    "FromName": "Sistema de Gestión de Ascensos",
    "FromEmail": "tu-email@gmail.com"
  }
}
```

---

## 🔧 **Configuración de Desarrollo**

### **Variables de Entorno**

Crear archivo `.env` en la raíz:

```env
# Configuración de Base de Datos
SGA_DB_SERVER=.\\SQLEXPRESS
SGA_DB_NAME=SGA_Main
SGA_DB_TRUSTED_CONNECTION=true

# Configuración JWT
SGA_JWT_SECRET=tu-clave-secreta-muy-larga-y-segura-minimo-64-caracteres
SGA_JWT_ISSUER=SGA.Api
SGA_JWT_AUDIENCE=SGA.Client
SGA_JWT_EXPIRATION_HOURS=8

# Configuración de Email
SGA_EMAIL_HOST=smtp.gmail.com
SGA_EMAIL_PORT=587
SGA_EMAIL_USER=tu-email@gmail.com
SGA_EMAIL_PASS=tu-app-password
SGA_EMAIL_SSL=true

# URLs del Sistema
SGA_API_URL=https://localhost:7030
SGA_WEB_URL=https://localhost:7149

# Configuración de Archivos
SGA_MAX_FILE_SIZE=10485760
SGA_ALLOWED_EXTENSIONS=.pdf,.doc,.docx

# Configuración de Logging
SGA_LOG_LEVEL=Information
SGA_LOG_FILE_PATH=Logs/sga-{Date}.txt
```

### **Configuración de Desarrollo en Visual Studio**

1. **Propiedades del Proyecto SGA.Api**:

   - URL de Inicio: `https://localhost:7030`
   - Variables de Entorno: Cargar desde `.env`

2. **Propiedades del Proyecto SGA.Web**:
   - URL de Inicio: `https://localhost:7149`
   - Variable `SGA_API_BASE_URL=https://localhost:7030`

### **Configuración de Entity Framework**

```powershell
# Instalar herramientas EF Core (si no están instaladas)
dotnet tool install --global dotnet-ef

# Agregar migración
dotnet ef migrations add NombreMigracion --startup-project SGA.Api --project SGA.Infrastructure

# Actualizar base de datos
dotnet ef database update --startup-project SGA.Api --project SGA.Infrastructure

# Ver migraciones pendientes
dotnet ef migrations list --startup-project SGA.Api --project SGA.Infrastructure
```

### **Casos de Prueba Básicos**

1. **Login de Administrador**:

   - Email: `admin@uta.edu.ec`
   - Password: `Admin123!`
   - Debe redirigir al dashboard admin

2. **Login de Docente**:

   - Email: `docente@uta.edu.ec`
   - Password: `Docente123!`
   - Debe redirigir al dashboard docente

3. **Crear Nueva Solicitud**:

   - Login como docente
   - Ir a "Nueva Solicitud"
   - Completar formulario
   - Subir documento PDF
   - Enviar solicitud

4. **Aprobar/Rechazar Solicitud**:
   - Login como administrador
   - Ir a "Gestión de Solicitudes"
   - Revisar solicitud pendiente
   - Aprobar o rechazar con comentarios

---
