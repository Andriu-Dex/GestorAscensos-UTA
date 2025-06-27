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
        // Períodos académicos basados en los requerimientos
        var periodos = new List<PeriodoAcademicoDAC>
        {
            new() { Id = 1, Nombre = "2021-1", FechaInicio = new DateTime(2021, 2, 1), FechaFin = new DateTime(2021, 6, 30), EstaActivo = false },
            new() { Id = 2, Nombre = "2021-2", FechaInicio = new DateTime(2021, 9, 1), FechaFin = new DateTime(2022, 1, 31), EstaActivo = false },
            new() { Id = 3, Nombre = "2022-1", FechaInicio = new DateTime(2022, 2, 1), FechaFin = new DateTime(2022, 6, 30), EstaActivo = false },
            new() { Id = 4, Nombre = "2022-2", FechaInicio = new DateTime(2022, 9, 1), FechaFin = new DateTime(2023, 1, 31), EstaActivo = false },
            new() { Id = 5, Nombre = "2023-1", FechaInicio = new DateTime(2023, 2, 1), FechaFin = new DateTime(2023, 6, 30), EstaActivo = false },
            new() { Id = 6, Nombre = "2023-2", FechaInicio = new DateTime(2023, 9, 1), FechaFin = new DateTime(2024, 1, 31), EstaActivo = false },
            new() { Id = 7, Nombre = "2024-1", FechaInicio = new DateTime(2024, 2, 1), FechaFin = new DateTime(2024, 6, 30), EstaActivo = false },
            new() { Id = 8, Nombre = "2024-2", FechaInicio = new DateTime(2024, 9, 1), FechaFin = new DateTime(2025, 1, 31), EstaActivo = false },
            new() { Id = 9, Nombre = "2025-1", FechaInicio = new DateTime(2025, 2, 1), FechaFin = new DateTime(2025, 6, 30), EstaActivo = true }
        };

        // Evaluaciones para diferentes docentes con datos más realistas
        var evaluaciones = new List<EvaluacionDocenteDAC>();
        var random = new Random(42); // Seed fijo para consistencia
        
        // Crear evaluaciones para 50 docentes en múltiples períodos
        for (int docente = 1; docente <= 50; docente++)
        {
            // Determinar cuántos períodos evaluar por docente (entre 4 y 8)
            var numPeriodos = random.Next(4, 9);
            var periodosParaDocente = periodos.OrderBy(x => random.Next()).Take(numPeriodos).OrderBy(p => p.Id);
            
            int evaluacionId = (docente - 1) * 8 + 1;
            
            foreach (var periodo in periodosParaDocente)
            {
                // Crear evaluaciones con diferentes tendencias por docente
                decimal baseScore;
                if (docente <= 15) // Primeros 15 docentes: excelentes (85-95%)
                    baseScore = 85 + (decimal)(random.NextDouble() * 10);
                else if (docente <= 35) // Siguientes 20: buenos (75-85%)
                    baseScore = 75 + (decimal)(random.NextDouble() * 10);
                else // Últimos 15: necesitan mejorar (65-78%)
                    baseScore = 65 + (decimal)(random.NextDouble() * 13);
                
                // Añadir variación pequeña por período
                var variacion = (decimal)(random.NextDouble() * 6) - 3; // ±3%
                var puntajeFinal = Math.Max(60, Math.Min(100, baseScore + variacion));
                
                evaluaciones.Add(new EvaluacionDocenteDAC
                {
                    Id = evaluacionId++,
                    Cedula = $"18000000{docente:00}",
                    PeriodoId = periodo.Id,
                    PuntajeTotal = puntajeFinal,
                    PuntajeMaximo = 100,
                    Porcentaje = puntajeFinal,
                    FechaEvaluacion = periodo.FechaFin.AddDays(-7), // Una semana antes del fin del período
                    Observaciones = GenerarObservacion(puntajeFinal)
                });
            }
        }

        modelBuilder.Entity<PeriodoAcademicoDAC>().HasData(periodos);
        modelBuilder.Entity<EvaluacionDocenteDAC>().HasData(evaluaciones);
        
        // Criterios de evaluación mejorados
        modelBuilder.Entity<CriterioEvaluacionDAC>().HasData(
            new CriterioEvaluacionDAC { Id = 1, Nombre = "Planificación Académica", Descripcion = "Planificación y organización de actividades académicas", PesoMaximo = 20, EstaActivo = true },
            new CriterioEvaluacionDAC { Id = 2, Nombre = "Ejecución Docente", Descripcion = "Desarrollo y ejecución de clases", PesoMaximo = 35, EstaActivo = true },
            new CriterioEvaluacionDAC { Id = 3, Nombre = "Evaluación Estudiantil", Descripcion = "Proceso de evaluación del aprendizaje estudiantil", PesoMaximo = 25, EstaActivo = true },
            new CriterioEvaluacionDAC { Id = 4, Nombre = "Tutoría y Orientación", Descripcion = "Actividades de tutoría y orientación estudiantil", PesoMaximo = 15, EstaActivo = true },
            new CriterioEvaluacionDAC { Id = 5, Nombre = "Investigación", Descripcion = "Actividades de investigación y publicación", PesoMaximo = 5, EstaActivo = true }
        );
    }
    
    private static string GenerarObservacion(decimal puntaje)
    {
        return puntaje switch
        {
            >= 90 => "Excelente desempeño docente. Cumple y supera todos los estándares.",
            >= 80 => "Muy buen desempeño docente. Cumple satisfactoriamente los estándares.",
            >= 75 => "Buen desempeño docente. Cumple los estándares mínimos.",
            >= 70 => "Desempeño aceptable. Requiere mejoras menores.",
            _ => "Desempeño deficiente. Requiere plan de mejoramiento."
        };
    }
}
