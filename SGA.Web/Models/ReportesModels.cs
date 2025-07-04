namespace SGA.Web.Models;

public class FiltroReporteAdminDTO
{
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public Guid? FacultadId { get; set; }
    public Guid? DepartamentoId { get; set; }
    public string? Estado { get; set; }
    public string? NivelAcademico { get; set; }
    public string? Periodo { get; set; } // "Mensual", "Trimestral", "Anual"
}

public class FacultadDTO
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public class ReporteProcesosPorEstadoDTO
{
    public DateTime FechaGeneracion { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public List<EstadoResumen> EstadosProcesos { get; set; } = new();
    public List<FacultadResumen> ProcesosPorFacultad { get; set; } = new();
    public int TotalProcesos { get; set; }
}

public class EstadoResumen
{
    public string Estado { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public double Porcentaje { get; set; }
}

public class FacultadResumen
{
    public string Nombre { get; set; } = string.Empty;
    public int TotalSolicitudes { get; set; }
    public int SolicitudesPendientes { get; set; }
    public int SolicitudesAprobadas { get; set; }
    public int SolicitudesRechazadas { get; set; }
    public List<DepartamentoResumen> Departamentos { get; set; } = new();
}

public class DepartamentoResumen
{
    public string Nombre { get; set; } = string.Empty;
    public int TotalSolicitudes { get; set; }
    public int SolicitudesPendientes { get; set; }
    public int SolicitudesAprobadas { get; set; }
    public int SolicitudesRechazadas { get; set; }
}

public class ReporteAscensosPorFacultadDTO
{
    public DateTime FechaGeneracion { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public List<FacultadResumen> Facultades { get; set; } = new();
    public int TotalAscensos { get; set; }
}

public class ReporteTiempoResolucionDTO
{
    public DateTime FechaGeneracion { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int TiempoPromedioDias { get; set; }
    public List<TiempoResolucionPorFacultad> TiemposPorFacultad { get; set; } = new();
    public int TotalProcesosAnalizados { get; set; }
}

public class TiempoResolucionPorFacultad
{
    public string Facultad { get; set; } = string.Empty;
    public int TiempoPromedioDias { get; set; }
    public int TotalProcesos { get; set; }
}

public class ReporteDistribucionDocentesDTO
{
    public DateTime FechaGeneracion { get; set; }
    public int TotalDocentes { get; set; }
    public Dictionary<string, int> DocentesPorNivel { get; set; } = new();
    public List<FacultadDistribucion> DistribucionPorFacultad { get; set; } = new();
}

public class FacultadDistribucion
{
    public string Nombre { get; set; } = string.Empty;
    public int TotalDocentes { get; set; }
    public Dictionary<string, int> DocentesPorNivel { get; set; } = new();
    public List<DepartamentoDistribucion> Departamentos { get; set; } = new();
}

public class DepartamentoDistribucion
{
    public string Nombre { get; set; } = string.Empty;
    public int TotalDocentes { get; set; }
    public Dictionary<string, int> DocentesPorNivel { get; set; } = new();
}

public class ReporteActividadPeriodoDTO
{
    public string Periodo { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int TotalSolicitudesNuevas { get; set; }
    public int TotalSolicitudesResueltas { get; set; }
    public int TotalAprobadas { get; set; }
    public int TotalRechazadas { get; set; }
    public List<ActividadMensual> ActividadPorMes { get; set; } = new();
    public List<FacultadActividad> ActividadPorFacultad { get; set; } = new();
}

public class ActividadMensual
{
    public string Mes { get; set; } = string.Empty;
    public int SolicitudesNuevas { get; set; }
    public int SolicitudesResueltas { get; set; }
    public int SolicitudesAprobadas { get; set; }
    public int SolicitudesRechazadas { get; set; }
}

public class FacultadActividad
{
    public string Facultad { get; set; } = string.Empty;
    public int SolicitudesNuevas { get; set; }
    public int SolicitudesResueltas { get; set; }
    public int SolicitudesAprobadas { get; set; }
    public int SolicitudesRechazadas { get; set; }
}

public class ReporteConsolidadoDTO
{
    public DateTime FechaGeneracion { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int TotalSolicitudesNuevas { get; set; }
    public int TotalSolicitudesResueltas { get; set; }
    public int TotalAprobadas { get; set; }
    public int TotalRechazadas { get; set; }
    public int TiempoPromedioResolucion { get; set; }
    public List<ActividadMensual> ActividadMensual { get; set; } = new();
    public List<FacultadActividad> ActividadPorFacultad { get; set; } = new();
}
