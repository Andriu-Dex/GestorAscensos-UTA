using System.ComponentModel.DataAnnotations;

namespace SGA.Application.DTOs
{
    /// <summary>
    /// DTO para actualizar información de un docente
    /// </summary>
    public class ActualizarDocenteDto
    {
        /// <summary>
        /// Email del docente (requerido)
        /// </summary>
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Debe proporcionar un email válido")]
        [StringLength(100, ErrorMessage = "El email no puede exceder 100 caracteres")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Teléfono de contacto del docente (requerido)
        /// </summary>
        [Required(ErrorMessage = "El teléfono de contacto es requerido")]
        [StringLength(20, MinimumLength = 10, ErrorMessage = "El teléfono debe tener entre 10 y 20 caracteres")]
        [RegularExpression(@"^[\d\-\+\(\)\s]+$", ErrorMessage = "El teléfono solo puede contener números, espacios, guiones, paréntesis y el signo +")]
        public string TelefonoContacto { get; set; } = string.Empty;

        /// <summary>
        /// Nueva contraseña (opcional, solo si desea cambiarla)
        /// </summary>
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string? NuevaPassword { get; set; }

        /// <summary>
        /// Confirmación de la nueva contraseña (requerida si se proporciona nueva contraseña)
        /// </summary>
        public string? ConfirmarPassword { get; set; }
    }
}
