using System.ComponentModel.DataAnnotations;

namespace SGA.Domain.Entities
{
    public class Facultad
    {
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Nombre { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Descripcion { get; set; }
        
        [StringLength(50)]
        public string? Codigo { get; set; }
        
        public bool EsActiva { get; set; } = true;
        
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        
        public DateTime? FechaActualizacion { get; set; }
        
        // Propiedades de navegación
        public virtual ICollection<Departamento> Departamentos { get; set; } = new List<Departamento>();
    }
}
