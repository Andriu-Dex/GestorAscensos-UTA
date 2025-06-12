using System.ComponentModel.DataAnnotations;

namespace SGA.Application.DTOs
{
    /// <summary>
    /// DTO para actualizar un tipo de documento existente
    /// </summary>
    public class UpdateTipoDocumentoDto
    {
        /// <summary>
        /// Código único del tipo de documento (requerido)
        /// </summary>
        [Required(ErrorMessage = "El código del tipo de documento es requerido")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "El código debe tener entre 2 y 20 caracteres")]
        [RegularExpression(@"^[A-Z0-9_]+$", ErrorMessage = "El código solo puede contener letras mayúsculas, números y guiones bajos")]
        public string Codigo { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del tipo de documento (requerido)
        /// </summary>
        [Required(ErrorMessage = "El nombre del tipo de documento es requerido")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Descripción del tipo de documento
        /// </summary>
        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string? Descripcion { get; set; }        /// <summary>
        /// Indica si este tipo de documento requiere validación
        /// </summary>
        public bool RequiereValidacion { get; set; }

        /// <summary>
        /// Indica si el documento es obligatorio para el proceso de ascenso
        /// </summary>
        public bool EsObligatorio { get; set; }

        /// <summary>
        /// Indica si el tipo de documento requiere validación (alias)
        /// </summary>
        public bool ValidacionRequerida { get; set; }

        /// <summary>
        /// Formato esperado del documento (ejemplo: "PDF", "DOC", "DOCX")
        /// </summary>
        [StringLength(50, ErrorMessage = "El formato esperado no puede exceder 50 caracteres")]
        public string? FormatoEsperado { get; set; }

        /// <summary>
        /// Formatos soportados del documento (alias para compatibilidad)
        /// </summary>
        [StringLength(100, ErrorMessage = "Los formatos soportados no pueden exceder 100 caracteres")]
        public string? FormatosSoportados { get; set; }

        /// <summary>
        /// Tamaño máximo del archivo en MB (debe ser entre 1 y 100 MB)
        /// </summary>
        [Range(1, 100, ErrorMessage = "El tamaño máximo debe estar entre 1 y 100 MB")]
        public int? TamanoMaximoMB { get; set; }

        /// <summary>
        /// Tamaño máximo del archivo en MB (alias para compatibilidad)
        /// </summary>
        [Range(1, 100, ErrorMessage = "El tamaño máximo debe estar entre 1 y 100 MB")]
        public int? TamanioMaximoMB { get; set; }        /// <summary>
        /// Indica si el tipo de documento está activo
        /// </summary>
        public bool Activo { get; set; }

        /// <summary>
        /// Indica si el tipo de documento está activo (nueva propiedad)
        /// </summary>
        public bool EsActivo { get; set; }
    }
}
