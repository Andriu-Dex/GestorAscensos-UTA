using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGA.Application.DTOs.Admin;
using SGA.Application.Interfaces;
using SGA.Domain.Entities;
using SGA.Domain.Enums;
using System.Text;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.IO.Font.Constants;

namespace SGA.Application.Services;

public class AdminDocenteService : IAdminDocenteService
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<AdminDocenteService> _logger;

    public AdminDocenteService(
        IApplicationDbContext context,
        ILogger<AdminDocenteService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<DocenteAdminDto>> GetAllDocentesAsync()
    {
        try
        {
            var docentes = await _context.Docentes
                .Include(d => d.Departamento)
                .Include(d => d.SolicitudesAscenso)
                .Include(d => d.Usuario) // Incluir información del usuario
                .Where(d => d.Usuario != null && d.Usuario.Rol == RolUsuario.Docente) // Filtrar solo docentes
                .OrderBy(d => d.Apellidos)
                .ThenBy(d => d.Nombres)
                .ToListAsync();

            var docentesDtos = new List<DocenteAdminDto>();

            foreach (var docente in docentes)
            {
                var dto = new DocenteAdminDto
                {
                    Id = docente.Id,
                    Cedula = docente.Cedula,
                    Nombres = docente.Nombres,
                    Apellidos = docente.Apellidos,
                    Email = docente.Email,
                    Facultad = docente.FacultadParaReporte,
                    Departamento = docente.DepartamentoParaReporte,
                    NivelActual = (int)docente.NivelActual,
                    FechaInicioNivelActual = docente.FechaInicioNivelActual,
                    TiempoEnNivelAnios = CalcularTiempoEnNivelAnios(docente.FechaInicioNivelActual),
                    Celular = docente.Celular,
                    FechaNombramiento = docente.FechaNombramiento,
                    FechaUltimoAscenso = docente.FechaUltimoAscenso,
                    PromedioEvaluaciones = (double)(docente.PromedioEvaluaciones ?? 0),
                    HorasCapacitacion = docente.HorasCapacitacion ?? 0,
                    NumeroObrasAcademicas = docente.NumeroObrasAcademicas ?? 0,
                    MesesInvestigacion = docente.MesesInvestigacion ?? 0,
                    SolicitudesPendientes = docente.SolicitudesAscenso.Count(s => s.Estado == EstadoSolicitud.Pendiente),
                    SolicitudesAprobadas = docente.SolicitudesAscenso.Count(s => s.Estado == EstadoSolicitud.Aprobada),
                    SolicitudesRechazadas = docente.SolicitudesAscenso.Count(s => s.Estado == EstadoSolicitud.Rechazada),
                    PuedeAscender = docente.PuedeAscender,
                    SiguienteNivel = docente.SiguienteNivel?.ToString()
                };

                docentesDtos.Add(dto);
            }

            return docentesDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los docentes");
            throw;
        }
    }

    public async Task<DocenteDetalleAdminDto?> GetDocenteDetalleAsync(Guid docenteId)
    {
        try
        {
            var docente = await _context.Docentes
                .Include(d => d.Departamento)
                .Include(d => d.SolicitudesAscenso)
                .Include(d => d.ObrasAcademicas)
                .FirstOrDefaultAsync(d => d.Id == docenteId);

            if (docente == null)
                return null;

            var dto = new DocenteDetalleAdminDto
            {
                Id = docente.Id,
                Cedula = docente.Cedula,
                Nombres = docente.Nombres,
                Apellidos = docente.Apellidos,
                Email = docente.Email,
                Facultad = docente.FacultadParaReporte,
                Departamento = docente.DepartamentoParaReporte,
                NivelActual = (int)docente.NivelActual,
                FechaInicioNivelActual = docente.FechaInicioNivelActual,
                TiempoEnNivelAnios = CalcularTiempoEnNivelAnios(docente.FechaInicioNivelActual),
                Celular = docente.Celular,
                FechaNombramiento = docente.FechaNombramiento,
                FechaUltimoAscenso = docente.FechaUltimoAscenso,
                PromedioEvaluaciones = (double)(docente.PromedioEvaluaciones ?? 0),
                HorasCapacitacion = docente.HorasCapacitacion ?? 0,
                NumeroObrasAcademicas = docente.NumeroObrasAcademicas ?? 0,
                MesesInvestigacion = docente.MesesInvestigacion ?? 0,
                FechaUltimaImportacion = docente.FechaUltimaImportacion,
                FotoPerfilBase64 = docente.FotoPerfilBase64,
                SolicitudesPendientes = docente.SolicitudesAscenso.Count(s => s.Estado == EstadoSolicitud.Pendiente),
                SolicitudesAprobadas = docente.SolicitudesAscenso.Count(s => s.Estado == EstadoSolicitud.Aprobada),
                SolicitudesRechazadas = docente.SolicitudesAscenso.Count(s => s.Estado == EstadoSolicitud.Rechazada),
                PuedeAscender = docente.PuedeAscender,
                SiguienteNivel = docente.SiguienteNivel?.ToString(),
                
                SolicitudesRecientes = docente.SolicitudesAscenso
                    .OrderByDescending(s => s.FechaSolicitud)
                    .Take(5)
                    .Select(s => new SolicitudResumenDto
                    {
                        Id = s.Id,
                        NivelActual = s.NivelActual.ToString(),
                        NivelSolicitado = s.NivelSolicitado.ToString(),
                        Estado = s.Estado,
                        FechaSolicitud = s.FechaSolicitud,
                        FechaRespuesta = s.FechaAprobacion,
                        Observaciones = s.Observaciones,
                        NombreRevisor = "Sistema"
                    }).ToList(),

                ObrasRecientes = docente.ObrasAcademicas
                    .OrderByDescending(o => o.FechaCreacion)
                    .Take(5)
                    .Select(o => new ObraAcademicaResumenDto
                    {
                        Id = o.Id,
                        Titulo = o.Titulo,
                        Tipo = "Obra Académica",
                        FechaPublicacion = o.FechaPublicacion,
                        Editorial = o.Editorial,
                        Revista = o.Revista,
                        Autores = o.Autores
                    }).ToList()
            };

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener detalle del docente {DocenteId}", docenteId);
            throw;
        }
    }

    public async Task<List<SolicitudResumenDto>> GetSolicitudesDocenteAsync(Guid docenteId)
    {
        try
        {
            var solicitudes = await _context.SolicitudesAscenso
                .Where(s => s.DocenteId == docenteId)
                .OrderByDescending(s => s.FechaSolicitud)
                .Select(s => new SolicitudResumenDto
                {
                    Id = s.Id,
                    NivelActual = s.NivelActual.ToString(),
                    NivelSolicitado = s.NivelSolicitado.ToString(),
                    Estado = s.Estado,
                    FechaSolicitud = s.FechaSolicitud,
                    FechaRespuesta = s.FechaAprobacion,
                    Observaciones = s.Observaciones,
                    NombreRevisor = "Sistema"
                })
                .ToListAsync();

            return solicitudes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener solicitudes del docente {DocenteId}", docenteId);
            throw;
        }
    }

    public async Task<byte[]> GenerarReporteDocenteAsync(Guid docenteId)
    {
        try
        {
            var docente = await GetDocenteDetalleAsync(docenteId);
            if (docente == null)
                throw new InvalidOperationException("Docente no encontrado");

            // Generar un PDF básico pero válido
            var pdfContent = GenerarPdfSimpleReporte(docente);
            
            return pdfContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar reporte del docente {DocenteId}", docenteId);
            throw;
        }
    }

    public async Task<bool> ActualizarNivelDocenteAsync(Guid docenteId, ActualizarNivelDocenteDto dto, string adminEmail)
    {
        try
        {
            var docente = await _context.Docentes.FirstOrDefaultAsync(d => d.Id == docenteId);
            if (docente == null)
                return false;

            var fechaEfectiva = dto.FechaEfectiva ?? DateTime.Now;

            // Actualizar el nivel del docente
            docente.NivelActual = (NivelTitular)dto.NuevoNivel;
            docente.FechaInicioNivelActual = fechaEfectiva;
            docente.FechaUltimoAscenso = fechaEfectiva;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Nivel del docente {DocenteId} actualizado a {NuevoNivel} por {AdminEmail}", 
                docenteId, dto.NuevoNivel, adminEmail);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar nivel del docente {DocenteId}", docenteId);
            throw;
        }
    }

    public async Task<EstadisticasDocentesDto> GetEstadisticasDocentesAsync()
    {
        try
        {
            var docentes = await _context.Docentes
                .Include(d => d.Departamento)
                .Include(d => d.SolicitudesAscenso)
                .Include(d => d.Usuario) // Incluir información del usuario
                .Where(d => d.Usuario != null && d.Usuario.Rol == RolUsuario.Docente) // Filtrar solo docentes
                .ToListAsync();

            var estadisticas = new EstadisticasDocentesDto
            {
                TotalDocentes = docentes.Count,
                DocentesPorNivel = docentes
                    .GroupBy(d => (int)d.NivelActual)
                    .ToDictionary(g => g.Key, g => g.Count()),
                DocentesPorFacultad = docentes
                    .GroupBy(d => d.FacultadParaReporte)
                    .ToDictionary(g => g.Key, g => g.Count()),
                SolicitudesPendientes = docentes
                    .SelectMany(d => d.SolicitudesAscenso)
                    .Count(s => s.Estado == EstadoSolicitud.Pendiente),
                SolicitudesAprobadas = docentes
                    .SelectMany(d => d.SolicitudesAscenso)
                    .Count(s => s.Estado == EstadoSolicitud.Aprobada),
                SolicitudesRechazadas = docentes
                    .SelectMany(d => d.SolicitudesAscenso)
                    .Count(s => s.Estado == EstadoSolicitud.Rechazada),
                PromedioTiempoEnNivel = docentes
                    .Select(d => CalcularTiempoEnNivelAnios(d.FechaInicioNivelActual))
                    .Average(),
                DocentesAptosPorAscenso = docentes
                    .Count(d => d.PuedeAscender)
            };

            return estadisticas;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estadísticas de docentes");
            throw;
        }
    }

    public async Task<List<string>> GetFacultadesAsync()
    {
        try
        {
            var facultades = await _context.Facultades
                .OrderBy(f => f.Nombre)
                .Select(f => f.Nombre)
                .ToListAsync();

            return facultades;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener facultades");
            throw;
        }
    }

    #region Métodos privados

    private static double CalcularTiempoEnNivelAnios(DateTime fechaInicioNivel)
    {
        var fechaActual = DateTime.Now;
        var diferencia = fechaActual - fechaInicioNivel;
        return Math.Round(diferencia.TotalDays / 365.25, 2);
    }

    private static string GenerarHtmlReporte(DocenteDetalleAdminDto docente)
    {
        var html = new StringBuilder();
        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html>");
        html.AppendLine("<head>");
        html.AppendLine("<meta charset='utf-8'>");
        html.AppendLine("<title>Reporte de Docente</title>");
        html.AppendLine("<style>");
        html.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; }");
        html.AppendLine("h1 { color: #8a1538; border-bottom: 2px solid #8a1538; }");
        html.AppendLine("h2 { color: #6d1029; }");
        html.AppendLine("table { width: 100%; border-collapse: collapse; margin: 20px 0; }");
        html.AppendLine("th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }");
        html.AppendLine("th { background-color: #8a1538; color: white; }");
        html.AppendLine(".section { margin: 20px 0; }");
        html.AppendLine(".badge { background-color: #8a1538; color: white; padding: 4px 8px; border-radius: 4px; }");
        html.AppendLine("</style>");
        html.AppendLine("</head>");
        html.AppendLine("<body>");
        
        html.AppendLine("<h1>Reporte de Docente</h1>");
        html.AppendLine($"<h2>{docente.Nombres} {docente.Apellidos}</h2>");
        
        html.AppendLine("<div class='section'>");
        html.AppendLine("<h3>Información Personal</h3>");
        html.AppendLine("<table>");
        html.AppendLine($"<tr><th>Cédula</th><td>{docente.Cedula}</td></tr>");
        html.AppendLine($"<tr><th>Email</th><td>{docente.Email}</td></tr>");
        html.AppendLine($"<tr><th>Teléfono</th><td>{docente.Celular ?? "No registrado"}</td></tr>");
        html.AppendLine($"<tr><th>Facultad</th><td>{docente.Facultad ?? "No asignada"}</td></tr>");
        html.AppendLine($"<tr><th>Departamento</th><td>{docente.Departamento ?? "No asignado"}</td></tr>");
        html.AppendLine("</table>");
        html.AppendLine("</div>");
        
        html.AppendLine("<div class='section'>");
        html.AppendLine("<h3>Información Académica</h3>");
        html.AppendLine("<table>");
        html.AppendLine($"<tr><th>Nivel Actual</th><td><span class='badge'>Titular {docente.NivelActual}</span></td></tr>");
        html.AppendLine($"<tr><th>Tiempo en Nivel</th><td>{docente.TiempoEnNivelAnios:F2} años</td></tr>");
        html.AppendLine($"<tr><th>Fecha Nombramiento</th><td>{docente.FechaNombramiento?.ToString("dd/MM/yyyy") ?? "No registrada"}</td></tr>");
        html.AppendLine($"<tr><th>Último Ascenso</th><td>{docente.FechaUltimoAscenso?.ToString("dd/MM/yyyy") ?? "No registrado"}</td></tr>");
        html.AppendLine($"<tr><th>Promedio Evaluaciones</th><td>{docente.PromedioEvaluaciones:F2}</td></tr>");
        html.AppendLine($"<tr><th>Horas Capacitación</th><td>{docente.HorasCapacitacion}</td></tr>");
        html.AppendLine($"<tr><th>Obras Académicas</th><td>{docente.NumeroObrasAcademicas}</td></tr>");
        html.AppendLine($"<tr><th>Meses Investigación</th><td>{docente.MesesInvestigacion}</td></tr>");
        html.AppendLine($"<tr><th>Puede Ascender</th><td>{(docente.PuedeAscender ? "Sí" : "No")}</td></tr>");
        if (!string.IsNullOrEmpty(docente.SiguienteNivel))
        {
            html.AppendLine($"<tr><th>Siguiente Nivel</th><td>{docente.SiguienteNivel}</td></tr>");
        }
        html.AppendLine("</table>");
        html.AppendLine("</div>");
        
        html.AppendLine("<div class='section'>");
        html.AppendLine("<h3>Estadísticas de Solicitudes</h3>");
        html.AppendLine("<table>");
        html.AppendLine($"<tr><th>Solicitudes Pendientes</th><td>{docente.SolicitudesPendientes}</td></tr>");
        html.AppendLine($"<tr><th>Solicitudes Aprobadas</th><td>{docente.SolicitudesAprobadas}</td></tr>");
        html.AppendLine($"<tr><th>Solicitudes Rechazadas</th><td>{docente.SolicitudesRechazadas}</td></tr>");
        html.AppendLine($"<tr><th>Total Solicitudes</th><td>{docente.SolicitudesPendientes + docente.SolicitudesAprobadas + docente.SolicitudesRechazadas}</td></tr>");
        html.AppendLine("</table>");
        html.AppendLine("</div>");
        
        html.AppendLine($"<div class='section'>");
        html.AppendLine($"<p><small>Reporte generado el {DateTime.Now:dd/MM/yyyy HH:mm}</small></p>");
        html.AppendLine("</div>");
        
        html.AppendLine("</body>");
        html.AppendLine("</html>");
        
        return html.ToString();
    }

    /// <summary>
    /// Genera un PDF real usando iText7 para el reporte del docente
    /// </summary>
    private byte[] GenerarPdfSimpleReporte(DocenteDetalleAdminDto docente)
    {
        using var stream = new MemoryStream();
        
        // Crear documento PDF
        using var writer = new PdfWriter(stream);
        using var pdf = new PdfDocument(writer);
        using var document = new Document(pdf);
        
        // Configurar fuentes
        var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
        var regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
        
        // Color corporativo
        var primaryColor = new DeviceRgb(138, 21, 56);
        
        // Título principal
        var titulo = new Paragraph("REPORTE DEL DOCENTE")
            .SetFont(boldFont)
            .SetFontSize(18)
            .SetFontColor(primaryColor)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetMarginBottom(20);
        document.Add(titulo);
        
        // Información personal
        var infoPersonal = new Paragraph("INFORMACIÓN PERSONAL")
            .SetFont(boldFont)
            .SetFontSize(14)
            .SetFontColor(primaryColor)
            .SetMarginBottom(10);
        document.Add(infoPersonal);
        
        // Crear tabla para información personal
        var tablaPersonal = new Table(2).UseAllAvailableWidth();
        tablaPersonal.AddCell(new Cell().Add(new Paragraph("Nombre:").SetFont(boldFont)));
        tablaPersonal.AddCell(new Cell().Add(new Paragraph($"{docente.Nombres} {docente.Apellidos}").SetFont(regularFont)));
        tablaPersonal.AddCell(new Cell().Add(new Paragraph("Cédula:").SetFont(boldFont)));
        tablaPersonal.AddCell(new Cell().Add(new Paragraph(docente.Cedula).SetFont(regularFont)));
        tablaPersonal.AddCell(new Cell().Add(new Paragraph("Email:").SetFont(boldFont)));
        tablaPersonal.AddCell(new Cell().Add(new Paragraph(docente.Email).SetFont(regularFont)));
        tablaPersonal.AddCell(new Cell().Add(new Paragraph("Facultad:").SetFont(boldFont)));
        tablaPersonal.AddCell(new Cell().Add(new Paragraph(docente.Facultad).SetFont(regularFont)));
        tablaPersonal.AddCell(new Cell().Add(new Paragraph("Departamento:").SetFont(boldFont)));
        tablaPersonal.AddCell(new Cell().Add(new Paragraph(docente.Departamento).SetFont(regularFont)));
        
        tablaPersonal.SetMarginBottom(20);
        document.Add(tablaPersonal);
        
        // Información académica
        var infoAcademica = new Paragraph("INFORMACIÓN ACADÉMICA")
            .SetFont(boldFont)
            .SetFontSize(14)
            .SetFontColor(primaryColor)
            .SetMarginBottom(10);
        document.Add(infoAcademica);
        
        var tablaAcademica = new Table(2).UseAllAvailableWidth();
        tablaAcademica.AddCell(new Cell().Add(new Paragraph("Nivel Actual:").SetFont(boldFont)));
        tablaAcademica.AddCell(new Cell().Add(new Paragraph($"Titular {docente.NivelActual}").SetFont(regularFont)));
        tablaAcademica.AddCell(new Cell().Add(new Paragraph("Fecha Inicio en Nivel:").SetFont(boldFont)));
        tablaAcademica.AddCell(new Cell().Add(new Paragraph(docente.FechaInicioNivelActual.ToString("dd/MM/yyyy")).SetFont(regularFont)));
        tablaAcademica.AddCell(new Cell().Add(new Paragraph("Tiempo en Nivel:").SetFont(boldFont)));
        tablaAcademica.AddCell(new Cell().Add(new Paragraph($"{docente.TiempoEnNivelAnios:F1} años").SetFont(regularFont)));
        tablaAcademica.AddCell(new Cell().Add(new Paragraph("Puede Ascender:").SetFont(boldFont)));
        tablaAcademica.AddCell(new Cell().Add(new Paragraph(docente.PuedeAscender ? "Sí" : "No").SetFont(regularFont)));
        
        if (!string.IsNullOrEmpty(docente.SiguienteNivel))
        {
            tablaAcademica.AddCell(new Cell().Add(new Paragraph("Siguiente Nivel:").SetFont(boldFont)));
            tablaAcademica.AddCell(new Cell().Add(new Paragraph(docente.SiguienteNivel).SetFont(regularFont)));
        }
        
        tablaAcademica.SetMarginBottom(20);
        document.Add(tablaAcademica);
        
        // Estadísticas de solicitudes
        var estadisticasSolicitudes = new Paragraph("ESTADÍSTICAS DE SOLICITUDES")
            .SetFont(boldFont)
            .SetFontSize(14)
            .SetFontColor(primaryColor)
            .SetMarginBottom(10);
        document.Add(estadisticasSolicitudes);
        
        var tablaSolicitudes = new Table(2).UseAllAvailableWidth();
        tablaSolicitudes.AddCell(new Cell().Add(new Paragraph("Solicitudes Pendientes:").SetFont(boldFont)));
        tablaSolicitudes.AddCell(new Cell().Add(new Paragraph(docente.SolicitudesPendientes.ToString()).SetFont(regularFont)));
        tablaSolicitudes.AddCell(new Cell().Add(new Paragraph("Solicitudes Aprobadas:").SetFont(boldFont)));
        tablaSolicitudes.AddCell(new Cell().Add(new Paragraph(docente.SolicitudesAprobadas.ToString()).SetFont(regularFont)));
        tablaSolicitudes.AddCell(new Cell().Add(new Paragraph("Solicitudes Rechazadas:").SetFont(boldFont)));
        tablaSolicitudes.AddCell(new Cell().Add(new Paragraph(docente.SolicitudesRechazadas.ToString()).SetFont(regularFont)));
        tablaSolicitudes.AddCell(new Cell().Add(new Paragraph("Total Solicitudes:").SetFont(boldFont)));
        tablaSolicitudes.AddCell(new Cell().Add(new Paragraph((docente.SolicitudesPendientes + docente.SolicitudesAprobadas + docente.SolicitudesRechazadas).ToString()).SetFont(regularFont)));
        
        tablaSolicitudes.SetMarginBottom(30);
        document.Add(tablaSolicitudes);
        
        // Pie de página
        var piePagina = new Paragraph($"Reporte generado el {DateTime.Now:dd/MM/yyyy HH:mm}")
            .SetFont(regularFont)
            .SetFontSize(10)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetFontColor(ColorConstants.GRAY);
        document.Add(piePagina);
        
        var sistema = new Paragraph("Sistema de Gestión de Ascensos - UTA")
            .SetFont(regularFont)
            .SetFontSize(10)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetFontColor(ColorConstants.GRAY);
        document.Add(sistema);
        
        document.Close();
        
        return stream.ToArray();
    }

    #endregion
}
