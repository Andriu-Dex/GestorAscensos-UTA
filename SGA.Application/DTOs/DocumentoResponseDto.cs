using System.ComponentModel.DataAnnotations;

namespace SGA.Application.DTOs
{
    /// <summary>
    /// DTO para la respuesta de información de un documento
    /// </summary>
    public class DocumentoResponseDto
    {
        /// <summary>
        /// Identificador único del documento
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID del docente propietario del documento
        /// </summary>
        public int DocenteId { get; set; }

        /// <summary>
        /// Nombre del documento
        /// </summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Descripción del documento
        /// </summary>
        public string? Descripcion { get; set; }

        /// <summary>
        /// ID del tipo de documento
        /// </summary>
        public int TipoDocumentoId { get; set; }

        /// <summary>
        /// Tipo de contenido del archivo (MIME type)
        /// </summary>
        public string ContentType { get; set; } = string.Empty;

        /// <summary>
        /// Tamaño del archivo en bytes
        /// </summary>
        public long TamanioBytes { get; set; }

        /// <summary>
        /// Fecha de subida del documento
        /// </summary>
        public DateTime FechaSubida { get; set; }

        /// <summary>
        /// Fecha de última modificación
        /// </summary>
        public DateTime? FechaModificacion { get; set; }

        /// <summary>
        /// Indica si el documento ha sido validado
        /// </summary>
        public bool Validado { get; set; }

        /// <summary>
        /// Fecha de validación del documento
        /// </summary>
        public DateTime? FechaValidacion { get; set; }

        /// <summary>
        /// ID del usuario que validó el documento
        /// </summary>
        public int? ValidadoPorId { get; set; }

        /// <summary>
        /// Observaciones del proceso de validación
        /// </summary>
        public string? ObservacionesValidacion { get; set; }

        /// <summary>
        /// Hash SHA256 del documento para verificar integridad
        /// </summary>
        public string? HashSHA256 { get; set; }

        /// <summary>
        /// Indica si el documento está activo
        /// </summary>
        public bool Activo { get; set; }
    }
}
