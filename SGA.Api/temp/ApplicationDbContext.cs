using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SGA.Api.temp;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Docente> Docentes { get; set; }

    public virtual DbSet<Documento> Documentos { get; set; }

    public virtual DbSet<HistorialDocumento> HistorialDocumentos { get; set; }

    public virtual DbSet<LogsAuditorium> LogsAuditoria { get; set; }

    public virtual DbSet<ObrasAcademica> ObrasAcademicas { get; set; }

    public virtual DbSet<SolicitudesAscenso> SolicitudesAscensos { get; set; }

    public virtual DbSet<SolicitudesCertificadosCapacitacion> SolicitudesCertificadosCapacitacions { get; set; }

    public virtual DbSet<SolicitudesEvidenciasInvestigacion> SolicitudesEvidenciasInvestigacions { get; set; }

    public virtual DbSet<SolicitudesObrasAcademica> SolicitudesObrasAcademicas { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Docente>(entity =>
        {
            entity.HasIndex(e => e.Cedula, "IX_Docentes_Cedula").IsUnique();

            entity.HasIndex(e => e.Email, "IX_Docentes_Email").IsUnique();

            entity.HasIndex(e => e.UsuarioId, "IX_Docentes_UsuarioId").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Apellidos).HasMaxLength(255);
            entity.Property(e => e.Cedula).HasMaxLength(10);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Nombres).HasMaxLength(255);
            entity.Property(e => e.PromedioEvaluaciones).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.Usuario).WithOne(p => p.Docente).HasForeignKey<Docente>(d => d.UsuarioId);
        });

        modelBuilder.Entity<Documento>(entity =>
        {
            entity.HasIndex(e => e.DocenteId, "IX_Documentos_DocenteId");

            entity.HasIndex(e => e.SolicitudAscensoId, "IX_Documentos_SolicitudAscensoId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ContentType).HasMaxLength(100);
            entity.Property(e => e.NombreArchivo).HasMaxLength(255);
            entity.Property(e => e.RutaArchivo).HasMaxLength(500);

            entity.HasOne(d => d.Docente).WithMany(p => p.Documentos).HasForeignKey(d => d.DocenteId);

            entity.HasOne(d => d.SolicitudAscenso).WithMany(p => p.Documentos)
                .HasForeignKey(d => d.SolicitudAscensoId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<HistorialDocumento>(entity =>
        {
            entity.HasIndex(e => e.ObraAcademicaId, "IX_HistorialDocumentos_ObraAcademicaId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Comentarios).HasMaxLength(1000);
            entity.Property(e => e.DireccionIp)
                .HasMaxLength(45)
                .HasColumnName("DireccionIP");
            entity.Property(e => e.RealizadoPorNombre).HasMaxLength(255);
            entity.Property(e => e.UserAgent).HasMaxLength(500);

            entity.HasOne(d => d.ObraAcademica).WithMany(p => p.HistorialDocumentos).HasForeignKey(d => d.ObraAcademicaId);
        });

        modelBuilder.Entity<LogsAuditorium>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Accion).HasMaxLength(255);
            entity.Property(e => e.DireccionIp)
                .HasMaxLength(45)
                .HasColumnName("DireccionIP");
            entity.Property(e => e.EntidadAfectada).HasMaxLength(255);
            entity.Property(e => e.UsuarioEmail).HasMaxLength(255);
        });

        modelBuilder.Entity<ObrasAcademica>(entity =>
        {
            entity.HasIndex(e => e.DocenteId, "IX_ObrasAcademicas_DocenteId");

            entity.HasIndex(e => new { e.DocenteId, e.FechaPublicacion }, "IX_ObrasAcademicas_DocenteId_FechaPublicacion");

            entity.HasIndex(e => e.Estado, "IX_ObrasAcademicas_Estado");

            entity.HasIndex(e => new { e.Titulo, e.DocenteId }, "IX_ObrasAcademicas_Titulo_DocenteId").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Autores).HasMaxLength(1000);
            entity.Property(e => e.ComentariosAdmin).HasMaxLength(1000);
            entity.Property(e => e.ComentariosDocente).HasMaxLength(1000);
            entity.Property(e => e.ContenidoArchivoPdf).HasColumnName("ContenidoArchivoPDF");
            entity.Property(e => e.ContentType).HasMaxLength(100);
            entity.Property(e => e.Descripcion).HasMaxLength(2000);
            entity.Property(e => e.Doi)
                .HasMaxLength(200)
                .HasColumnName("DOI");
            entity.Property(e => e.Editorial).HasMaxLength(255);
            entity.Property(e => e.Estado).HasDefaultValue("");
            entity.Property(e => e.IndiceIndexacion).HasMaxLength(100);
            entity.Property(e => e.IsbnIssn)
                .HasMaxLength(50)
                .HasColumnName("ISBN_ISSN");
            entity.Property(e => e.NombreArchivo).HasMaxLength(255);
            entity.Property(e => e.OrigenDatos).HasMaxLength(50);
            entity.Property(e => e.Revista).HasMaxLength(255);
            entity.Property(e => e.TipoObra).HasMaxLength(100);
            entity.Property(e => e.Titulo).HasMaxLength(500);

            entity.HasOne(d => d.Docente).WithMany(p => p.ObrasAcademicas).HasForeignKey(d => d.DocenteId);
        });

        modelBuilder.Entity<SolicitudesAscenso>(entity =>
        {
            entity.ToTable("SolicitudesAscenso");

            entity.HasIndex(e => e.AprobadoPorId, "IX_SolicitudesAscenso_AprobadoPorId");

            entity.HasIndex(e => e.DocenteId, "IX_SolicitudesAscenso_DocenteId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.PromedioEvaluaciones).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.AprobadoPor).WithMany(p => p.SolicitudesAscensos).HasForeignKey(d => d.AprobadoPorId);

            entity.HasOne(d => d.Docente).WithMany(p => p.SolicitudesAscensos)
                .HasForeignKey(d => d.DocenteId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<SolicitudesCertificadosCapacitacion>(entity =>
        {
            entity.ToTable("SolicitudesCertificadosCapacitacion");

            entity.HasIndex(e => e.DocenteCedula, "IX_SolicitudesCertificadosCapacitacion_DocenteCedula");

            entity.HasIndex(e => e.DocenteId, "IX_SolicitudesCertificadosCapacitacion_DocenteId");

            entity.HasIndex(e => e.Estado, "IX_SolicitudesCertificadosCapacitacion_Estado");

            entity.HasIndex(e => e.FechaFin, "IX_SolicitudesCertificadosCapacitacion_FechaFin");

            entity.HasIndex(e => e.FechaInicio, "IX_SolicitudesCertificadosCapacitacion_FechaInicio");

            entity.HasIndex(e => e.RevisadoPorId, "IX_SolicitudesCertificadosCapacitacion_RevisadoPorId");

            entity.HasIndex(e => e.SolicitudGrupoId, "IX_SolicitudesCertificadosCapacitacion_SolicitudGrupoId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ArchivoEstaComprimido).HasDefaultValue(true);
            entity.Property(e => e.ArchivoNombre).HasMaxLength(255);
            entity.Property(e => e.ArchivoTipo).HasMaxLength(100);
            entity.Property(e => e.AreaTematica).HasMaxLength(200);
            entity.Property(e => e.ComentariosRevision).HasMaxLength(1000);
            entity.Property(e => e.ComentariosSolicitud).HasMaxLength(1000);
            entity.Property(e => e.Descripcion).HasMaxLength(2000);
            entity.Property(e => e.DocenteCedula).HasMaxLength(10);
            entity.Property(e => e.Estado).HasMaxLength(50);
            entity.Property(e => e.InstitucionOfertante).HasMaxLength(255);
            entity.Property(e => e.Modalidad).HasMaxLength(50);
            entity.Property(e => e.MotivoRechazo).HasMaxLength(1000);
            entity.Property(e => e.NombreCurso).HasMaxLength(500);
            entity.Property(e => e.NumeroRegistro).HasMaxLength(100);
            entity.Property(e => e.TipoCapacitacion).HasMaxLength(100);

            entity.HasOne(d => d.Docente).WithMany(p => p.SolicitudesCertificadosCapacitacions)
                .HasForeignKey(d => d.DocenteId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.RevisadoPor).WithMany(p => p.SolicitudesCertificadosCapacitacions)
                .HasForeignKey(d => d.RevisadoPorId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<SolicitudesEvidenciasInvestigacion>(entity =>
        {
            entity.ToTable("SolicitudesEvidenciasInvestigacion");

            entity.HasIndex(e => e.DocenteCedula, "IX_SolicitudesEvidenciasInvestigacion_DocenteCedula");

            entity.HasIndex(e => e.DocenteId, "IX_SolicitudesEvidenciasInvestigacion_DocenteId");

            entity.HasIndex(e => e.Estado, "IX_SolicitudesEvidenciasInvestigacion_Estado");

            entity.HasIndex(e => e.FechaFin, "IX_SolicitudesEvidenciasInvestigacion_FechaFin");

            entity.HasIndex(e => e.FechaInicio, "IX_SolicitudesEvidenciasInvestigacion_FechaInicio");

            entity.HasIndex(e => e.TipoEvidencia, "IX_SolicitudesEvidenciasInvestigacion_TipoEvidencia");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ArchivoContenido).HasDefaultValueSql("(0x)");
            entity.Property(e => e.ArchivoEstaComprimido).HasDefaultValue(true);
            entity.Property(e => e.ArchivoNombre).HasMaxLength(255);
            entity.Property(e => e.ArchivoTipo).HasMaxLength(100);
            entity.Property(e => e.AreaTematica).HasMaxLength(200);
            entity.Property(e => e.CodigoProyecto).HasMaxLength(100);
            entity.Property(e => e.ComentariosRevision).HasMaxLength(1000);
            entity.Property(e => e.ComentariosSolicitud).HasMaxLength(1000);
            entity.Property(e => e.Descripcion).HasMaxLength(2000);
            entity.Property(e => e.DocenteCedula).HasMaxLength(10);
            entity.Property(e => e.Estado).HasMaxLength(50);
            entity.Property(e => e.InstitucionFinanciadora).HasMaxLength(255);
            entity.Property(e => e.MotivoRechazo).HasMaxLength(1000);
            entity.Property(e => e.RolInvestigador).HasMaxLength(100);
            entity.Property(e => e.TipoEvidencia).HasMaxLength(100);
            entity.Property(e => e.TituloProyecto).HasMaxLength(500);

            entity.HasOne(d => d.Docente).WithMany(p => p.SolicitudesEvidenciasInvestigacions)
                .HasForeignKey(d => d.DocenteId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<SolicitudesObrasAcademica>(entity =>
        {
            entity.HasIndex(e => e.DocenteCedula, "IX_SolicitudesObrasAcademicas_DocenteCedula");

            entity.HasIndex(e => e.DocenteId, "IX_SolicitudesObrasAcademicas_DocenteId");

            entity.HasIndex(e => e.Estado, "IX_SolicitudesObrasAcademicas_Estado");

            entity.HasIndex(e => e.RevisadoPorId, "IX_SolicitudesObrasAcademicas_RevisadoPorId");

            entity.HasIndex(e => e.SolicitudGrupoId, "IX_SolicitudesObrasAcademicas_SolicitudGrupoId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ArchivoNombre).HasMaxLength(255);
            entity.Property(e => e.ArchivoRuta).HasMaxLength(500);
            entity.Property(e => e.ArchivoTipo).HasMaxLength(100);
            entity.Property(e => e.Autores).HasMaxLength(1000);
            entity.Property(e => e.ComentariosRevision).HasMaxLength(1000);
            entity.Property(e => e.ComentariosSolicitud).HasMaxLength(1000);
            entity.Property(e => e.Descripcion).HasMaxLength(2000);
            entity.Property(e => e.DocenteCedula).HasMaxLength(10);
            entity.Property(e => e.Doi)
                .HasMaxLength(200)
                .HasColumnName("DOI");
            entity.Property(e => e.Editorial).HasMaxLength(255);
            entity.Property(e => e.Estado).HasMaxLength(50);
            entity.Property(e => e.IndiceIndexacion).HasMaxLength(100);
            entity.Property(e => e.IsbnIssn)
                .HasMaxLength(50)
                .HasColumnName("ISBN_ISSN");
            entity.Property(e => e.MotivoRechazo).HasMaxLength(1000);
            entity.Property(e => e.Revista).HasMaxLength(255);
            entity.Property(e => e.TipoObra).HasMaxLength(100);
            entity.Property(e => e.Titulo).HasMaxLength(500);

            entity.HasOne(d => d.Docente).WithMany(p => p.SolicitudesObrasAcademicas)
                .HasForeignKey(d => d.DocenteId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.RevisadoPor).WithMany(p => p.SolicitudesObrasAcademicas)
                .HasForeignKey(d => d.RevisadoPorId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasIndex(e => e.Email, "IX_Usuarios_Email").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Email).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
