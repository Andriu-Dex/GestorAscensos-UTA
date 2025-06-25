using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities.External;

namespace SGA.Infrastructure.Data.External;

public class DACDbContext : DbContext
{
    public DACDbContext(DbContextOptions<DACDbContext> options) : base(options)
    {
    }

    public DbSet<EvaluacionDocenteDAC> Evaluaciones { get; set; }
    public DbSet<PeriodoAcademicoDAC> Periodos { get; set; }
    public DbSet<CriterioEvaluacionDAC> Criterios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EvaluacionDocenteDAC>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Cedula).IsRequired().HasMaxLength(10);
            entity.Property(e => e.PuntajeTotal).HasColumnType("decimal(5,2)");
            entity.Property(e => e.PuntajeMaximo).HasColumnType("decimal(5,2)");
            entity.Property(e => e.Porcentaje).HasColumnType("decimal(5,2)");
            entity.HasIndex(e => e.Cedula);
            
            entity.HasOne(e => e.Periodo)
                  .WithMany(p => p.Evaluaciones)
                  .HasForeignKey(e => e.PeriodoId);
        });

        modelBuilder.Entity<PeriodoAcademicoDAC>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<CriterioEvaluacionDAC>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Descripcion).HasMaxLength(500);
            entity.Property(e => e.PesoMaximo).HasColumnType("decimal(5,2)");
        });

        // Datos semilla para DAC
        SeedDACData(modelBuilder);
    }

    private void SeedDACData(ModelBuilder modelBuilder)
    {
        // Períodos académicos
        var periodos = new List<PeriodoAcademicoDAC>
        {
            new() { Id = 1, Nombre = "2023-I", FechaInicio = new DateTime(2023, 3, 1), FechaFin = new DateTime(2023, 7, 31), EstaActivo = false },
            new() { Id = 2, Nombre = "2023-II", FechaInicio = new DateTime(2023, 9, 1), FechaFin = new DateTime(2024, 2, 28), EstaActivo = false },
            new() { Id = 3, Nombre = "2024-I", FechaInicio = new DateTime(2024, 3, 1), FechaFin = new DateTime(2024, 7, 31), EstaActivo = false },
            new() { Id = 4, Nombre = "2024-II", FechaInicio = new DateTime(2024, 9, 1), FechaFin = new DateTime(2025, 2, 28), EstaActivo = true }
        };

        // Evaluaciones para diferentes docentes
        var evaluaciones = new List<EvaluacionDocenteDAC>();
        var random = new Random();
        
        for (int docente = 1; docente <= 20; docente++)
        {
            for (int periodo = 1; periodo <= 4; periodo++)
            {
                var puntaje = random.Next(70, 100);
                evaluaciones.Add(new EvaluacionDocenteDAC
                {
                    Id = (docente - 1) * 4 + periodo,
                    Cedula = $"18000000{docente:00}",
                    PeriodoId = periodo,
                    PuntajeTotal = puntaje,
                    PuntajeMaximo = 100,
                    Porcentaje = puntaje,
                    FechaEvaluacion = DateTime.UtcNow.AddMonths(-(5 - periodo) * 6),
                    Observaciones = $"Evaluación período {periodo}"
                });
            }
        }

        modelBuilder.Entity<PeriodoAcademicoDAC>().HasData(periodos);
        modelBuilder.Entity<EvaluacionDocenteDAC>().HasData(evaluaciones);
        
        modelBuilder.Entity<CriterioEvaluacionDAC>().HasData(
            new CriterioEvaluacionDAC { Id = 1, Nombre = "Planificación", Descripcion = "Planificación de actividades académicas", PesoMaximo = 20, EstaActivo = true },
            new CriterioEvaluacionDAC { Id = 2, Nombre = "Ejecución", Descripcion = "Ejecución de clases", PesoMaximo = 30, EstaActivo = true },
            new CriterioEvaluacionDAC { Id = 3, Nombre = "Evaluación", Descripcion = "Proceso de evaluación estudiantil", PesoMaximo = 25, EstaActivo = true },
            new CriterioEvaluacionDAC { Id = 4, Nombre = "Tutoría", Descripcion = "Actividades de tutoría", PesoMaximo = 15, EstaActivo = true },
            new CriterioEvaluacionDAC { Id = 5, Nombre = "Investigación", Descripcion = "Actividades de investigación", PesoMaximo = 10, EstaActivo = true }
        );
    }
}
