using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities.External;

namespace SGA.Infrastructure.Data.External;

public class DIRINVDbContext : DbContext
{
    public DIRINVDbContext(DbContextOptions<DIRINVDbContext> options) : base(options)
    {
    }

    public DbSet<ObraAcademicaDIRINV> ObrasAcademicas { get; set; }
    public DbSet<ProyectoInvestigacionDIRINV> ProyectosInvestigacion { get; set; }
    public DbSet<ParticipacionProyectoDIRINV> ParticipacionesProyecto { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ObraAcademicaDIRINV>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Cedula).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Titulo).IsRequired().HasMaxLength(500);
            entity.Property(e => e.TipoObra).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Editorial).HasMaxLength(255);
            entity.Property(e => e.Revista).HasMaxLength(255);
            entity.Property(e => e.ISBN_ISSN).HasMaxLength(50);
            entity.Property(e => e.IndiceIndexacion).HasMaxLength(100);
            entity.HasIndex(e => e.Cedula);
        });

        modelBuilder.Entity<ProyectoInvestigacionDIRINV>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Titulo).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Descripcion).HasMaxLength(1000);
            entity.Property(e => e.Estado).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PresupuestoTotal).HasColumnType("decimal(12,2)");
            entity.Property(e => e.FuenteFinanciamiento).HasMaxLength(255);
        });

        modelBuilder.Entity<ParticipacionProyectoDIRINV>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Cedula).IsRequired().HasMaxLength(10);
            entity.Property(e => e.RolEnProyecto).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Cedula);
            
            entity.HasOne(e => e.Proyecto)
                  .WithMany(p => p.Participaciones)
                  .HasForeignKey(e => e.ProyectoId);
        });

        // Datos semilla para DIRINV
        SeedDIRINVData(modelBuilder);
    }

    private void SeedDIRINVData(ModelBuilder modelBuilder)
    {
        var obras = new List<ObraAcademicaDIRINV>();
        var proyectos = new List<ProyectoInvestigacionDIRINV>();
        var participaciones = new List<ParticipacionProyectoDIRINV>();
        var random = new Random();

        // Proyectos de investigación
        for (int i = 1; i <= 10; i++)
        {
            var fechaInicio = DateTime.UtcNow.AddMonths(-random.Next(12, 48));
            var duracion = random.Next(12, 36);
            
            proyectos.Add(new ProyectoInvestigacionDIRINV
            {
                Id = i,
                Titulo = $"Proyecto de Investigación {i}",
                Descripcion = $"Descripción del proyecto de investigación número {i}",
                FechaInicio = fechaInicio,
                FechaFin = fechaInicio.AddMonths(duracion),
                Estado = random.Next(0, 3) switch
                {
                    0 => "En curso",
                    1 => "Finalizado",
                    _ => "Suspendido"
                },
                PresupuestoTotal = random.Next(10000, 50000),
                FuenteFinanciamiento = random.Next(0, 2) == 0 ? "Universidad" : "Externo"
            });
        }

        // Obras académicas para cada docente
        int obraId = 1;
        for (int docente = 1; docente <= 20; docente++)
        {
            var numObras = random.Next(0, 6); // Entre 0 y 5 obras por docente
            
            for (int obra = 0; obra < numObras; obra++)
            {
                var tipoObra = random.Next(0, 4) switch
                {
                    0 => "Artículo",
                    1 => "Libro",
                    2 => "Capítulo",
                    _ => "Ponencia"
                };
                
                obras.Add(new ObraAcademicaDIRINV
                {
                    Id = obraId++,
                    Cedula = $"18000000{docente:00}",
                    Titulo = $"Obra Académica {obra + 1} del Docente {docente}",
                    TipoObra = tipoObra,
                    FechaPublicacion = DateTime.UtcNow.AddMonths(-random.Next(6, 60)),
                    Editorial = tipoObra == "Libro" ? $"Editorial {random.Next(1, 5)}" : null,
                    Revista = tipoObra == "Artículo" ? $"Revista Científica {random.Next(1, 5)}" : null,
                    ISBN_ISSN = $"ISSN-{random.Next(1000, 9999)}-{random.Next(1000, 9999)}",
                    EsIndexada = random.Next(0, 2) == 1,
                    IndiceIndexacion = random.Next(0, 2) == 1 ? "Scopus" : "Latindex"
                });
            }
        }

        // Participaciones en proyectos
        int participacionId = 1;
        for (int docente = 1; docente <= 20; docente++)
        {
            var numProyectos = random.Next(0, 3); // Entre 0 y 2 proyectos por docente
            var proyectosSeleccionados = proyectos.OrderBy(x => random.Next()).Take(numProyectos);
            
            foreach (var proyecto in proyectosSeleccionados)
            {
                var rol = random.Next(0, 3) switch
                {
                    0 => "Director",
                    1 => "Investigador",
                    _ => "Colaborador"
                };
                
                participaciones.Add(new ParticipacionProyectoDIRINV
                {
                    Id = participacionId++,
                    Cedula = $"18000000{docente:00}",
                    ProyectoId = proyecto.Id,
                    RolEnProyecto = rol,
                    FechaInicio = proyecto.FechaInicio,
                    FechaFin = proyecto.FechaFin,
                    HorasSemanales = random.Next(5, 20)
                });
            }
        }

        modelBuilder.Entity<ObraAcademicaDIRINV>().HasData(obras);
        modelBuilder.Entity<ProyectoInvestigacionDIRINV>().HasData(proyectos);
        modelBuilder.Entity<ParticipacionProyectoDIRINV>().HasData(participaciones);
    }
}
