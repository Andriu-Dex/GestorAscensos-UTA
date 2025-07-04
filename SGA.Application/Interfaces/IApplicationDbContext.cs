using Microsoft.EntityFrameworkCore;
using SGA.Domain.Entities;

namespace SGA.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Usuario> Usuarios { get; set; }
    DbSet<Docente> Docentes { get; set; }
    DbSet<SolicitudAscenso> SolicitudesAscenso { get; set; }
    DbSet<Documento> Documentos { get; set; }
    DbSet<LogAuditoria> LogsAuditoria { get; set; }
    DbSet<ObraAcademica> ObrasAcademicas { get; set; }
    DbSet<SolicitudObraAcademica> SolicitudesObrasAcademicas { get; set; }
    DbSet<SolicitudCertificadoCapacitacion> SolicitudesCertificadosCapacitacion { get; set; }
    DbSet<SolicitudEvidenciaInvestigacion> SolicitudesEvidenciasInvestigacion { get; set; }
    DbSet<ConfiguracionRequisito> ConfiguracionesRequisitos { get; set; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
