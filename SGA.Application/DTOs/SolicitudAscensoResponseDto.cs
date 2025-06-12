using System.ComponentModel.DataAnnotations;

namespace SGA.Application.DTOs
{
    /// <summary>
    /// DTO para la respuesta de información de una solicitud de ascenso
    /// </summary>
    public class SolicitudAscensoResponseDto
    {
        /// <summary>
        /// Identificador único de la solicitud
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID del docente solicitante
        /// </summary>
        public int DocenteId { get; set; }

        /// <summary>
        /// Nombre completo del docente
        /// </summary>
        public string NombreDocente { get; set; } = string.Empty;

        /// <summary>
        /// Cédula del docente
        /// </summary>
        public string CedulaDocente { get; set; } = string.Empty;

        /// <summary>
        /// ID del estado de la solicitud
        /// </summary>
        public int EstadoSolicitudId { get; set; }

        /// <summary>
        /// Nombre del estado de la solicitud
        /// </summary>
        public string NombreEstado { get; set; } = string.Empty;

        /// <summary>
        /// Color del estado para la interfaz
        /// </summary>
        public string? ColorEstado { get; set; }

        /// <summary>
        /// Fecha de la solicitud
        /// </summary>
        public DateTime FechaSolicitud { get; set; }

        /// <summary>
        /// Nivel actual del docente al momento de la solicitud
        /// </summary>
        public int NivelActual { get; set; }

        /// <summary>
        /// Nivel solicitado
        /// </summary>
        public int NivelSolicitado { get; set; }

        /// <summary>
        /// Motivo de rechazo (si aplica)
        /// </summary>
        public string? MotivoRechazo { get; set; }

        /// <summary>
        /// Fecha de revisión
        /// </summary>
        public DateTime? FechaRevision { get; set; }

        /// <summary>
        /// ID del revisor
        /// </summary>
        public int? RevisorId { get; set; }

        /// <summary>
        /// Nombre del revisor
        /// </summary>
        public string? NombreRevisor { get; set; }        /// <summary>
        /// Observaciones del revisor
        /// </summary>
        public string? ObservacionesRevisor { get; set; }

        /// <summary>
        /// Tiempo en rol actual (en años) - Propiedad directa para compatibilidad
        /// </summary>
        public int TiempoEnRol { get; set; }

        /// <summary>
        /// Número de obras - Propiedad directa para compatibilidad
        /// </summary>
        public int NumeroObras { get; set; }

        /// <summary>
        /// Puntaje de evaluación - Propiedad directa para compatibilidad
        /// </summary>
        public decimal PuntajeEvaluacion { get; set; }

        /// <summary>
        /// Horas de capacitación - Propiedad directa para compatibilidad
        /// </summary>
        public int HorasCapacitacion { get; set; }

        /// <summary>
        /// Tiempo de investigación (en meses) - Propiedad directa para compatibilidad
        /// </summary>
        public int TiempoInvestigacion { get; set; }

        /// <summary>
        /// Cumple con tiempo - Propiedad directa para compatibilidad
        /// </summary>
        public bool CumpleTiempo { get; set; }

        /// <summary>
        /// Cumple con obras - Propiedad directa para compatibilidad
        /// </summary>
        public bool CumpleObras { get; set; }

        /// <summary>
        /// Cumple con evaluación - Propiedad directa para compatibilidad
        /// </summary>
        public bool CumpleEvaluacion { get; set; }

        /// <summary>
        /// Cumple con capacitación - Propiedad directa para compatibilidad
        /// </summary>
        public bool CumpleCapacitacion { get; set; }

        /// <summary>
        /// Cumple con investigación - Propiedad directa para compatibilidad
        /// </summary>
        public bool CumpleInvestigacion { get; set; }

        /// <summary>
        /// Fecha de creación - Propiedad directa para compatibilidad
        /// </summary>
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Datos de requisitos al momento de solicitar (snapshot)
        /// </summary>
        public RequisitosSolicitudDto Requisitos { get; set; } = new RequisitosSolicitudDto();

        /// <summary>
        /// Indicadores de cumplimiento de requisitos
        /// </summary>
        public CumplimientoRequisitosDto Cumplimiento { get; set; } = new CumplimientoRequisitosDto();

        /// <summary>
        /// Lista de documentos adjuntos
        /// </summary>
        public List<DocumentoSolicitudDto> Documentos { get; set; } = new List<DocumentoSolicitudDto>();

        /// <summary>
        /// Fecha de última actualización
        /// </summary>
        public DateTime FechaActualizacion { get; set; }
    }

    /// <summary>
    /// DTO para los requisitos al momento de la solicitud
    /// </summary>
    public class RequisitosSolicitudDto
    {
        /// <summary>
        /// Tiempo en rol actual (en años)
        /// </summary>
        public int TiempoEnRol { get; set; }

        /// <summary>
        /// Número de obras
        /// </summary>
        public int NumeroObras { get; set; }

        /// <summary>
        /// Puntaje de evaluación
        /// </summary>
        public decimal PuntajeEvaluacion { get; set; }

        /// <summary>
        /// Horas de capacitación
        /// </summary>
        public int HorasCapacitacion { get; set; }

        /// <summary>
        /// Tiempo en investigación (en meses)
        /// </summary>
        public int TiempoInvestigacion { get; set; }
    }

    /// <summary>
    /// DTO para el cumplimiento de requisitos
    /// </summary>
    public class CumplimientoRequisitosDto
    {
        /// <summary>
        /// Cumple tiempo mínimo
        /// </summary>
        public bool CumpleTiempo { get; set; }

        /// <summary>
        /// Cumple número mínimo de obras
        /// </summary>
        public bool CumpleObras { get; set; }

        /// <summary>
        /// Cumple puntaje mínimo de evaluación
        /// </summary>
        public bool CumpleEvaluacion { get; set; }

        /// <summary>
        /// Cumple horas mínimas de capacitación
        /// </summary>
        public bool CumpleCapacitacion { get; set; }

        /// <summary>
        /// Cumple tiempo mínimo de investigación
        /// </summary>
        public bool CumpleInvestigacion { get; set; }
    }

    /// <summary>
    /// DTO para un documento asociado a una solicitud
    /// </summary>
    public class DocumentoSolicitudDto
    {
        /// <summary>
        /// ID del documento
        /// </summary>
        public int DocumentoId { get; set; }

        /// <summary>
        /// Nombre del documento
        /// </summary>
        public string NombreDocumento { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de asociación con la solicitud
        /// </summary>
        public DateTime FechaAsociacion { get; set; }

        /// <summary>
        /// Indica si es obligatorio
        /// </summary>
        public bool EsObligatorio { get; set; }

        /// <summary>
        /// Observaciones sobre el documento
        /// </summary>
        public string? Observaciones { get; set; }
    }
}
