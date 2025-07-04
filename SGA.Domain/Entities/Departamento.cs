using System.ComponentModel.DataAnnotations;

namespace SGA.Domain.Entities
{
    public class Departamento
    {
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Nombre { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Descripcion { get; set; }
        
        [StringLength(50)]
        public string? Codigo { get; set; }
        
        public bool EsActivo { get; set; } = true;
        
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        
        public DateTime? FechaActualizacion { get; set; }
        
        // Relación con Facultad
        public Guid FacultadId { get; set; }
        public virtual Facultad Facultad { get; set; } = null!;
        
        // Propiedades de navegación
        public virtual ICollection<Docente> Docentes { get; set; } = new List<Docente>();
    }
}
