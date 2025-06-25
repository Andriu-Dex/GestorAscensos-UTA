namespace SGA.Application.DTOs.Auditorias;

public class AuditoriaResponseDto
{
    public int Id { get; set; }
    public string Accion { get; set; } = string.Empty;
    public string Usuario { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string? Detalles { get; set; }
    public string? EntidadId { get; set; }
    public string DireccionIP { get; set; } = string.Empty;
}
