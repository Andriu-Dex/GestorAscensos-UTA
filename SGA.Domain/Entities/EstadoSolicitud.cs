namespace SGA.Domain.Entities
{
    public class EstadoSolicitud
    {
        public int Id { get; set; }
        public required string Codigo { get; set; }
        public required string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public bool EsEstadoFinal { get; set; } = false;
        public bool RequiereRevision { get; set; } = false;
        public string? Color { get; set; } // Para mostrar en la UI (renamed from ColorHex for consistency)
        public int Orden { get; set; } // Para ordenar los estados
        public bool EsActivo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime? FechaActualizacion { get; set; }
        
        // Navegación
        public ICollection<SolicitudAscenso> SolicitudesAscenso { get; set; } = new List<SolicitudAscenso>();
    }
}
