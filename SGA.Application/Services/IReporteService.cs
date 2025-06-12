using System.Threading.Tasks;

namespace SGA.Application.Services
{
    public interface IReporteService
    {
        /// <summary>
        /// Genera un reporte PDF con el estado actual del docente y su proceso de ascenso
        /// </summary>
        /// <param name="docenteId">ID del docente</param>
        /// <returns>Arreglo de bytes del documento PDF generado</returns>
        Task<byte[]> GenerarReporteDocenteAsync(int docenteId);
        
        /// <summary>
        /// Genera un reporte PDF de una solicitud de ascenso específica
        /// </summary>
        /// <param name="solicitudId">ID de la solicitud</param>
        /// <returns>Arreglo de bytes del documento PDF generado</returns>
        Task<byte[]> GenerarReporteSolicitudAsync(int solicitudId);
    }
}
