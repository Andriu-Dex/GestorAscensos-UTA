using System.Collections.Generic;

namespace SGA.Domain.Entities
{
    public class Facultad
    {
        public Facultad()
        {
            Docentes = new List<Docente>();
            DatosTTHH = new List<DatosTTHH>();
        }

        public int Id { get; set; }        public required string Codigo { get; set; }
        public required string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public string? Color { get; set; } // Para representación visual en UI
        public bool EsActiva { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime? FechaActualizacion { get; set; }

        // Navegación
        public ICollection<Docente> Docentes { get; set; }
        public ICollection<DatosTTHH> DatosTTHH { get; set; }
    }
}
