using System.ComponentModel.DataAnnotations;

namespace SGA.Application.DTOs.Docentes;

public class ActualizarPerfilDto
{
    [Required(ErrorMessage = "Los nombres son obligatorios")]
    [StringLength(100, ErrorMessage = "Los nombres no pueden exceder 100 caracteres")]
    public string Nombres { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los apellidos son obligatorios")]
    [StringLength(100, ErrorMessage = "Los apellidos no pueden exceder 100 caracteres")]
    public string Apellidos { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo electrónico es obligatorio")]
    [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@uta\.edu\.ec$", 
        ErrorMessage = "El correo debe pertenecer al dominio @uta.edu.ec")]
    public string Email { get; set; } = string.Empty;

    [RegularExpression(@"^09\d{8}$", 
        ErrorMessage = "El número de celular debe tener 10 dígitos y empezar con 09")]
    public string? Celular { get; set; }
}
