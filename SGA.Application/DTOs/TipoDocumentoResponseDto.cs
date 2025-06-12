using System.ComponentModel.DataAnnotations;

namespace SGA.Application.DTOs
{
    /// <summary>
    /// DTO para la respuesta de información de un tipo de documento
    /// </summary>
    public class TipoDocumentoResponseDto
    {
        /// <summary>
        /// Identificador único del tipo de documento
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Código único del tipo de documento
        /// </summary>
        public string Codigo { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del tipo de documento
        /// </summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Descripción del tipo de documento
        /// </summary>
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
        /// Formato esperado del documento (PDF, DOC, etc.)
        /// </summary>
        public string? FormatoEsperado { get; set; }

        /// <summary>
        /// Formatos soportados del documento (alias para compatibilidad)
        /// </summary>
        public string? FormatosSoportados { get; set; }

        /// <summary>
        /// Tamaño máximo del archivo en MB
        /// </summary>
        public int? TamanoMaximoMB { get; set; }

        /// <summary>
        /// Tamaño máximo del archivo en MB (alias para compatibilidad)
        /// </summary>
        public int? TamanioMaximoMB { get; set; }        /// <summary>
        /// Indica si el tipo de documento está activo
        /// </summary>
        public bool Activo { get; set; }

        /// <summary>
        /// Indica si el tipo de documento está activo (nueva propiedad)
        /// </summary>
        public bool EsActivo { get; set; }

        /// <summary>
        /// Fecha de creación del tipo de documento
        /// </summary>
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Fecha de última actualización
        /// </summary>
        public DateTime? FechaActualizacion { get; set; }

        /// <summary>
        /// Número de documentos de este tipo en el sistema
        /// </summary>
        public int TotalDocumentos { get; set; }

        /// <summary>
        /// Número de solicitudes que incluyen este tipo de documento
        /// </summary>
        public int SolicitudesConEsteDocumento { get; set; }
    }
}
