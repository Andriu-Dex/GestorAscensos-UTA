using System;
using System.Collections.Generic;

namespace SGA.Domain.Entities
{
    public class SolicitudAscenso
    {
        public SolicitudAscenso()
        {
            Documentos = new List<DocumentoSolicitud>();
        }

        public int Id { get; set; }
        public int DocenteId { get; set; }        public int EstadoSolicitudId { get; set; }
        public DateTime FechaSolicitud { get; set; } = DateTime.Now;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public int NivelActual { get; set; }
        public int NivelSolicitado { get; set; }
        public string? MotivoRechazo { get; set; }
        public DateTime? FechaRevision { get; set; }
        public int? RevisorId { get; set; }
        public string? ObservacionesRevisor { get; set; }
        
        // Datos de requisitos al momento de solicitar (snapshot)
        public int TiempoEnRol { get; set; } // En años
        public int NumeroObras { get; set; }
        public decimal PuntajeEvaluacion { get; set; }
        public int HorasCapacitacion { get; set; }
        public int TiempoInvestigacion { get; set; } // En meses
        
        // Indicadores de cumplimiento de requisitos
        public bool CumpleTiempo { get; set; }
        public bool CumpleObras { get; set; }
        public bool CumpleEvaluacion { get; set; }
        public bool CumpleCapacitacion { get; set; }
        public bool CumpleInvestigacion { get; set; }
        
        // Fechas de actualización automática
        public DateTime FechaActualizacion { get; set; } = DateTime.Now;
        
        // Navegación
        public required Docente Docente { get; set; }
        public required EstadoSolicitud EstadoSolicitud { get; set; }
        public Docente? Revisor { get; set; }
        public ICollection<DocumentoSolicitud> Documentos { get; set; }
    }
    
    public class DocumentoSolicitud
    {
        public int Id { get; set; }
        public int SolicitudId { get; set; }
        public int DocumentoId { get; set; }
        public DateTime FechaAsociacion { get; set; } = DateTime.Now;
        public bool EsObligatorio { get; set; } = false;
        public string? Observaciones { get; set; }
        
        // Navegación
        public required SolicitudAscenso Solicitud { get; set; }
        public required Documento Documento { get; set; }
    }
}
