using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities;

namespace SGA.Infrastructure.Data
{    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }        public DbSet<Docente> Docentes { get; set; }
        public DbSet<Documento> Documentos { get; set; }
        public DbSet<SolicitudAscenso> SolicitudesAscenso { get; set; }
        public DbSet<DocumentoSolicitud> DocumentosSolicitud { get; set; }
        public DbSet<DatosTTHH> DatosTTHH { get; set; }
        public DbSet<Facultad> Facultades { get; set; }
        public DbSet<TipoDocumento> TiposDocumento { get; set; }
        public DbSet<IndicadorDocente> IndicadoresDocente { get; set; }
        public DbSet<EstadoSolicitud> EstadosSolicitud { get; set; }
        public DbSet<ServicioExterno> ServiciosExternos { get; set; }
        public DbSet<LogAuditoria> LogsAuditoria { get; set; }
        public DbSet<ConfiguracionSistema> ConfiguracionesSistema { get; set; }        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigurarFacultad(modelBuilder);
            ConfigurarDocente(modelBuilder);
            ConfigurarDatosTTHH(modelBuilder);
            ConfigurarTipoDocumento(modelBuilder);
            ConfigurarDocumento(modelBuilder);
            ConfigurarIndicadorDocente(modelBuilder);
            ConfigurarEstadoSolicitud(modelBuilder);
            ConfigurarSolicitudAscenso(modelBuilder);
            ConfigurarDocumentoSolicitud(modelBuilder);
            ConfigurarServicioExterno(modelBuilder);
            ConfigurarLogAuditoria(modelBuilder);
            ConfigurarConfiguracionSistema(modelBuilder);
            
            SeedData(modelBuilder);
        }

        private void ConfigurarFacultad(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Facultad>(entity =>
            {
                entity.HasKey(f => f.Id);
                entity.HasIndex(f => f.Codigo).IsUnique();
                entity.Property(f => f.Codigo).HasMaxLength(10).IsRequired();
                entity.Property(f => f.Nombre).HasMaxLength(200).IsRequired();
                entity.Property(f => f.Descripcion).HasMaxLength(500);
            });
        }

        private void ConfigurarDocente(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Docente>(entity =>
            {
                entity.HasKey(d => d.Id);
                entity.HasIndex(d => d.Cedula).IsUnique();
                entity.HasIndex(d => d.NombreUsuario).IsUnique();
                entity.HasIndex(d => d.Email).IsUnique();
                
                entity.Property(d => d.Cedula).HasMaxLength(10).IsRequired();
                entity.Property(d => d.Nombres).HasMaxLength(100).IsRequired();
                entity.Property(d => d.Apellidos).HasMaxLength(100).IsRequired();
                entity.Property(d => d.Email).HasMaxLength(150).IsRequired();
                entity.Property(d => d.TelefonoContacto).HasMaxLength(15).IsRequired();
                entity.Property(d => d.NombreUsuario).HasMaxLength(50).IsRequired();
                entity.Property(d => d.PasswordHash).HasMaxLength(500).IsRequired();
                entity.Property(d => d.MotivoBaja).HasMaxLength(500);
                
                entity.HasOne(d => d.Facultad)
                    .WithMany(f => f.Docentes)
                    .HasForeignKey(d => d.FacultadId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigurarDatosTTHH(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DatosTTHH>(entity =>
            {
                entity.HasKey(dt => dt.Id);
                entity.HasIndex(dt => dt.Cedula).IsUnique();
                
                entity.Property(dt => dt.Cedula).HasMaxLength(10).IsRequired();
                entity.Property(dt => dt.Nombres).HasMaxLength(100).IsRequired();
                entity.Property(dt => dt.Apellidos).HasMaxLength(100).IsRequired();
                entity.Property(dt => dt.Celular).HasMaxLength(15);
                entity.Property(dt => dt.TelefonoConvencional).HasMaxLength(15);
                entity.Property(dt => dt.EmailPersonal).HasMaxLength(150);
                entity.Property(dt => dt.Direccion).HasMaxLength(300);
                entity.Property(dt => dt.EstadoCivil).HasMaxLength(20);
                
                entity.HasOne(dt => dt.Facultad)
                    .WithMany(f => f.DatosTTHH)
                    .HasForeignKey(dt => dt.FacultadId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigurarTipoDocumento(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TipoDocumento>(entity =>
            {
                entity.HasKey(td => td.Id);
                entity.HasIndex(td => td.Codigo).IsUnique();
                
                entity.Property(td => td.Codigo).HasMaxLength(20).IsRequired();
                entity.Property(td => td.Nombre).HasMaxLength(100).IsRequired();
                entity.Property(td => td.Descripcion).HasMaxLength(500);
                entity.Property(td => td.FormatoEsperado).HasMaxLength(10);
            });
        }

        private void ConfigurarDocumento(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Documento>(entity =>
            {
                entity.HasKey(d => d.Id);
                
                entity.Property(d => d.Nombre).HasMaxLength(200).IsRequired();
                entity.Property(d => d.Descripcion).HasMaxLength(1000);
                entity.Property(d => d.ContentType).HasMaxLength(100).IsRequired();
                entity.Property(d => d.ObservacionesValidacion).HasMaxLength(1000);
                entity.Property(d => d.HashSHA256).HasMaxLength(64);
                
                entity.HasOne(d => d.Docente)
                    .WithMany(doc => doc.Documentos)
                    .HasForeignKey(d => d.DocenteId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(d => d.TipoDocumento)
                    .WithMany(td => td.Documentos)
                    .HasForeignKey(d => d.TipoDocumentoId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(d => d.ValidadoPor)
                    .WithMany()
                    .HasForeignKey(d => d.ValidadoPorId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }

        private void ConfigurarIndicadorDocente(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IndicadorDocente>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.HasIndex(i => i.DocenteId).IsUnique();
                
                entity.Property(i => i.PuntajeEvaluacion).HasColumnType("decimal(5,2)");
                entity.Property(i => i.FuenteObras).HasMaxLength(100);
                entity.Property(i => i.FuenteEvaluacion).HasMaxLength(100);
                entity.Property(i => i.FuenteCapacitacion).HasMaxLength(100);
                entity.Property(i => i.FuenteInvestigacion).HasMaxLength(100);
                
                entity.HasOne(i => i.Docente)
                    .WithOne(d => d.Indicadores)
                    .HasForeignKey<IndicadorDocente>(i => i.DocenteId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private void ConfigurarEstadoSolicitud(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EstadoSolicitud>(entity =>
            {
                entity.HasKey(es => es.Id);
                entity.HasIndex(es => es.Codigo).IsUnique();
                  entity.Property(es => es.Codigo).HasMaxLength(20).IsRequired();
                entity.Property(es => es.Nombre).HasMaxLength(100).IsRequired();
                entity.Property(es => es.Descripcion).HasMaxLength(500);
                entity.Property(es => es.Color).HasMaxLength(7);
            });
        }

        private void ConfigurarSolicitudAscenso(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SolicitudAscenso>(entity =>
            {
                entity.HasKey(s => s.Id);
                
                entity.Property(s => s.PuntajeEvaluacion).HasColumnType("decimal(5,2)");
                entity.Property(s => s.MotivoRechazo).HasMaxLength(1000);
                entity.Property(s => s.ObservacionesRevisor).HasMaxLength(1000);
                
                entity.HasOne(s => s.Docente)
                    .WithMany(d => d.Solicitudes)
                    .HasForeignKey(s => s.DocenteId)
                    .OnDelete(DeleteBehavior.Restrict);
                  entity.HasOne(s => s.EstadoSolicitud)
                    .WithMany(es => es.SolicitudesAscenso)
                    .HasForeignKey(s => s.EstadoSolicitudId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(s => s.Revisor)
                    .WithMany()
                    .HasForeignKey(s => s.RevisorId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }

        private void ConfigurarDocumentoSolicitud(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DocumentoSolicitud>(entity =>
            {
                entity.HasKey(ds => ds.Id);
                
                entity.Property(ds => ds.Observaciones).HasMaxLength(500);
                
                entity.HasOne(ds => ds.Solicitud)
                    .WithMany(s => s.Documentos)
                    .HasForeignKey(ds => ds.SolicitudId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(ds => ds.Documento)
                    .WithMany(d => d.DocumentosSolicitud)
                    .HasForeignKey(ds => ds.DocumentoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigurarServicioExterno(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ServicioExterno>(entity =>
            {
                entity.HasKey(se => se.Id);
                entity.HasIndex(se => se.Codigo).IsUnique();
                
                entity.Property(se => se.Codigo).HasMaxLength(20).IsRequired();
                entity.Property(se => se.Nombre).HasMaxLength(100).IsRequired();
                entity.Property(se => se.Descripcion).HasMaxLength(500);
                entity.Property(se => se.UrlBase).HasMaxLength(500).IsRequired();
                entity.Property(se => se.ApiKey).HasMaxLength(500);
                entity.Property(se => se.UltimoError).HasMaxLength(1000);
            });
        }

        private void ConfigurarLogAuditoria(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LogAuditoria>(entity =>
            {
                entity.HasKey(la => la.Id);
                
                entity.Property(la => la.Accion).HasMaxLength(100).IsRequired();
                entity.Property(la => la.Entidad).HasMaxLength(100);
                entity.Property(la => la.DireccionIP).HasMaxLength(45).IsRequired();
                entity.Property(la => la.UserAgent).HasMaxLength(500);
                entity.Property(la => la.Observaciones).HasMaxLength(1000);
                
                entity.HasOne(la => la.Docente)
                    .WithMany(d => d.LogsAuditoria)
                    .HasForeignKey(la => la.DocenteId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }

        private void ConfigurarConfiguracionSistema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ConfiguracionSistema>(entity =>
            {
                entity.HasKey(cs => cs.Id);
                entity.HasIndex(cs => cs.Clave).IsUnique();
                
                entity.Property(cs => cs.Clave).HasMaxLength(100).IsRequired();
                entity.Property(cs => cs.Valor).HasMaxLength(500).IsRequired();
                entity.Property(cs => cs.Descripcion).HasMaxLength(500);
                entity.Property(cs => cs.TipoDato).HasMaxLength(20).IsRequired();
                entity.Property(cs => cs.GrupoConfiguracion).HasMaxLength(50);
            });
        }        private void SeedData(ModelBuilder modelBuilder)
        {
            // Fecha estática para datos semilla - para hacer el modelo determinista
            var seedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);            // Datos iniciales para Facultades
            modelBuilder.Entity<Facultad>().HasData(
                new Facultad { Id = 1, Codigo = "FISEI", Nombre = "Facultad de Ingeniería en Sistemas, Electrónica e Industrial", EsActiva = true, FechaCreacion = seedDate },
                new Facultad { Id = 2, Codigo = "FCIAL", Nombre = "Facultad de Ciencias de la Alimentación", EsActiva = true, FechaCreacion = seedDate },
                new Facultad { Id = 3, Codigo = "FCHE", Nombre = "Facultad de Ciencias Humanas y de la Educación", EsActiva = true, FechaCreacion = seedDate },
                new Facultad { Id = 4, Codigo = "FCA", Nombre = "Facultad de Contabilidad y Auditoría", EsActiva = true, FechaCreacion = seedDate },
                new Facultad { Id = 5, Codigo = "FCJSE", Nombre = "Facultad de Ciencias Jurídicas y Sociales", EsActiva = true, FechaCreacion = seedDate }
            );            // Datos iniciales para TiposDocumento
            modelBuilder.Entity<TipoDocumento>().HasData(
                new TipoDocumento { Id = 1, Codigo = "OBRA", Nombre = "Obra Publicada", Descripcion = "Documentos que acreditan obras publicadas", FormatoEsperado = "PDF", TamanoMaximoMB = 10, EsActivo = true, FechaCreacion = seedDate },
                new TipoDocumento { Id = 2, Codigo = "CAPACITACION", Nombre = "Capacitación", Descripcion = "Certificados de capacitación y cursos", FormatoEsperado = "PDF", TamanoMaximoMB = 5, EsActivo = true, FechaCreacion = seedDate },
                new TipoDocumento { Id = 3, Codigo = "INVESTIGACION", Nombre = "Investigación", Descripcion = "Documentos relacionados a proyectos de investigación", FormatoEsperado = "PDF", TamanoMaximoMB = 15, EsActivo = true, FechaCreacion = seedDate },
                new TipoDocumento { Id = 4, Codigo = "EVALUACION", Nombre = "Evaluación Docente", Descripcion = "Resultados de evaluación docente", FormatoEsperado = "PDF", TamanoMaximoMB = 5, EsActivo = true, FechaCreacion = seedDate },
                new TipoDocumento { Id = 5, Codigo = "ACCION_PERSONAL", Nombre = "Acción de Personal", Descripcion = "Documentos de TTHH", FormatoEsperado = "PDF", TamanoMaximoMB = 5, EsActivo = true, FechaCreacion = seedDate }
            );            // Datos iniciales para EstadosSolicitud
            modelBuilder.Entity<EstadoSolicitud>().HasData(
                new EstadoSolicitud { Id = 1, Codigo = "ENVIADA", Nombre = "Enviada", Descripcion = "Solicitud enviada y pendiente de revisión", Color = "#FFA500", Orden = 1, EsActivo = true, FechaCreacion = seedDate },
                new EstadoSolicitud { Id = 2, Codigo = "EN_PROCESO", Nombre = "En Proceso", Descripcion = "Solicitud en proceso de revisión", Color = "#0066CC", Orden = 2, EsActivo = true, FechaCreacion = seedDate },
                new EstadoSolicitud { Id = 3, Codigo = "APROBADA", Nombre = "Aprobada", Descripcion = "Solicitud aprobada", Color = "#28A745", Orden = 3, EsEstadoFinal = true, EsActivo = true, FechaCreacion = seedDate },
                new EstadoSolicitud { Id = 4, Codigo = "RECHAZADA", Nombre = "Rechazada", Descripcion = "Solicitud rechazada", Color = "#DC3545", Orden = 4, EsEstadoFinal = true, EsActivo = true, FechaCreacion = seedDate },
                new EstadoSolicitud { Id = 5, Codigo = "ARCHIVADA", Nombre = "Archivada", Descripcion = "Solicitud archivada", Color = "#6C757D", Orden = 5, EsEstadoFinal = true, EsActivo = true, FechaCreacion = seedDate }
            );

            // Datos iniciales para ServiciosExternos
            modelBuilder.Entity<ServicioExterno>().HasData(
                new ServicioExterno { Id = 1, Codigo = "DITIC", Nombre = "DITIC - Cursos", Descripcion = "Servicio para obtener datos de capacitación", UrlBase = "https://api.ditic.uta.edu.ec", Activo = true, FechaCreacion = seedDate },
                new ServicioExterno { Id = 2, Codigo = "DAC", Nombre = "DAC - Evaluaciones", Descripcion = "Servicio para obtener evaluaciones docente", UrlBase = "https://api.dac.uta.edu.ec", Activo = true, FechaCreacion = seedDate },
                new ServicioExterno { Id = 3, Codigo = "TTHH", Nombre = "TTHH - Acción Personal", Descripcion = "Servicio de Talento Humano", UrlBase = "https://api.tthh.uta.edu.ec", Activo = true, FechaCreacion = seedDate },
                new ServicioExterno { Id = 4, Codigo = "INVESTIGACION", Nombre = "Dirección de Investigación", Descripcion = "Servicio para obtener datos de investigación", UrlBase = "https://api.investigacion.uta.edu.ec", Activo = true, FechaCreacion = seedDate }
            );

            // Configuraciones iniciales del sistema
            modelBuilder.Entity<ConfiguracionSistema>().HasData(
                new ConfiguracionSistema { Id = 1, Clave = "TIEMPO_BLOQUEO_MINUTOS", Valor = "15", Descripcion = "Tiempo de bloqueo en minutos después de 3 intentos fallidos", TipoDato = "int", GrupoConfiguracion = "Seguridad", FechaCreacion = seedDate, FechaActualizacion = seedDate },
                new ConfiguracionSistema { Id = 2, Clave = "INTENTOS_MAXIMOS_LOGIN", Valor = "3", Descripcion = "Número máximo de intentos de login antes del bloqueo", TipoDato = "int", GrupoConfiguracion = "Seguridad", FechaCreacion = seedDate, FechaActualizacion = seedDate },
                new ConfiguracionSistema { Id = 3, Clave = "TOKEN_EXPIRACION_HORAS", Valor = "8", Descripcion = "Tiempo de expiración del token JWT en horas", TipoDato = "int", GrupoConfiguracion = "Seguridad", FechaCreacion = seedDate, FechaActualizacion = seedDate },
                new ConfiguracionSistema { Id = 4, Clave = "TAMAÑO_MAXIMO_ARCHIVO_MB", Valor = "20", Descripcion = "Tamaño máximo permitido para archivos en MB", TipoDato = "int", GrupoConfiguracion = "Documentos", FechaCreacion = seedDate, FechaActualizacion = seedDate }
            );
        }
    }
}
