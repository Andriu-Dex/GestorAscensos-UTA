using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities.External;

namespace SGA.Infrastructure.Data.External;

public class TTHHDbContext : DbContext
{
    public TTHHDbContext(DbContextOptions<TTHHDbContext> options) : base(options)
    {
    }

    public DbSet<EmpleadoTTHH> Empleados { get; set; }
    public DbSet<AccionPersonalTTHH> AccionesPersonal { get; set; }
    public DbSet<CargoTTHH> Cargos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EmpleadoTTHH>(entity =>
        {
            entity.ToTable("EmpleadosTTHH");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Cedula).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Nombres).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Apellidos).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.CorreoInstitucional).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Celular).IsRequired().HasMaxLength(15);
            entity.Property(e => e.CargoActual).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Facultad).IsRequired().HasMaxLength(100);
            entity.Property(e => e.NivelAcademico).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Direccion).IsRequired().HasMaxLength(500);
            entity.Property(e => e.EstadoCivil).IsRequired().HasMaxLength(50);
            entity.Property(e => e.TipoContrato).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Cedula);
            entity.HasIndex(e => e.CorreoInstitucional).IsUnique();
        });

        modelBuilder.Entity<AccionPersonalTTHH>(entity =>
        {
            entity.ToTable("AccionesPersonalTTHH");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Cedula).IsRequired().HasMaxLength(10);
            entity.Property(e => e.TipoAccion).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CargoAnterior).HasMaxLength(255);
            entity.Property(e => e.CargoNuevo).HasMaxLength(255);
            entity.HasIndex(e => e.Cedula);
        });

        modelBuilder.Entity<CargoTTHH>(entity =>
        {
            entity.ToTable("CargosTTHH");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NombreCargo).IsRequired().HasMaxLength(255);
            entity.Property(e => e.NivelTitular).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Descripcion).HasMaxLength(500);
        });

    }
}
