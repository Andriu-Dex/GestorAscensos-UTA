using SGA.Domain.Enums;

namespace SGA.Application.DTOs.Admin;

public class DocenteAdminDto
{
    public Guid Id { get; set; }
    public string Cedula { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Facultad { get; set; }
    public string? Departamento { get; set; }
    public int NivelActual { get; set; }
    public DateTime FechaInicioNivelActual { get; set; }
    public double TiempoEnNivelAnios { get; set; }
    public int SolicitudesPendientes { get; set; }
    public int SolicitudesAprobadas { get; set; }
    public int SolicitudesRechazadas { get; set; }
    public string? Celular { get; set; }
    public DateTime? FechaNombramiento { get; set; }
    public DateTime? FechaUltimoAscenso { get; set; }
    public double PromedioEvaluaciones { get; set; }
    public int HorasCapacitacion { get; set; }
    public int NumeroObrasAcademicas { get; set; }
    public int MesesInvestigacion { get; set; }
    public bool PuedeAscender { get; set; }
    public string? SiguienteNivel { get; set; }
}

public class DocenteDetalleAdminDto : DocenteAdminDto
{
    public DateTime? FechaUltimaImportacion { get; set; }
    public string? FotoPerfilBase64 { get; set; }
    public List<SolicitudResumenDto> SolicitudesRecientes { get; set; } = new();
    public List<EvaluacionResumenDto> EvaluacionesRecientes { get; set; } = new();
    public List<CapacitacionResumenDto> CapacitacionesRecientes { get; set; } = new();
    public List<ObraAcademicaResumenDto> ObrasRecientes { get; set; } = new();
    public List<InvestigacionResumenDto> InvestigacionesRecientes { get; set; } = new();
}

public class SolicitudResumenDto
{
    public Guid Id { get; set; }
    public string NivelActual { get; set; } = string.Empty;
    public string NivelSolicitado { get; set; } = string.Empty;
    public EstadoSolicitud Estado { get; set; }
    public DateTime FechaSolicitud { get; set; }
    public DateTime? FechaRespuesta { get; set; }
    public string? Observaciones { get; set; }
    public string? NombreRevisor { get; set; }
}

public class EvaluacionResumenDto
{
    public Guid Id { get; set; }
    public string? Periodo { get; set; }
    public double? Puntaje { get; set; }
    public DateTime? FechaEvaluacion { get; set; }
    public string? Observaciones { get; set; }
}

public class CapacitacionResumenDto
{
    public Guid Id { get; set; }
    public string? Nombre { get; set; }
    public string? Institucion { get; set; }
    public int? Horas { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string? TipoCertificado { get; set; }
}

public class ObraAcademicaResumenDto
{
    public Guid Id { get; set; }
    public string? Titulo { get; set; }
    public string? Tipo { get; set; }
    public DateTime? FechaPublicacion { get; set; }
    public string? Editorial { get; set; }
    public string? Revista { get; set; }
    public string? Autores { get; set; }
}

public class InvestigacionResumenDto
{
    public Guid Id { get; set; }
    public string? Titulo { get; set; }
    public string? Tipo { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public int? Duracion { get; set; }
    public string? Estado { get; set; }
}

public class ActualizarNivelDocenteDto
{
    public int NuevoNivel { get; set; }
    public string? Observaciones { get; set; }
    public DateTime? FechaEfectiva { get; set; }
}

public class EstadisticasDocentesDto
{
    public int TotalDocentes { get; set; }
    public Dictionary<int, int> DocentesPorNivel { get; set; } = new();
    public Dictionary<string, int> DocentesPorFacultad { get; set; } = new();
    public int SolicitudesPendientes { get; set; }
    public int SolicitudesAprobadas { get; set; }
    public int SolicitudesRechazadas { get; set; }
    public double PromedioTiempoEnNivel { get; set; }
    public int DocentesAptosPorAscenso { get; set; }
}
