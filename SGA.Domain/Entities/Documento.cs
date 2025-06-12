using System;

namespace SGA.Domain.Entities
{
    public class Documento
    {
        public int Id { get; set; }
        public int DocenteId { get; set; }
        public int TipoDocumentoId { get; set; }
        public required string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public required byte[] Contenido { get; set; }
        public required string ContentType { get; set; }
        public long TamanioBytes { get; set; }
        public DateTime FechaSubida { get; set; } = DateTime.Now;
        public DateTime? FechaModificacion { get; set; }
        public bool Activo { get; set; } = true;
        
        // Validación y verificación
        public bool Validado { get; set; } = false;
        public DateTime? FechaValidacion { get; set; }
        public int? ValidadoPorId { get; set; }
        public string? ObservacionesValidacion { get; set; }
        
        // Hash para verificar integridad
        public string? HashSHA256 { get; set; }
        
        // Navegación
        public required Docente Docente { get; set; }
        public required TipoDocumento TipoDocumento { get; set; }
        public Docente? ValidadoPor { get; set; }
        public ICollection<DocumentoSolicitud> DocumentosSolicitud { get; set; } = new List<DocumentoSolicitud>();
    }
}
