# Implementación del Sistema de Login y Registro para GestorAscensos-UTA

## Resumen de lo implementado

Se ha creado un sistema de autenticación con las siguientes características:

- Autenticación mediante JWT Tokens
- Protección contra intentos fallidos de login (bloqueo temporal después de 3 intentos)
- Validación de datos de registro
- Hashing seguro de contraseñas (SHA256)
- Persistencia del token JWT en el almacenamiento local del navegador
- Control de autorización basado en roles (Administrador/Docente)

## Backend (API)

### AuthController.cs

Ya está implementado y contiene los siguientes endpoints:

- POST /api/auth/login: Autenticación de usuarios
- POST /api/auth/register: Registro de nuevos docentes
- GET /api/auth/me: Obtención de información del usuario autenticado (protegido)

### Entidad Docente

Se actualizó para incluir:

- Propiedades de autenticación (nombre de usuario, hash de contraseña)
- Control de intentos fallidos y bloqueo temporal
- Propiedad EsAdministrador para control de roles
- FechaRegistro para auditoría

## Frontend (Blazor WebAssembly)

### Modelos

Se crearon modelos para:

- LoginModel: Para el formulario de inicio de sesión
- RegisterModel: Para el formulario de registro con validaciones
- UserInfoModel: Para almacenar la información del usuario autenticado

### Servicios

Se implementaron servicios para:

- AuthService: Comunicación con el API para login, registro y obtención de info del usuario
- LocalStorageService: Almacenamiento y recuperación del token JWT
- ApiAuthenticationStateProvider: Manejo del estado de autenticación para Blazor

### Páginas

Se crearon páginas para:

- Login: Formulario de inicio de sesión con validación
- Register: Formulario de registro con validaciones

### Componentes de navegación

Se actualizó el NavMenu para mostrar opciones según el estado de autenticación y rol del usuario

## Próximos pasos para completar la implementación

1. **Corregir posibles errores de compilación**:

   - Asegúrese de que todos los proyectos referencien correctamente los paquetes NuGet necesarios
   - Verifique que las relaciones entre proyectos sean correctas

2. **Actualizar el DocenteService**:

   ```csharp
   public async Task<Docente> GetDocenteByUsernameAsync(string username)
   {
       return await _docenteRepository.GetByUsernameAsync(username);
   }

   public async Task UpdateDocenteAsync(Docente docente)
   {
       var existingDocente = await _docenteRepository.GetByIdAsync(docente.Id);
       if (existingDocente == null)
           throw new Exception("Docente no encontrado");

       // Actualizar propiedades
       existingDocente.Cedula = docente.Cedula;
       existingDocente.Nombres = docente.Nombres;
       existingDocente.Apellidos = docente.Apellidos;
       existingDocente.Email = docente.Email;
       existingDocente.TelefonoContacto = docente.TelefonoContacto;
       existingDocente.Facultad = docente.Facultad;
       existingDocente.Departamento = docente.Departamento;
       // ... otras propiedades
       existingDocente.IntentosFallidos = docente.IntentosFallidos;
       existingDocente.Bloqueado = docente.Bloqueado;
       existingDocente.FechaBloqueo = docente.FechaBloqueo;
       existingDocente.EsAdministrador = docente.EsAdministrador;

       await _docenteRepository.UpdateAsync(existingDocente);
   }

   public async Task CreateDocenteAsync(Docente docente)
   {
       // Verificar duplicados
       var existingByUsername = await _docenteRepository.GetByUsernameAsync(docente.NombreUsuario);
       if (existingByUsername != null)
           throw new Exception("Ya existe un docente con este nombre de usuario");

       var existingByCedula = await _docenteRepository.GetByCedulaAsync(docente.Cedula);
       if (existingByCedula != null)
           throw new Exception("Ya existe un docente con esta cédula");

       await _docenteRepository.AddAsync(docente);
   }
   ```

3. **Configurar el registro de servicios en Program.cs (Blazor WebAssembly)**:

   ```csharp
   // Servicios de autenticación
   builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
   builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
   builder.Services.AddScoped<AuthService>();
   builder.Services.AddAuthorizationCore();
   ```

4. **Pruebas**:

   - Verificar el registro de un nuevo docente
   - Comprobar el inicio de sesión con credenciales válidas
   - Probar el bloqueo después de 3 intentos fallidos
   - Comprobar el desbloqueo automático después de 15 minutos
   - Verificar la redirección a páginas protegidas
   - Probar las diferentes opciones según el rol del usuario

5. **Seguridad adicional**:
   - Considerar implementar HTTPS para toda la comunicación
   - Proteger contra ataques CSRF
   - Implementar validación adicional en el backend
   - Considerar añadir captcha para prevenir ataques automatizados

Este documento contiene las instrucciones necesarias para completar y mantener el sistema de autenticación implementado.
