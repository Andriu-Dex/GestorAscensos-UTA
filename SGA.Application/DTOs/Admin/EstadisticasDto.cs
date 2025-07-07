namespace SGA.Application.DTOs.Admin
{
    public class EstadisticasCompletasDto
    {
        public int TotalDocentes { get; set; }
        public int TotalSolicitudes { get; set; }
        public int SolicitudesPendientes { get; set; }
        public int SolicitudesEnProceso { get; set; }
        public int SolicitudesAprobadas { get; set; }
        public int SolicitudesRechazadas { get; set; }
        public int SolicitudesEsteMes { get; set; }
        public int AscensosEsteAnio { get; set; }
        public Dictionary<int, int> DocentesPorNivel { get; set; } = new();
        public DateTime FechaActualizacion { get; set; }
    }

    public class EstadisticasGeneralesDto
    {
        public int TotalDocentes { get; set; }
        public int SolicitudesPendientes { get; set; }
        public int SolicitudesAprobadas { get; set; }
        public int SolicitudesRechazadas { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }

    public class EstadisticasFacultadDto
    {
        public string Facultad { get; set; } = string.Empty;
        public int TotalDocentes { get; set; }
        public int SolicitudesPendientes { get; set; }
        public int SolicitudesAprobadas { get; set; }
        public int SolicitudesRechazadas { get; set; }
        public int AscensosEsteAnio { get; set; }
        public Dictionary<int, int> DocentesPorNivel { get; set; } = new();
    }

    public class EstadisticasNivelDto
    {
        public int Nivel { get; set; }
        public string NombreNivel { get; set; } = string.Empty;
        public int TotalDocentes { get; set; }
        public int SolicitudesPendientes { get; set; }
        public int SolicitudesAprobadas { get; set; }
        public int SolicitudesRechazadas { get; set; }
        public double PorcentajeDistribucion { get; set; }
    }

    public class EstadisticasActividadMensualDto
    {
        public int Mes { get; set; }
        public int Anio { get; set; }
        public string NombreMes { get; set; } = string.Empty;
        public int SolicitudesCreadas { get; set; }
        public int SolicitudesAprobadas { get; set; }
        public int SolicitudesRechazadas { get; set; }
        public int AscensosRealizados { get; set; }
    }
}
