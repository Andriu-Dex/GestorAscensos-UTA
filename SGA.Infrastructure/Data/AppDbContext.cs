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
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<AcademicDegree> AcademicDegrees { get; set; }
        public DbSet<Requirement> Requirements { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentObservation> DocumentObservations { get; set; }
        public DbSet<PromotionObservation> PromotionObservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);            // Configuración del modelo Teacher
            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.IdentificationNumber).HasMaxLength(20).IsRequired();
                entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Password).HasMaxLength(100).IsRequired();
                entity.Property(e => e.EvaluationScore).HasPrecision(5, 2);
                
                // Relación con UserType
                entity.HasOne(e => e.UserType)
                      .WithMany(t => t.Teachers)
                      .HasForeignKey(e => e.UserTypeId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración del modelo UserType
            modelBuilder.Entity<UserType>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(255);
            });
            
            // Configuración del modelo AcademicDegree
            modelBuilder.Entity<AcademicDegree>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DegreeType).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Title).HasMaxLength(100).IsRequired();
                entity.Property(e => e.IssuingInstitution).HasMaxLength(100).IsRequired();
                
                // Relación con Teacher
                entity.HasOne(e => e.Teacher)
                      .WithMany(t => t.AcademicDegrees)
                      .HasForeignKey(e => e.TeacherId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            
            // Configuración del modelo Requirement
            modelBuilder.Entity<Requirement>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.MinimumEvaluationScore).HasPrecision(5, 2);
            });
            
            // Configuración del modelo Document
            modelBuilder.Entity<Document>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(255);
                entity.Property(e => e.DocumentType).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Department).HasMaxLength(100);
                entity.Property(e => e.IssuingInstitution).HasMaxLength(100);
                
                // Relación con Teacher (Propietario)
                entity.HasOne(e => e.Teacher)
                      .WithMany(t => t.Documents)
                      .HasForeignKey(e => e.TeacherId)
                      .OnDelete(DeleteBehavior.Restrict);
                
                // Relación con Teacher (Revisor)
                entity.HasOne(e => e.Reviewer)
                      .WithMany()
                      .HasForeignKey(e => e.ReviewerId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired(false);
                
                // Relación con Requirement
                entity.HasOne(e => e.Requirement)
                      .WithMany(r => r.Documents)
                      .HasForeignKey(e => e.RequirementId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .IsRequired(false);
            });
            
            // Configuración del modelo DocumentObservation
            modelBuilder.Entity<DocumentObservation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Description).HasMaxLength(255).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                
                // Relación con Document
                entity.HasOne(e => e.Document)
                      .WithMany(d => d.Observations)
                      .HasForeignKey(e => e.DocumentId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                // Relación con Teacher (Revisor)
                entity.HasOne(e => e.Reviewer)
                      .WithMany()
                      .HasForeignKey(e => e.ReviewerId)
                      .OnDelete(DeleteBehavior.Restrict);
            });            // Configuración del modelo PromotionRequest
            modelBuilder.Entity<PromotionRequest>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.Comments).HasMaxLength(500);

                // Relación con Teacher (Solicitante)
                entity.HasOne(e => e.Teacher)
                      .WithMany(t => t.PromotionRequests)
                      .HasForeignKey(e => e.TeacherId)
                      .OnDelete(DeleteBehavior.Restrict);
                
                // Relación con Teacher (Revisor)
                entity.HasOne(e => e.Reviewer)
                      .WithMany()
                      .HasForeignKey(e => e.ReviewerId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired(false);
                
                // Relación con Document
                entity.HasOne(e => e.Document)
                      .WithMany()
                      .HasForeignKey(e => e.DocumentId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .IsRequired(false);
            });
            
            // Configuración del modelo PromotionObservation
            modelBuilder.Entity<PromotionObservation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Description).HasMaxLength(255).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                
                // Relación con PromotionRequest
                entity.HasOne(e => e.PromotionRequest)
                      .WithMany(p => p.Observations)
                      .HasForeignKey(e => e.PromotionRequestId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Datos iniciales
            SeedData(modelBuilder);
        }        private void SeedData(ModelBuilder modelBuilder)
        {
            // Datos iniciales para UserType
            modelBuilder.Entity<UserType>().HasData(
                new 
                {
                    Id = 1,
                    Name = "Administrador",
                    Description = "Usuario con acceso total al sistema"
                },
                new 
                {
                    Id = 2,
                    Name = "Docente",
                    Description = "Docente que puede solicitar ascensos"
                },
                new 
                {
                    Id = 3,
                    Name = "Evaluador",
                    Description = "Usuario que evalúa las solicitudes de ascenso"
                }
            );
            
            // Datos iniciales para Requirement
            modelBuilder.Entity<Requirement>().HasData(
                new 
                {
                    Id = 1,
                    Name = "Requisitos para Titular 2",
                    YearsInCurrentRank = 4,
                    RequiredWorks = 1,
                    MinimumEvaluationScore = 80m,
                    RequiredTrainingHours = 100,
                    RequiredResearchMonths = 0
                },
                new 
                {
                    Id = 2,
                    Name = "Requisitos para Titular 3",
                    YearsInCurrentRank = 4,
                    RequiredWorks = 2,
                    MinimumEvaluationScore = 85m,
                    RequiredTrainingHours = 150,
                    RequiredResearchMonths = 12
                },
                new 
                {
                    Id = 3,
                    Name = "Requisitos para Titular 4",
                    YearsInCurrentRank = 5,
                    RequiredWorks = 3,
                    MinimumEvaluationScore = 85m,
                    RequiredTrainingHours = 200,
                    RequiredResearchMonths = 24
                },
                new 
                {
                    Id = 4,
                    Name = "Requisitos para Titular 5",
                    YearsInCurrentRank = 5,
                    RequiredWorks = 4,
                    MinimumEvaluationScore = 90m,
                    RequiredTrainingHours = 250,
                    RequiredResearchMonths = 36
                }
            );

            // Crear algunos docentes iniciales para pruebas
            modelBuilder.Entity<Teacher>().HasData(
                new 
                {
                    Id = 1,
                    IdentificationNumber = "0123456789",
                    FirstName = "Juan",
                    LastName = "Pérez",
                    Email = "juan.perez@uta.edu.ec",
                    Password = "Contraseña123", // En producción usaríamos contraseñas hasheadas
                    UserTypeId = 2, // Tipo docente
                    CurrentRank = AcademicRank.Titular1,
                    StartDateInCurrentRank = new DateTime(2020, 1, 1),
                    DaysInCurrentRank = 1440, // Aproximadamente 4 años
                    Works = 1,
                    EvaluationScore = 85m,
                    TrainingHours = 100,
                    ResearchMonths = 0,
                    YearsInCurrentRank = 4
                },
                new 
                {
                    Id = 2,
                    IdentificationNumber = "9876543210",
                    FirstName = "María",
                    LastName = "Gómez",
                    Email = "maria.gomez@uta.edu.ec",
                    Password = "Contraseña456", // En producción usaríamos contraseñas hasheadas
                    UserTypeId = 2, // Tipo docente
                    CurrentRank = AcademicRank.Titular2,
                    StartDateInCurrentRank = new DateTime(2019, 5, 15),
                    DaysInCurrentRank = 1800, // Aproximadamente 5 años
                    Works = 2,
                    EvaluationScore = 90m,
                    TrainingHours = 120,
                    ResearchMonths = 18,
                    YearsInCurrentRank = 5
                },
                new 
                {
                    Id = 3,
                    IdentificationNumber = "1122334455",
                    FirstName = "Carlos",
                    LastName = "Rodríguez",
                    Email = "carlos.rodriguez@uta.edu.ec",
                    Password = "Contraseña789", // En producción usaríamos contraseñas hasheadas
                    UserTypeId = 3, // Tipo evaluador
                    CurrentRank = AcademicRank.Titular5,
                    StartDateInCurrentRank = new DateTime(2015, 3, 10),
                    DaysInCurrentRank = 3000, // Más de 8 años
                    Works = 10,
                    EvaluationScore = 95m,
                    TrainingHours = 500,
                    ResearchMonths = 60,
                    YearsInCurrentRank = 8
                }
            );
            
            // Datos iniciales para AcademicDegree
            modelBuilder.Entity<AcademicDegree>().HasData(
                new 
                {
                    Id = 1,
                    DegreeType = "Maestría",
                    Title = "Máster en Ciencias de la Computación",
                    IssuingInstitution = "Universidad Técnica de Ambato",
                    TeacherId = 1
                },
                new 
                {
                    Id = 2,
                    DegreeType = "Doctorado",
                    Title = "PhD en Ingeniería de Software",
                    IssuingInstitution = "Universidad de Barcelona",
                    TeacherId = 2
                },
                new 
                {
                    Id = 3,
                    DegreeType = "Doctorado",
                    Title = "PhD en Ciencias de la Computación",
                    IssuingInstitution = "Universidad Politécnica de Madrid",
                    TeacherId = 3
                }
            );
        }
    }
}
