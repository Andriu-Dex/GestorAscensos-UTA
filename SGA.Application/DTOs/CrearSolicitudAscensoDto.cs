using System.ComponentModel.DataAnnotations;

namespace SGA.Application.DTOs
{
    /// <summary>
    /// DTO para crear una nueva solicitud de ascenso
    /// </summary>
    public class CrearSolicitudAscensoDto
    {
        /// <summary>
        /// Lista de IDs de documentos a adjuntar a la solicitud (requerido)
        /// </summary>
        [Required(ErrorMessage = "Debe adjuntar al menos un documento")]
        [MinLength(1, ErrorMessage = "Debe adjuntar al menos un documento")]
        public List<int> DocumentosIds { get; set; } = new List<int>();

        /// <summary>
        /// Observaciones adicionales del docente (opcional)
        /// </summary>
        [StringLength(1000, ErrorMessage = "Las observaciones no pueden exceder 1000 caracteres")]
        public string? Observaciones { get; set; }
    }
}
