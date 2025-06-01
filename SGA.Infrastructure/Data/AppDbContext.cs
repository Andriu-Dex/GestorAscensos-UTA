using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities;

namespace SGA.Infrastructure.Data
{    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Docente> Docentes { get; set; }
        public DbSet<Documento> Documentos { get; set; }
        public DbSet<SolicitudAscenso> SolicitudesAscenso { get; set; }
        public DbSet<DocumentoSolicitud> DocumentosSolicitud { get; set; }
        public DbSet<DatosTTHH> DatosTTHH { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración para Docente
            modelBuilder.Entity<Docente>()
                .HasKey(d => d.Id);
                
            modelBuilder.Entity<Docente>()
                .HasIndex(d => d.Cedula)
                .IsUnique();
                
            modelBuilder.Entity<Docente>()
                .HasIndex(d => d.NombreUsuario)
                .IsUnique();

            // Configuración para Documento
            modelBuilder.Entity<Documento>()
                .HasKey(d => d.Id);
                
            modelBuilder.Entity<Documento>()
                .HasOne(d => d.Docente)
                .WithMany(d => d.Documentos)
                .HasForeignKey(d => d.DocenteId)
                .OnDelete(DeleteBehavior.Restrict); // Cambiado a Restrict

            // Configuración para SolicitudAscenso
            modelBuilder.Entity<SolicitudAscenso>()
                .HasKey(s => s.Id);
                
            modelBuilder.Entity<SolicitudAscenso>()
                .HasOne(s => s.Docente)
                .WithMany(d => d.Solicitudes)
                .HasForeignKey(s => s.DocenteId)
                .OnDelete(DeleteBehavior.Cascade);            // Configuración para DocumentoSolicitud
            modelBuilder.Entity<DocumentoSolicitud>()
                .HasKey(ds => ds.Id);
                
            modelBuilder.Entity<DocumentoSolicitud>()
                .HasOne(ds => ds.Solicitud)
                .WithMany(s => s.Documentos)
                .HasForeignKey(ds => ds.SolicitudId)
                .OnDelete(DeleteBehavior.Cascade);
                
            modelBuilder.Entity<DocumentoSolicitud>()
                .HasOne(ds => ds.Documento)
                .WithMany()
                .HasForeignKey(ds => ds.DocumentoId)
                .OnDelete(DeleteBehavior.Restrict); // Cambiado a Restrict
                
            // Configuración para DatosTTHH
            modelBuilder.Entity<DatosTTHH>()
                .HasKey(dt => dt.Id);
                
            modelBuilder.Entity<DatosTTHH>()
                .HasIndex(dt => dt.Cedula)
                .IsUnique();
        }
    }
}
