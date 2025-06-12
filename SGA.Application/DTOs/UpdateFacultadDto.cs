using System.ComponentModel.DataAnnotations;

namespace SGA.Application.DTOs
{
    /// <summary>
    /// DTO para actualizar una facultad existente
    /// </summary>
    public class UpdateFacultadDto
    {
        /// <summary>
        /// Nombre de la facultad (requerido)
        /// </summary>
        [Required(ErrorMessage = "El nombre de la facultad es requerido")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Código único de la facultad (requerido)
        /// </summary>
        [Required(ErrorMessage = "El código de la facultad es requerido")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "El código debe tener entre 2 y 20 caracteres")]
        [RegularExpression(@"^[A-Z0-9]+$", ErrorMessage = "El código solo puede contener letras mayúsculas y números")]
        public string Codigo { get; set; } = string.Empty;

        /// <summary>
        /// Descripción opcional de la facultad
        /// </summary>
        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string? Descripcion { get; set; }        /// <summary>
        /// Indica si la facultad está activa
        /// </summary>
        public bool EsActiva { get; set; }

        /// <summary>
        /// Color representativo de la facultad (para UI)
        /// </summary>
        [StringLength(7, ErrorMessage = "El color debe ser un código hexadecimal válido (#RRGGBB)")]
        public string? Color { get; set; }

        /// <summary>
        /// Indica si la facultad está activa (alias para compatibilidad)
        /// </summary>
        public bool Activa { get; set; }
    }
}
