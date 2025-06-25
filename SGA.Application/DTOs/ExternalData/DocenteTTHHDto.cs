namespace SGA.Application.DTOs.ExternalData;

public class DocenteTTHHDto
{
    public string Identificacion { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public DateTime FechaIngreso { get; set; }
    public bool EstaActivo { get; set; }
    public string? TituloAcademico { get; set; }
    public decimal? AniosExperiencia { get; set; }
}
