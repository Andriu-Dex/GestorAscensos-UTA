using System;
using System.Collections.Generic;

namespace SGA.Domain.Entities
{
    public class SolicitudAscenso
    {
        public int Id { get; set; }
        public int DocenteId { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public int NivelActual { get; set; }
        public int NivelSolicitado { get; set; }
        public EstadoSolicitud Estado { get; set; }
        public string MotivoRechazo { get; set; }
        public DateTime? FechaRevision { get; set; }
        public int? RevisorId { get; set; }
        
        // Datos de requisitos al momento de solicitar
        public int TiempoEnRol { get; set; } // En años
        public int NumeroObras { get; set; }
        public decimal PuntajeEvaluacion { get; set; }
        public int HorasCapacitacion { get; set; }
        public int TiempoInvestigacion { get; set; } // En meses
        
        // Navegación
        public Docente Docente { get; set; }
        public ICollection<DocumentoSolicitud> Documentos { get; set; }
    }
    
    public enum EstadoSolicitud
    {
        Enviada,
        EnProceso,
        Aprobada,
        Rechazada,
        Archivada
    }
    
    public class DocumentoSolicitud
    {
        public int Id { get; set; }
        public int SolicitudId { get; set; }
        public int DocumentoId { get; set; }
        
        // Navegación
        public SolicitudAscenso Solicitud { get; set; }
        public Documento Documento { get; set; }
    }
}
