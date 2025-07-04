using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGA.Application.DTOs;
using SGA.Application.Interfaces;
using SGA.Domain.Enums;

namespace SGA.Application.Services
{
    public class ReporteAdminService : IReporteAdminService
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<ReporteAdminService> _logger;
        
        // Color institucional
        private readonly DeviceRgb ColorPrimario = new DeviceRgb(138, 21, 56);
        private readonly DeviceRgb ColorSecundario = new DeviceRgb(248, 249, 250);

        public ReporteAdminService(IApplicationDbContext context, ILogger<ReporteAdminService> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Reportes de Procesos por Estado

        public async Task<byte[]> GenerarReporteProcesosPorEstadoAsync(FiltroReporteAdminDTO filtro)
        {
            try
            {
                var datos = await ObtenerDatosProcesosPorEstadoAsync(filtro);
                return GenerarPdfProcesosPorEstado(datos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando reporte de procesos por estado");
                throw;
            }
        }

        public async Task<string> GenerarVistaReporteProcesosPorEstadoAsync(FiltroReporteAdminDTO filtro)
        {
            try
            {
                var datos = await ObtenerDatosProcesosPorEstadoAsync(filtro);
                return GenerarHtmlProcesosPorEstado(datos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando vista HTML de procesos por estado");
                throw;
            }
        }

        private async Task<ReporteProcesosPorEstadoDTO> ObtenerDatosProcesosPorEstadoAsync(FiltroReporteAdminDTO filtro)
        {
            var fechaInicio = filtro.FechaInicio ?? DateTime.Now.AddMonths(-3);
            var fechaFin = filtro.FechaFin ?? DateTime.Now;

            var solicitudesQuery = _context.SolicitudesAscenso
                .Include(s => s.Docente)
                .ThenInclude(d => d!.Departamento)
                .ThenInclude(dep => dep!.Facultad)
                .Where(s => s.FechaSolicitud >= fechaInicio && s.FechaSolicitud <= fechaFin);

            if (!string.IsNullOrEmpty(filtro.Estado))
            {
                if (Enum.TryParse<EstadoSolicitud>(filtro.Estado, out var estadoEnum))
                {
                    solicitudesQuery = solicitudesQuery.Where(s => s.Estado == estadoEnum);
                }
            }

            if (filtro.FacultadId.HasValue)
            {
                solicitudesQuery = solicitudesQuery.Where(s => s.Docente.Departamento != null && s.Docente.Departamento.FacultadId == filtro.FacultadId.Value);
            }

            var solicitudes = await solicitudesQuery.ToListAsync();

            var reporte = new ReporteProcesosPorEstadoDTO
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                TotalPendientes = solicitudes.Count(s => s.Estado == EstadoSolicitud.Pendiente),
                TotalAprobados = solicitudes.Count(s => s.Estado == EstadoSolicitud.Aprobada),
                TotalRechazados = solicitudes.Count(s => s.Estado == EstadoSolicitud.Rechazada),
                TotalEnRevision = solicitudes.Count(s => s.Estado == EstadoSolicitud.EnProceso),
                DetallesProcesos = solicitudes.Select(s => new ProcesoEstadoDetalle
                {
                    Id = s.Id,
                    DocenteNombre = $"{s.Docente.Nombres} {s.Docente.Apellidos}",
                    DocenteCedula = s.Docente.Cedula,
                    NivelActual = s.Docente.NivelActual.ToString(),
                    NivelSolicitado = s.NivelSolicitado.ToString(),
                    FechaSolicitud = s.FechaSolicitud,
                    Estado = s.Estado.ToString(),
                    Facultad = s.Docente.Departamento?.Facultad?.Nombre ?? "N/A",
                    Departamento = s.Docente.Departamento?.Nombre ?? "N/A",
                    DiasProceso = (DateTime.Now - s.FechaSolicitud).Days
                }).ToList()
            };

            return reporte;
        }

        #endregion

        #region Reportes por Facultad

        public async Task<byte[]> GenerarReporteAscensosPorFacultadAsync(FiltroReporteAdminDTO filtro)
        {
            try
            {
                var datos = await ObtenerDatosAscensosPorFacultadAsync(filtro);
                return GenerarPdfAscensosPorFacultad(datos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando reporte de ascensos por facultad");
                throw;
            }
        }

        public async Task<string> GenerarVistaReporteAscensosPorFacultadAsync(FiltroReporteAdminDTO filtro)
        {
            try
            {
                var datos = await ObtenerDatosAscensosPorFacultadAsync(filtro);
                return GenerarHtmlAscensosPorFacultad(datos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando vista HTML de ascensos por facultad");
                throw;
            }
        }

        private async Task<ReporteAscensosPorFacultadDTO> ObtenerDatosAscensosPorFacultadAsync(FiltroReporteAdminDTO filtro)
        {
            var fechaInicio = filtro.FechaInicio ?? DateTime.Now.AddMonths(-6);
            var fechaFin = filtro.FechaFin ?? DateTime.Now;

            var solicitudes = await _context.SolicitudesAscenso
                .Include(s => s.Docente)
                .ThenInclude(d => d!.Departamento)
                .ThenInclude(dep => dep!.Facultad)
                .Where(s => s.FechaSolicitud >= fechaInicio && s.FechaSolicitud <= fechaFin)
                .ToListAsync();

            var facultadesAgrupadas = solicitudes
                .GroupBy(s => s.Docente.Departamento?.Facultad?.Nombre ?? "Sin Facultad")
                .Select(f => new FacultadResumen
                {
                    Nombre = f.Key,
                    TotalSolicitudes = f.Count(),
                    SolicitudesPendientes = f.Count(s => s.Estado == EstadoSolicitud.Pendiente),
                    SolicitudesAprobadas = f.Count(s => s.Estado == EstadoSolicitud.Aprobada),
                    SolicitudesRechazadas = f.Count(s => s.Estado == EstadoSolicitud.Rechazada),
                    Departamentos = f
                        .GroupBy(s => s.Docente.Departamento?.Nombre ?? "Sin Departamento")
                        .Select(d => new DepartamentoResumen
                        {
                            Nombre = d.Key,
                            TotalSolicitudes = d.Count(),
                            SolicitudesPendientes = d.Count(s => s.Estado == EstadoSolicitud.Pendiente),
                            SolicitudesAprobadas = d.Count(s => s.Estado == EstadoSolicitud.Aprobada),
                            SolicitudesRechazadas = d.Count(s => s.Estado == EstadoSolicitud.Rechazada)
                        }).ToList()
                }).ToList();

            return new ReporteAscensosPorFacultadDTO
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                Facultades = facultadesAgrupadas,
                TotalSolicitudes = solicitudes.Count,
                TotalAprobados = solicitudes.Count(s => s.Estado == EstadoSolicitud.Aprobada),
                TotalRechazados = solicitudes.Count(s => s.Estado == EstadoSolicitud.Rechazada)
            };
        }

        #endregion

        #region Reportes de Tiempo de Resolución

        public async Task<byte[]> GenerarReporteTiempoResolucionAsync(FiltroReporteAdminDTO filtro)
        {
            try
            {
                var datos = await ObtenerDatosTiempoResolucionAsync(filtro);
                return GenerarPdfTiempoResolucion(datos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando reporte de tiempo de resolución");
                throw;
            }
        }

        public async Task<string> GenerarVistaReporteTiempoResolucionAsync(FiltroReporteAdminDTO filtro)
        {
            try
            {
                var datos = await ObtenerDatosTiempoResolucionAsync(filtro);
                return GenerarHtmlTiempoResolucion(datos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando vista HTML de tiempo de resolución");
                throw;
            }
        }

        private async Task<ReporteTiempoResolucionDTO> ObtenerDatosTiempoResolucionAsync(FiltroReporteAdminDTO filtro)
        {
            var fechaInicio = filtro.FechaInicio ?? DateTime.Now.AddMonths(-6);
            var fechaFin = filtro.FechaFin ?? DateTime.Now;

            var solicitudesFinalizadas = await _context.SolicitudesAscenso
                .Include(s => s.Docente)
                .ThenInclude(d => d!.Departamento)
                .ThenInclude(dep => dep!.Facultad)
                .Where(s => s.FechaSolicitud >= fechaInicio && 
                           s.FechaSolicitud <= fechaFin &&
                           (s.Estado == EstadoSolicitud.Aprobada || s.Estado == EstadoSolicitud.Rechazada))
                .ToListAsync();

            var tiempoPromedio = solicitudesFinalizadas.Any() ? 
                (int)solicitudesFinalizadas.Average(s => (s.FechaAprobacion ?? DateTime.Now).Subtract(s.FechaSolicitud).TotalDays) : 0;

            var tiemposPorFacultad = solicitudesFinalizadas
                .GroupBy(s => s.Docente.Departamento?.Facultad?.Nombre ?? "Sin Facultad")
                .Select(f => new TiempoResolucionPorFacultad
                {
                    Facultad = f.Key,
                    TiempoPromedioDias = (int)f.Average(s => (s.FechaAprobacion ?? DateTime.Now).Subtract(s.FechaSolicitud).TotalDays),
                    TotalProcesos = f.Count()
                }).ToList();

            var tiemposPorNivel = solicitudesFinalizadas
                .GroupBy(s => s.NivelSolicitado)
                .Select(n => new TiempoResolucionPorNivel
                {
                    NivelDestino = n.Key.ToString(),
                    TiempoPromedioDias = (int)n.Average(s => (s.FechaAprobacion ?? DateTime.Now).Subtract(s.FechaSolicitud).TotalDays),
                    TotalProcesos = n.Count()
                }).ToList();

            var procesosMasLargos = solicitudesFinalizadas
                .OrderByDescending(s => (s.FechaAprobacion ?? DateTime.Now).Subtract(s.FechaSolicitud).TotalDays)
                .Take(10)
                .Select(s => new ProcesoTiempoDetalle
                {
                    Id = s.Id,
                    DocenteNombre = $"{s.Docente.Nombres} {s.Docente.Apellidos}",
                    Facultad = s.Docente.Departamento?.Facultad?.Nombre ?? "N/A",
                    NivelSolicitado = s.NivelSolicitado.ToString(),
                    FechaInicio = s.FechaSolicitud,
                    FechaResolucion = s.FechaAprobacion,
                    DiasProceso = (int)(s.FechaAprobacion ?? DateTime.Now).Subtract(s.FechaSolicitud).TotalDays,
                    Estado = s.Estado.ToString()
                }).ToList();

            return new ReporteTiempoResolucionDTO
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                TiempoPromedioTotalDias = tiempoPromedio,
                TiemposPorFacultad = tiemposPorFacultad,
                TiemposPorNivel = tiemposPorNivel,
                ProcesosMasLargos = procesosMasLargos
            };
        }

        #endregion

        #region Distribución de Docentes

        public async Task<byte[]> GenerarReporteDistribucionDocentesAsync(FiltroReporteAdminDTO filtro)
        {
            try
            {
                var datos = await ObtenerDatosDistribucionDocentesAsync(filtro);
                return GenerarPdfDistribucionDocentes(datos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando reporte de distribución de docentes");
                throw;
            }
        }

        public async Task<string> GenerarVistaReporteDistribucionDocentesAsync(FiltroReporteAdminDTO filtro)
        {
            try
            {
                var datos = await ObtenerDatosDistribucionDocentesAsync(filtro);
                return GenerarHtmlDistribucionDocentes(datos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando vista HTML de distribución de docentes");
                throw;
            }
        }

        private async Task<ReporteDistribucionDocentesDTO> ObtenerDatosDistribucionDocentesAsync(FiltroReporteAdminDTO filtro)
        {
            var docentesQuery = _context.Docentes
                .Include(d => d.Departamento)
                .ThenInclude(dep => dep!.Facultad)
                .Where(d => d.EstaActivo);

            if (filtro.FacultadId.HasValue)
            {
                docentesQuery = docentesQuery.Where(d => d.Departamento != null && d.Departamento.FacultadId == filtro.FacultadId.Value);
            }

            var docentes = await docentesQuery.ToListAsync();

            var distribucionPorNivel = docentes
                .GroupBy(d => d.NivelActual.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            var distribucionPorFacultad = docentes
                .GroupBy(d => d.Departamento?.Facultad?.Nombre ?? "Sin Facultad")
                .Select(f => new FacultadDistribucion
                {
                    Nombre = f.Key,
                    TotalDocentes = f.Count(),
                    DocentesPorNivel = f.GroupBy(d => d.NivelActual.ToString()).ToDictionary(g => g.Key, g => g.Count()),
                    Departamentos = f
                        .GroupBy(d => d.Departamento?.Nombre ?? "Sin Departamento")
                        .Select(dep => new DepartamentoDistribucion
                        {
                            Nombre = dep.Key,
                            TotalDocentes = dep.Count(),
                            DocentesPorNivel = dep.GroupBy(d => d.NivelActual.ToString()).ToDictionary(g => g.Key, g => g.Count())
                        }).ToList()
                }).ToList();

            return new ReporteDistribucionDocentesDTO
            {
                FechaGeneracion = DateTime.Now,
                TotalDocentes = docentes.Count,
                DocentesPorNivel = distribucionPorNivel,
                DistribucionPorFacultad = distribucionPorFacultad
            };
        }

        #endregion

        #region Actividad por Período

        public async Task<byte[]> GenerarReporteActividadPeriodoAsync(FiltroReporteAdminDTO filtro)
        {
            try
            {
                var datos = await ObtenerDatosActividadPeriodoAsync(filtro);
                return GenerarPdfActividadPeriodo(datos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando reporte de actividad por período");
                throw;
            }
        }

        public async Task<string> GenerarVistaReporteActividadPeriodoAsync(FiltroReporteAdminDTO filtro)
        {
            try
            {
                var datos = await ObtenerDatosActividadPeriodoAsync(filtro);
                return GenerarHtmlActividadPeriodo(datos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando vista HTML de actividad por período");
                throw;
            }
        }

        private async Task<ReporteActividadPeriodoDTO> ObtenerDatosActividadPeriodoAsync(FiltroReporteAdminDTO filtro)
        {
            var periodo = filtro.Periodo ?? "Anual";
            var fechaInicio = filtro.FechaInicio ?? DateTime.Now.AddYears(-1);
            var fechaFin = filtro.FechaFin ?? DateTime.Now;

            var solicitudes = await _context.SolicitudesAscenso
                .Include(s => s.Docente)
                .ThenInclude(d => d!.Departamento)
                .ThenInclude(dep => dep!.Facultad)
                .Where(s => s.FechaSolicitud >= fechaInicio && s.FechaSolicitud <= fechaFin)
                .ToListAsync();

            var actividadPorMes = new List<ActividadMensual>();
            var mesActual = new DateTime(fechaInicio.Year, fechaInicio.Month, 1);
            
            while (mesActual <= fechaFin)
            {
                var solicitudesMes = solicitudes.Where(s => 
                    s.FechaSolicitud.Year == mesActual.Year && 
                    s.FechaSolicitud.Month == mesActual.Month).ToList();

                actividadPorMes.Add(new ActividadMensual
                {
                    Mes = mesActual.ToString("yyyy-MM"),
                    SolicitudesNuevas = solicitudesMes.Count,
                    SolicitudesResueltas = solicitudesMes.Count(s => s.Estado == EstadoSolicitud.Aprobada || s.Estado == EstadoSolicitud.Rechazada),
                    SolicitudesAprobadas = solicitudesMes.Count(s => s.Estado == EstadoSolicitud.Aprobada),
                    SolicitudesRechazadas = solicitudesMes.Count(s => s.Estado == EstadoSolicitud.Rechazada)
                });

                mesActual = mesActual.AddMonths(1);
            }

            var actividadPorFacultad = solicitudes
                .GroupBy(s => s.Docente.Departamento?.Facultad?.Nombre ?? "Sin Facultad")
                .Select(f => new FacultadActividad
                {
                    Facultad = f.Key,
                    SolicitudesNuevas = f.Count(),
                    SolicitudesResueltas = f.Count(s => s.Estado == EstadoSolicitud.Aprobada || s.Estado == EstadoSolicitud.Rechazada),
                    SolicitudesAprobadas = f.Count(s => s.Estado == EstadoSolicitud.Aprobada),
                    SolicitudesRechazadas = f.Count(s => s.Estado == EstadoSolicitud.Rechazada)
                }).ToList();

            return new ReporteActividadPeriodoDTO
            {
                Periodo = periodo,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                TotalSolicitudesNuevas = solicitudes.Count,
                TotalSolicitudesResueltas = solicitudes.Count(s => s.Estado == EstadoSolicitud.Aprobada || s.Estado == EstadoSolicitud.Rechazada),
                TotalAprobadas = solicitudes.Count(s => s.Estado == EstadoSolicitud.Aprobada),
                TotalRechazadas = solicitudes.Count(s => s.Estado == EstadoSolicitud.Rechazada),
                ActividadPorMes = actividadPorMes,
                ActividadPorFacultad = actividadPorFacultad
            };
        }

        #endregion

        #region Reporte Consolidado

        public async Task<byte[]> GenerarReporteConsolidadoGestionAsync(FiltroReporteAdminDTO filtro)
        {
            try
            {
                // Este reporte combina elementos de todos los anteriores
                var procesosPorEstado = await ObtenerDatosProcesosPorEstadoAsync(filtro);
                var ascensosPorFacultad = await ObtenerDatosAscensosPorFacultadAsync(filtro);
                var tiempoResolucion = await ObtenerDatosTiempoResolucionAsync(filtro);
                var distribucionDocentes = await ObtenerDatosDistribucionDocentesAsync(filtro);

                return GenerarPdfConsolidado(procesosPorEstado, ascensosPorFacultad, 
                    tiempoResolucion, distribucionDocentes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando reporte consolidado de gestión");
                throw;
            }
        }

        public async Task<string> GenerarVistaReporteConsolidadoGestionAsync(FiltroReporteAdminDTO filtro)
        {
            try
            {
                var procesosPorEstado = await ObtenerDatosProcesosPorEstadoAsync(filtro);
                var ascensosPorFacultad = await ObtenerDatosAscensosPorFacultadAsync(filtro);
                var tiempoResolucion = await ObtenerDatosTiempoResolucionAsync(filtro);
                var distribucionDocentes = await ObtenerDatosDistribucionDocentesAsync(filtro);

                return GenerarHtmlConsolidado(procesosPorEstado, ascensosPorFacultad, 
                    tiempoResolucion, distribucionDocentes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando vista HTML consolidada de gestión");
                throw;
            }
        }

        #endregion

        #region Generación de PDFs

        private byte[] GenerarPdfProcesosPorEstado(ReporteProcesosPorEstadoDTO datos)
        {
            using var stream = new MemoryStream();
            using var writer = new PdfWriter(stream);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf);

            // Título
            document.Add(new Paragraph("REPORTE DE PROCESOS POR ESTADO")
                .SetFontSize(18)
                .SetFontColor(ColorPrimario)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBold());

            // Período
            document.Add(new Paragraph($"Período: {datos.FechaInicio:dd/MM/yyyy} - {datos.FechaFin:dd/MM/yyyy}")
                .SetFontSize(12)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(20));

            // Resumen
            var tablaResumen = new Table(4).UseAllAvailableWidth();
            tablaResumen.AddHeaderCell(new Cell().Add(new Paragraph("Pendientes").SetBold()));
            tablaResumen.AddHeaderCell(new Cell().Add(new Paragraph("Aprobados").SetBold()));
            tablaResumen.AddHeaderCell(new Cell().Add(new Paragraph("Rechazados").SetBold()));
            tablaResumen.AddHeaderCell(new Cell().Add(new Paragraph("En Revisión").SetBold()));

            tablaResumen.AddCell(datos.TotalPendientes.ToString());
            tablaResumen.AddCell(datos.TotalAprobados.ToString());
            tablaResumen.AddCell(datos.TotalRechazados.ToString());
            tablaResumen.AddCell(datos.TotalEnRevision.ToString());

            document.Add(tablaResumen);
            document.Add(new Paragraph("\n"));

            // Detalle de procesos
            document.Add(new Paragraph("DETALLE DE PROCESOS")
                .SetFontSize(14)
                .SetFontColor(ColorPrimario)
                .SetBold());

            var tablaDetalle = new Table(7).UseAllAvailableWidth();
            tablaDetalle.AddHeaderCell(new Cell().Add(new Paragraph("Docente").SetBold()));
            tablaDetalle.AddHeaderCell(new Cell().Add(new Paragraph("Cédula").SetBold()));
            tablaDetalle.AddHeaderCell(new Cell().Add(new Paragraph("Nivel Actual").SetBold()));
            tablaDetalle.AddHeaderCell(new Cell().Add(new Paragraph("Nivel Solicitado").SetBold()));
            tablaDetalle.AddHeaderCell(new Cell().Add(new Paragraph("Fecha Solicitud").SetBold()));
            tablaDetalle.AddHeaderCell(new Cell().Add(new Paragraph("Estado").SetBold()));
            tablaDetalle.AddHeaderCell(new Cell().Add(new Paragraph("Días Proceso").SetBold()));

            foreach (var proceso in datos.DetallesProcesos.OrderBy(p => p.Estado).ThenBy(p => p.FechaSolicitud))
            {
                tablaDetalle.AddCell(proceso.DocenteNombre);
                tablaDetalle.AddCell(proceso.DocenteCedula);
                tablaDetalle.AddCell(proceso.NivelActual);
                tablaDetalle.AddCell(proceso.NivelSolicitado);
                tablaDetalle.AddCell(proceso.FechaSolicitud.ToString("dd/MM/yyyy"));
                tablaDetalle.AddCell(proceso.Estado);
                tablaDetalle.AddCell(proceso.DiasProceso.ToString());
            }

            document.Add(tablaDetalle);

            return stream.ToArray();
        }

        private byte[] GenerarPdfAscensosPorFacultad(ReporteAscensosPorFacultadDTO datos)
        {
            using var stream = new MemoryStream();
            using var writer = new PdfWriter(stream);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf);

            // Título
            document.Add(new Paragraph("REPORTE DE ASCENSOS POR FACULTAD")
                .SetFontSize(18)
                .SetFontColor(ColorPrimario)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBold());

            // Período
            document.Add(new Paragraph($"Período: {datos.FechaInicio:dd/MM/yyyy} - {datos.FechaFin:dd/MM/yyyy}")
                .SetFontSize(12)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(20));

            // Resumen general
            var tablaResumen = new Table(3).UseAllAvailableWidth();
            tablaResumen.AddHeaderCell(new Cell().Add(new Paragraph("Total Solicitudes").SetBold()));
            tablaResumen.AddHeaderCell(new Cell().Add(new Paragraph("Total Aprobados").SetBold()));
            tablaResumen.AddHeaderCell(new Cell().Add(new Paragraph("Total Rechazados").SetBold()));

            tablaResumen.AddCell(datos.TotalSolicitudes.ToString());
            tablaResumen.AddCell(datos.TotalAprobados.ToString());
            tablaResumen.AddCell(datos.TotalRechazados.ToString());

            document.Add(tablaResumen);
            document.Add(new Paragraph("\n"));

            // Detalle por facultad
            foreach (var facultad in datos.Facultades)
            {
                document.Add(new Paragraph($"FACULTAD: {facultad.Nombre}")
                    .SetFontSize(14)
                    .SetFontColor(ColorPrimario)
                    .SetBold());

                var tablaFacultad = new Table(4).UseAllAvailableWidth();
                tablaFacultad.AddHeaderCell("Total");
                tablaFacultad.AddHeaderCell("Pendientes");
                tablaFacultad.AddHeaderCell("Aprobadas");
                tablaFacultad.AddHeaderCell("Rechazadas");

                tablaFacultad.AddCell(facultad.TotalSolicitudes.ToString());
                tablaFacultad.AddCell(facultad.SolicitudesPendientes.ToString());
                tablaFacultad.AddCell(facultad.SolicitudesAprobadas.ToString());
                tablaFacultad.AddCell(facultad.SolicitudesRechazadas.ToString());

                document.Add(tablaFacultad);
                document.Add(new Paragraph("\n"));
            }

            return stream.ToArray();
        }

        // Métodos de PDF para otros reportes
        private byte[] GenerarPdfTiempoResolucion(ReporteTiempoResolucionDTO datos)
        {
            using var stream = new MemoryStream();
            using var writer = new PdfWriter(stream);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf);

            document.Add(new Paragraph("REPORTE DE TIEMPO DE RESOLUCIÓN")
                .SetFontSize(18)
                .SetFontColor(ColorPrimario)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBold());

            document.Add(new Paragraph($"Período: {datos.FechaInicio:dd/MM/yyyy} - {datos.FechaFin:dd/MM/yyyy}")
                .SetFontSize(12)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(20));

            document.Add(new Paragraph($"Tiempo Promedio General: {datos.TiempoPromedioTotalDias} días")
                .SetFontSize(14)
                .SetBold());

            // Tabla de tiempos por facultad
            if (datos.TiemposPorFacultad.Any())
            {
                document.Add(new Paragraph("TIEMPOS POR FACULTAD")
                    .SetFontSize(14)
                    .SetFontColor(ColorPrimario)
                    .SetBold());

                var tabla = new Table(3).UseAllAvailableWidth();
                tabla.AddHeaderCell("Facultad");
                tabla.AddHeaderCell("Tiempo Promedio (días)");
                tabla.AddHeaderCell("Total Procesos");

                foreach (var tiempo in datos.TiemposPorFacultad)
                {
                    tabla.AddCell(tiempo.Facultad);
                    tabla.AddCell(tiempo.TiempoPromedioDias.ToString());
                    tabla.AddCell(tiempo.TotalProcesos.ToString());
                }

                document.Add(tabla);
            }

            return stream.ToArray();
        }

        private byte[] GenerarPdfDistribucionDocentes(ReporteDistribucionDocentesDTO datos)
        {
            using var stream = new MemoryStream();
            using var writer = new PdfWriter(stream);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf);

            document.Add(new Paragraph("REPORTE DE DISTRIBUCIÓN DE DOCENTES")
                .SetFontSize(18)
                .SetFontColor(ColorPrimario)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBold());

            document.Add(new Paragraph($"Fecha de Generación: {datos.FechaGeneracion:dd/MM/yyyy}")
                .SetFontSize(12)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(20));

            document.Add(new Paragraph($"Total de Docentes: {datos.TotalDocentes}")
                .SetFontSize(14)
                .SetBold());

            // Distribución por nivel
            if (datos.DocentesPorNivel.Any())
            {
                document.Add(new Paragraph("DISTRIBUCIÓN POR NIVEL ACADÉMICO")
                    .SetFontSize(14)
                    .SetFontColor(ColorPrimario)
                    .SetBold());

                var tabla = new Table(2).UseAllAvailableWidth();
                tabla.AddHeaderCell("Nivel Académico");
                tabla.AddHeaderCell("Cantidad de Docentes");

                foreach (var nivel in datos.DocentesPorNivel)
                {
                    tabla.AddCell(nivel.Key);
                    tabla.AddCell(nivel.Value.ToString());
                }

                document.Add(tabla);
            }

            return stream.ToArray();
        }

        private byte[] GenerarPdfActividadPeriodo(ReporteActividadPeriodoDTO datos)
        {
            using var stream = new MemoryStream();
            using var writer = new PdfWriter(stream);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf);

            document.Add(new Paragraph("REPORTE DE ACTIVIDAD POR PERÍODO")
                .SetFontSize(18)
                .SetFontColor(ColorPrimario)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBold());

            document.Add(new Paragraph($"Período {datos.Periodo}: {datos.FechaInicio:dd/MM/yyyy} - {datos.FechaFin:dd/MM/yyyy}")
                .SetFontSize(12)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(20));

            // Resumen general
            var tablaResumen = new Table(4).UseAllAvailableWidth();
            tablaResumen.AddHeaderCell("Solicitudes Nuevas");
            tablaResumen.AddHeaderCell("Solicitudes Resueltas");
            tablaResumen.AddHeaderCell("Aprobadas");
            tablaResumen.AddHeaderCell("Rechazadas");

            tablaResumen.AddCell(datos.TotalSolicitudesNuevas.ToString());
            tablaResumen.AddCell(datos.TotalSolicitudesResueltas.ToString());
            tablaResumen.AddCell(datos.TotalAprobadas.ToString());
            tablaResumen.AddCell(datos.TotalRechazadas.ToString());

            document.Add(tablaResumen);

            return stream.ToArray();
        }

        private byte[] GenerarPdfConsolidado(ReporteProcesosPorEstadoDTO procesos, ReporteAscensosPorFacultadDTO facultades, 
            ReporteTiempoResolucionDTO tiempos, ReporteDistribucionDocentesDTO distribucion)
        {
            using var stream = new MemoryStream();
            using var writer = new PdfWriter(stream);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf);

            document.Add(new Paragraph("REPORTE CONSOLIDADO DE GESTIÓN")
                .SetFontSize(20)
                .SetFontColor(ColorPrimario)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBold());

            document.Add(new Paragraph($"Fecha de Generación: {DateTime.Now:dd/MM/yyyy}")
                .SetFontSize(12)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(30));

            // Resumen ejecutivo
            document.Add(new Paragraph("RESUMEN EJECUTIVO")
                .SetFontSize(16)
                .SetFontColor(ColorPrimario)
                .SetBold());

            var tablaEjecutiva = new Table(2).UseAllAvailableWidth();
            tablaEjecutiva.AddCell("Total de Docentes");
            tablaEjecutiva.AddCell(distribucion.TotalDocentes.ToString());
            tablaEjecutiva.AddCell("Procesos Pendientes");
            tablaEjecutiva.AddCell(procesos.TotalPendientes.ToString());
            tablaEjecutiva.AddCell("Procesos Aprobados");
            tablaEjecutiva.AddCell(procesos.TotalAprobados.ToString());
            tablaEjecutiva.AddCell("Tiempo Promedio Resolución");
            tablaEjecutiva.AddCell($"{tiempos.TiempoPromedioTotalDias} días");

            document.Add(tablaEjecutiva);

            return stream.ToArray();
        }

        #endregion

        #region Generación de HTML

        private string GenerarHtmlProcesosPorEstado(ReporteProcesosPorEstadoDTO datos)
        {
            var html = new StringBuilder();
            
            html.Append(@"
                <div style='font-family: Arial, sans-serif; margin: 20px;'>
                    <h2 style='color: #8a1538; text-align: center; margin-bottom: 20px;'>
                        REPORTE DE PROCESOS POR ESTADO
                    </h2>
                    <p style='text-align: center; color: #666; margin-bottom: 30px;'>
                        Período: " + datos.FechaInicio.ToString("dd/MM/yyyy") + " - " + datos.FechaFin.ToString("dd/MM/yyyy") + @"
                    </p>");

            // Resumen en cards
            html.Append(@"
                <div style='display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 15px; margin-bottom: 30px;'>
                    <div style='background: #f8f9fa; border-left: 4px solid #ffc107; padding: 15px; border-radius: 5px;'>
                        <h4 style='margin: 0; color: #8a1538;'>Pendientes</h4>
                        <p style='font-size: 24px; font-weight: bold; margin: 5px 0; color: #ffc107;'>" + datos.TotalPendientes + @"</p>
                    </div>
                    <div style='background: #f8f9fa; border-left: 4px solid #28a745; padding: 15px; border-radius: 5px;'>
                        <h4 style='margin: 0; color: #8a1538;'>Aprobados</h4>
                        <p style='font-size: 24px; font-weight: bold; margin: 5px 0; color: #28a745;'>" + datos.TotalAprobados + @"</p>
                    </div>
                    <div style='background: #f8f9fa; border-left: 4px solid #dc3545; padding: 15px; border-radius: 5px;'>
                        <h4 style='margin: 0; color: #8a1538;'>Rechazados</h4>
                        <p style='font-size: 24px; font-weight: bold; margin: 5px 0; color: #dc3545;'>" + datos.TotalRechazados + @"</p>
                    </div>
                    <div style='background: #f8f9fa; border-left: 4px solid #007bff; padding: 15px; border-radius: 5px;'>
                        <h4 style='margin: 0; color: #8a1538;'>En Revisión</h4>
                        <p style='font-size: 24px; font-weight: bold; margin: 5px 0; color: #007bff;'>" + datos.TotalEnRevision + @"</p>
                    </div>
                </div>");

            // Tabla de detalles
            html.Append(@"
                <h3 style='color: #8a1538; margin-top: 30px;'>Detalle de Procesos</h3>
                <div style='overflow-x: auto;'>
                    <table style='width: 100%; border-collapse: collapse; margin-top: 10px;'>
                        <thead>
                            <tr style='background-color: #8a1538; color: white;'>
                                <th style='padding: 12px; text-align: left; border: 1px solid #ddd;'>Docente</th>
                                <th style='padding: 12px; text-align: left; border: 1px solid #ddd;'>Cédula</th>
                                <th style='padding: 12px; text-align: left; border: 1px solid #ddd;'>Nivel Actual</th>
                                <th style='padding: 12px; text-align: left; border: 1px solid #ddd;'>Nivel Solicitado</th>
                                <th style='padding: 12px; text-align: left; border: 1px solid #ddd;'>Fecha Solicitud</th>
                                <th style='padding: 12px; text-align: left; border: 1px solid #ddd;'>Estado</th>
                                <th style='padding: 12px; text-align: left; border: 1px solid #ddd;'>Días Proceso</th>
                            </tr>
                        </thead>
                        <tbody>");

            foreach (var proceso in datos.DetallesProcesos.OrderBy(p => p.Estado).ThenBy(p => p.FechaSolicitud))
            {
                var colorEstado = proceso.Estado switch
                {
                    "Aprobado" => "#28a745",
                    "Rechazado" => "#dc3545",
                    "Pendiente" => "#ffc107",
                    "En Revisión" => "#007bff",
                    _ => "#6c757d"
                };

                html.Append($@"
                    <tr style='border-bottom: 1px solid #ddd;'>
                        <td style='padding: 10px; border: 1px solid #ddd;'>{proceso.DocenteNombre}</td>
                        <td style='padding: 10px; border: 1px solid #ddd;'>{proceso.DocenteCedula}</td>
                        <td style='padding: 10px; border: 1px solid #ddd;'>{proceso.NivelActual}</td>
                        <td style='padding: 10px; border: 1px solid #ddd;'>{proceso.NivelSolicitado}</td>
                        <td style='padding: 10px; border: 1px solid #ddd;'>{proceso.FechaSolicitud:dd/MM/yyyy}</td>
                        <td style='padding: 10px; border: 1px solid #ddd;'>
                            <span style='background-color: {colorEstado}; color: white; padding: 4px 8px; border-radius: 4px; font-size: 12px;'>
                                {proceso.Estado}
                            </span>
                        </td>
                        <td style='padding: 10px; border: 1px solid #ddd;'>{proceso.DiasProceso}</td>
                    </tr>");
            }

            html.Append(@"
                        </tbody>
                    </table>
                </div>
            </div>");

            return html.ToString();
        }

        // Métodos similares para generar HTML de otros reportes
        private string GenerarHtmlAscensosPorFacultad(ReporteAscensosPorFacultadDTO datos)
        {
            var html = new StringBuilder();
            
            html.Append(@"
                <div style='font-family: Arial, sans-serif; margin: 20px;'>
                    <h2 style='color: #8a1538; text-align: center; margin-bottom: 20px;'>
                        REPORTE DE ASCENSOS POR FACULTAD
                    </h2>
                    <p style='text-align: center; color: #666; margin-bottom: 30px;'>
                        Período: " + datos.FechaInicio.ToString("dd/MM/yyyy") + " - " + datos.FechaFin.ToString("dd/MM/yyyy") + @"
                    </p>");

            // Resumen general
            html.Append(@"
                <div style='display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 15px; margin-bottom: 30px;'>
                    <div style='background: #f8f9fa; border-left: 4px solid #007bff; padding: 15px; border-radius: 5px;'>
                        <h4 style='margin: 0; color: #8a1538;'>Total Solicitudes</h4>
                        <p style='font-size: 24px; font-weight: bold; margin: 5px 0; color: #007bff;'>" + datos.TotalSolicitudes + @"</p>
                    </div>
                    <div style='background: #f8f9fa; border-left: 4px solid #28a745; padding: 15px; border-radius: 5px;'>
                        <h4 style='margin: 0; color: #8a1538;'>Total Aprobados</h4>
                        <p style='font-size: 24px; font-weight: bold; margin: 5px 0; color: #28a745;'>" + datos.TotalAprobados + @"</p>
                    </div>
                    <div style='background: #f8f9fa; border-left: 4px solid #dc3545; padding: 15px; border-radius: 5px;'>
                        <h4 style='margin: 0; color: #8a1538;'>Total Rechazados</h4>
                        <p style='font-size: 24px; font-weight: bold; margin: 5px 0; color: #dc3545;'>" + datos.TotalRechazados + @"</p>
                    </div>
                </div>");

            // Detalle por facultades
            html.Append(@"<h3 style='color: #8a1538; margin-top: 30px;'>Detalle por Facultades</h3>");

            foreach (var facultad in datos.Facultades)
            {
                html.Append($@"
                    <div style='margin-bottom: 30px; border: 1px solid #ddd; border-radius: 8px; overflow: hidden;'>
                        <div style='background-color: #8a1538; color: white; padding: 15px;'>
                            <h4 style='margin: 0;'>{facultad.Nombre}</h4>
                        </div>
                        <div style='padding: 15px;'>
                            <div style='display: grid; grid-template-columns: repeat(auto-fit, minmax(150px, 1fr)); gap: 10px; margin-bottom: 15px;'>
                                <div style='text-align: center; padding: 10px; background: #f8f9fa; border-radius: 5px;'>
                                    <strong style='color: #8a1538;'>Total</strong><br>
                                    <span style='font-size: 18px; color: #007bff;'>{facultad.TotalSolicitudes}</span>
                                </div>
                                <div style='text-align: center; padding: 10px; background: #f8f9fa; border-radius: 5px;'>
                                    <strong style='color: #8a1538;'>Pendientes</strong><br>
                                    <span style='font-size: 18px; color: #ffc107;'>{facultad.SolicitudesPendientes}</span>
                                </div>
                                <div style='text-align: center; padding: 10px; background: #f8f9fa; border-radius: 5px;'>
                                    <strong style='color: #8a1538;'>Aprobadas</strong><br>
                                    <span style='font-size: 18px; color: #28a745;'>{facultad.SolicitudesAprobadas}</span>
                                </div>
                                <div style='text-align: center; padding: 10px; background: #f8f9fa; border-radius: 5px;'>
                                    <strong style='color: #8a1538;'>Rechazadas</strong><br>
                                    <span style='font-size: 18px; color: #dc3545;'>{facultad.SolicitudesRechazadas}</span>
                                </div>
                            </div>");

                if (facultad.Departamentos.Any())
                {
                    html.Append(@"
                        <h5 style='color: #8a1538; margin-top: 20px;'>Departamentos:</h5>
                        <div style='overflow-x: auto;'>
                            <table style='width: 100%; border-collapse: collapse;'>
                                <thead>
                                    <tr style='background-color: #f8f9fa;'>
                                        <th style='padding: 8px; text-align: left; border: 1px solid #ddd;'>Departamento</th>
                                        <th style='padding: 8px; text-align: center; border: 1px solid #ddd;'>Total</th>
                                        <th style='padding: 8px; text-align: center; border: 1px solid #ddd;'>Pendientes</th>
                                        <th style='padding: 8px; text-align: center; border: 1px solid #ddd;'>Aprobadas</th>
                                        <th style='padding: 8px; text-align: center; border: 1px solid #ddd;'>Rechazadas</th>
                                    </tr>
                                </thead>
                                <tbody>");

                    foreach (var departamento in facultad.Departamentos)
                    {
                        html.Append($@"
                            <tr>
                                <td style='padding: 8px; border: 1px solid #ddd;'>{departamento.Nombre}</td>
                                <td style='padding: 8px; text-align: center; border: 1px solid #ddd;'>{departamento.TotalSolicitudes}</td>
                                <td style='padding: 8px; text-align: center; border: 1px solid #ddd;'>{departamento.SolicitudesPendientes}</td>
                                <td style='padding: 8px; text-align: center; border: 1px solid #ddd;'>{departamento.SolicitudesAprobadas}</td>
                                <td style='padding: 8px; text-align: center; border: 1px solid #ddd;'>{departamento.SolicitudesRechazadas}</td>
                            </tr>");
                    }

                    html.Append(@"
                                </tbody>
                            </table>
                        </div>");
                }

                html.Append("</div></div>");
            }

            html.Append("</div>");
            return html.ToString();
        }

        // Métodos HTML para otros reportes
        private string GenerarHtmlTiempoResolucion(ReporteTiempoResolucionDTO datos)
        {
            var html = new StringBuilder();
            
            html.Append(@"
                <div style='font-family: Arial, sans-serif; margin: 20px;'>
                    <h2 style='color: #8a1538; text-align: center; margin-bottom: 20px;'>
                        REPORTE DE TIEMPO DE RESOLUCIÓN
                    </h2>
                    <p style='text-align: center; color: #666; margin-bottom: 30px;'>
                        Período: " + datos.FechaInicio.ToString("dd/MM/yyyy") + " - " + datos.FechaFin.ToString("dd/MM/yyyy") + @"
                    </p>
                    <div style='text-align: center; background: #f8f9fa; border-left: 4px solid #8a1538; padding: 20px; margin-bottom: 30px;'>
                        <h3 style='margin: 0; color: #8a1538;'>Tiempo Promedio General</h3>
                        <p style='font-size: 36px; font-weight: bold; margin: 10px 0; color: #8a1538;'>" + datos.TiempoPromedioTotalDias + @" días</p>
                    </div>");

            if (datos.TiemposPorFacultad.Any())
            {
                html.Append(@"
                    <h3 style='color: #8a1538; margin-top: 30px;'>Tiempos por Facultad</h3>
                    <div style='overflow-x: auto;'>
                        <table style='width: 100%; border-collapse: collapse;'>
                            <thead>
                                <tr style='background-color: #8a1538; color: white;'>
                                    <th style='padding: 12px; text-align: left; border: 1px solid #ddd;'>Facultad</th>
                                    <th style='padding: 12px; text-align: center; border: 1px solid #ddd;'>Tiempo Promedio (días)</th>
                                    <th style='padding: 12px; text-align: center; border: 1px solid #ddd;'>Total Procesos</th>
                                </tr>
                            </thead>
                            <tbody>");

                foreach (var tiempo in datos.TiemposPorFacultad)
                {
                    html.Append($@"
                        <tr>
                            <td style='padding: 10px; border: 1px solid #ddd;'>{tiempo.Facultad}</td>
                            <td style='padding: 10px; text-align: center; border: 1px solid #ddd;'>{tiempo.TiempoPromedioDias}</td>
                            <td style='padding: 10px; text-align: center; border: 1px solid #ddd;'>{tiempo.TotalProcesos}</td>
                        </tr>");
                }

                html.Append(@"
                            </tbody>
                        </table>
                    </div>");
            }

            html.Append("</div>");
            return html.ToString();
        }

        private string GenerarHtmlDistribucionDocentes(ReporteDistribucionDocentesDTO datos)
        {
            var html = new StringBuilder();
            
            html.Append(@"
                <div style='font-family: Arial, sans-serif; margin: 20px;'>
                    <h2 style='color: #8a1538; text-align: center; margin-bottom: 20px;'>
                        REPORTE DE DISTRIBUCIÓN DE DOCENTES
                    </h2>
                    <p style='text-align: center; color: #666; margin-bottom: 30px;'>
                        Fecha de Generación: " + datos.FechaGeneracion.ToString("dd/MM/yyyy") + @"
                    </p>
                    <div style='text-align: center; background: #f8f9fa; border-left: 4px solid #8a1538; padding: 20px; margin-bottom: 30px;'>
                        <h3 style='margin: 0; color: #8a1538;'>Total de Docentes</h3>
                        <p style='font-size: 36px; font-weight: bold; margin: 10px 0; color: #8a1538;'>" + datos.TotalDocentes + @"</p>
                    </div>");

            if (datos.DocentesPorNivel.Any())
            {
                html.Append(@"
                    <h3 style='color: #8a1538; margin-top: 30px;'>Distribución por Nivel Académico</h3>
                    <div style='display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 15px; margin-bottom: 30px;'>");

                foreach (var nivel in datos.DocentesPorNivel)
                {
                    html.Append($@"
                        <div style='background: #f8f9fa; border-left: 4px solid #007bff; padding: 15px; border-radius: 5px; text-align: center;'>
                            <h4 style='margin: 0; color: #8a1538;'>{nivel.Key}</h4>
                            <p style='font-size: 24px; font-weight: bold; margin: 10px 0; color: #007bff;'>{nivel.Value}</p>
                        </div>");
                }

                html.Append("</div>");
            }

            html.Append("</div>");
            return html.ToString();
        }

        private string GenerarHtmlActividadPeriodo(ReporteActividadPeriodoDTO datos)
        {
            var html = new StringBuilder();
            
            html.Append(@"
                <div style='font-family: Arial, sans-serif; margin: 20px;'>
                    <h2 style='color: #8a1538; text-align: center; margin-bottom: 20px;'>
                        REPORTE DE ACTIVIDAD POR PERÍODO
                    </h2>
                    <p style='text-align: center; color: #666; margin-bottom: 30px;'>
                        Período " + datos.Periodo + ": " + datos.FechaInicio.ToString("dd/MM/yyyy") + " - " + datos.FechaFin.ToString("dd/MM/yyyy") + @"
                    </p>");

            // Resumen general
            html.Append(@"
                <div style='display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 15px; margin-bottom: 30px;'>
                    <div style='background: #f8f9fa; border-left: 4px solid #007bff; padding: 15px; border-radius: 5px;'>
                        <h4 style='margin: 0; color: #8a1538;'>Solicitudes Nuevas</h4>
                        <p style='font-size: 24px; font-weight: bold; margin: 5px 0; color: #007bff;'>" + datos.TotalSolicitudesNuevas + @"</p>
                    </div>
                    <div style='background: #f8f9fa; border-left: 4px solid #17a2b8; padding: 15px; border-radius: 5px;'>
                        <h4 style='margin: 0; color: #8a1538;'>Solicitudes Resueltas</h4>
                        <p style='font-size: 24px; font-weight: bold; margin: 5px 0; color: #17a2b8;'>" + datos.TotalSolicitudesResueltas + @"</p>
                    </div>
                    <div style='background: #f8f9fa; border-left: 4px solid #28a745; padding: 15px; border-radius: 5px;'>
                        <h4 style='margin: 0; color: #8a1538;'>Aprobadas</h4>
                        <p style='font-size: 24px; font-weight: bold; margin: 5px 0; color: #28a745;'>" + datos.TotalAprobadas + @"</p>
                    </div>
                    <div style='background: #f8f9fa; border-left: 4px solid #dc3545; padding: 15px; border-radius: 5px;'>
                        <h4 style='margin: 0; color: #8a1538;'>Rechazadas</h4>
                        <p style='font-size: 24px; font-weight: bold; margin: 5px 0; color: #dc3545;'>" + datos.TotalRechazadas + @"</p>
                    </div>
                </div>");

            html.Append("</div>");
            return html.ToString();
        }

        private string GenerarHtmlConsolidado(ReporteProcesosPorEstadoDTO procesos, ReporteAscensosPorFacultadDTO facultades, 
            ReporteTiempoResolucionDTO tiempos, ReporteDistribucionDocentesDTO distribucion)
        {
            var html = new StringBuilder();
            
            html.Append(@"
                <div style='font-family: Arial, sans-serif; margin: 20px;'>
                    <h2 style='color: #8a1538; text-align: center; margin-bottom: 20px;'>
                        REPORTE CONSOLIDADO DE GESTIÓN
                    </h2>
                    <p style='text-align: center; color: #666; margin-bottom: 30px;'>
                        Fecha de Generación: " + DateTime.Now.ToString("dd/MM/yyyy") + @"
                    </p>
                    
                    <h3 style='color: #8a1538; margin-top: 30px;'>Resumen Ejecutivo</h3>
                    <div style='display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 15px; margin-bottom: 30px;'>
                        <div style='background: #f8f9fa; border-left: 4px solid #8a1538; padding: 15px; border-radius: 5px;'>
                            <h4 style='margin: 0; color: #8a1538;'>Total Docentes</h4>
                            <p style='font-size: 24px; font-weight: bold; margin: 5px 0; color: #8a1538;'>" + distribucion.TotalDocentes + @"</p>
                        </div>
                        <div style='background: #f8f9fa; border-left: 4px solid #ffc107; padding: 15px; border-radius: 5px;'>
                            <h4 style='margin: 0; color: #8a1538;'>Procesos Pendientes</h4>
                            <p style='font-size: 24px; font-weight: bold; margin: 5px 0; color: #ffc107;'>" + procesos.TotalPendientes + @"</p>
                        </div>
                        <div style='background: #f8f9fa; border-left: 4px solid #28a745; padding: 15px; border-radius: 5px;'>
                            <h4 style='margin: 0; color: #8a1538;'>Procesos Aprobados</h4>
                            <p style='font-size: 24px; font-weight: bold; margin: 5px 0; color: #28a745;'>" + procesos.TotalAprobados + @"</p>
                        </div>
                        <div style='background: #f8f9fa; border-left: 4px solid #007bff; padding: 15px; border-radius: 5px;'>
                            <h4 style='margin: 0; color: #8a1538;'>Tiempo Promedio</h4>
                            <p style='font-size: 24px; font-weight: bold; margin: 5px 0; color: #007bff;'>" + tiempos.TiempoPromedioTotalDias + @" días</p>
                        </div>
                    </div>
                </div>");

            return html.ToString();
        }

        #endregion
    }
}
