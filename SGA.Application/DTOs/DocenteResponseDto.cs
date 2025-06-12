using System.ComponentModel.DataAnnotations;

namespace SGA.Application.DTOs
{
    /// <summary>
    /// DTO para la respuesta de información de un docente
    /// </summary>
    public class DocenteResponseDto
    {
        /// <summary>
        /// Identificador único del docente
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Cédula de identidad del docente
        /// </summary>
        public string Cedula { get; set; } = string.Empty;

        /// <summary>
        /// Nombres del docente
        /// </summary>
        public string Nombres { get; set; } = string.Empty;

        /// <summary>
        /// Apellidos del docente
        /// </summary>
        public string Apellidos { get; set; } = string.Empty;

        /// <summary>
        /// Email del docente
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Teléfono de contacto del docente
        /// </summary>
        public string TelefonoContacto { get; set; } = string.Empty;

        /// <summary>
        /// ID de la facultad a la que pertenece
        /// </summary>
        public int FacultadId { get; set; }

        /// <summary>
        /// Nombre de la facultad
        /// </summary>
        public string NombreFacultad { get; set; } = string.Empty;

        /// <summary>
        /// Nivel actual del docente (1-5)
        /// </summary>
        public int NivelActual { get; set; }

        /// <summary>
        /// Fecha de ingreso al nivel actual
        /// </summary>
        public DateTime FechaIngresoNivelActual { get; set; }

        /// <summary>
        /// Nombre de usuario
        /// </summary>
        public string NombreUsuario { get; set; } = string.Empty;

        /// <summary>
        /// Número de intentos fallidos de login
        /// </summary>
        public int IntentosFallidos { get; set; }

        /// <summary>
        /// Indica si la cuenta está bloqueada
        /// </summary>
        public bool Bloqueado { get; set; }

        /// <summary>
        /// Fecha de bloqueo de la cuenta
        /// </summary>
        public DateTime? FechaBloqueo { get; set; }

        /// <summary>
        /// Indica si es administrador
        /// </summary>
        public bool EsAdministrador { get; set; }

        /// <summary>
        /// Fecha de registro en el sistema
        /// </summary>
        public DateTime FechaRegistro { get; set; }

        /// <summary>
        /// Indica si el docente está activo
        /// </summary>
        public bool Activo { get; set; }

        /// <summary>
        /// Fecha de baja (si aplica)
        /// </summary>
        public DateTime? FechaBaja { get; set; }

        /// <summary>
        /// Motivo de baja (si aplica)
        /// </summary>
        public string? MotivoBaja { get; set; }

        /// <summary>
        /// Indicadores del docente
        /// </summary>
        public IndicadorDocenteDto? Indicadores { get; set; }

        /// <summary>
        /// Número de documentos subidos
        /// </summary>
        public int TotalDocumentos { get; set; }

        /// <summary>
        /// Número de solicitudes de ascenso
        /// </summary>
        public int TotalSolicitudes { get; set; }
    }

    /// <summary>
    /// DTO para los indicadores de un docente
    /// </summary>
    public class IndicadorDocenteDto
    {
        /// <summary>
        /// Tiempo en rol actual (en años)
        /// </summary>
        public int TiempoEnRolActual { get; set; }

        /// <summary>
        /// Número de obras publicadas
        /// </summary>
        public int NumeroObras { get; set; }

        /// <summary>
        /// Puntaje de evaluación (0-100)
        /// </summary>
        public decimal PuntajeEvaluacion { get; set; }

        /// <summary>
        /// Horas de capacitación completadas
        /// </summary>
        public int HorasCapacitacion { get; set; }

        /// <summary>
        /// Tiempo en investigación (en meses)
        /// </summary>
        public int TiempoInvestigacion { get; set; }

        /// <summary>
        /// Fecha de última actualización
        /// </summary>
        public DateTime? FechaActualizacion { get; set; }
    }
}
