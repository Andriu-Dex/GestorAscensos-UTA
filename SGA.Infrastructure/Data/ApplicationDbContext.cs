using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities;

namespace SGA.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Docente> Docentes { get; set; }
    public DbSet<SolicitudAscenso> SolicitudesAscenso { get; set; }
    public DbSet<Documento> Documentos { get; set; }
    public DbSet<LogAuditoria> LogsAuditoria { get; set; }

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

        // Datos semilla
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Usuario administrador por defecto
        var adminId = Guid.NewGuid();
        var adminDocenteId = Guid.NewGuid();

        modelBuilder.Entity<Usuario>().HasData(
            new Usuario
            {
                Id = adminId,
                Email = "admin@uta.edu.ec",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Rol = Domain.Enums.RolUsuario.Administrador,
                EstaActivo = true,
                FechaCreacion = DateTime.UtcNow
            }
        );

        modelBuilder.Entity<Docente>().HasData(
            new Docente
            {
                Id = adminDocenteId,
                Cedula = "1800000000",
                Nombres = "Administrador",
                Apellidos = "Sistema",
                Email = "admin@uta.edu.ec",
                NivelActual = Domain.Enums.NivelTitular.Titular5,
                FechaInicioNivelActual = DateTime.UtcNow.AddYears(-5),
                UsuarioId = adminId,
                EstaActivo = true,
                FechaCreacion = DateTime.UtcNow
            }
        );
    }
}
