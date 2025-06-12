namespace SGA.Domain.Entities
{
    public class ServicioExterno
    {
        public int Id { get; set; }
        public required string Codigo { get; set; }
        public required string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public required string UrlBase { get; set; }
        public string? ApiKey { get; set; }
        public int TimeoutSegundos { get; set; } = 30;
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime? FechaUltimaConexion { get; set; }
        public string? UltimoError { get; set; }
    }
}
