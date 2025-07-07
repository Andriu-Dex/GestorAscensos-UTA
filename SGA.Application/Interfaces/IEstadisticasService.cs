using SGA.Application.DTOs.Admin;

namespace SGA.Application.Interfaces
{
    public interface IEstadisticasService
    {
        /// <summary>
        /// Obtiene estadísticas completas del sistema
        /// </summary>
        Task<EstadisticasCompletasDto> GetEstadisticasCompletasAsync();

        /// <summary>
        /// Obtiene estadísticas generales para el dashboard
        /// </summary>
        Task<EstadisticasGeneralesDto> GetEstadisticasGeneralesAsync();

        /// <summary>
        /// Obtiene estadísticas organizadas por facultad
        /// </summary>
        Task<List<EstadisticasFacultadDto>> GetEstadisticasPorFacultadAsync();

        /// <summary>
        /// Obtiene estadísticas organizadas por nivel académico
        /// </summary>
        Task<List<EstadisticasNivelDto>> GetEstadisticasPorNivelAsync();

        /// <summary>
        /// Obtiene estadísticas de actividad mensual
        /// </summary>
        Task<List<EstadisticasActividadMensualDto>> GetEstadisticasActividadMensualAsync();

        /// <summary>
        /// Obtiene lista de facultades disponibles
        /// </summary>
        Task<List<string>> GetFacultadesAsync();
    }
}
