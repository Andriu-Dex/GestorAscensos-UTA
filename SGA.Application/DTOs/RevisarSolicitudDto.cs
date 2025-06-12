using System.ComponentModel.DataAnnotations;

namespace SGA.Application.DTOs
{
    /// <summary>
    /// DTO para revisar una solicitud de ascenso
    /// </summary>
    public class RevisarSolicitudDto
    {
        /// <summary>
        /// ID del nuevo estado de la solicitud (requerido)
        /// </summary>
        [Required(ErrorMessage = "Debe especificar el estado de la solicitud")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un estado válido")]
        public int EstadoSolicitudId { get; set; }

        /// <summary>
        /// Motivo de rechazo (requerido si el estado es "Rechazada")
        /// </summary>
        [StringLength(1000, ErrorMessage = "El motivo de rechazo no puede exceder 1000 caracteres")]
        public string? MotivoRechazo { get; set; }        /// <summary>
        /// Observaciones adicionales del revisor (opcional)
        /// </summary>
        [StringLength(1000, ErrorMessage = "Las observaciones no pueden exceder 1000 caracteres")]
        public string? ObservacionesRevisor { get; set; }

        /// <summary>
        /// Observaciones adicionales del revisor (alias para compatibilidad)
        /// </summary>
        [StringLength(1000, ErrorMessage = "Las observaciones no pueden exceder 1000 caracteres")]
        public string? Observaciones { get; set; }
    }
}
