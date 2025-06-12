using System.ComponentModel.DataAnnotations;

namespace SGA.Application.DTOs
{
    /// <summary>
    /// DTO para la respuesta de información de un estado de solicitud
    /// </summary>
    public class EstadoSolicitudResponseDto
    {        /// <summary>
        /// Identificador único del estado de solicitud
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Código único del estado de solicitud
        /// </summary>
        public string Codigo { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del estado de solicitud
        /// </summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Descripción del estado de solicitud
        /// </summary>
        public string? Descripcion { get; set; }        /// <summary>
        /// Color hexadecimal para mostrar este estado en la interfaz de usuario
        /// </summary>
        public string? Color { get; set; }

        /// <summary>
        /// Color hexadecimal para mostrar este estado (alias para compatibilidad)
        /// </summary>
        public string? ColorHex { get; set; }

        /// <summary>
        /// Indica si este es un estado final (no permite transiciones a otros estados)
        /// </summary>
        public bool EsEstadoFinal { get; set; }

        /// <summary>
        /// Indica si este estado requiere revisión manual
        /// </summary>
        public bool RequiereRevision { get; set; }

        /// <summary>
        /// Indica si el estado está activo
        /// </summary>
        public bool EsActivo { get; set; }

        /// <summary>
        /// Orden de visualización del estado
        /// </summary>
        public int Orden { get; set; }

        /// <summary>
        /// Fecha de creación del estado
        /// </summary>
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Fecha de última actualización
        /// </summary>
        public DateTime? FechaActualizacion { get; set; }

        /// <summary>
        /// Número de solicitudes en este estado
        /// </summary>
        public int TotalSolicitudes { get; set; }

        /// <summary>
        /// Estados a los que se puede transicionar desde este estado
        /// </summary>
        public List<int> EstadosPermitidos { get; set; } = new List<int>();

        /// <summary>
        /// Nombres de los estados a los que se puede transicionar
        /// </summary>
        public List<string> NombresEstadosPermitidos { get; set; } = new List<string>();
    }
}
