using System.Collections.Generic;

namespace SGA.Domain.Entities
{
    public class TipoDocumento
    {
        public TipoDocumento()
        {
            Documentos = new List<Documento>();
        }

        public int Id { get; set; }
        public required string Codigo { get; set; }
        public required string Nombre { get; set; }
        public string? Descripcion { get; set; }        public bool RequiereValidacion { get; set; } = false;
        public string? FormatoEsperado { get; set; } // PDF, DOC, etc.
        public int? TamanoMaximoMB { get; set; }
        public bool EsActivo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime? FechaActualizacion { get; set; }

        // Navegación
        public ICollection<Documento> Documentos { get; set; }
    }
}
