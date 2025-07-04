using System;
using System.Collections.Generic;

namespace SGA.Application.DTOs
{
    public class ReporteProcesosPorEstadoDTO
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int TotalPendientes { get; set; }
        public int TotalAprobados { get; set; }
        public int TotalRechazados { get; set; }
        public int TotalEnRevision { get; set; }
        public List<ProcesoEstadoDetalle> DetallesProcesos { get; set; } = new List<ProcesoEstadoDetalle>();
    }

    public class ProcesoEstadoDetalle
    {
        public Guid Id { get; set; }
        public string DocenteNombre { get; set; } = string.Empty;
        public string DocenteCedula { get; set; } = string.Empty;
        public string NivelActual { get; set; } = string.Empty;
        public string NivelSolicitado { get; set; } = string.Empty;
        public DateTime FechaSolicitud { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string Facultad { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
        public int DiasProceso { get; set; }
    }

    public class ReporteAscensosPorFacultadDTO
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public List<FacultadResumen> Facultades { get; set; } = new List<FacultadResumen>();
        public int TotalSolicitudes { get; set; }
        public int TotalAprobados { get; set; }
        public int TotalRechazados { get; set; }
    }

    public class FacultadResumen
    {
        public string Nombre { get; set; } = string.Empty;
        public int TotalSolicitudes { get; set; }
        public int SolicitudesPendientes { get; set; }
        public int SolicitudesAprobadas { get; set; }
        public int SolicitudesRechazadas { get; set; }
        public List<DepartamentoResumen> Departamentos { get; set; } = new List<DepartamentoResumen>();
    }

    public class DepartamentoResumen
    {
        public string Nombre { get; set; } = string.Empty;
        public int TotalSolicitudes { get; set; }
        public int SolicitudesPendientes { get; set; }
        public int SolicitudesAprobadas { get; set; }
        public int SolicitudesRechazadas { get; set; }
    }

    public class ReporteTiempoResolucionDTO
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int TiempoPromedioTotalDias { get; set; }
        public List<TiempoResolucionPorFacultad> TiemposPorFacultad { get; set; } = new List<TiempoResolucionPorFacultad>();
        public List<TiempoResolucionPorNivel> TiemposPorNivel { get; set; } = new List<TiempoResolucionPorNivel>();
        public List<ProcesoTiempoDetalle> ProcesosMasLargos { get; set; } = new List<ProcesoTiempoDetalle>();
    }

    public class TiempoResolucionPorFacultad
    {
        public string Facultad { get; set; } = string.Empty;
        public int TiempoPromedioDias { get; set; }
        public int TotalProcesos { get; set; }
    }

    public class TiempoResolucionPorNivel
    {
        public string NivelDestino { get; set; } = string.Empty;
        public int TiempoPromedioDias { get; set; }
        public int TotalProcesos { get; set; }
    }

    public class ProcesoTiempoDetalle
    {
        public Guid Id { get; set; }
        public string DocenteNombre { get; set; } = string.Empty;
        public string Facultad { get; set; } = string.Empty;
        public string NivelSolicitado { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaResolucion { get; set; }
        public int DiasProceso { get; set; }
        public string Estado { get; set; } = string.Empty;
    }

    public class ReporteDistribucionDocentesDTO
    {
        public DateTime FechaGeneracion { get; set; }
        public int TotalDocentes { get; set; }
        public Dictionary<string, int> DocentesPorNivel { get; set; } = new Dictionary<string, int>();
        public List<FacultadDistribucion> DistribucionPorFacultad { get; set; } = new List<FacultadDistribucion>();
    }

    public class FacultadDistribucion
    {
        public string Nombre { get; set; } = string.Empty;
        public int TotalDocentes { get; set; }
        public Dictionary<string, int> DocentesPorNivel { get; set; } = new Dictionary<string, int>();
        public List<DepartamentoDistribucion> Departamentos { get; set; } = new List<DepartamentoDistribucion>();
    }

    public class DepartamentoDistribucion
    {
        public string Nombre { get; set; } = string.Empty;
        public int TotalDocentes { get; set; }
        public Dictionary<string, int> DocentesPorNivel { get; set; } = new Dictionary<string, int>();
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
        public List<ActividadMensual> ActividadPorMes { get; set; } = new List<ActividadMensual>();
        public List<FacultadActividad> ActividadPorFacultad { get; set; } = new List<FacultadActividad>();
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
}
