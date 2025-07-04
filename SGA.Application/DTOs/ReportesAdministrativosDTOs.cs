using SGA.Domain.Enums;

namespace SGA.Application.DTOs;

/// <summary>
/// DTOs para reportes administrativos del sistema
/// </summary>

public class ReporteGeneralDocentesDto
{
    public List<DocenteResumenDto> Docentes { get; set; } = new();
    public EstadisticasGeneralesDto Estadisticas { get; set; } = new();
    public DateTime FechaGeneracion { get; set; } = DateTime.Now;
}

public class DocenteResumenDto
{
    public Guid Id { get; set; }
    public string Cedula { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Facultad { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
    public NivelTitular NivelActual { get; set; }
    public DateTime FechaInicioNivelActual { get; set; }
    public double TiempoEnNivelAnios { get; set; }
    public bool EstaActivo { get; set; }
    public bool PuedeAscender { get; set; }
    public string SiguienteNivel { get; set; } = string.Empty;
    public int SolicitudesPendientes { get; set; }
    public int SolicitudesAprobadas { get; set; }
    public int SolicitudesRechazadas { get; set; }
    public DateTime? FechaUltimaSolicitud { get; set; }
    
    // Datos académicos
    public decimal? PromedioEvaluaciones { get; set; }
    public int? HorasCapacitacion { get; set; }
    public int? NumeroObrasAcademicas { get; set; }
    public int? MesesInvestigacion { get; set; }
}

public class EstadisticasGeneralesDto
{
    public int TotalDocentes { get; set; }
    public int DocentesActivos { get; set; }
    public int DocentesInactivos { get; set; }
    public int DocentesPuedenAscender { get; set; }
    public Dictionary<string, int> DocentesPorNivel { get; set; } = new();
    public Dictionary<string, int> DocentesPorFacultad { get; set; } = new();
    public Dictionary<string, int> SolicitudesPorEstado { get; set; } = new();
}

public class ReporteSolicitudesDto
{
    public List<SolicitudResumenDto> Solicitudes { get; set; } = new();
    public EstadisticasSolicitudesDto Estadisticas { get; set; } = new();
    public DateTime FechaGeneracion { get; set; } = DateTime.Now;
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string? FiltroEstado { get; set; }
    public string? FiltroFacultad { get; set; }
}

public class SolicitudResumenDto
{
    public Guid Id { get; set; }
    public string DocenteNombre { get; set; } = string.Empty;
    public string DocenteCedula { get; set; } = string.Empty;
    public string DocenteFacultad { get; set; } = string.Empty;
    public NivelTitular NivelActual { get; set; }
    public NivelTitular NivelSolicitado { get; set; }
    public EstadoSolicitud Estado { get; set; }
    public DateTime FechaSolicitud { get; set; }
    public DateTime? FechaRevision { get; set; }
    public DateTime? FechaAprobacion { get; set; }
    public string? RevisadoPor { get; set; }
    public int DiasEnProceso { get; set; }
    public string? Observaciones { get; set; }
    public string? MotivoRechazo { get; set; }
    public int NumeroDocumentos { get; set; }
}

public class EstadisticasSolicitudesDto
{
    public int TotalSolicitudes { get; set; }
    public int SolicitudesPendientes { get; set; }
    public int SolicitudesAprobadas { get; set; }
    public int SolicitudesRechazadas { get; set; }
    public int SolicitudesEnProceso { get; set; }
    public double TiempoPromedioRevision { get; set; }
    public Dictionary<string, int> SolicitudesPorMes { get; set; } = new();
    public Dictionary<string, int> SolicitudesPorNivel { get; set; } = new();
    public Dictionary<string, double> TiemposPorEstado { get; set; } = new();
}

public class ReporteProgresoAscensosDto
{
    public List<ProgresoDocenteDto> Progresos { get; set; } = new();
    public ResumenProgresoDto Resumen { get; set; } = new();
    public DateTime FechaGeneracion { get; set; } = DateTime.Now;
}

public class ProgresoDocenteDto
{
    public Guid DocenteId { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Cedula { get; set; } = string.Empty;
    public string Facultad { get; set; } = string.Empty;
    public NivelTitular NivelActual { get; set; }
    public NivelTitular? SiguienteNivel { get; set; }
    public bool PuedeAscender { get; set; }
    public List<RequisitoProgresoDto> Requisitos { get; set; } = new();
    public double PorcentajeCompletitud { get; set; }
    public string EstadoGeneral { get; set; } = string.Empty;
}

public class RequisitoProgresoDto
{
    public string Nombre { get; set; } = string.Empty;
    public string ValorActual { get; set; } = string.Empty;
    public string ValorRequerido { get; set; } = string.Empty;
    public bool Cumple { get; set; }
    public double PorcentajeCumplimiento { get; set; }
    public string Descripcion { get; set; } = string.Empty;
}

public class ResumenProgresoDto
{
    public int TotalDocentes { get; set; }
    public int DocentesListosParaAscender { get; set; }
    public int DocentesCercanos { get; set; } // >75% de requisitos
    public int DocentesEnProgreso { get; set; } // 25-75% de requisitos
    public int DocentesIniciales { get; set; } // <25% de requisitos
    public Dictionary<string, int> ProgresosPorNivel { get; set; } = new();
}

public class ReporteCapacitacionesGeneralDto
{
    public List<CapacitacionDocenteDto> Capacitaciones { get; set; } = new();
    public EstadisticasCapacitacionDto Estadisticas { get; set; } = new();
    public DateTime FechaGeneracion { get; set; } = DateTime.Now;
}

public class CapacitacionDocenteDto
{
    public Guid DocenteId { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Cedula { get; set; } = string.Empty;
    public string Facultad { get; set; } = string.Empty;
    public NivelTitular NivelActual { get; set; }
    public int HorasCapacitacionTotal { get; set; }
    public int HorasRequeridas { get; set; }
    public bool CumpleRequisito { get; set; }
    public DateTime? FechaUltimaCapacitacion { get; set; }
    public List<CertificadoCapacitacionDto> Certificados { get; set; } = new();
}

public class CertificadoCapacitacionDto
{
    public string NombreCurso { get; set; } = string.Empty;
    public string Institucion { get; set; } = string.Empty;
    public int Horas { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public string Estado { get; set; } = string.Empty;
}

public class EstadisticasCapacitacionDto
{
    public int TotalDocentes { get; set; }
    public int DocentesCumplenRequisito { get; set; }
    public double PromedioHorasPorDocente { get; set; }
    public int TotalCertificados { get; set; }
    public Dictionary<string, int> CapacitacionesPorInstitucion { get; set; } = new();
    public Dictionary<string, int> CapacitacionesPorTipo { get; set; } = new();
}

public class ReporteObrasAcademicasGeneralDto
{
    public List<ObrasDocenteDto> ObrasDocentes { get; set; } = new();
    public EstadisticasObrasDto Estadisticas { get; set; } = new();
    public DateTime FechaGeneracion { get; set; } = DateTime.Now;
}

public class ObrasDocenteDto
{
    public Guid DocenteId { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Cedula { get; set; } = string.Empty;
    public string Facultad { get; set; } = string.Empty;
    public NivelTitular NivelActual { get; set; }
    public int NumeroObrasTotal { get; set; }
    public int ObrasRequeridas { get; set; }
    public bool CumpleRequisito { get; set; }
    public DateTime? FechaUltimaObra { get; set; }
    public List<ObraAcademicaResumenDto> Obras { get; set; } = new();
}

public class ObraAcademicaResumenDto
{
    public string Titulo { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public DateTime FechaPublicacion { get; set; }
    public string Editorial { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
}

public class EstadisticasObrasDto
{
    public int TotalDocentes { get; set; }
    public int DocentesCumplenRequisito { get; set; }
    public double PromedioObrasPorDocente { get; set; }
    public int TotalObras { get; set; }
    public Dictionary<string, int> ObrasPorTipo { get; set; } = new();
    public Dictionary<string, int> ObrasPorAnio { get; set; } = new();
}

public class ReporteIndicadoresGestionDto
{
    public IndicadoresRendimientoDto Rendimiento { get; set; } = new();
    public IndicadoresCalidadDto Calidad { get; set; } = new();
    public IndicadoresTiempoDto Tiempos { get; set; } = new();
    public List<TendenciaDto> Tendencias { get; set; } = new();
    public DateTime FechaGeneracion { get; set; } = DateTime.Now;
}

public class IndicadoresRendimientoDto
{
    public double TasaAscensoAnual { get; set; }
    public double TasaAprobacionSolicitudes { get; set; }
    public double TasaRechazoPorDeficiencias { get; set; }
    public double PromedioDocentesPorNivel { get; set; }
    public int SolicitudesPromedioMensual { get; set; }
}

public class IndicadoresCalidadDto
{
    public double PromedioEvaluacionesDocentes { get; set; }
    public double PromedioHorasCapacitacion { get; set; }
    public double PromedioObrasAcademicas { get; set; }
    public double PorcentajeDocentesActivos { get; set; }
    public double IndiceCompletitudExpedientes { get; set; }
}

public class IndicadoresTiempoDto
{
    public double TiempoPromedioRevisionDias { get; set; }
    public double TiempoPromedioAprobacionDias { get; set; }
    public double TiempoPromedioEnNivelAnios { get; set; }
    public double TiempoMaximoRevisionDias { get; set; }
    public double EficienciaProcesamiento { get; set; }
}

public class TendenciaDto
{
    public string Periodo { get; set; } = string.Empty;
    public int SolicitudesRecibidas { get; set; }
    public int SolicitudesAprobadas { get; set; }
    public int SolicitudesRechazadas { get; set; }
    public double TasaAprobacion { get; set; }
    public double TiempoPromedioRevision { get; set; }
}

public class ReporteTiemposProcesamientoDto
{
    public List<TiempoProcesamientoDto> Procesamiento { get; set; } = new();
    public EstadisticasTiemposDto Estadisticas { get; set; } = new();
    public DateTime FechaGeneracion { get; set; } = DateTime.Now;
}

public class TiempoProcesamientoDto
{
    public Guid SolicitudId { get; set; }
    public string DocenteNombre { get; set; } = string.Empty;
    public string DocenteCedula { get; set; } = string.Empty;
    public DateTime FechaSolicitud { get; set; }
    public DateTime? FechaRevision { get; set; }
    public DateTime? FechaAprobacion { get; set; }
    public int DiasParaRevision { get; set; }
    public int DiasParaAprobacion { get; set; }
    public int DiasTotal { get; set; }
    public EstadoSolicitud Estado { get; set; }
    public string? RevisadoPor { get; set; }
}

public class EstadisticasTiemposDto
{
    public double TiempoPromedioRevision { get; set; }
    public double TiempoPromedioAprobacion { get; set; }
    public double TiempoPromedioTotal { get; set; }
    public int TiempoMinimoRevision { get; set; }
    public int TiempoMaximoRevision { get; set; }
    public double DesviacionEstandarRevision { get; set; }
    public Dictionary<string, double> TiemposPorRevisor { get; set; } = new();
}
