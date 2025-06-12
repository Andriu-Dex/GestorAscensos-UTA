using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SGA.Application.DTOs
{
    /// <summary>
    /// DTO para subir un nuevo documento
    /// </summary>
    public class SubirDocumentoDto
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

        /// <summary>
        /// ID del tipo de documento (requerido)
        /// </summary>
        [Required(ErrorMessage = "El tipo de documento es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un tipo de documento válido")]
        public int TipoDocumentoId { get; set; }

        /// <summary>
        /// Archivo a subir (requerido)
        /// </summary>
        [Required(ErrorMessage = "Debe seleccionar un archivo")]
        public IFormFile Archivo { get; set; } = null!;
    }
}
