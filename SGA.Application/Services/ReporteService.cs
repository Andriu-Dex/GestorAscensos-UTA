using SGA.Application.Interfaces;
using SGA.Application.Interfaces.Repositories;
using System.Text;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using SGA.Domain.Enums;

namespace SGA.Application.Services;

public class ReporteService : IReporteService
{
    private readonly IDocenteRepository _docenteRepository;
    private readonly ISolicitudAscensoRepository _solicitudRepository;
    private readonly IDocumentoRepository _documentoRepository;

    public ReporteService(
        IDocenteRepository docenteRepository,
        ISolicitudAscensoRepository solicitudRepository,
        IDocumentoRepository documentoRepository)
    {
        _docenteRepository = docenteRepository;
        _solicitudRepository = solicitudRepository;
        _documentoRepository = documentoRepository;
    }
    public async Task<byte[]> GenerarHojaVidaAsync(Guid docenteId)
    {
        var docente = await _docenteRepository.GetByIdAsync(docenteId);
        if (docente == null)
            throw new ArgumentException("Docente no encontrado");

        using var memoryStream = new MemoryStream();
        using var writer = new PdfWriter(memoryStream);
        using var pdf = new PdfDocument(writer);
        var document = new Document(pdf);

        // Configuración de página
        document.SetMargins(36, 36, 36, 36);
        
        // Estilos
        var tituloEstilo = new iText.Layout.Style()
            .SetFontSize(20)
            .SetBold()
            .SetFontColor(new iText.Kernel.Colors.DeviceRgb(0, 51, 102))
            .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
            
        var subtituloEstilo = new iText.Layout.Style()
            .SetFontSize(14)
            .SetBold()
            .SetFontColor(new iText.Kernel.Colors.DeviceRgb(0, 102, 153))
            .SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(240, 240, 240))
            .SetPaddingLeft(5)
            .SetPaddingRight(5)
            .SetPaddingTop(3)
            .SetPaddingBottom(3);
            
        var etiquetaEstilo = new iText.Layout.Style()
            .SetBold()
            .SetFontColor(new iText.Kernel.Colors.DeviceRgb(68, 68, 68));
            
        var fechaEstilo = new iText.Layout.Style()
            .SetFontSize(10)
            .SetItalic()
            .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT);

        // Encabezado con logotipo (simulado con texto)
        var headerTable = new iText.Layout.Element.Table(new float[] { 1, 3 })
            .UseAllAvailableWidth();
            
        var cell1 = new iText.Layout.Element.Cell()
            .Add(new Paragraph("UTA").SetFontSize(36).SetBold())
            .SetBorder(iText.Layout.Borders.Border.NO_BORDER)
            .SetVerticalAlignment(iText.Layout.Properties.VerticalAlignment.MIDDLE);
            
        var cell2 = new iText.Layout.Element.Cell()
            .Add(new Paragraph("UNIVERSIDAD TÉCNICA DE AMBATO\nSISTEMA DE GESTIÓN DE ASCENSOS")
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT))
            .SetBorder(iText.Layout.Borders.Border.NO_BORDER)
            .SetVerticalAlignment(iText.Layout.Properties.VerticalAlignment.MIDDLE);
            
        headerTable.AddCell(cell1);
        headerTable.AddCell(cell2);
        document.Add(headerTable);
        
        // Línea separadora
        document.Add(new Paragraph(new string('_', 100))
            .SetFontColor(new iText.Kernel.Colors.DeviceRgb(0, 102, 153))
            .SetMarginTop(5)
            .SetMarginBottom(15));

        // Título
        document.Add(new Paragraph("HOJA DE VIDA ACADÉMICA").AddStyle(tituloEstilo));
        document.Add(new Paragraph("\n"));

        // Datos personales
        document.Add(new Paragraph("DATOS PERSONALES").AddStyle(subtituloEstilo));
        
        var datosPersonalesTable = new iText.Layout.Element.Table(new float[] { 1, 2 })
            .UseAllAvailableWidth()
            .SetMarginTop(10)
            .SetMarginBottom(10);
            
        // Fila 1: Nombres y Apellidos
        datosPersonalesTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph("Nombres:").AddStyle(etiquetaEstilo)).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
        datosPersonalesTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph(docente.Nombres)).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
        
        datosPersonalesTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph("Apellidos:").AddStyle(etiquetaEstilo)).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
        datosPersonalesTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph(docente.Apellidos)).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
        
        datosPersonalesTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph("Cédula:").AddStyle(etiquetaEstilo)).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
        datosPersonalesTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph(docente.Cedula)).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
        
        datosPersonalesTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph("Email:").AddStyle(etiquetaEstilo)).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
        datosPersonalesTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph(docente.Email)).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
        
        datosPersonalesTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph("Nivel Actual:").AddStyle(etiquetaEstilo)).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
        datosPersonalesTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph(docente.NivelActual.ToString())).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
        
        if (docente.FechaNombramiento.HasValue)
        {
            datosPersonalesTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph("Fecha Nombramiento:").AddStyle(etiquetaEstilo)).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
            datosPersonalesTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph(docente.FechaNombramiento.Value.ToString("dd/MM/yyyy"))).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
        }
        
        document.Add(datosPersonalesTable);

        // Datos académicos
        document.Add(new Paragraph("DATOS ACADÉMICOS").AddStyle(subtituloEstilo));
        
        var datosAcademicosTable = new iText.Layout.Element.Table(new float[] { 1, 2 })
            .UseAllAvailableWidth()
            .SetMarginTop(10)
            .SetMarginBottom(10);
        
        datosAcademicosTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph("Promedio Evaluaciones:").AddStyle(etiquetaEstilo)).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
        datosAcademicosTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph($"{docente.PromedioEvaluaciones ?? 0:F2}/100")).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
        
        datosAcademicosTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph("Horas Capacitación:").AddStyle(etiquetaEstilo)).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
        datosAcademicosTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph($"{docente.HorasCapacitacion ?? 0}")).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
        
        datosAcademicosTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph("Obras Académicas:").AddStyle(etiquetaEstilo)).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
        datosAcademicosTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph($"{docente.NumeroObrasAcademicas ?? 0}")).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
        
        datosAcademicosTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph("Meses en Investigación:").AddStyle(etiquetaEstilo)).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
        datosAcademicosTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph($"{docente.MesesInvestigacion ?? 0}")).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
        
        var tiempoEnNivel = DateTime.UtcNow - docente.FechaInicioNivelActual;
        datosAcademicosTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph("Tiempo en Nivel Actual:").AddStyle(etiquetaEstilo)).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
        datosAcademicosTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph($"{tiempoEnNivel.TotalDays / 365.25:F1} años")).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
        
        document.Add(datosAcademicosTable);
        
        // Historial de solicitudes
        var solicitudes = await _solicitudRepository.GetByDocenteIdAsync(docenteId);
        if (solicitudes.Any())
        {
            document.Add(new Paragraph("HISTORIAL DE SOLICITUDES DE ASCENSO").AddStyle(subtituloEstilo));
            
            var solicitudesTable = new iText.Layout.Element.Table(new float[] { 1, 1, 1, 1 })
                .UseAllAvailableWidth()
                .SetMarginTop(10)
                .SetMarginBottom(10);
                
            // Encabezados
            solicitudesTable.AddHeaderCell(new iText.Layout.Element.Cell().Add(new Paragraph("Fecha").SetBold()));
            solicitudesTable.AddHeaderCell(new iText.Layout.Element.Cell().Add(new Paragraph("Nivel Solicitado").SetBold()));
            solicitudesTable.AddHeaderCell(new iText.Layout.Element.Cell().Add(new Paragraph("Estado").SetBold()));
            solicitudesTable.AddHeaderCell(new iText.Layout.Element.Cell().Add(new Paragraph("Fecha Aprobación").SetBold()));
            
            foreach (var sol in solicitudes.OrderByDescending(s => s.FechaSolicitud))
            {
                solicitudesTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph(sol.FechaSolicitud.ToString("dd/MM/yyyy"))));
                solicitudesTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph(sol.NivelSolicitado.ToString())));
                
                // Colorear estado según valor
                var estadoCell = new iText.Layout.Element.Cell();
                var estadoParagraph = new Paragraph(sol.Estado.ToString());
                
                switch (sol.Estado)
                {
                    case EstadoSolicitud.Aprobada:
                        estadoParagraph.SetFontColor(new iText.Kernel.Colors.DeviceRgb(0, 128, 0));
                        break;
                    case EstadoSolicitud.Rechazada:
                        estadoParagraph.SetFontColor(new iText.Kernel.Colors.DeviceRgb(192, 0, 0));
                        break;
                    case EstadoSolicitud.EnProceso:
                        estadoParagraph.SetFontColor(new iText.Kernel.Colors.DeviceRgb(0, 0, 192));
                        break;
                }
                
                estadoCell.Add(estadoParagraph);
                solicitudesTable.AddCell(estadoCell);
                
                // Fecha de aprobación
                if (sol.FechaAprobacion.HasValue)
                    solicitudesTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph(sol.FechaAprobacion.Value.ToString("dd/MM/yyyy"))));
                else
                    solicitudesTable.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph("-")));
            }
            
            document.Add(solicitudesTable);
        }
        
        // Pie de página con fecha de generación
        document.Add(new Paragraph("\n"));
        document.Add(new Paragraph($"Documento generado el {DateTime.Now:dd/MM/yyyy HH:mm:ss}")
            .AddStyle(fechaEstilo));

        document.Close();
        return memoryStream.ToArray();
    }

    public async Task<byte[]> GenerarReporteSolicitudAsync(Guid solicitudId, Guid docenteId, bool esAdmin)
    {
        var solicitud = await _solicitudRepository.GetByIdAsync(solicitudId);
        if (solicitud == null)
            throw new ArgumentException("Solicitud no encontrada");

        // Verificar permisos
        if (!esAdmin && solicitud.DocenteId != docenteId)
            throw new UnauthorizedAccessException("No tiene permisos para ver este reporte");

        var docente = await _docenteRepository.GetByIdAsync(solicitud.DocenteId);
        if (docente == null)
            throw new ArgumentException("Docente no encontrado");

        var documentos = await _documentoRepository.GetBySolicitudIdAsync(solicitudId);
        
        using var memoryStream = new MemoryStream();
        using var writer = new PdfWriter(memoryStream);
        using var pdf = new PdfDocument(writer);
        var document = new Document(pdf);

        // Título
        document.Add(new Paragraph("REPORTE DE SOLICITUD DE ASCENSO")
            .SetFontSize(18)
            .SetBold()
            .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));
        
        document.Add(new Paragraph("\n"));

        // Datos de la solicitud
        document.Add(new Paragraph("DATOS DE LA SOLICITUD").SetFontSize(14).SetBold());
        document.Add(new Paragraph($"Docente: {docente.Nombres} {docente.Apellidos}"));
        document.Add(new Paragraph($"Cédula: {docente.Cedula}"));
        document.Add(new Paragraph($"Fecha Solicitud: {solicitud.FechaSolicitud:dd/MM/yyyy}"));
        document.Add(new Paragraph($"Nivel Actual: {solicitud.NivelActual}"));
        document.Add(new Paragraph($"Nivel Solicitado: {solicitud.NivelSolicitado}"));
        document.Add(new Paragraph($"Estado: {solicitud.Estado}"));
        
        if (solicitud.FechaAprobacion.HasValue)
            document.Add(new Paragraph($"Fecha Aprobación: {solicitud.FechaAprobacion.Value:dd/MM/yyyy}"));
        
        if (!string.IsNullOrEmpty(solicitud.MotivoRechazo))
            document.Add(new Paragraph($"Motivo Rechazo: {solicitud.MotivoRechazo}"));
        
        document.Add(new Paragraph("\n"));

        // Datos académicos
        document.Add(new Paragraph("DATOS ACADÉMICOS AL MOMENTO DE LA SOLICITUD").SetFontSize(14).SetBold());
        document.Add(new Paragraph($"Promedio Evaluaciones: {solicitud.PromedioEvaluaciones:F2}/100"));
        document.Add(new Paragraph($"Horas Capacitación: {solicitud.HorasCapacitacion}"));
        document.Add(new Paragraph($"Obras Académicas: {solicitud.NumeroObrasAcademicas}"));
        document.Add(new Paragraph($"Meses en Investigación: {solicitud.MesesInvestigacion}"));
        document.Add(new Paragraph($"Tiempo en Nivel: {solicitud.TiempoEnNivelDias / 365.25:F1} años"));
        
        if (documentos.Any())
        {
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("DOCUMENTOS ADJUNTOS").SetFontSize(14).SetBold());
            foreach (var doc in documentos)
            {
                document.Add(new Paragraph($"• {doc.NombreArchivo} | {doc.TipoDocumento} | {doc.FechaCreacion:dd/MM/yyyy}"));
            }
        }
        
        document.Add(new Paragraph("\n"));
        document.Add(new Paragraph($"Documento generado el {DateTime.Now:dd/MM/yyyy HH:mm:ss}")
            .SetFontSize(10)
            .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT));

        document.Close();
        return memoryStream.ToArray();
    }

    public async Task<byte[]> GenerarEstadisticasAsync()
    {
        var solicitudes = await _solicitudRepository.GetAllAsync();
        var docentes = await _docenteRepository.GetAllAsync();
        
        using var memoryStream = new MemoryStream();
        using var writer = new PdfWriter(memoryStream);
        using var pdf = new PdfDocument(writer);
        var document = new Document(pdf);

        // Título
        document.Add(new Paragraph("ESTADÍSTICAS DE ASCENSOS DOCENTES")
            .SetFontSize(18)
            .SetBold()
            .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));
        
        document.Add(new Paragraph("\n"));

        // Datos generales
        document.Add(new Paragraph("DATOS GENERALES").SetFontSize(14).SetBold());
        document.Add(new Paragraph($"Total Docentes: {docentes.Count}"));
        document.Add(new Paragraph($"Total Solicitudes: {solicitudes.Count}"));
        document.Add(new Paragraph($"Solicitudes Aprobadas: {solicitudes.Count(s => s.Estado == EstadoSolicitud.Aprobada)}"));
        document.Add(new Paragraph($"Solicitudes Rechazadas: {solicitudes.Count(s => s.Estado == EstadoSolicitud.Rechazada)}"));
        document.Add(new Paragraph($"Solicitudes Pendientes: {solicitudes.Count(s => s.Estado == EstadoSolicitud.Pendiente)}"));
        document.Add(new Paragraph($"Solicitudes En Proceso: {solicitudes.Count(s => s.Estado == EstadoSolicitud.EnProceso)}"));
        
        document.Add(new Paragraph("\n"));

        // Estadísticas por nivel
        document.Add(new Paragraph("ESTADÍSTICAS POR NIVEL").SetFontSize(14).SetBold());
        foreach (NivelTitular nivel in Enum.GetValues(typeof(NivelTitular)))
        {
            document.Add(new Paragraph($"{nivel}:").SetBold());
            document.Add(new Paragraph($"  Docentes: {docentes.Count(d => d.NivelActual == nivel)}"));
            document.Add(new Paragraph($"  Solicitudes: {solicitudes.Count(s => s.NivelActual == nivel)}"));
            document.Add(new Paragraph($"  Aprobadas: {solicitudes.Count(s => s.NivelActual == nivel && s.Estado == EstadoSolicitud.Aprobada)}"));
        }
        
        document.Add(new Paragraph("\n"));
        document.Add(new Paragraph($"Documento generado el {DateTime.Now:dd/MM/yyyy HH:mm:ss}")
            .SetFontSize(10)
            .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT));

        document.Close();
        return memoryStream.ToArray();
    }
}
