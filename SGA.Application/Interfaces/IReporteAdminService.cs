using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SGA.Application.DTOs;

namespace SGA.Application.Interfaces
{
    public interface IReporteAdminService
    {
        // Reportes de gestión por estado
        Task<byte[]> GenerarReporteProcesosPorEstadoAsync(FiltroReporteAdminDTO filtro);
        Task<string> GenerarVistaReporteProcesosPorEstadoAsync(FiltroReporteAdminDTO filtro);
        
        // Reportes por facultad/departamento
        Task<byte[]> GenerarReporteAscensosPorFacultadAsync(FiltroReporteAdminDTO filtro);
        Task<string> GenerarVistaReporteAscensosPorFacultadAsync(FiltroReporteAdminDTO filtro);
        
        // Reportes de tiempo de resolución
        Task<byte[]> GenerarReporteTiempoResolucionAsync(FiltroReporteAdminDTO filtro);
        Task<string> GenerarVistaReporteTiempoResolucionAsync(FiltroReporteAdminDTO filtro);
        
        // Reportes de distribución de docentes por nivel
        Task<byte[]> GenerarReporteDistribucionDocentesAsync(FiltroReporteAdminDTO filtro);
        Task<string> GenerarVistaReporteDistribucionDocentesAsync(FiltroReporteAdminDTO filtro);
        
        // Reportes de actividad por períodos
        Task<byte[]> GenerarReporteActividadPeriodoAsync(FiltroReporteAdminDTO filtro);
        Task<string> GenerarVistaReporteActividadPeriodoAsync(FiltroReporteAdminDTO filtro);

        // Reporte consolidado de gestión
        Task<byte[]> GenerarReporteConsolidadoGestionAsync(FiltroReporteAdminDTO filtro);
        Task<string> GenerarVistaReporteConsolidadoGestionAsync(FiltroReporteAdminDTO filtro);
    }
}
