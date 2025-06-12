namespace SGA.Domain.Entities
{
    public class IndicadorDocente
    {
        public int Id { get; set; }
        public int DocenteId { get; set; }
        
        // Tiempo en rol actual (en años)
        public int TiempoEnRolActual { get; set; }
        
        // Número de obras publicadas
        public int NumeroObras { get; set; }
        
        // Puntaje de evaluación docente (porcentaje)
        public decimal PuntajeEvaluacion { get; set; }
        
        // Horas de capacitación acumuladas
        public int HorasCapacitacion { get; set; }
        
        // Tiempo en investigación (en meses)
        public int TiempoInvestigacion { get; set; }
        
        // Fechas de última actualización
        public DateTime? FechaActualizacionObras { get; set; }
        public DateTime? FechaActualizacionEvaluacion { get; set; }
        public DateTime? FechaActualizacionCapacitacion { get; set; }
        public DateTime? FechaActualizacionInvestigacion { get; set; }
        
        // Servicios externos de donde provienen los datos
        public string? FuenteObras { get; set; } // Dirección de Investigación
        public string? FuenteEvaluacion { get; set; } // DAC
        public string? FuenteCapacitacion { get; set; } // DITIC
        public string? FuenteInvestigacion { get; set; } // Dirección de Investigación
        
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaActualizacion { get; set; } = DateTime.Now;

        // Navegación
        public required Docente Docente { get; set; }
    }
}
