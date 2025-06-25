using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities.External;

namespace SGA.Infrastructure.Data.External;

public class DITICDbContext : DbContext
{
    public DITICDbContext(DbContextOptions<DITICDbContext> options) : base(options)
    {
    }

    public DbSet<CursoDITIC> Cursos { get; set; }
    public DbSet<ParticipacionCursoDITIC> Participaciones { get; set; }
    public DbSet<CertificacionDITIC> Certificaciones { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CursoDITIC>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Descripcion).HasMaxLength(500);
            entity.Property(e => e.Modalidad).IsRequired().HasMaxLength(50);
        });

        modelBuilder.Entity<ParticipacionCursoDITIC>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Cedula).IsRequired().HasMaxLength(10);
            entity.Property(e => e.NotaFinal).HasColumnType("decimal(4,2)");
            entity.HasIndex(e => e.Cedula);
            
            entity.HasOne(e => e.Curso)
                  .WithMany(c => c.Participaciones)
                  .HasForeignKey(e => e.CursoId);
        });

        modelBuilder.Entity<CertificacionDITIC>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Cedula).IsRequired().HasMaxLength(10);
            entity.Property(e => e.NombreCertificacion).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Institucion).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => e.Cedula);
        });

        // Datos semilla para DITIC
        SeedDITICData(modelBuilder);
    }

    private void SeedDITICData(ModelBuilder modelBuilder)
    {
        // Cursos disponibles
        var cursos = new List<CursoDITIC>
        {
            new() { Id = 1, Nombre = "Metodologías Ágiles", Descripcion = "Curso sobre metodologías ágiles de desarrollo", HorasDuracion = 40, Modalidad = "Virtual", EstaActivo = true },
            new() { Id = 2, Nombre = "Pedagogía Universitaria", Descripcion = "Fundamentos de pedagogía para educación superior", HorasDuracion = 60, Modalidad = "Presencial", EstaActivo = true },
            new() { Id = 3, Nombre = "Investigación Científica", Descripcion = "Metodología de investigación científica", HorasDuracion = 80, Modalidad = "Mixta", EstaActivo = true },
            new() { Id = 4, Nombre = "Tecnologías Educativas", Descripcion = "Uso de tecnología en educación", HorasDuracion = 32, Modalidad = "Virtual", EstaActivo = true },
            new() { Id = 5, Nombre = "Evaluación Educativa", Descripcion = "Técnicas de evaluación del aprendizaje", HorasDuracion = 48, Modalidad = "Presencial", EstaActivo = true }
        };

        // Participaciones de docentes en cursos
        var participaciones = new List<ParticipacionCursoDITIC>();
        var certificaciones = new List<CertificacionDITIC>();
        var random = new Random();
        
        for (int docente = 1; docente <= 20; docente++)
        {
            var numCursos = random.Next(2, 6); // Entre 2 y 5 cursos por docente
            var cursosSeleccionados = cursos.OrderBy(x => random.Next()).Take(numCursos);
            
            int participacionId = (docente - 1) * 5 + 1;
            
            foreach (var curso in cursosSeleccionados)
            {
                var fechaInicio = DateTime.UtcNow.AddMonths(-random.Next(6, 36));
                var fechaFin = fechaInicio.AddDays(curso.HorasDuracion);
                var nota = random.Next(70, 100);
                
                participaciones.Add(new ParticipacionCursoDITIC
                {
                    Id = participacionId++,
                    Cedula = $"18000000{docente:00}",
                    CursoId = curso.Id,
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    Aprobado = nota >= 70,
                    NotaFinal = nota,
                    CertificadoEmitido = nota >= 70
                });
                
                if (nota >= 70)
                {
                    certificaciones.Add(new CertificacionDITIC
                    {
                        Id = participacionId,
                        Cedula = $"18000000{docente:00}",
                        NombreCertificacion = curso.Nombre,
                        Institucion = "Universidad Técnica de Ambato",
                        FechaEmision = fechaFin,
                        FechaVencimiento = fechaFin.AddYears(3),
                        HorasEquivalentes = curso.HorasDuracion
                    });
                }
            }
        }

        modelBuilder.Entity<CursoDITIC>().HasData(cursos);
        modelBuilder.Entity<ParticipacionCursoDITIC>().HasData(participaciones);
        modelBuilder.Entity<CertificacionDITIC>().HasData(certificaciones);
    }
}
