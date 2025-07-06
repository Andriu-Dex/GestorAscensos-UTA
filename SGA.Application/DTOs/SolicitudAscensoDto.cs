using SGA.Application.DTOs.Docentes;
using SGA.Domain.Enums;
using SGA.Domain.Extensions;

namespace SGA.Application.DTOs;

public class SolicitudAscensoDto
{
    public Guid Id { get; set; }
    public Guid DocenteId { get; set; }
    public DocenteDto? Docente { get; set; }
    public NivelTitular NivelActual { get; set; }
    public NivelTitular NivelSolicitado { get; set; }
    public EstadoSolicitud Estado { get; set; }
    public DateTime FechaSolicitud { get; set; }
    public DateTime? FechaRevision { get; set; }
    public DateTime? FechaAprobacion { get; set; }
    public string? Observaciones { get; set; }
    public string? MotivoRechazo { get; set; }
    public Guid? RevisadoPorId { get; set; }
    public string? RevisadoPor { get; set; }
    
    // Documentos adjuntos
    public List<DocumentoDto> Documentos { get; set; } = new();
    
    // Propiedades calculadas
    public string EstadoTexto => Estado.GetDescription();
    public string NivelActualTexto => NivelActual.GetDescription();
    public string NivelSolicitadoTexto => NivelSolicitado.GetDescription();
    public bool PuedeSerAprobada => Estado == EstadoSolicitud.Pendiente || Estado == EstadoSolicitud.EnProceso;
    public bool PuedeSerRechazada => Estado == EstadoSolicitud.Pendiente || Estado == EstadoSolicitud.EnProceso;
}

public class CreateSolicitudAscensoDto
{
    public Guid DocenteId { get; set; }
    public NivelTitular NivelSolicitado { get; set; }
    public string? Observaciones { get; set; }
    public List<CreateDocumentoDto> Documentos { get; set; } = new();
}

public class UpdateSolicitudAscensoDto
{
    public Guid Id { get; set; }
    public EstadoSolicitud Estado { get; set; }
    public string? Observaciones { get; set; }
    public string? MotivoRechazo { get; set; }
    public Guid? RevisadoPorId { get; set; }
}

public class AprobacionSolicitudDto
{
    public Guid SolicitudId { get; set; }
    public bool Aprobada { get; set; }
    public string? Observaciones { get; set; }
    public string? MotivoRechazo { get; set; }
}
