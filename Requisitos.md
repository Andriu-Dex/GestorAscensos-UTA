# Guía Completa de Estructura de Proyecto: Sistema de Gestión de Ascensos Docentes Universitarios

## Introducción

Esta guía proporciona una estructura de proyecto profesional y completa para un sistema web de gestión de ascensos docentes universitarios, implementando las mejores prácticas de arquitectura de software moderna y específicamente diseñada para cumplir con los requisitos académicos y de seguridad institucional.

## 1. Estructura General de la Solución

### Organización Principal del Proyecto

```
GestionAscensosDocentes.sln
├── src/
│   ├── Core/                                    # Capa de Dominio (Onion Core)
│   │   ├── GestionAscensos.Domain/
│   │   └── GestionAscensos.Application/
│   ├── Infrastructure/                          # Capa de Infraestructura
│   │   ├── GestionAscensos.Infrastructure/
│   │   └── GestionAscensos.Persistence/
│   ├── Presentation/                            # Capa de Presentación
│   │   ├── GestionAscensos.Api/                # ASP.NET Core 9 Web API
│   │   └── GestionAscensos.BlazorWasm/         # Blazor WebAssembly Client
│   └── Shared/                                  # Componentes Compartidos
│       └── GestionAscensos.Shared/
├── tests/
│   ├── UnitTests/
│   ├── IntegrationTests/
│   └── ArchitecturalTests/
├── docs/                                        # Documentación
└── scripts/                                     # Scripts de BD y despliegue
```

## 2. Implementación de Onion Architecture

### 2.1 Capa de Dominio (Core)

**GestionAscensos.Domain/** - Contiene las entidades de negocio, objetos de valor, eventos de dominio y especificaciones:

- **Entities/**: Docente, SolicitudAscenso, Documento, EvaluacionDocente, ObraAcademica, ProyectoInvestigacion
- **ValueObjects/**: Email, NivelTitular, EstadoSolicitud, DocumentoTipo
- **DomainEvents/**: Eventos para notificaciones y workflows
- **Specifications/**: Lógica de consultas complejas encapsulada
- **Exceptions/**: Excepciones específicas del dominio

**Ejemplo de Entidad Docente:**

```csharp
public class Docente : BaseEntity, IAuditableEntity
{
    public string Nombre { get; private set; }
    public string Apellido { get; private set; }
    public Email Email { get; private set; }
    public NivelTitular NivelActual { get; private set; }
    public DateTime FechaIngresoNivel { get; private set; }

    public bool CumpleRequisitosParaAscenso(NivelTitular nivelDestino)
    {
        var requisitos = RequisitosPorNivel.ObtenerRequisitos(nivelDestino);
        // Validar tiempo en nivel, obras, evaluaciones, capacitación, investigación
        return ValidarTodosLosRequisitos(requisitos);
    }
}
```

### 2.2 Capa de Aplicación

**GestionAscensos.Application/** - Implementa casos de uso con patrón CQRS:

- **Features/**: Organizados por funcionalidad (Docentes, SolicitudesAscenso, Documentos, Reportes)
- **Commands/**: Operaciones de escritura (CreateSolicitudAscenso, ProcessSolicitud)
- **Queries/**: Operaciones de lectura (GetDocente, GetSolicitudesList)
- **Behaviors/**: Pipeline behaviors para validación, logging, caching, autorización

**Ejemplo de Command Handler:**

```csharp
public class CreateSolicitudAscensoCommandHandler : IRequestHandler<CreateSolicitudAscensoCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateSolicitudAscensoCommand request, CancellationToken cancellationToken)
    {
        // Validar requisitos de ascenso
        // Crear solicitud
        // Procesar documentos con compresión PDF
        // Guardar en base de datos
        // Enviar notificaciones
    }
}
```

## 3. Estructura de la API RESTful

### 3.1 Controladores RESTful

**Endpoints principales:**

- `GET /api/docentes` - Lista paginada de docentes
- `POST /api/solicitudesascenso` - Crear nueva solicitud
- `GET /api/solicitudesascenso/{id}` - Obtener solicitud específica
- `PUT /api/solicitudesascenso/{id}/aprobar` - Aprobar solicitud
- `POST /api/documentos/upload` - Subir documentos con compresión
- `GET /api/reportes/ascensos` - Generar reportes PDF

### 3.2 Configuración de Seguridad JWT

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!)),
            ClockSkew = TimeSpan.Zero
        };
    });
```

## 4. Cliente Blazor WebAssembly

### 4.1 Estructura del Cliente

```
GestionAscensos.BlazorWasm/
├── Components/
│   ├── Pages/                    # Páginas principales
│   ├── Shared/                   # Componentes reutilizables
│   └── Forms/                    # Formularios específicos
├── Services/
│   ├── Api/                      # Servicios de API
│   ├── Authentication/           # Manejo de autenticación
│   └── State/                    # Gestión de estado
├── Models/                       # DTOs y ViewModels
└── Infrastructure/               # Helpers y extensiones
```

### 4.2 Componentes de Ejemplo

**Formulario de Solicitud de Ascenso:**

```razor
@page "/solicitudes/crear"
@attribute [Authorize(Roles = "Docente,Administrador")]

<EditForm Model="@Model" OnValidSubmit="@HandleValidSubmit">
    <FluentValidator TValidator="CreateSolicitudViewModelValidator" />

    <div class="form-group">
        <InputSelect @bind-Value="Model.DocenteId" class="form-control">
            @foreach (var docente in Docentes)
            {
                <option value="@docente.Id">@docente.NombreCompleto</option>
            }
        </InputSelect>
    </div>

    <FileUpload @bind-Files="Model.DocumentosAdjuntos"
                AcceptedFileTypes=".pdf"
                MaxFileSize="@(10 * 1024 * 1024)" />

    <button type="submit" class="btn btn-primary">Crear Solicitud</button>
</EditForm>
```

## 5. Configuración de Base de Datos

### 5.1 Entity Framework Core 9

**ApplicationDbContext con auditoría automática:**

```csharp
public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = _currentUserService.UserId;
                    entry.Entity.CreatedAt = _dateTime.Now;
                    break;
                case EntityState.Modified:
                    entry.Entity.ModifiedBy = _currentUserService.UserId;
                    entry.Entity.ModifiedAt = _dateTime.Now;
                    break;
            }
        }
        return await base.SaveChangesAsync(cancellationToken);
    }
}
```

### 5.2 Configuraciones de Entidad

**Configuración de normalización 3FN:**

```csharp
public class DocenteConfiguration : IEntityTypeConfiguration<Docente>
{
    public void Configure(EntityTypeBuilder<Docente> builder)
    {
        builder.ToTable("Docentes");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Nombre).HasMaxLength(100).IsRequired();
        builder.Property(d => d.DocumentoIdentidad).HasMaxLength(20).IsRequired();
        builder.HasIndex(d => d.DocumentoIdentidad).IsUnique();

        // Configuración de Value Object
        builder.OwnsOne(d => d.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")
                .HasMaxLength(255)
                .IsRequired();
        });

        // Relaciones con restricciones
        builder.HasMany(d => d.SolicitudesAscenso)
            .WithOne(s => s.Docente)
            .HasForeignKey(s => s.DocenteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

## 6. Servicios Específicos del Dominio

### 6.1 Servicio de Compresión de PDFs

```csharp
public class PdfCompressionService : IPdfCompressionService
{
    public async Task<byte[]> CompressAsync(IFormFile pdfFile, CancellationToken cancellationToken = default)
    {
        using var inputStream = new MemoryStream();
        await pdfFile.CopyToAsync(inputStream, cancellationToken);

        using var outputStream = new MemoryStream();
        using var reader = new PdfReader(inputStream.ToArray());
        using var stamper = new PdfStamper(reader, outputStream);

        stamper.SetFullCompression();
        stamper.Writer.CompressionLevel = PdfStream.BEST_COMPRESSION;

        // Comprimir imágenes página por página
        for (int i = 1; i <= reader.NumberOfPages; i++)
        {
            CompressPageImages(reader.GetPageN(i), _settings.CompressionQuality);
        }

        return outputStream.ToArray();
    }
}
```

### 6.2 Servicio de Importación de Datos

```csharp
public class ImportacionDatosService : IImportacionDatosService
{
    public async Task<ImportResult> ImportFromDITICAsync(CancellationToken cancellationToken = default)
    {
        using var connection = new SqlConnection(_settings.DITIC.ConnectionString);

        // Importar docentes, evaluaciones, obras académicas
        var docentesImportados = await ImportDocentesFromDITIC(connection, cancellationToken);
        var evaluacionesImportadas = await ImportEvaluacionesFromDITIC(connection, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ImportResult
        {
            Success = true,
            DocentesImportados = docentesImportados,
            EvaluacionesImportadas = evaluacionesImportadas
        };
    }
}
```

## 7. Testing Comprehensivo

### 7.1 Estructura de Pruebas

```
tests/
├── UnitTests/
│   ├── Domain.UnitTests/         # Pruebas de entidades y lógica de negocio
│   ├── Application.UnitTests/    # Pruebas de handlers y behaviors
│   └── Infrastructure.UnitTests/ # Pruebas de servicios
├── IntegrationTests/
│   ├── Api.IntegrationTests/     # Pruebas de endpoints
│   └── BlazorWasm.IntegrationTests/ # Pruebas de componentes
└── ArchitecturalTests/           # Validación de arquitectura
```

### 7.2 Ejemplos de Pruebas

**Unit Test de Dominio:**

```csharp
[Fact]
public void CumpleRequisitosParaAscenso_WithValidRequirements_ShouldReturnTrue()
{
    // Arrange
    var docente = DocenteTestHelper.CreateDocenteWithCompleteData();
    var nivelDestino = NivelTitular.Titular2;

    // Act
    var cumpleRequisitos = docente.CumpleRequisitosParaAscenso(nivelDestino);

    // Assert
    cumpleRequisitos.Should().BeTrue();
}
```

**Integration Test de API:**

```csharp
[Fact]
public async Task CreateSolicitud_WithValidData_ShouldReturnCreated()
{
    // Arrange
    var token = await GenerateJwtTokenAsync("Docente");
    Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    // Act
    var response = await Client.PostAsJsonAsync("/api/solicitudesascenso", request);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.Created);
}
```

## 8. Despliegue y DevOps

### 8.1 Configuración Docker

**Multi-stage Dockerfile para API:**

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["GestionAscensos.Api.csproj", "."]
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GestionAscensos.Api.dll"]
```

### 8.2 Docker Compose para Desarrollo

```yaml
version: "3.8"
services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - SA_PASSWORD=YourStrong@Passw0rd
      - ACCEPT_EULA=Y
    ports:
      - "1433:1433"

  api:
    build: .
    ports:
      - "7001:443"
    depends_on:
      - sqlserver
    environment:
      - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=GestionAscensosDb;User Id=sa;Password=YourStrong@Passw0rd
```

### 8.3 CI/CD con GitHub Actions

```yaml
name: CI/CD Pipeline
on:
  push:
    branches: [main, develop]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: "9.0.x"
      - name: Run tests
        run: dotnet test --configuration Release

  deploy:
    needs: test
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/main'
    steps:
      - name: Build and push Docker images
        # Comandos de build y push a registry
```

## 9. Configuración de Entornos

### 9.1 Configuración Fuertemente Tipada

```csharp
public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenExpirationMinutes { get; set; } = 15;
    public int RefreshTokenExpirationDays { get; set; } = 7;
}

public class FileStorageSettings
{
    public string StorageType { get; set; } = "Local";
    public string LocalPath { get; set; } = "wwwroot/uploads";
    public int MaxFileSizeMB { get; set; } = 10;
    public List<string> AllowedExtensions { get; set; } = new();
    public bool EnableCompression { get; set; } = true;
}
```

### 9.2 Configuración por Ambiente

**Development:**

- SQL Server local
- JWT con expiración larga
- Logging detallado
- CORS permisivos

**Production:**

- SQL Server en la nube
- JWT con expiración corta
- Logging mínimo
- CORS restrictivos
- Secrets en Azure Key Vault

## 10. Optimización de Rendimiento

### 10.1 Caché Distribuido

```csharp
public class CacheService : ICacheService
{
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (_settings.EnableRedis && _redisDatabase != null)
        {
            var value = await _redisDatabase.StringGetAsync(key);
            return value.HasValue ? JsonSerializer.Deserialize<T>(value!) : null;
        }
        return _memoryCache.Get<T>(key);
    }
}
```

### 10.2 Consultas Optimizadas

```csharp
public async Task<PaginatedList<Docente>> GetDocentesWithFiltersAsync(DocenteFilter filter)
{
    var query = Context.Docentes
        .Where(d => filter.Nombre == null || d.Nombre.Contains(filter.Nombre))
        .Where(d => filter.Departamento == null || d.Departamento == filter.Departamento);

    // Include solo datos necesarios
    if (filter.IncludeEvaluaciones)
    {
        query = query.Include(d => d.Evaluaciones.Where(e => e.Periodo >= DateTime.UtcNow.AddYears(-4)));
    }

    return await PaginatedList<Docente>.CreateAsync(query, filter.PageNumber, filter.PageSize);
}
```

## 11. Reglas de Negocio Implementadas

### 11.1 Validación de Requisitos de Ascenso

- **Tiempo en rol**: Mínimo 4 años por nivel
- **Obras académicas**: Cantidad variable según nivel destino
- **Evaluación docente**: Promedio mínimo 75%
- **Horas de capacitación**: Requisitos específicos por nivel
- **Proyectos de investigación**: Tiempo mínimo de participación

### 11.2 Workflow de Solicitudes

1. **Creación**: Docente crea solicitud con documentos
2. **Validación automática**: Sistema verifica requisitos
3. **Revisión**: Administradores evalúan solicitud
4. **Decisión**: Aprobación o rechazo con observaciones
5. **Notificación**: Email automático al docente
6. **Actualización**: Si aprobado, actualizar nivel y reiniciar contadores

## 12. Comandos de Inicio Rápido

```bash
# Clonar e inicializar
git clone <repository-url>
cd GestionAscensosDocentes
dotnet restore

# Ejecutar migraciones
dotnet ef database update --project src/Infrastructure/GestionAscensos.Persistence

# Ejecutar pruebas
dotnet test

# Ejecutar aplicación (desarrollo)
dotnet run --project src/Presentation/GestionAscensos.Api
dotnet run --project src/Presentation/GestionAscensos.BlazorWasm

# Docker desarrollo
docker-compose up -d

# Build producción
dotnet publish -c Release
```

## Conclusión

Esta guía proporciona una arquitectura completa y profesional para un sistema de gestión de ascensos docentes universitarios, implementando:

✅ **Onion Architecture** con separación clara de capas
✅ **CQRS + MediatR** para operaciones complejas
✅ **JWT Authentication** con roles y políticas
✅ **Entity Framework Core 9** con configuración 3FN
✅ **Blazor WebAssembly** con componentes reutilizables
✅ **Testing completo** (Unit, Integration, Architectural)
✅ **Docker + CI/CD** para despliegue automatizado
✅ **Compresión de PDFs** y gestión documental
✅ **Importación de datos** desde múltiples fuentes
✅ **Reportes PDF** dinámicos
✅ **Optimización de rendimiento** con caché
✅ **Configuración por ambiente** robusta

El sistema está diseñado para ser mantenible, escalable, seguro y específicamente adaptado a las necesidades del dominio académico universitario, cumpliendo con todos los requisitos técnicos y de negocio especificados.
