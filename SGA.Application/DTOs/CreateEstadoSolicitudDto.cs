using System.ComponentModel.DataAnnotations;

namespace SGA.Application.DTOs
{    /// <summary>
    /// DTO para crear un nuevo estado de solicitud
    /// </summary>
    public class CreateEstadoSolicitudDto
    {
        /// <summary>
        /// Código único del estado (requerido)
        /// </summary>
        [Required(ErrorMessage = "El código del estado es requerido")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "El código debe tener entre 2 y 20 caracteres")]
        [RegularExpression(@"^[A-Z0-9_]+$", ErrorMessage = "El código solo puede contener letras mayúsculas, números y guiones bajos")]
        public string Codigo { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del estado de solicitud (requerido)
        /// </summary>
        [Required(ErrorMessage = "El nombre del estado es requerido")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Descripción del estado de solicitud
        /// </summary>
        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string? Descripcion { get; set; }/// <summary>
        /// Color hexadecimal para mostrar este estado en la interfaz de usuario (ejemplo: #FF5733)
        /// </summary>
        [StringLength(7, MinimumLength = 7, ErrorMessage = "El color debe ser un código hexadecimal válido (#RRGGBB)")]
        [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "El color debe ser un código hexadecimal válido (ejemplo: #FF5733)")]
        public string? Color { get; set; }

        /// <summary>
        /// Color hexadecimal para mostrar este estado (alias para compatibilidad)
        /// </summary>
        [StringLength(7, MinimumLength = 7, ErrorMessage = "El color debe ser un código hexadecimal válido (#RRGGBB)")]
        [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "El color debe ser un código hexadecimal válido (ejemplo: #FF5733)")]
        public string? ColorHex { get; set; }

        /// <summary>
        /// Indica si este es un estado final (no permite transiciones a otros estados)
        /// </summary>
        public bool EsEstadoFinal { get; set; } = false;

        /// <summary>
        /// Indica si este estado requiere revisión manual
        /// </summary>
        public bool RequiereRevision { get; set; } = false;

        /// <summary>
        /// Indica si el estado debe estar activo al momento de la creación (por defecto true)
        /// </summary>
        public bool EsActivo { get; set; } = true;

        /// <summary>
        /// Orden de visualización del estado (debe ser único)
        /// </summary>
        [Range(1, 999, ErrorMessage = "El orden debe estar entre 1 y 999")]
        public int Orden { get; set; }

        /// <summary>
        /// IDs de los estados a los que se puede transicionar desde este estado
        /// </summary>
        public List<int> EstadosPermitidos { get; set; } = new List<int>();
    }
}
