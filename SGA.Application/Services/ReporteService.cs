using SGA.Application.Interfaces;
using SGA.Application.Interfaces.Repositories;
using SGA.Application.DTOs;
using SGA.Domain.Enums;
using SGA.Domain.Extensions;
using System.Text;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Colors;
using iText.Layout.Properties;
using iText.Layout.Borders;

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

    // Nuevos reportes para usuarios
    public async Task<byte[]> GenerarReporteEstadoRequisitosPorNivelAsync(Guid docenteId)
    {
        var docente = await _docenteRepository.GetByIdAsync(docenteId);
        if (docente == null)
            throw new ArgumentException("Docente no encontrado");

        using var memoryStream = new MemoryStream();
        using var writer = new PdfWriter(memoryStream);
        using var pdf = new PdfDocument(writer);
        var document = new Document(pdf);

        var colorPrimario = new DeviceRgb(138, 21, 56); // #8a1538
        
        document.SetMargins(36, 36, 36, 36);

        // Título
        document.Add(new Paragraph("REPORTE DE ESTADO DE REQUISITOS PARA ASCENSO")
            .SetFontSize(16)
            .SetBold()
            .SetFontColor(colorPrimario)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetMarginBottom(20));

        // Información del docente
        document.Add(new Paragraph("DATOS DEL DOCENTE")
            .SetFontSize(12)
            .SetBold()
            .SetFontColor(colorPrimario)
            .SetMarginBottom(10));

        var infoTable = new Table(new float[] { 1, 2 }).UseAllAvailableWidth();
        infoTable.AddCell(new Cell().Add(new Paragraph("Nombres:").SetBold()).SetBorder(Border.NO_BORDER));
        infoTable.AddCell(new Cell().Add(new Paragraph($"{docente.Nombres} {docente.Apellidos}")).SetBorder(Border.NO_BORDER));
        infoTable.AddCell(new Cell().Add(new Paragraph("Cédula:").SetBold()).SetBorder(Border.NO_BORDER));
        infoTable.AddCell(new Cell().Add(new Paragraph(docente.Cedula)).SetBorder(Border.NO_BORDER));
        infoTable.AddCell(new Cell().Add(new Paragraph("Nivel Actual:").SetBold()).SetBorder(Border.NO_BORDER));
        infoTable.AddCell(new Cell().Add(new Paragraph(docente.NivelActual.GetDescription())).SetBorder(Border.NO_BORDER));
        
        document.Add(infoTable);

        if (!docente.NivelActual.EsNivelMaximo())
        {
            var siguienteNivel = docente.NivelActual.GetSiguienteNivel();
            var estadoRequisitos = docente.GetEstadoRequisitos();

            document.Add(new Paragraph($"\nREQUISITOS PARA ASCENDER A {siguienteNivel!.Value.GetDescription()}")
                .SetFontSize(12)
                .SetBold()
                .SetFontColor(colorPrimario)
                .SetMarginTop(20)
                .SetMarginBottom(10));

            var reqTable = new Table(new float[] { 2, 1, 1, 1 }).UseAllAvailableWidth();
            
            // Headers
            reqTable.AddHeaderCell(new Cell().Add(new Paragraph("Requisito").SetBold()));
            reqTable.AddHeaderCell(new Cell().Add(new Paragraph("Requerido").SetBold()));
            reqTable.AddHeaderCell(new Cell().Add(new Paragraph("Actual").SetBold()));
            reqTable.AddHeaderCell(new Cell().Add(new Paragraph("Estado").SetBold()));

            // Tiempo en nivel
            var colorTiempo = estadoRequisitos.CumpleTiempo ? ColorConstants.GREEN : ColorConstants.RED;
            reqTable.AddCell(new Cell().Add(new Paragraph("Tiempo en nivel")));
            reqTable.AddCell(new Cell().Add(new Paragraph("4 años")));
            reqTable.AddCell(new Cell().Add(new Paragraph($"{estadoRequisitos.DiasEnNivel / 365.25:F1} años")));
            reqTable.AddCell(new Cell().Add(new Paragraph(estadoRequisitos.CumpleTiempo ? "✓" : "✗").SetFontColor(colorTiempo)));

            // Obras académicas
            var colorObras = estadoRequisitos.CumpleObras ? ColorConstants.GREEN : ColorConstants.RED;
            reqTable.AddCell(new Cell().Add(new Paragraph("Obras académicas")));
            reqTable.AddCell(new Cell().Add(new Paragraph("Según nivel")));
            reqTable.AddCell(new Cell().Add(new Paragraph($"{estadoRequisitos.ObrasActuales}")));
            reqTable.AddCell(new Cell().Add(new Paragraph(estadoRequisitos.CumpleObras ? "✓" : "✗").SetFontColor(colorObras)));

            // Evaluaciones
            var colorEval = estadoRequisitos.CumpleEvaluaciones ? ColorConstants.GREEN : ColorConstants.RED;
            reqTable.AddCell(new Cell().Add(new Paragraph("Promedio evaluaciones")));
            reqTable.AddCell(new Cell().Add(new Paragraph("75%")));
            reqTable.AddCell(new Cell().Add(new Paragraph($"{estadoRequisitos.PromedioActual:F1}%")));
            reqTable.AddCell(new Cell().Add(new Paragraph(estadoRequisitos.CumpleEvaluaciones ? "✓" : "✗").SetFontColor(colorEval)));

            // Capacitación
            var colorCap = estadoRequisitos.CumpleCapacitacion ? ColorConstants.GREEN : ColorConstants.RED;
            reqTable.AddCell(new Cell().Add(new Paragraph("Horas capacitación")));
            reqTable.AddCell(new Cell().Add(new Paragraph("Según nivel")));
            reqTable.AddCell(new Cell().Add(new Paragraph($"{estadoRequisitos.HorasActuales}h")));
            reqTable.AddCell(new Cell().Add(new Paragraph(estadoRequisitos.CumpleCapacitacion ? "✓" : "✗").SetFontColor(colorCap)));

            // Investigación
            var colorInv = estadoRequisitos.CumpleInvestigacion ? ColorConstants.GREEN : ColorConstants.RED;
            reqTable.AddCell(new Cell().Add(new Paragraph("Meses investigación")));
            reqTable.AddCell(new Cell().Add(new Paragraph("Según nivel")));
            reqTable.AddCell(new Cell().Add(new Paragraph($"{estadoRequisitos.MesesInvestigacionActuales}")));
            reqTable.AddCell(new Cell().Add(new Paragraph(estadoRequisitos.CumpleInvestigacion ? "✓" : "✗").SetFontColor(colorInv)));

            document.Add(reqTable);

            // Resumen
            var resumenColor = estadoRequisitos.CumpleTodosLosRequisitos ? ColorConstants.GREEN : ColorConstants.RED;
            var resumenTexto = estadoRequisitos.CumpleTodosLosRequisitos 
                ? "✓ CUMPLE TODOS LOS REQUISITOS PARA ASCENDER" 
                : "✗ NO CUMPLE TODOS LOS REQUISITOS";
            
            document.Add(new Paragraph($"\nRESUMEN: {resumenTexto}")
                .SetFontSize(12)
                .SetBold()
                .SetFontColor(resumenColor)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginTop(20));
        }
        else
        {
            document.Add(new Paragraph("\nEl docente se encuentra en el nivel máximo del escalafón.")
                .SetFontSize(12)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginTop(20));
        }

        document.Add(new Paragraph($"\nDocumento generado el {DateTime.Now:dd/MM/yyyy HH:mm:ss}")
            .SetFontSize(10)
            .SetTextAlignment(TextAlignment.RIGHT)
            .SetMarginTop(30));

        document.Close();
        return memoryStream.ToArray();
    }

    public async Task<byte[]> GenerarReporteHistorialAscensosAsync(Guid docenteId)
    {
        var docente = await _docenteRepository.GetByIdAsync(docenteId);
        if (docente == null)
            throw new ArgumentException("Docente no encontrado");

        var solicitudes = await _solicitudRepository.GetByDocenteIdAsync(docenteId);

        using var memoryStream = new MemoryStream();
        using var writer = new PdfWriter(memoryStream);
        using var pdf = new PdfDocument(writer);
        var document = new Document(pdf);

        var colorPrimario = new DeviceRgb(138, 21, 56);
        document.SetMargins(36, 36, 36, 36);

        document.Add(new Paragraph("HISTORIAL DE ASCENSOS")
            .SetFontSize(16)
            .SetBold()
            .SetFontColor(colorPrimario)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetMarginBottom(20));

        // Información del docente
        document.Add(new Paragraph($"Docente: {docente.Nombres} {docente.Apellidos}")
            .SetFontSize(12)
            .SetBold());
        document.Add(new Paragraph($"Cédula: {docente.Cedula}"));
        document.Add(new Paragraph($"Nivel Actual: {docente.NivelActual.GetDescription()}"));
        
        if (docente.FechaInicioNivelActual != default)
        {
            document.Add(new Paragraph($"Fecha Inicio Nivel Actual: {docente.FechaInicioNivelActual:dd/MM/yyyy}"));
        }

        if (solicitudes.Any())
        {
            document.Add(new Paragraph("\nHISTORIAL DE SOLICITUDES")
                .SetFontSize(12)
                .SetBold()
                .SetFontColor(colorPrimario)
                .SetMarginTop(20)
                .SetMarginBottom(10));

            var table = new Table(new float[] { 1, 1.5f, 1, 1, 2 }).UseAllAvailableWidth();
            
            table.AddHeaderCell(new Cell().Add(new Paragraph("Fecha").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Nivel Solicitado").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Estado").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Fecha Resolución").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Observaciones").SetBold()));

            foreach (var solicitud in solicitudes.OrderByDescending(s => s.FechaSolicitud))
            {
                table.AddCell(new Cell().Add(new Paragraph(solicitud.FechaSolicitud.ToString("dd/MM/yyyy"))));
                table.AddCell(new Cell().Add(new Paragraph(solicitud.NivelSolicitado.GetDescription())));
                
                var estadoColor = solicitud.Estado switch
                {
                    EstadoSolicitud.Aprobada => ColorConstants.GREEN,
                    EstadoSolicitud.Rechazada => ColorConstants.RED,
                    EstadoSolicitud.EnProceso => ColorConstants.BLUE,
                    _ => ColorConstants.BLACK
                };
                
                table.AddCell(new Cell().Add(new Paragraph(solicitud.Estado.ToString()).SetFontColor(estadoColor)));
                
                var fechaResolucion = solicitud.FechaAprobacion?.ToString("dd/MM/yyyy") ?? "-";
                table.AddCell(new Cell().Add(new Paragraph(fechaResolucion)));
                
                var observaciones = !string.IsNullOrEmpty(solicitud.MotivoRechazo) 
                    ? solicitud.MotivoRechazo 
                    : solicitud.Observaciones ?? "-";
                table.AddCell(new Cell().Add(new Paragraph(observaciones)));
            }

            document.Add(table);
        }
        else
        {
            document.Add(new Paragraph("\nNo hay solicitudes de ascenso registradas.")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginTop(20));
        }

        document.Add(new Paragraph($"\nDocumento generado el {DateTime.Now:dd/MM/yyyy HH:mm:ss}")
            .SetFontSize(10)
            .SetTextAlignment(TextAlignment.RIGHT)
            .SetMarginTop(30));

        document.Close();
        return memoryStream.ToArray();
    }

    public async Task<byte[]> GenerarReporteCapacitacionesAsync(Guid docenteId)
    {
        var docente = await _docenteRepository.GetByIdAsync(docenteId);
        if (docente == null)
            throw new ArgumentException("Docente no encontrado");

        using var memoryStream = new MemoryStream();
        using var writer = new PdfWriter(memoryStream);
        using var pdf = new PdfDocument(writer);
        var document = new Document(pdf);

        var colorPrimario = new DeviceRgb(138, 21, 56);
        document.SetMargins(36, 36, 36, 36);

        document.Add(new Paragraph("REPORTE DE CAPACITACIONES")
            .SetFontSize(16)
            .SetBold()
            .SetFontColor(colorPrimario)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetMarginBottom(20));

        document.Add(new Paragraph($"Docente: {docente.Nombres} {docente.Apellidos}")
            .SetFontSize(12)
            .SetBold());
        document.Add(new Paragraph($"Cédula: {docente.Cedula}"));
        document.Add(new Paragraph($"Total Horas Capacitación: {docente.HorasCapacitacion ?? 0} horas"));

        // Aquí podrías agregar lógica para obtener capacitaciones detalladas si tienes esa entidad
        document.Add(new Paragraph("\nNOTA: Para obtener el detalle completo de capacitaciones, " +
            "debe realizar la importación desde el sistema DITIC.")
            .SetFontSize(10)
            .SetItalic()
            .SetMarginTop(20));

        document.Add(new Paragraph($"\nDocumento generado el {DateTime.Now:dd/MM/yyyy HH:mm:ss}")
            .SetFontSize(10)
            .SetTextAlignment(TextAlignment.RIGHT)
            .SetMarginTop(30));

        document.Close();
        return memoryStream.ToArray();
    }

    public async Task<byte[]> GenerarReporteObrasAcademicasAsync(Guid docenteId)
    {
        var docente = await _docenteRepository.GetByIdAsync(docenteId);
        if (docente == null)
            throw new ArgumentException("Docente no encontrado");

        using var memoryStream = new MemoryStream();
        using var writer = new PdfWriter(memoryStream);
        using var pdf = new PdfDocument(writer);
        var document = new Document(pdf);

        var colorPrimario = new DeviceRgb(138, 21, 56);
        document.SetMargins(36, 36, 36, 36);

        document.Add(new Paragraph("REPORTE DE PRODUCCIÓN ACADÉMICA")
            .SetFontSize(16)
            .SetBold()
            .SetFontColor(colorPrimario)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetMarginBottom(20));

        document.Add(new Paragraph($"Docente: {docente.Nombres} {docente.Apellidos}")
            .SetFontSize(12)
            .SetBold());
        document.Add(new Paragraph($"Cédula: {docente.Cedula}"));
        document.Add(new Paragraph($"Total Obras Académicas: {docente.NumeroObrasAcademicas ?? 0}"));
        document.Add(new Paragraph($"Meses en Investigación: {docente.MesesInvestigacion ?? 0}"));

        if (docente.ObrasAcademicas.Any())
        {
            document.Add(new Paragraph("\nDETALLE DE OBRAS ACADÉMICAS")
                .SetFontSize(12)
                .SetBold()
                .SetFontColor(colorPrimario)
                .SetMarginTop(20)
                .SetMarginBottom(10));

            var table = new Table(new float[] { 2, 1, 1, 1 }).UseAllAvailableWidth();
            
            table.AddHeaderCell(new Cell().Add(new Paragraph("Título").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Tipo").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Fecha").SetBold()));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Estado").SetBold()));

            foreach (var obra in docente.ObrasAcademicas.OrderByDescending(o => o.FechaCreacion))
            {
                table.AddCell(new Cell().Add(new Paragraph(obra.Titulo)));
                table.AddCell(new Cell().Add(new Paragraph(obra.TipoObra)));
                table.AddCell(new Cell().Add(new Paragraph(obra.FechaPublicacion.ToString("dd/MM/yyyy"))));
                table.AddCell(new Cell().Add(new Paragraph(obra.EsVerificada ? "Verificada" : "Pendiente")
                    .SetFontColor(obra.EsVerificada ? ColorConstants.GREEN : ColorConstants.ORANGE)));
            }

            document.Add(table);
        }
        else
        {
            document.Add(new Paragraph("\nNOTA: Para obtener el detalle completo de obras académicas, " +
                "debe realizar la importación desde el sistema DIR INV.")
                .SetFontSize(10)
                .SetItalic()
                .SetMarginTop(20));
        }

        document.Add(new Paragraph($"\nDocumento generado el {DateTime.Now:dd/MM/yyyy HH:mm:ss}")
            .SetFontSize(10)
            .SetTextAlignment(TextAlignment.RIGHT)
            .SetMarginTop(30));

        document.Close();
        return memoryStream.ToArray();
    }

    public async Task<byte[]> GenerarReporteCompletoAscensoAsync(Guid docenteId)
    {
        var docente = await _docenteRepository.GetByIdAsync(docenteId);
        if (docente == null)
            throw new ArgumentException("Docente no encontrado");

        var solicitudes = await _solicitudRepository.GetByDocenteIdAsync(docenteId);

        using var memoryStream = new MemoryStream();
        using var writer = new PdfWriter(memoryStream);
        using var pdf = new PdfDocument(writer);
        var document = new Document(pdf);

        var colorPrimario = new DeviceRgb(138, 21, 56);
        document.SetMargins(36, 36, 36, 36);

        // Portada
        document.Add(new Paragraph("REPORTE COMPLETO DE ASCENSO DOCENTE")
            .SetFontSize(18)
            .SetBold()
            .SetFontColor(colorPrimario)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetMarginBottom(30));

        document.Add(new Paragraph("UNIVERSIDAD TÉCNICA DE AMBATO")
            .SetFontSize(14)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetMarginBottom(20));

        // Datos del docente
        document.Add(new Paragraph("INFORMACIÓN DEL DOCENTE")
            .SetFontSize(14)
            .SetBold()
            .SetFontColor(colorPrimario)
            .SetMarginBottom(10));

        var infoTable = new Table(new float[] { 1, 2 }).UseAllAvailableWidth();
        infoTable.AddCell(new Cell().Add(new Paragraph("Nombres:").SetBold()).SetBorder(Border.NO_BORDER));
        infoTable.AddCell(new Cell().Add(new Paragraph($"{docente.Nombres} {docente.Apellidos}")).SetBorder(Border.NO_BORDER));
        infoTable.AddCell(new Cell().Add(new Paragraph("Cédula:").SetBold()).SetBorder(Border.NO_BORDER));
        infoTable.AddCell(new Cell().Add(new Paragraph(docente.Cedula)).SetBorder(Border.NO_BORDER));
        infoTable.AddCell(new Cell().Add(new Paragraph("Email:").SetBold()).SetBorder(Border.NO_BORDER));
        infoTable.AddCell(new Cell().Add(new Paragraph(docente.Email)).SetBorder(Border.NO_BORDER));
        infoTable.AddCell(new Cell().Add(new Paragraph("Nivel Actual:").SetBold()).SetBorder(Border.NO_BORDER));
        infoTable.AddCell(new Cell().Add(new Paragraph(docente.NivelActual.GetDescription())).SetBorder(Border.NO_BORDER));
        
        document.Add(infoTable);

        // Estado de requisitos
        if (!docente.NivelActual.EsNivelMaximo())
        {
            var estadoRequisitos = docente.GetEstadoRequisitos();
            
            document.Add(new Paragraph("\nESTADO DE REQUISITOS PARA ASCENSO")
                .SetFontSize(14)
                .SetBold()
                .SetFontColor(colorPrimario)
                .SetMarginTop(20)
                .SetMarginBottom(10));

            var progreso = (estadoRequisitos.CumpleTiempo ? 1 : 0) +
                          (estadoRequisitos.CumpleObras ? 1 : 0) +
                          (estadoRequisitos.CumpleEvaluaciones ? 1 : 0) +
                          (estadoRequisitos.CumpleCapacitacion ? 1 : 0) +
                          (estadoRequisitos.CumpleInvestigacion ? 1 : 0);

            document.Add(new Paragraph($"Progreso: {progreso}/5 requisitos cumplidos ({(progreso * 100 / 5):F0}%)")
                .SetFontSize(12)
                .SetMarginBottom(10));

            // Aquí puedes agregar más detalles del estado de requisitos
        }

        document.Add(new Paragraph($"\nDocumento generado el {DateTime.Now:dd/MM/yyyy HH:mm:ss}")
            .SetFontSize(10)
            .SetTextAlignment(TextAlignment.RIGHT)
            .SetMarginTop(30));

        document.Close();
        return memoryStream.ToArray();
    }

    public async Task<byte[]> GenerarCertificadoEstadoDocenteAsync(Guid docenteId)
    {
        var docente = await _docenteRepository.GetByIdAsync(docenteId);
        if (docente == null)
            throw new ArgumentException("Docente no encontrado");

        using var memoryStream = new MemoryStream();
        using var writer = new PdfWriter(memoryStream);
        using var pdf = new PdfDocument(writer);
        var document = new Document(pdf);

        var colorPrimario = new DeviceRgb(138, 21, 56);
        document.SetMargins(50, 50, 50, 50);

        // Encabezado oficial
        document.Add(new Paragraph("UNIVERSIDAD TÉCNICA DE AMBATO")
            .SetFontSize(18)
            .SetBold()
            .SetTextAlignment(TextAlignment.CENTER)
            .SetMarginBottom(10));

        document.Add(new Paragraph("VICERRECTORADO ACADÉMICO")
            .SetFontSize(14)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetMarginBottom(30));

        // Título del certificado
        document.Add(new Paragraph("CERTIFICADO DE ESTADO DOCENTE")
            .SetFontSize(16)
            .SetBold()
            .SetFontColor(colorPrimario)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetMarginBottom(30));

        // Cuerpo del certificado
        var texto = $"El suscrito Vicerrector Académico de la Universidad Técnica de Ambato, " +
                   $"CERTIFICA que el/la docente {docente.Nombres} {docente.Apellidos}, " +
                   $"con número de cédula {docente.Cedula}, se encuentra registrado/a en el " +
                   $"sistema de gestión de ascensos con el nivel de {docente.NivelActual.GetDescription()}.";

        document.Add(new Paragraph(texto)
            .SetTextAlignment(TextAlignment.JUSTIFIED)
            .SetMarginBottom(20));

        document.Add(new Paragraph($"Fecha de inicio en el nivel actual: {docente.FechaInicioNivelActual:dd/MM/yyyy}")
            .SetMarginBottom(10));

        if (docente.FechaUltimoAscenso.HasValue)
        {
            document.Add(new Paragraph($"Fecha último ascenso: {docente.FechaUltimoAscenso.Value:dd/MM/yyyy}")
                .SetMarginBottom(20));
        }

        document.Add(new Paragraph("Este certificado es expedido a petición del interesado " +
                                 "para los fines que estime conveniente.")
            .SetTextAlignment(TextAlignment.JUSTIFIED)
            .SetMarginBottom(40));

        document.Add(new Paragraph($"Ambato, {DateTime.Now:dd} de {DateTime.Now:MMMM} de {DateTime.Now:yyyy}")
            .SetMarginBottom(60));

        // Firma
        document.Add(new Paragraph("_________________________")
            .SetTextAlignment(TextAlignment.CENTER)
            .SetMarginBottom(5));
        
        document.Add(new Paragraph("VICERRECTOR ACADÉMICO")
            .SetTextAlignment(TextAlignment.CENTER)
            .SetBold());

        document.Close();
        return memoryStream.ToArray();
    }

    // Métodos para generar vistas HTML (para modales)
    public async Task<string> GenerarVistaHojaVidaAsync(Guid docenteId)
    {
        var docente = await _docenteRepository.GetByIdAsync(docenteId);
        if (docente == null)
            throw new ArgumentException("Docente no encontrado");

        var html = new StringBuilder();
        html.AppendLine("<div class='reporte-container'>");
        html.AppendLine("<style>");
        html.AppendLine(@"
            .reporte-container { font-family: Arial, sans-serif; }
            .reporte-title { color: #8a1538; font-size: 24px; font-weight: bold; text-align: center; margin-bottom: 20px; }
            .seccion-title { color: #8a1538; font-size: 18px; font-weight: bold; margin-top: 20px; margin-bottom: 10px; border-bottom: 2px solid #8a1538; }
            .info-table { width: 100%; margin-bottom: 15px; }
            .info-table td { padding: 8px; border-bottom: 1px solid #eee; }
            .info-table .label { font-weight: bold; width: 30%; }
            .estado-cumple { color: green; font-weight: bold; }
            .estado-no-cumple { color: red; font-weight: bold; }
        ");
        html.AppendLine("</style>");

        html.AppendLine("<h1 class='reporte-title'>HOJA DE VIDA ACADÉMICA</h1>");

        html.AppendLine("<div class='seccion-title'>DATOS PERSONALES</div>");
        html.AppendLine("<table class='info-table'>");
        html.AppendLine($"<tr><td class='label'>Nombres:</td><td>{docente.Nombres}</td></tr>");
        html.AppendLine($"<tr><td class='label'>Apellidos:</td><td>{docente.Apellidos}</td></tr>");
        html.AppendLine($"<tr><td class='label'>Cédula:</td><td>{docente.Cedula}</td></tr>");
        html.AppendLine($"<tr><td class='label'>Email:</td><td>{docente.Email}</td></tr>");
        html.AppendLine($"<tr><td class='label'>Nivel Actual:</td><td>{docente.NivelActual.GetDescription()}</td></tr>");
        if (docente.FechaNombramiento.HasValue)
            html.AppendLine($"<tr><td class='label'>Fecha Nombramiento:</td><td>{docente.FechaNombramiento.Value:dd/MM/yyyy}</td></tr>");
        html.AppendLine("</table>");

        html.AppendLine("<div class='seccion-title'>DATOS ACADÉMICOS</div>");
        html.AppendLine("<table class='info-table'>");
        html.AppendLine($"<tr><td class='label'>Promedio Evaluaciones:</td><td>{docente.PromedioEvaluaciones ?? 0:F2}/100</td></tr>");
        html.AppendLine($"<tr><td class='label'>Horas Capacitación:</td><td>{docente.HorasCapacitacion ?? 0}</td></tr>");
        html.AppendLine($"<tr><td class='label'>Obras Académicas:</td><td>{docente.NumeroObrasAcademicas ?? 0}</td></tr>");
        html.AppendLine($"<tr><td class='label'>Meses en Investigación:</td><td>{docente.MesesInvestigacion ?? 0}</td></tr>");
        
        var tiempoEnNivel = DateTime.UtcNow - docente.FechaInicioNivelActual;
        html.AppendLine($"<tr><td class='label'>Tiempo en Nivel Actual:</td><td>{tiempoEnNivel.TotalDays / 365.25:F1} años</td></tr>");
        html.AppendLine("</table>");

        if (!docente.NivelActual.EsNivelMaximo())
        {
            var estadoRequisitos = docente.GetEstadoRequisitos();
            var siguienteNivel = docente.NivelActual.GetSiguienteNivel();
            
            html.AppendLine($"<div class='seccion-title'>REQUISITOS PARA {siguienteNivel!.Value.GetDescription()}</div>");
            html.AppendLine("<table class='info-table'>");
            html.AppendLine($"<tr><td class='label'>Tiempo en nivel:</td><td class='{(estadoRequisitos.CumpleTiempo ? "estado-cumple" : "estado-no-cumple")}'>{(estadoRequisitos.CumpleTiempo ? "✓ Cumple" : "✗ No cumple")}</td></tr>");
            html.AppendLine($"<tr><td class='label'>Obras académicas:</td><td class='{(estadoRequisitos.CumpleObras ? "estado-cumple" : "estado-no-cumple")}'>{(estadoRequisitos.CumpleObras ? "✓ Cumple" : "✗ No cumple")}</td></tr>");
            html.AppendLine($"<tr><td class='label'>Evaluaciones:</td><td class='{(estadoRequisitos.CumpleEvaluaciones ? "estado-cumple" : "estado-no-cumple")}'>{(estadoRequisitos.CumpleEvaluaciones ? "✓ Cumple" : "✗ No cumple")}</td></tr>");
            html.AppendLine($"<tr><td class='label'>Capacitación:</td><td class='{(estadoRequisitos.CumpleCapacitacion ? "estado-cumple" : "estado-no-cumple")}'>{(estadoRequisitos.CumpleCapacitacion ? "✓ Cumple" : "✗ No cumple")}</td></tr>");
            html.AppendLine($"<tr><td class='label'>Investigación:</td><td class='{(estadoRequisitos.CumpleInvestigacion ? "estado-cumple" : "estado-no-cumple")}'>{(estadoRequisitos.CumpleInvestigacion ? "✓ Cumple" : "✗ No cumple")}</td></tr>");
            html.AppendLine("</table>");
        }

        html.AppendLine("</div>");
        return html.ToString();
    }

    public async Task<string> GenerarVistaSolicitudAsync(Guid solicitudId, Guid docenteId, bool esAdmin)
    {
        var solicitud = await _solicitudRepository.GetByIdAsync(solicitudId);
        if (solicitud == null)
            throw new ArgumentException("Solicitud no encontrada");

        if (!esAdmin && solicitud.DocenteId != docenteId)
            throw new UnauthorizedAccessException("No tiene permisos para ver este reporte");

        var docente = await _docenteRepository.GetByIdAsync(solicitud.DocenteId);
        if (docente == null)
            throw new ArgumentException("Docente no encontrado");

        var html = new StringBuilder();
        html.AppendLine("<div class='reporte-container'>");
        html.AppendLine("<style>");
        html.AppendLine(@"
            .reporte-container { font-family: Arial, sans-serif; }
            .reporte-title { color: #8a1538; font-size: 24px; font-weight: bold; text-align: center; margin-bottom: 20px; }
            .seccion-title { color: #8a1538; font-size: 18px; font-weight: bold; margin-top: 20px; margin-bottom: 10px; border-bottom: 2px solid #8a1538; }
            .info-table { width: 100%; margin-bottom: 15px; }
            .info-table td { padding: 8px; border-bottom: 1px solid #eee; }
            .info-table .label { font-weight: bold; width: 30%; }
            .estado-aprobada { color: green; font-weight: bold; }
            .estado-rechazada { color: red; font-weight: bold; }
            .estado-proceso { color: blue; font-weight: bold; }
            .estado-pendiente { color: orange; font-weight: bold; }
        ");
        html.AppendLine("</style>");

        html.AppendLine("<h1 class='reporte-title'>REPORTE DE SOLICITUD DE ASCENSO</h1>");

        html.AppendLine("<div class='seccion-title'>DATOS DE LA SOLICITUD</div>");
        html.AppendLine("<table class='info-table'>");
        html.AppendLine($"<tr><td class='label'>Docente:</td><td>{docente.Nombres} {docente.Apellidos}</td></tr>");
        html.AppendLine($"<tr><td class='label'>Cédula:</td><td>{docente.Cedula}</td></tr>");
        html.AppendLine($"<tr><td class='label'>Fecha Solicitud:</td><td>{solicitud.FechaSolicitud:dd/MM/yyyy}</td></tr>");
        html.AppendLine($"<tr><td class='label'>Nivel Actual:</td><td>{solicitud.NivelActual.GetDescription()}</td></tr>");
        html.AppendLine($"<tr><td class='label'>Nivel Solicitado:</td><td>{solicitud.NivelSolicitado.GetDescription()}</td></tr>");
        
        var estadoClass = solicitud.Estado switch
        {
            EstadoSolicitud.Aprobada => "estado-aprobada",
            EstadoSolicitud.Rechazada => "estado-rechazada",
            EstadoSolicitud.EnProceso => "estado-proceso",
            _ => "estado-pendiente"
        };
        
        html.AppendLine($"<tr><td class='label'>Estado:</td><td class='{estadoClass}'>{solicitud.Estado}</td></tr>");
        
        if (solicitud.FechaAprobacion.HasValue)
            html.AppendLine($"<tr><td class='label'>Fecha Resolución:</td><td>{solicitud.FechaAprobacion.Value:dd/MM/yyyy}</td></tr>");
        
        if (!string.IsNullOrEmpty(solicitud.MotivoRechazo))
            html.AppendLine($"<tr><td class='label'>Motivo Rechazo:</td><td>{solicitud.MotivoRechazo}</td></tr>");
        html.AppendLine("</table>");

        html.AppendLine("<div class='seccion-title'>DATOS ACADÉMICOS AL MOMENTO DE LA SOLICITUD</div>");
        html.AppendLine("<table class='info-table'>");
        html.AppendLine($"<tr><td class='label'>Promedio Evaluaciones:</td><td>{solicitud.PromedioEvaluaciones:F2}/100</td></tr>");
        html.AppendLine($"<tr><td class='label'>Horas Capacitación:</td><td>{solicitud.HorasCapacitacion}</td></tr>");
        html.AppendLine($"<tr><td class='label'>Obras Académicas:</td><td>{solicitud.NumeroObrasAcademicas}</td></tr>");
        html.AppendLine($"<tr><td class='label'>Meses en Investigación:</td><td>{solicitud.MesesInvestigacion}</td></tr>");
        html.AppendLine($"<tr><td class='label'>Tiempo en Nivel:</td><td>{solicitud.TiempoEnNivelDias / 365.25:F1} años</td></tr>");
        html.AppendLine("</table>");

        html.AppendLine("</div>");
        return html.ToString();
    }

    public async Task<string> GenerarVistaEstadoRequisitosAsync(Guid docenteId)
    {
        var docente = await _docenteRepository.GetByIdAsync(docenteId);
        if (docente == null)
            throw new ArgumentException("Docente no encontrado");

        var html = new StringBuilder();
        html.AppendLine("<div class='reporte-container'>");
        html.AppendLine("<style>");
        html.AppendLine(@"
            .reporte-container { font-family: Arial, sans-serif; }
            .reporte-title { color: #8a1538; font-size: 24px; font-weight: bold; text-align: center; margin-bottom: 20px; }
            .info-table { width: 100%; margin-bottom: 15px; border-collapse: collapse; }
            .info-table th, .info-table td { padding: 12px; border: 1px solid #ddd; text-align: left; }
            .info-table th { background-color: #8a1538; color: white; }
            .cumple { background-color: #d4edda; color: #155724; font-weight: bold; }
            .no-cumple { background-color: #f8d7da; color: #721c24; font-weight: bold; }
            .resumen { text-align: center; font-size: 18px; font-weight: bold; margin-top: 20px; padding: 15px; border-radius: 5px; }
            .resumen.aprobado { background-color: #d4edda; color: #155724; }
            .resumen.rechazado { background-color: #f8d7da; color: #721c24; }
        ");
        html.AppendLine("</style>");

        html.AppendLine("<h1 class='reporte-title'>ESTADO DE REQUISITOS PARA ASCENSO</h1>");

        html.AppendLine($"<p><strong>Docente:</strong> {docente.Nombres} {docente.Apellidos}</p>");
        html.AppendLine($"<p><strong>Nivel Actual:</strong> {docente.NivelActual.GetDescription()}</p>");

        if (!docente.NivelActual.EsNivelMaximo())
        {
            var siguienteNivel = docente.NivelActual.GetSiguienteNivel();
            var estadoRequisitos = docente.GetEstadoRequisitos();

            html.AppendLine($"<p><strong>Nivel Objetivo:</strong> {siguienteNivel!.Value.GetDescription()}</p>");

            html.AppendLine("<table class='info-table'>");
            html.AppendLine("<tr><th>Requisito</th><th>Requerido</th><th>Actual</th><th>Estado</th></tr>");
            
            var cumpleTiempo = estadoRequisitos.CumpleTiempo ? "cumple" : "no-cumple";
            html.AppendLine($"<tr><td>Tiempo en nivel</td><td>4 años</td><td>{estadoRequisitos.DiasEnNivel / 365.25:F1} años</td><td class='{cumpleTiempo}'>{(estadoRequisitos.CumpleTiempo ? "✓ Cumple" : "✗ No cumple")}</td></tr>");
            
            var cumpleObras = estadoRequisitos.CumpleObras ? "cumple" : "no-cumple";
            html.AppendLine($"<tr><td>Obras académicas</td><td>Según nivel</td><td>{estadoRequisitos.ObrasActuales}</td><td class='{cumpleObras}'>{(estadoRequisitos.CumpleObras ? "✓ Cumple" : "✗ No cumple")}</td></tr>");
            
            var cumpleEval = estadoRequisitos.CumpleEvaluaciones ? "cumple" : "no-cumple";
            html.AppendLine($"<tr><td>Promedio evaluaciones</td><td>75%</td><td>{estadoRequisitos.PromedioActual:F1}%</td><td class='{cumpleEval}'>{(estadoRequisitos.CumpleEvaluaciones ? "✓ Cumple" : "✗ No cumple")}</td></tr>");
            
            var cumpleCap = estadoRequisitos.CumpleCapacitacion ? "cumple" : "no-cumple";
            html.AppendLine($"<tr><td>Horas capacitación</td><td>Según nivel</td><td>{estadoRequisitos.HorasActuales}h</td><td class='{cumpleCap}'>{(estadoRequisitos.CumpleCapacitacion ? "✓ Cumple" : "✗ No cumple")}</td></tr>");
            
            var cumpleInv = estadoRequisitos.CumpleInvestigacion ? "cumple" : "no-cumple";
            html.AppendLine($"<tr><td>Meses investigación</td><td>Según nivel</td><td>{estadoRequisitos.MesesInvestigacionActuales}</td><td class='{cumpleInv}'>{(estadoRequisitos.CumpleInvestigacion ? "✓ Cumple" : "✗ No cumple")}</td></tr>");
            
            html.AppendLine("</table>");

            var resumenClass = estadoRequisitos.CumpleTodosLosRequisitos ? "aprobado" : "rechazado";
            var resumenTexto = estadoRequisitos.CumpleTodosLosRequisitos 
                ? "✓ CUMPLE TODOS LOS REQUISITOS PARA ASCENDER" 
                : "✗ NO CUMPLE TODOS LOS REQUISITOS";
            
            html.AppendLine($"<div class='resumen {resumenClass}'>{resumenTexto}</div>");
        }
        else
        {
            html.AppendLine("<p style='text-align: center; font-size: 16px; margin-top: 20px;'>El docente se encuentra en el nivel máximo del escalafón.</p>");
        }

        html.AppendLine("</div>");
        return html.ToString();
    }

    public async Task<string> GenerarVistaHistorialAscensosAsync(Guid docenteId)
    {
        var docente = await _docenteRepository.GetByIdAsync(docenteId);
        if (docente == null)
            throw new ArgumentException("Docente no encontrado");

        var solicitudes = await _solicitudRepository.GetByDocenteIdAsync(docenteId);

        var html = new StringBuilder();
        html.AppendLine("<div class='reporte-container'>");
        html.AppendLine("<style>");
        html.AppendLine(@"
            .reporte-container { font-family: Arial, sans-serif; }
            .reporte-title { color: #8a1538; font-size: 24px; font-weight: bold; text-align: center; margin-bottom: 20px; }
            .info-table { width: 100%; margin-bottom: 15px; border-collapse: collapse; }
            .info-table th, .info-table td { padding: 10px; border: 1px solid #ddd; text-align: left; }
            .info-table th { background-color: #8a1538; color: white; }
            .estado-aprobada { color: green; font-weight: bold; }
            .estado-rechazada { color: red; font-weight: bold; }
            .estado-proceso { color: blue; font-weight: bold; }
            .estado-pendiente { color: orange; font-weight: bold; }
        ");
        html.AppendLine("</style>");

        html.AppendLine("<h1 class='reporte-title'>HISTORIAL DE ASCENSOS</h1>");

        html.AppendLine($"<p><strong>Docente:</strong> {docente.Nombres} {docente.Apellidos}</p>");
        html.AppendLine($"<p><strong>Cédula:</strong> {docente.Cedula}</p>");
        html.AppendLine($"<p><strong>Nivel Actual:</strong> {docente.NivelActual.GetDescription()}</p>");

        if (solicitudes.Any())
        {
            html.AppendLine("<h3>Historial de Solicitudes</h3>");
            html.AppendLine("<table class='info-table'>");
            html.AppendLine("<tr><th>Fecha</th><th>Nivel Solicitado</th><th>Estado</th><th>Fecha Resolución</th><th>Observaciones</th></tr>");

            foreach (var solicitud in solicitudes.OrderByDescending(s => s.FechaSolicitud))
            {
                var estadoClass = solicitud.Estado switch
                {
                    EstadoSolicitud.Aprobada => "estado-aprobada",
                    EstadoSolicitud.Rechazada => "estado-rechazada",
                    EstadoSolicitud.EnProceso => "estado-proceso",
                    _ => "estado-pendiente"
                };

                var fechaResolucion = solicitud.FechaAprobacion?.ToString("dd/MM/yyyy") ?? "-";
                var observaciones = !string.IsNullOrEmpty(solicitud.MotivoRechazo) 
                    ? solicitud.MotivoRechazo 
                    : solicitud.Observaciones ?? "-";

                html.AppendLine($"<tr>");
                html.AppendLine($"<td>{solicitud.FechaSolicitud:dd/MM/yyyy}</td>");
                html.AppendLine($"<td>{solicitud.NivelSolicitado.GetDescription()}</td>");
                html.AppendLine($"<td class='{estadoClass}'>{solicitud.Estado}</td>");
                html.AppendLine($"<td>{fechaResolucion}</td>");
                html.AppendLine($"<td>{observaciones}</td>");
                html.AppendLine($"</tr>");
            }

            html.AppendLine("</table>");
        }
        else
        {
            html.AppendLine("<p style='text-align: center; margin-top: 20px;'>No hay solicitudes de ascenso registradas.</p>");
        }

        html.AppendLine("</div>");
        return html.ToString();
    }

    public async Task<string> GenerarVistaCapacitacionesAsync(Guid docenteId)
    {
        var docente = await _docenteRepository.GetByIdAsync(docenteId);
        if (docente == null)
            throw new ArgumentException("Docente no encontrado");

        var html = new StringBuilder();
        html.AppendLine("<div class='reporte-container'>");
        html.AppendLine("<style>");
        html.AppendLine(@"
            .reporte-container { font-family: Arial, sans-serif; }
            .reporte-title { color: #8a1538; font-size: 24px; font-weight: bold; text-align: center; margin-bottom: 20px; }
            .info-table { width: 100%; margin-bottom: 15px; border-collapse: collapse; }
            .info-table th, .info-table td { padding: 10px; border: 1px solid #ddd; text-align: left; }
            .info-table th { background-color: #8a1538; color: white; }
            .seccion-title { color: #8a1538; font-size: 18px; font-weight: bold; margin-top: 20px; margin-bottom: 10px; border-bottom: 2px solid #8a1538; }
        ");
        html.AppendLine("</style>");

        html.AppendLine("<h1 class='reporte-title'>REPORTE DE CAPACITACIONES</h1>");

        html.AppendLine($"<p><strong>Docente:</strong> {docente.Nombres} {docente.Apellidos}</p>");
        html.AppendLine($"<p><strong>Cédula:</strong> {docente.Cedula}</p>");
        html.AppendLine($"<p><strong>Total Horas Acumuladas:</strong> {docente.HorasCapacitacion ?? 0}</p>");

        html.AppendLine("<div class='seccion-title'>RESUMEN DE CAPACITACIONES</div>");
        html.AppendLine("<p>Las capacitaciones se importan automáticamente desde el sistema DITIC de la UTA.</p>");
        
        if (docente.FechaUltimaImportacion.HasValue)
        {
            html.AppendLine($"<p><strong>Última actualización:</strong> {docente.FechaUltimaImportacion.Value:dd/MM/yyyy HH:mm}</p>");
        }

        html.AppendLine("<div class='seccion-title'>NOTA</div>");
        html.AppendLine("<p>Para ver el detalle completo de las capacitaciones realizadas, debe acceder al sistema DITIC o solicitar la importación actualizada de datos.</p>");

        html.AppendLine("</div>");
        return html.ToString();
    }

    public async Task<string> GenerarVistaObrasAcademicasAsync(Guid docenteId)
    {
        var docente = await _docenteRepository.GetByIdAsync(docenteId);
        if (docente == null)
            throw new ArgumentException("Docente no encontrado");

        var html = new StringBuilder();
        html.AppendLine("<div class='reporte-container'>");
        html.AppendLine("<style>");
        html.AppendLine(@"
            .reporte-container { font-family: Arial, sans-serif; }
            .reporte-title { color: #8a1538; font-size: 24px; font-weight: bold; text-align: center; margin-bottom: 20px; }
            .info-table { width: 100%; margin-bottom: 15px; border-collapse: collapse; }
            .info-table th, .info-table td { padding: 10px; border: 1px solid #ddd; text-align: left; }
            .info-table th { background-color: #8a1538; color: white; }
            .seccion-title { color: #8a1538; font-size: 18px; font-weight: bold; margin-top: 20px; margin-bottom: 10px; border-bottom: 2px solid #8a1538; }
            .verificada { color: green; font-weight: bold; }
            .no-verificada { color: orange; font-weight: bold; }
        ");
        html.AppendLine("</style>");

        html.AppendLine("<h1 class='reporte-title'>REPORTE DE OBRAS ACADÉMICAS</h1>");

        html.AppendLine($"<p><strong>Docente:</strong> {docente.Nombres} {docente.Apellidos}</p>");
        html.AppendLine($"<p><strong>Cédula:</strong> {docente.Cedula}</p>");
        html.AppendLine($"<p><strong>Total Obras Registradas:</strong> {docente.NumeroObrasAcademicas ?? 0}</p>");

        html.AppendLine("<div class='seccion-title'>RESUMEN DE PRODUCCIÓN ACADÉMICA</div>");
        html.AppendLine("<p>Las obras académicas se validan y verifican a través del sistema DIR INV de la UTA.</p>");
        
        if (docente.FechaUltimaImportacion.HasValue)
        {
            html.AppendLine($"<p><strong>Última actualización:</strong> {docente.FechaUltimaImportacion.Value:dd/MM/yyyy HH:mm}</p>");
        }

        html.AppendLine("<div class='seccion-title'>NOTA</div>");
        html.AppendLine("<p>Para ver el detalle completo de las obras académicas, debe acceder al sistema DIR INV o solicitar la importación actualizada de datos desde el sistema de investigación.</p>");

        html.AppendLine("</div>");
        return html.ToString();
    }

    public async Task<string> GenerarVistaReporteCompletoAsync(Guid docenteId)
    {
        var docente = await _docenteRepository.GetByIdAsync(docenteId);
        if (docente == null)
            throw new ArgumentException("Docente no encontrado");

        var solicitudes = await _solicitudRepository.GetByDocenteIdAsync(docenteId);

        var html = new StringBuilder();
        html.AppendLine("<div class='reporte-container'>");
        html.AppendLine("<style>");
        html.AppendLine(@"
            .reporte-container { font-family: Arial, sans-serif; }
            .reporte-title { color: #8a1538; font-size: 24px; font-weight: bold; text-align: center; margin-bottom: 20px; }
            .info-table { width: 100%; margin-bottom: 15px; border-collapse: collapse; }
            .info-table th, .info-table td { padding: 10px; border: 1px solid #ddd; text-align: left; }
            .info-table th { background-color: #8a1538; color: white; }
            .seccion-title { color: #8a1538; font-size: 18px; font-weight: bold; margin-top: 20px; margin-bottom: 10px; border-bottom: 2px solid #8a1538; }
            .cumple { color: green; font-weight: bold; }
            .no-cumple { color: red; font-weight: bold; }
        ");
        html.AppendLine("</style>");

        html.AppendLine("<h1 class='reporte-title'>REPORTE COMPLETO DE ASCENSO DOCENTE</h1>");

        // Información del docente
        html.AppendLine("<div class='seccion-title'>INFORMACIÓN DEL DOCENTE</div>");
        html.AppendLine("<table class='info-table'>");
        html.AppendLine($"<tr><td><strong>Nombres:</strong></td><td>{docente.Nombres} {docente.Apellidos}</td></tr>");
        html.AppendLine($"<tr><td><strong>Cédula:</strong></td><td>{docente.Cedula}</td></tr>");
        html.AppendLine($"<tr><td><strong>Email:</strong></td><td>{docente.Email}</td></tr>");
        html.AppendLine($"<tr><td><strong>Nivel Actual:</strong></td><td>{docente.NivelActual.GetDescription()}</td></tr>");
        html.AppendLine("</table>");

        // Datos académicos
        html.AppendLine("<div class='seccion-title'>DATOS ACADÉMICOS</div>");
        html.AppendLine("<table class='info-table'>");
        html.AppendLine($"<tr><td><strong>Promedio Evaluaciones:</strong></td><td>{docente.PromedioEvaluaciones ?? 0:F2}/100</td></tr>");
        html.AppendLine($"<tr><td><strong>Horas Capacitación:</strong></td><td>{docente.HorasCapacitacion ?? 0}</td></tr>");
        html.AppendLine($"<tr><td><strong>Obras Académicas:</strong></td><td>{docente.NumeroObrasAcademicas ?? 0}</td></tr>");
        html.AppendLine($"<tr><td><strong>Meses en Investigación:</strong></td><td>{docente.MesesInvestigacion ?? 0}</td></tr>");
        
        var tiempoEnNivel = DateTime.UtcNow - docente.FechaInicioNivelActual;
        html.AppendLine($"<tr><td><strong>Tiempo en Nivel Actual:</strong></td><td>{tiempoEnNivel.TotalDays / 365.25:F1} años</td></tr>");
        html.AppendLine("</table>");

        // Estado de requisitos para ascenso
        if (!docente.NivelActual.EsNivelMaximo())
        {
            var estadoRequisitos = docente.GetEstadoRequisitos();
            var siguienteNivel = docente.NivelActual.GetSiguienteNivel();
            
            html.AppendLine($"<div class='seccion-title'>ESTADO DE REQUISITOS PARA {siguienteNivel!.Value.GetDescription()}</div>");
            html.AppendLine("<table class='info-table'>");
            html.AppendLine("<tr><th>Requisito</th><th>Estado</th></tr>");
            html.AppendLine($"<tr><td>Tiempo en nivel</td><td class='{(estadoRequisitos.CumpleTiempo ? "cumple" : "no-cumple")}'>{(estadoRequisitos.CumpleTiempo ? "✓ Cumple" : "✗ No cumple")}</td></tr>");
            html.AppendLine($"<tr><td>Obras académicas</td><td class='{(estadoRequisitos.CumpleObras ? "cumple" : "no-cumple")}'>{(estadoRequisitos.CumpleObras ? "✓ Cumple" : "✗ No cumple")}</td></tr>");
            html.AppendLine($"<tr><td>Evaluaciones</td><td class='{(estadoRequisitos.CumpleEvaluaciones ? "cumple" : "no-cumple")}'>{(estadoRequisitos.CumpleEvaluaciones ? "✓ Cumple" : "✗ No cumple")}</td></tr>");
            html.AppendLine($"<tr><td>Capacitación</td><td class='{(estadoRequisitos.CumpleCapacitacion ? "cumple" : "no-cumple")}'>{(estadoRequisitos.CumpleCapacitacion ? "✓ Cumple" : "✗ No cumple")}</td></tr>");
            html.AppendLine($"<tr><td>Investigación</td><td class='{(estadoRequisitos.CumpleInvestigacion ? "cumple" : "no-cumple")}'>{(estadoRequisitos.CumpleInvestigacion ? "✓ Cumple" : "✗ No cumple")}</td></tr>");
            html.AppendLine("</table>");

            var progreso = (estadoRequisitos.CumpleTiempo ? 1 : 0) +
                          (estadoRequisitos.CumpleObras ? 1 : 0) +
                          (estadoRequisitos.CumpleEvaluaciones ? 1 : 0) +
                          (estadoRequisitos.CumpleCapacitacion ? 1 : 0) +
                          (estadoRequisitos.CumpleInvestigacion ? 1 : 0);

            html.AppendLine($"<p><strong>Progreso:</strong> {progreso}/5 requisitos cumplidos ({(progreso * 100 / 5):F0}%)</p>");
        }

        // Historial de solicitudes
        if (solicitudes.Any())
        {
            html.AppendLine("<div class='seccion-title'>HISTORIAL DE SOLICITUDES</div>");
            html.AppendLine("<table class='info-table'>");
            html.AppendLine("<tr><th>Fecha</th><th>Nivel Solicitado</th><th>Estado</th><th>Fecha Resolución</th></tr>");

            foreach (var solicitud in solicitudes.OrderByDescending(s => s.FechaSolicitud))
            {
                var fechaResolucion = solicitud.FechaAprobacion?.ToString("dd/MM/yyyy") ?? "-";
                html.AppendLine($"<tr>");
                html.AppendLine($"<td>{solicitud.FechaSolicitud:dd/MM/yyyy}</td>");
                html.AppendLine($"<td>{solicitud.NivelSolicitado.GetDescription()}</td>");
                html.AppendLine($"<td>{solicitud.Estado}</td>");
                html.AppendLine($"<td>{fechaResolucion}</td>");
                html.AppendLine($"</tr>");
            }

            html.AppendLine("</table>");
        }

        html.AppendLine("</div>");
        return html.ToString();
    }

    public async Task<string> GenerarVistaCertificadoEstadoAsync(Guid docenteId)
    {
        var docente = await _docenteRepository.GetByIdAsync(docenteId);
        if (docente == null)
            throw new ArgumentException("Docente no encontrado");

        var html = new StringBuilder();
        html.AppendLine("<div class='reporte-container'>");
        html.AppendLine("<style>");
        html.AppendLine(@"
            .reporte-container { font-family: Arial, sans-serif; text-align: center; }
            .header { color: #8a1538; font-size: 20px; font-weight: bold; margin-bottom: 30px; }
            .certificado-title { color: #8a1538; font-size: 24px; font-weight: bold; margin: 30px 0; }
            .contenido { text-align: justify; margin: 20px 0; line-height: 1.6; }
            .firma-seccion { margin-top: 50px; }
            .membrete { background-color: #8a1538; color: white; padding: 15px; margin-bottom: 20px; }
        ");
        html.AppendLine("</style>");

        html.AppendLine("<div class='membrete'>");
        html.AppendLine("<h2>UNIVERSIDAD TÉCNICA DE AMBATO</h2>");
        html.AppendLine("<h3>VICERRECTORADO ACADÉMICO</h3>");
        html.AppendLine("<p>SISTEMA DE GESTIÓN DE ASCENSOS DOCENTES</p>");
        html.AppendLine("</div>");

        html.AppendLine("<h1 class='certificado-title'>CERTIFICADO DE ESTADO DOCENTE</h1>");

        html.AppendLine("<div class='contenido'>");
        html.AppendLine("<p>El Vicerrectorado Académico de la Universidad Técnica de Ambato</p>");
        html.AppendLine("<p><strong>CERTIFICA:</strong></p>");
        
        html.AppendLine($"<p>Que el/la docente <strong>{docente.Nombres} {docente.Apellidos}</strong>, ");
        html.AppendLine($"con cédula de ciudadanía N° <strong>{docente.Cedula}</strong>, ");
        html.AppendLine($"se encuentra actualmente en el nivel <strong>{docente.NivelActual.GetDescription()}</strong> ");
        html.AppendLine("del escalafón docente de la Universidad Técnica de Ambato.");

        if (docente.FechaInicioNivelActual != default)
        {
            html.AppendLine($"<p>El docente alcanzó su nivel actual el <strong>{docente.FechaInicioNivelActual:dd/MM/yyyy}</strong>.");
        }

        if (docente.FechaNombramiento.HasValue)
        {
            html.AppendLine($"<p>Fecha de nombramiento: <strong>{docente.FechaNombramiento.Value:dd/MM/yyyy}</strong>.</p>");
        }

        html.AppendLine("<p>El presente certificado se expide a solicitud del interesado para los fines que estime convenientes.</p>");
        html.AppendLine("</div>");

        html.AppendLine("<div class='firma-seccion'>");
        html.AppendLine($"<p>Ambato, {DateTime.Now:dd} de {DateTime.Now:MMMM} de {DateTime.Now:yyyy}</p>");
        html.AppendLine("<br><br>");
        html.AppendLine("<p>_________________________________</p>");
        html.AppendLine("<p><strong>VICERRECTOR ACADÉMICO</strong></p>");
        html.AppendLine("<p>Universidad Técnica de Ambato</p>");
        html.AppendLine("</div>");

        html.AppendLine("</div>");
        return html.ToString();
    }
}
