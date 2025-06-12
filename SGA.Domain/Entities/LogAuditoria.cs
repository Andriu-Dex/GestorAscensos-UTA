namespace SGA.Domain.Entities
{
    using SGA.Domain.Enums;
    
    public class LogAuditoria
    {
        public int Id { get; set; }
        public int? DocenteId { get; set; }
        public required string Accion { get; set; }
        public string? Entidad { get; set; }
        public int? EntidadId { get; set; }
        public string? ValoresAnteriores { get; set; }
        public string? ValoresNuevos { get; set; }
        public required string DireccionIP { get; set; }
        public string? UserAgent { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public string? Observaciones { get; set; }
        public TipoLog TipoLog { get; set; } = TipoLog.Informacion;

        // Navegación
        public Docente? Docente { get; set; }
    }
}
