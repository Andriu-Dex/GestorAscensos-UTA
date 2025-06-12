namespace SGA.Domain.Entities
{
    public class ConfiguracionSistema
    {
        public int Id { get; set; }
        public required string Clave { get; set; }
        public required string Valor { get; set; }
        public string? Descripcion { get; set; }
        public required string TipoDato { get; set; } // string, int, bool, decimal
        public string? GrupoConfiguracion { get; set; }
        public bool EsEditable { get; set; } = true;
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}
