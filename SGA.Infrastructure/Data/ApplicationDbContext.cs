using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities;
using SGA.Application.Interfaces;

namespace SGA.Infrastructure.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        // Suprimir temporalmente la advertencia de cambios pendientes
        optionsBuilder.ConfigureWarnings(warnings => 
            warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
    }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Docente> Docentes { get; set; }
    public DbSet<SolicitudAscenso> SolicitudesAscenso { get; set; }
    public DbSet<Documento> Documentos { get; set; }
    public DbSet<LogAuditoria> LogsAuditoria { get; set; }
    public DbSet<SolicitudObraAcademica> SolicitudesObrasAcademicas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Rol).HasConversion<string>();
            entity.HasIndex(e => e.Email).IsUnique();
        });        // Configuración Docente
        modelBuilder.Entity<Docente>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Cedula).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Nombres).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Apellidos).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.NivelActual).HasConversion<string>();
            entity.Property(e => e.PromedioEvaluaciones).HasColumnType("decimal(5,2)");
            entity.HasIndex(e => e.Cedula).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.UsuarioId).IsUnique();
            
            entity.HasOne(e => e.Usuario)
                  .WithOne(u => u.Docente)
                  .HasForeignKey<Docente>(e => e.UsuarioId);
        });

        // Configuración SolicitudAscenso
        modelBuilder.Entity<SolicitudAscenso>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NivelActual).HasConversion<string>();
            entity.Property(e => e.NivelSolicitado).HasConversion<string>();
            entity.Property(e => e.Estado).HasConversion<string>();
            entity.Property(e => e.PromedioEvaluaciones).HasColumnType("decimal(5,2)");
            
            entity.HasOne(e => e.Docente)
                  .WithMany(d => d.SolicitudesAscenso)
                  .HasForeignKey(e => e.DocenteId)
                  .OnDelete(DeleteBehavior.NoAction);
                  
            entity.HasOne(e => e.AprobadoPor)
                  .WithMany()
                  .HasForeignKey(e => e.AprobadoPorId)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // Configuración Documento
        modelBuilder.Entity<Documento>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NombreArchivo).IsRequired().HasMaxLength(255);
            entity.Property(e => e.RutaArchivo).IsRequired().HasMaxLength(500);
            entity.Property(e => e.ContentType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.TipoDocumento).HasConversion<string>();
            
            entity.HasOne(e => e.SolicitudAscenso)
                  .WithMany(s => s.Documentos)
                  .HasForeignKey(e => e.SolicitudAscensoId);
        });

        // Configuración LogAuditoria
        modelBuilder.Entity<LogAuditoria>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Accion).IsRequired().HasMaxLength(255);
            entity.Property(e => e.UsuarioEmail).HasMaxLength(255);
            entity.Property(e => e.EntidadAfectada).HasMaxLength(255);
            entity.Property(e => e.DireccionIP).HasMaxLength(45);
        });

        // Configuración SolicitudObraAcademica
        modelBuilder.Entity<SolicitudObraAcademica>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DocenteCedula).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Titulo).IsRequired().HasMaxLength(500);
            entity.Property(e => e.TipoObra).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Editorial).HasMaxLength(255);
            entity.Property(e => e.Revista).HasMaxLength(255);
            entity.Property(e => e.ISBN_ISSN).HasMaxLength(50);
            entity.Property(e => e.DOI).HasMaxLength(200);
            entity.Property(e => e.IndiceIndexacion).HasMaxLength(100);
            entity.Property(e => e.Autores).HasMaxLength(1000);
            entity.Property(e => e.Descripcion).HasMaxLength(2000);
            entity.Property(e => e.ArchivoNombre).HasMaxLength(255);
            entity.Property(e => e.ArchivoRuta).HasMaxLength(500);
            entity.Property(e => e.ArchivoTipo).HasMaxLength(100);
            entity.Property(e => e.Estado).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ComentariosRevision).HasMaxLength(1000);
            entity.Property(e => e.MotivoRechazo).HasMaxLength(1000);
            entity.Property(e => e.ComentariosSolicitud).HasMaxLength(1000);
            
            entity.HasIndex(e => e.DocenteCedula);
            entity.HasIndex(e => e.SolicitudGrupoId);
            entity.HasIndex(e => e.Estado);
            
            entity.HasOne(e => e.Docente)
                  .WithMany()
                  .HasForeignKey(e => e.DocenteId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(e => e.RevisadoPor)
                  .WithMany()
                  .HasForeignKey(e => e.RevisadoPorId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Datos semilla
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // IDs fijos para facilitar las relaciones
        var adminId = Guid.Parse("c24cd969-b99a-4354-b49f-0cae93b0b7ad");
        var adminDocenteId = Guid.Parse("8ef569a9-342c-4e85-a8e1-29b5e697f2b6");
        var stevenId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479");
        var stevenDocenteId = Guid.Parse("6ba7b810-9dad-11d1-80b4-00c04fd430c8");

        // Usuarios
        modelBuilder.Entity<Usuario>().HasData(
            new Usuario
            {
                Id = adminId,
                Email = "admin@uta.edu.ec",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin12345"),
                Rol = Domain.Enums.RolUsuario.Administrador,
                EstaActivo = true,
                IntentosLogin = 0,
                UltimoLogin = DateTime.UtcNow.AddYears(-5),
                FechaCreacion = DateTime.UtcNow
            },
            new Usuario
            {
                Id = stevenId,
                Email = "sparedes@uta.edu.ec",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                Rol = Domain.Enums.RolUsuario.Docente,
                EstaActivo = true,
                IntentosLogin = 0,
                UltimoLogin = DateTime.UtcNow.AddDays(-30),
                FechaCreacion = DateTime.UtcNow
            }
        );

        // Docentes
        modelBuilder.Entity<Docente>().HasData(
            new Docente
            {
                Id = adminDocenteId,
                Cedula = "999999999",
                Nombres = "Admin",
                Apellidos = "Global",
                Email = "admin@uta.edu.ec",
                NivelActual = Domain.Enums.NivelTitular.Titular5,
                FechaInicioNivelActual = DateTime.UtcNow.AddYears(-5),
                UsuarioId = adminId,
                EstaActivo = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Docente
            {
                Id = stevenDocenteId,
                Cedula = "1801000000",
                Nombres = "Steven",
                Apellidos = "Paredes",
                Email = "sparedes@uta.edu.ec",
                NivelActual = Domain.Enums.NivelTitular.Titular1,
                FechaInicioNivelActual = DateTime.UtcNow.AddYears(-2),
                UsuarioId = stevenId,
                EstaActivo = true,
                FechaCreacion = DateTime.UtcNow
            }
        );
    }
}
