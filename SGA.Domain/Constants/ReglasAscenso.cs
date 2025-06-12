namespace SGA.Domain.Constants
{    public static class ReglasAscenso
    {
        public const int NivelMaximo = 5; // Titular 5 es el nivel máximo
        
        public static readonly RequisitoAscenso[] Requisitos = new RequisitoAscenso[]
        {
            // Titular 1 a Titular 2
            new RequisitoAscenso
            {
                NivelActual = 1,
                NivelSolicitado = 2,
                TiempoMinimo = 48, // 4 años en meses
                ObrasMinimas = 1,
                PuntajeEvaluacionMinimo = 75,
                HorasCapacitacionMinimas = 96,
                TiempoInvestigacionMinimo = 0 // No requerido
            },
              // Titular 2 a Titular 3
            new RequisitoAscenso
            {
                NivelActual = 2,
                NivelSolicitado = 3,
                TiempoMinimo = 48, // 4 años en meses
                ObrasMinimas = 2,
                PuntajeEvaluacionMinimo = 75,
                HorasCapacitacionMinimas = 96,
                TiempoInvestigacionMinimo = 12 // 12 meses
            },
              // Titular 3 a Titular 4
            new RequisitoAscenso
            {
                NivelActual = 3,
                NivelSolicitado = 4,
                TiempoMinimo = 48, // 4 años en meses
                ObrasMinimas = 3,
                PuntajeEvaluacionMinimo = 75,
                HorasCapacitacionMinimas = 128,
                TiempoInvestigacionMinimo = 24 // 24 meses
            },
              // Titular 4 a Titular 5
            new RequisitoAscenso
            {
                NivelActual = 4,
                NivelSolicitado = 5,
                TiempoMinimo = 48, // 4 años en meses
                ObrasMinimas = 5,
                PuntajeEvaluacionMinimo = 75,
                HorasCapacitacionMinimas = 160,
                TiempoInvestigacionMinimo = 24 // 24 meses
            }
        };
        
        /// <summary>
        /// Obtener requisito para un nivel específico
        /// </summary>
        public static RequisitoAscenso? ObtenerRequisitoParaNivel(int nivelActual)
        {
            return Array.Find(Requisitos, r => r.NivelActual == nivelActual);
        }
    }

    public class RequisitoAscenso
    {
        public int NivelActual { get; set; }
        public int NivelSolicitado { get; set; }
        public int TiempoMinimo { get; set; } // En años
        public int ObrasMinimas { get; set; }
        public decimal PuntajeEvaluacionMinimo { get; set; } // En porcentaje
        public int HorasCapacitacionMinimas { get; set; }
        public int TiempoInvestigacionMinimo { get; set; } // En meses
    }
}
