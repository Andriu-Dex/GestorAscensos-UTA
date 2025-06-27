namespace SGA.Application.DTOs.ExternalData;

public class PromocionDto
{
    public DateTime Fecha { get; set; }
    public string NivelAnterior { get; set; } = string.Empty;
    public string NivelNuevo { get; set; } = string.Empty;
    public string Detalle { get; set; } = string.Empty;
}
