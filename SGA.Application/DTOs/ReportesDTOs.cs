using SGA.Domain.Enums;

namespace SGA.Application.DTOs;

public class ReportePreviewDto
{
    public string HtmlContent { get; set; } = string.Empty;
    public string TituloReporte { get; set; } = string.Empty;
    public string NombreArchivo { get; set; } = string.Empty;
}

public class ReporteDocenteDto
{
    public Guid Id { get; set; }
    public string Cedula { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Celular { get; set; }
    public NivelTitular NivelActual { get; set; }
    public DateTime FechaInicioNivelActual { get; set; }
    public DateTime? FechaUltimoAscenso { get; set; }
    public DateTime? FechaNombramiento { get; set; }
    public decimal? PromedioEvaluaciones { get; set; }
    public int? HorasCapacitacion { get; set; }
    public int? NumeroObrasAcademicas { get; set; }
    public int? MesesInvestigacion { get; set; }
    public DateTime? FechaUltimaImportacion { get; set; }
    public string FacultadNombre { get; set; } = string.Empty;
    public string CargoActual { get; set; } = string.Empty;
    public int TiempoEnNivelAnios { get; set; }
    public int TiempoEnNivelMeses { get; set; }
    public bool PuedeAscender { get; set; }
}

public class ReporteSolicitudDto
{
    public Guid Id { get; set; }
    public string DocenteNombres { get; set; } = string.Empty;
    public string DocenteApellidos { get; set; } = string.Empty;
    public string DocenteCedula { get; set; } = string.Empty;
    public string DocenteEmail { get; set; } = string.Empty;
    public NivelTitular NivelActual { get; set; }
    public NivelTitular NivelSolicitado { get; set; }
    public EstadoSolicitud Estado { get; set; }
    public DateTime FechaSolicitud { get; set; }
    public DateTime? FechaAprobacion { get; set; }
    public string? MotivoRechazo { get; set; }
    public string? Observaciones { get; set; }
    public decimal PromedioEvaluaciones { get; set; }
    public int HorasCapacitacion { get; set; }
    public int NumeroObrasAcademicas { get; set; }
    public int MesesInvestigacion { get; set; }
    public int TiempoEnNivelDias { get; set; }
    public string AprobadoPorNombre { get; set; } = string.Empty;
    public List<DocumentoReporteDto> Documentos { get; set; } = new();
}

public class DocumentoReporteDto
{
    public string NombreArchivo { get; set; } = string.Empty;
    public string TipoDocumento { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public long TamanoBytes { get; set; }
}

public class ReporteRequisitoDto
{
    public string NombreRequisito { get; set; } = string.Empty;
    public string ValorRequerido { get; set; } = string.Empty;
    public string ValorActual { get; set; } = string.Empty;
    public bool Cumple { get; set; }
    public decimal PorcentajeCumplimiento { get; set; }
    public string Observacion { get; set; } = string.Empty;
}

public class ReporteCapacitacionDto
{
    public string NombreCurso { get; set; } = string.Empty;
    public int Horas { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public string Institucion { get; set; } = string.Empty;
    public string TipoCertificacion { get; set; } = string.Empty;
    public bool Aprobado { get; set; }
}

public class ReporteObraAcademicaDto
{
    public string Titulo { get; set; } = string.Empty;
    public string TipoObra { get; set; } = string.Empty;
    public DateTime FechaPublicacion { get; set; }
    public string Editorial { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public string Revista { get; set; } = string.Empty;
    public string Autores { get; set; } = string.Empty;
    public bool EsPrincipal { get; set; }
}

public class ReporteResumenAscensoDto
{
    public ReporteDocenteDto Docente { get; set; } = new();
    public List<ReporteRequisitoDto> Requisitos { get; set; } = new();
    public List<ReporteSolicitudDto> HistorialSolicitudes { get; set; } = new();
    public List<ReporteCapacitacionDto> Capacitaciones { get; set; } = new();
    public List<ReporteObraAcademicaDto> ObrasAcademicas { get; set; } = new();
    public DateTime FechaGeneracion { get; set; } = DateTime.Now;
    public string NivelSiguiente { get; set; } = string.Empty;
    public bool PuedeAscender { get; set; }
    public string ObservacionesGenerales { get; set; } = string.Empty;
}
