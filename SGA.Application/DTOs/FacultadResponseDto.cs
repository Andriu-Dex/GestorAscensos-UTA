using System.ComponentModel.DataAnnotations;

namespace SGA.Application.DTOs
{
    /// <summary>
    /// DTO para la respuesta de información de una facultad
    /// </summary>
    public class FacultadResponseDto
    {
        /// <summary>
        /// Identificador único de la facultad
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre de la facultad
        /// </summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Código único de la facultad
        /// </summary>
        public string Codigo { get; set; } = string.Empty;

        /// <summary>
        /// Descripción de la facultad
        /// </summary>
        public string? Descripcion { get; set; }        /// <summary>
        /// Indica si la facultad está activa
        /// </summary>
        public bool EsActiva { get; set; }

        /// <summary>
        /// Indica si la facultad está activa (alias para compatibilidad)
        /// </summary>
        public bool Activa { get; set; }

        /// <summary>
        /// Color representativo de la facultad (para UI)
        /// </summary>
        public string? Color { get; set; }

        /// <summary>
        /// Fecha de creación de la facultad
        /// </summary>
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Fecha de última actualización
        /// </summary>
        public DateTime? FechaActualizacion { get; set; }

        /// <summary>
        /// Número de docentes asociados a esta facultad
        /// </summary>
        public int TotalDocentes { get; set; }

        /// <summary>
        /// Número de solicitudes de ascenso activas de esta facultad
        /// </summary>
        public int SolicitudesActivas { get; set; }
    }
}
