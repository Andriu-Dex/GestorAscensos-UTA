using System.ComponentModel.DataAnnotations;

namespace SGA.Web.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        [RegularExpression(@"^[^@]+@uta\.edu\.ec$", ErrorMessage = "El correo debe ser un correo institucional (@uta.edu.ec)")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Password { get; set; } = string.Empty;
    }
}
