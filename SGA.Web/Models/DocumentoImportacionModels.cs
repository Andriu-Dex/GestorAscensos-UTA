using System.ComponentModel.DataAnnotations;

namespace SGA.Web.Models
{
    public class DocumentoImportacionDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string TipoDocumento { get; set; } = string.Empty;
        public string TipoDocumentoTexto { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public long TamanoArchivo { get; set; }
        public string TamanoFormateado { get; set; } = string.Empty;
        public string? SolicitudOrigen { get; set; }
        public bool EsImportable { get; set; }
        public string? MotivoNoImportable { get; set; }
    }

    public class FiltrosImportacionDto
    {
        public string? TipoDocumento { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string? TextoBusqueda { get; set; }
        public bool SoloImportables { get; set; } = true;
    }

    public class ImportarDocumentosRequestDto
    {
        [Required]
        public List<Guid> DocumentosIds { get; set; } = new();
    }

    public class ImportarDocumentosResponseDto
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public List<Guid> DocumentosImportados { get; set; } = new();
        public List<string> Errores { get; set; } = new();
    }
}
