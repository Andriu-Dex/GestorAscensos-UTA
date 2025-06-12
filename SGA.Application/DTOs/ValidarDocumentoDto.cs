using System.ComponentModel.DataAnnotations;

namespace SGA.Application.DTOs
{
    /// <summary>
    /// DTO para validar documentos
    /// </summary>
    public class ValidarDocumentoDto
    {
        /// <summary>
        /// Indica si el documento es válido
        /// </summary>
        [Required(ErrorMessage = "El campo Validado es requerido")]
        public bool Validado { get; set; }

        /// <summary>
        /// Observaciones sobre la validación del documento
        /// </summary>
        [MaxLength(1000, ErrorMessage = "Las observaciones no pueden exceder 1000 caracteres")]
        public string? Observaciones { get; set; }
    }
}
