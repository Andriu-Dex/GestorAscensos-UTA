using System.ComponentModel.DataAnnotations;

namespace SGA.Application.DTOs
{
    /// <summary>
    /// DTO para actualizar un documento existente
    /// </summary>
    public class ActualizarDocumentoDto
    {
        /// <summary>
        /// Nombre del documento (requerido)
        /// </summary>
        [Required(ErrorMessage = "El nombre del documento es requerido")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 200 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Descripción del documento
        /// </summary>
        [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        public string? Descripcion { get; set; }
    }
}
