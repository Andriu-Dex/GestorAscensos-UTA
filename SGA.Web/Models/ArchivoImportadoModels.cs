using System;

namespace SGA.Web.Models
{
    public class ArchivoImportado
    {
        public Guid Id { get; set; }
        public string NombreArchivo { get; set; } = string.Empty;
        public string TipoDocumento { get; set; } = string.Empty;
        public DateTime FechaImportacion { get; set; }
        public string Estado { get; set; } = "Importado";
        public string? DocenteNombre { get; set; }
        public long TamanoArchivo { get; set; }
        public string? RutaArchivo { get; set; }
        public byte[]? ContenidoArchivo { get; set; }
        public string? MimeType { get; set; }
        public DateTime? FechaEnvioValidacion { get; set; }
        public string? ComentariosValidacion { get; set; }
        public Guid? SolicitudId { get; set; }
    }
    
    public class FormularioValidacion
    {
        // Campos comunes
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string ComentariosAdicionales { get; set; } = string.Empty;
        
        // Campos para Obras Académicas
        public string TipoObra { get; set; } = string.Empty;
        public DateTime? FechaPublicacion { get; set; }
        public string Editorial { get; set; } = string.Empty;
        public string Revista { get; set; } = string.Empty;
        public string ISBN_ISSN { get; set; } = string.Empty;
        public string DOI { get; set; } = string.Empty;
        public bool EsIndexada { get; set; } = false;
        public string IndiceIndexacion { get; set; } = string.Empty;
        public string Autores { get; set; } = string.Empty;
        
        // Campos para Certificados de Capacitación
        public string NombreCurso { get; set; } = string.Empty;
        public string InstitucionOfertante { get; set; } = string.Empty;
        public string TipoCapacitacion { get; set; } = string.Empty;
        public string Modalidad { get; set; } = string.Empty;
        public int HorasDuracion { get; set; } = 0;
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string NumeroRegistro { get; set; } = string.Empty;
        public string AreaTematica { get; set; } = string.Empty;
        
        // Campos para Evidencias de Investigación
        public string TipoEvidencia { get; set; } = string.Empty;
        public string TituloProyecto { get; set; } = string.Empty;
        public string InstitucionFinanciadora { get; set; } = string.Empty;
        public string RolInvestigador { get; set; } = string.Empty;
        public int MesesDuracion { get; set; } = 0;
        public string CodigoProyecto { get; set; } = string.Empty;
    }
}
