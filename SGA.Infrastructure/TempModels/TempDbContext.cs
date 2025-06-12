using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SGA.Infrastructure.TempModels;

public partial class TempDbContext : DbContext
{
    public TempDbContext()
    {
    }

    public TempDbContext(DbContextOptions<TempDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AcademicDegree> AcademicDegrees { get; set; }

    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<DocumentObservation> DocumentObservations { get; set; }

    public virtual DbSet<PromotionObservation> PromotionObservations { get; set; }

    public virtual DbSet<PromotionRequest> PromotionRequests { get; set; }

    public virtual DbSet<Requirement> Requirements { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    public virtual DbSet<UserType> UserTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=SistemaGestionAscensos;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True;Encrypt=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AcademicDegree>(entity =>
        {
            entity.HasIndex(e => e.TeacherId, "IX_AcademicDegrees_TeacherId");

            entity.Property(e => e.DegreeType).HasMaxLength(50);
            entity.Property(e => e.IssuingInstitution).HasMaxLength(100);
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.Teacher).WithMany(p => p.AcademicDegrees).HasForeignKey(d => d.TeacherId);
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasIndex(e => e.RequirementId, "IX_Documents_RequirementId");

            entity.HasIndex(e => e.ReviewerId, "IX_Documents_ReviewerId");

            entity.HasIndex(e => e.TeacherId, "IX_Documents_TeacherId");

            entity.Property(e => e.Department).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.DocumentType).HasMaxLength(50);
            entity.Property(e => e.IssuingInstitution).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Requirement).WithMany(p => p.Documents)
                .HasForeignKey(d => d.RequirementId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(d => d.Reviewer).WithMany(p => p.DocumentReviewers).HasForeignKey(d => d.ReviewerId);

            entity.HasOne(d => d.Teacher).WithMany(p => p.DocumentTeachers)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<DocumentObservation>(entity =>
        {
            entity.HasIndex(e => e.DocumentId, "IX_DocumentObservations_DocumentId");

            entity.HasIndex(e => e.ReviewerId, "IX_DocumentObservations_ReviewerId");

            entity.Property(e => e.Description).HasMaxLength(255);

            entity.HasOne(d => d.Document).WithMany(p => p.DocumentObservations).HasForeignKey(d => d.DocumentId);

            entity.HasOne(d => d.Reviewer).WithMany(p => p.DocumentObservations)
                .HasForeignKey(d => d.ReviewerId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PromotionObservation>(entity =>
        {
            entity.HasIndex(e => e.PromotionRequestId, "IX_PromotionObservations_PromotionRequestId");

            entity.Property(e => e.Description).HasMaxLength(255);

            entity.HasOne(d => d.PromotionRequest).WithMany(p => p.PromotionObservations).HasForeignKey(d => d.PromotionRequestId);
        });

        modelBuilder.Entity<PromotionRequest>(entity =>
        {
            entity.HasIndex(e => e.DocumentId, "IX_PromotionRequests_DocumentId");

            entity.HasIndex(e => e.ReviewerId, "IX_PromotionRequests_ReviewerId");

            entity.HasIndex(e => e.TeacherId, "IX_PromotionRequests_TeacherId");

            entity.Property(e => e.Comments).HasMaxLength(500);

            entity.HasOne(d => d.Document).WithMany(p => p.PromotionRequests)
                .HasForeignKey(d => d.DocumentId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(d => d.Reviewer).WithMany(p => p.PromotionRequestReviewers).HasForeignKey(d => d.ReviewerId);

            entity.HasOne(d => d.Teacher).WithMany(p => p.PromotionRequestTeachers)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Requirement>(entity =>
        {
            entity.Property(e => e.MinimumEvaluationScore).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasIndex(e => e.UserTypeId, "IX_Teachers_UserTypeId");

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.EvaluationScore).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.IdentificationNumber).HasMaxLength(20);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(100);

            entity.HasOne(d => d.UserType).WithMany(p => p.Teachers)
                .HasForeignKey(d => d.UserTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<UserType>(entity =>
        {
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
