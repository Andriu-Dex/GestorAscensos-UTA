using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities;
using SGA.Domain.Enums;

namespace SGA.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<PromotionRequest> PromotionRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración del modelo Teacher
            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.IdentificationNumber).HasMaxLength(20).IsRequired();
                entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
                entity.Property(e => e.EvaluationScore).HasPrecision(5, 2);
            });

            // Configuración del modelo PromotionRequest
            modelBuilder.Entity<PromotionRequest>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.Comments).HasMaxLength(500);

                // Relación con Teacher
                entity.HasOne(e => e.Teacher)
                      .WithMany(t => t.PromotionRequests)
                      .HasForeignKey(e => e.TeacherId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Datos iniciales
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Crear algunos docentes iniciales para pruebas
            modelBuilder.Entity<Teacher>().HasData(
                new Teacher
                {
                    Id = 1,
                    IdentificationNumber = "0123456789",
                    FirstName = "Juan",
                    LastName = "Pérez",
                    Email = "juan.perez@uta.edu.ec",
                    CurrentRank = AcademicRank.Titular1,
                    StartDateInCurrentRank = new DateTime(2020, 1, 1),
                    Works = 1,
                    EvaluationScore = 85,
                    TrainingHours = 100,
                    ResearchMonths = 0,
                    YearsInCurrentRank = 4
                },
                new Teacher
                {
                    Id = 2,
                    IdentificationNumber = "9876543210",
                    FirstName = "María",
                    LastName = "Gómez",
                    Email = "maria.gomez@uta.edu.ec",
                    CurrentRank = AcademicRank.Titular2,
                    StartDateInCurrentRank = new DateTime(2019, 5, 15),
                    Works = 2,
                    EvaluationScore = 90,
                    TrainingHours = 120,
                    ResearchMonths = 18,
                    YearsInCurrentRank = 5
                }
            );
        }
    }
}
