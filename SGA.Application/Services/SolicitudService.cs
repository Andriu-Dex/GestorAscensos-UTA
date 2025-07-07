using SGA.Application.DTOs.Solicitudes;
using SGA.Application.DTOs.Documentos;
using SGA.Application.Interfaces;
using SGA.Application.Interfaces.Repositories;
using SGA.Domain.Entities;
using SGA.Domain.Enums;
using SGA.Domain.Extensions;

namespace SGA.Application.Services;

public class SolicitudService : ISolicitudService
{
    private readonly ISolicitudAscensoRepository _solicitudRepository;
    private readonly IDocenteRepository _docenteRepository;
    private readonly IDocumentoService _documentoService;
    private readonly IDocenteService _docenteService;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IDocumentoRepository _documentoRepository;
    private readonly DocumentoConversionService _documentoConversionService;
    private readonly INotificationService _notificationService;
    private readonly INotificacionTiempoRealService _notificacionTiempoReal;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IDocumentoUtilizacionService _documentoUtilizacionService;

    public SolicitudService(
        ISolicitudAscensoRepository solicitudRepository,
        IDocenteRepository docenteRepository,
        IDocumentoService documentoService,
        IDocenteService docenteService,
        IAuditoriaService auditoriaService,
        IDocumentoRepository documentoRepository,
        DocumentoConversionService documentoConversionService,
        INotificationService notificationService,
        INotificacionTiempoRealService notificacionTiempoReal,
        IUsuarioRepository usuarioRepository,
        IDocumentoUtilizacionService documentoUtilizacionService)
    {
        _solicitudRepository = solicitudRepository;
        _docenteRepository = docenteRepository;
        _documentoService = documentoService;
        _docenteService = docenteService;
        _auditoriaService = auditoriaService;
        _documentoRepository = documentoRepository;
        _documentoConversionService = documentoConversionService;
        _notificationService = notificationService;
        _notificacionTiempoReal = notificacionTiempoReal;
        _usuarioRepository = usuarioRepository;
        _documentoUtilizacionService = documentoUtilizacionService;
    }

    public async Task<SolicitudAscensoDto> CrearSolicitudAsync(Guid docenteId, CrearSolicitudRequest request)
    {
        var docente = await _docenteRepository.GetByIdAsync(docenteId);
        if (docente == null)
            throw new ArgumentException("Docente no encontrado");

        // Verificar que no tenga solicitudes activas
        if (await TieneDocumenteSolicitudActivaAsync(docenteId))
            throw new InvalidOperationException("Ya tiene una solicitud activa");

        // Validar que el nivel solicitado sea válido
        if (request.NivelSolicitado < 1 || request.NivelSolicitado > 5)
            throw new ArgumentException("Nivel solicitado inválido");

        // Convertir nivel numérico a enum
        var nivelSolicitadoEnum = (NivelTitular)request.NivelSolicitado;

        var solicitud = new SolicitudAscenso
        {
            DocenteId = docenteId,
            NivelActual = (NivelTitular)request.NivelActual,
            NivelSolicitado = nivelSolicitadoEnum,
            Estado = EstadoSolicitud.Pendiente,
            FechaSolicitud = DateTime.UtcNow,
            PromedioEvaluaciones = request.PuntajeEvaluacion,
            HorasCapacitacion = request.HorasCapacitacion,
            NumeroObrasAcademicas = request.NumeroObras,
            MesesInvestigacion = request.TiempoInvestigacion,
            TiempoEnNivelDias = request.TiempoRol * 30, // Convertir meses a días aproximados
            Observaciones = request.Observaciones
        };

        solicitud = await _solicitudRepository.CreateAsync(solicitud);

        // Asociar documentos existentes a la solicitud
        var documentosIds = new List<Guid>();
        
        // Si hay documentos seleccionados por tipo, convertirlos a documentos genéricos
        if (request.DocumentosSeleccionados != null && request.DocumentosSeleccionados.Any())
        {
            var documentosConvertidos = await _documentoConversionService.ConvertirYCrearDocumentosAsync(request.DocumentosSeleccionados);
            documentosIds.AddRange(documentosConvertidos);
        }
        
        // También incluir documentos genéricos si están presentes (legacy)
        if (request.DocumentosIds != null && request.DocumentosIds.Any())
        {
            documentosIds.AddRange(request.DocumentosIds);
        }
        
        if (documentosIds.Any())
        {
            await AsociarDocumentosASolicitudAsync(solicitud.Id, documentosIds);
            
            // Recargar la solicitud desde la base de datos para asegurar que los documentos están disponibles
            var solicitudRecargada = await _solicitudRepository.GetByIdAsync(solicitud.Id);
            if (solicitudRecargada != null)
            {
                solicitud = solicitudRecargada;
            }
        }

        await _auditoriaService.RegistrarAccionAsync("CREAR_SOLICITUD", 
            docente.Id.ToString(), docente.Email, "SolicitudAscenso", 
            null, $"Nivel solicitado: Titular{request.NivelSolicitado}", null);

        // Enviar notificación en tiempo real a los administradores
        await _notificacionTiempoReal.EnviarNotificacionAdministradoresAsync(
            "Nueva Solicitud de Ascenso",
            $"El docente {docente.NombreCompleto} ha enviado una nueva solicitud de ascenso de {solicitud.NivelActual} a {solicitud.NivelSolicitado}.",
            TipoNotificacion.NuevaSolicitud,
            $"/admin/solicitudes"
        );

        return await ConvertToDto(solicitud);
    }

    public async Task<List<SolicitudAscensoDto>> GetSolicitudesByDocenteAsync(Guid docenteId)
    {
        var solicitudes = await _solicitudRepository.GetByDocenteIdAsync(docenteId);
        var result = new List<SolicitudAscensoDto>();

        foreach (var solicitud in solicitudes)
        {
            var dto = await ConvertToDto(solicitud);
            result.Add(dto);
        }

        return result;
    }

    public async Task<List<SolicitudAscensoDto>> GetTodasLasSolicitudesAsync()
    {
        var solicitudes = await _solicitudRepository.GetAllAsync();
        var result = new List<SolicitudAscensoDto>();

        foreach (var solicitud in solicitudes)
        {
            var dto = await ConvertToDto(solicitud);
            result.Add(dto);
        }

        return result;
    }

    public async Task<SolicitudAscensoDto?> GetSolicitudByIdAsync(Guid solicitudId)
    {
        var solicitud = await _solicitudRepository.GetByIdAsync(solicitudId);
        if (solicitud == null) 
        {
            return null;
        }

        // Cargar documentos directamente para asegurar completitud
        var documentosDirectos = await _documentoRepository.GetBySolicitudIdAsync(solicitudId);
        
        var dto = await ConvertToDto(solicitud);
        
        return dto;
    }

    public async Task<bool> ProcesarSolicitudAsync(Guid solicitudId, ProcesarSolicitudRequest request, Guid administradorId)
    {
        var solicitud = await _solicitudRepository.GetByIdAsync(solicitudId);
        if (solicitud == null) return false;

        if (solicitud.Estado != EstadoSolicitud.Pendiente && solicitud.Estado != EstadoSolicitud.EnProceso)
            return false;

        // Obtener información del docente antes de procesar
        var docente = await _docenteRepository.GetByIdAsync(solicitud.DocenteId);
        if (docente == null) return false;

        // Obtener información del usuario asociado al docente para las notificaciones en tiempo real
        var usuario = await _usuarioRepository.GetByEmailAsync(docente.Email);

        var nivelAnterior = solicitud.NivelActual.ToString();
        var nivelSolicitado = solicitud.NivelSolicitado.ToString();

        solicitud.Estado = request.Aprobar ? EstadoSolicitud.Aprobada : EstadoSolicitud.Rechazada;
        solicitud.FechaAprobacion = DateTime.UtcNow;
        solicitud.AprobadoPorId = administradorId;
        solicitud.MotivoRechazo = request.MotivoRechazo;

        await _solicitudRepository.UpdateAsync(solicitud);

        // Si se aprueba, actualizar nivel del docente y enviar notificaciones
        if (request.Aprobar)
        {
            // ✅ NUEVO: Marcar documentos como utilizados antes de actualizar el nivel
            await _documentoUtilizacionService.MarcarDocumentosComoUtilizadosAsync(solicitudId);
            
            await _docenteService.ActualizarNivelDocenteAsync(solicitud.DocenteId, solicitud.NivelSolicitado.ToString());
            
            // Enviar notificación de felicitación por email
            await _notificationService.NotificarAprobacionAscensoAsync(
                docente.Email, 
                docente.NombreCompleto, 
                nivelAnterior, 
                nivelSolicitado);

            // Enviar notificación en tiempo real via SignalR
            if (usuario != null)
            {
                await _notificacionTiempoReal.NotificarAscensoAprobadoAsync(
                    usuario.Id, 
                    nivelAnterior, 
                    nivelSolicitado);
            }
        }
        else
        {
            // Enviar notificación de rechazo por email
            await _notificationService.NotificarRechazoAscensoAsync(
                docente.Email, 
                docente.NombreCompleto, 
                nivelSolicitado, 
                request.MotivoRechazo ?? "No especificado");

            // Enviar notificación en tiempo real via SignalR
            if (usuario != null)
            {
                await _notificacionTiempoReal.NotificarAscensoRechazadoAsync(
                    usuario.Id, 
                    nivelSolicitado, 
                    request.MotivoRechazo ?? "No especificado");
            }
        }

        await _auditoriaService.RegistrarAccionAsync(
            request.Aprobar ? "APROBAR_SOLICITUD" : "RECHAZAR_SOLICITUD",
            administradorId.ToString(), null, "SolicitudAscenso",
            solicitud.Estado.ToString(), request.Aprobar ? "Aprobada" : "Rechazada", null);

        return true;
    }

    public async Task<bool> CancelarSolicitudAsync(Guid solicitudId, Guid docenteId)
    {
        var solicitud = await _solicitudRepository.GetByIdAsync(solicitudId);
        if (solicitud == null) return false;

        // Verificar que la solicitud pertenece al docente
        if (solicitud.DocenteId != docenteId) return false;

        // Solo se pueden cancelar solicitudes en estado Pendiente
        if (solicitud.Estado != EstadoSolicitud.Pendiente) return false;

        // Registrar la cancelación en auditoría antes de eliminar
        var docente = await _docenteRepository.GetByIdAsync(docenteId);
        await _auditoriaService.RegistrarAccionAsync(
            "CANCELAR_SOLICITUD",
            docenteId.ToString(), 
            docente?.Email, 
            "SolicitudAscenso",
            "Pendiente", 
            "Eliminada por cancelación del docente", 
            solicitudId.ToString());

        // Eliminar la solicitud completamente de la base de datos
        var eliminada = await _solicitudRepository.DeleteAsync(solicitudId);

        return eliminada;
    }

    public async Task<bool> TieneDocumenteSolicitudActivaAsync(Guid docenteId)
    {
        return await _solicitudRepository.TieneDocumenteSolicitudActivaAsync(docenteId);
    }

    public async Task<byte[]?> DescargarDocumentoAsync(Guid documentoId)
    {
        return await _documentoService.DescargarDocumentoAsync(documentoId);
    }

    private async Task<SolicitudAscensoDto> ConvertToDto(SolicitudAscenso solicitud)
    {
        var docente = await _docenteRepository.GetByIdAsync(solicitud.DocenteId);
        
        // Para obtener información del aprobador, usamos el AprobadoPor de la navegación
        string? aprobadorNombre = null;
        if (solicitud.AprobadoPor != null && solicitud.AprobadoPor.Docente != null)
        {
            aprobadorNombre = solicitud.AprobadoPor.Docente.NombreCompleto;
        }
        
        // Obtener documentos de la solicitud directamente desde el repositorio
        // para asegurar que siempre se cargan correctamente
        var documentosEntity = await _documentoRepository.GetBySolicitudIdAsync(solicitud.Id);
        var documentos = new List<DocumentoDto>();
        
        // Usar los documentos de la consulta directa
        foreach (var doc in documentosEntity)
        {
            documentos.Add(new DocumentoDto
            {
                Id = doc.Id,
                NombreArchivo = doc.NombreArchivo,
                Nombre = doc.NombreArchivo,
                TamanoArchivo = doc.TamanoArchivo,
                TipoDocumento = doc.TipoDocumento.ToString(),
                FechaCreacion = doc.FechaCreacion
            });
        }
        
        return new SolicitudAscensoDto
        {
            Id = solicitud.Id,
            DocenteId = solicitud.DocenteId,
            DocenteNombre = docente?.NombreCompleto ?? "N/A",
            DocenteNombres = docente?.Nombres ?? "N/A",
            DocenteApellidos = docente?.Apellidos ?? "N/A", 
            DocenteEmail = docente?.Email ?? "N/A",
            DocenteCedula = docente?.Cedula ?? "N/A",
            NivelActual = solicitud.NivelActual.GetDescription(),
            NivelSolicitado = solicitud.NivelSolicitado.GetDescription(),
            Estado = solicitud.Estado.GetDescription(),
            MotivoRechazo = solicitud.MotivoRechazo,
            FechaSolicitud = solicitud.FechaSolicitud,
            FechaAprobacion = solicitud.FechaAprobacion,
            AprobadoPor = aprobadorNombre,
            PromedioEvaluaciones = solicitud.PromedioEvaluaciones,
            HorasCapacitacion = solicitud.HorasCapacitacion,
            NumeroObrasAcademicas = solicitud.NumeroObrasAcademicas,
            MesesInvestigacion = solicitud.MesesInvestigacion,
            TiempoEnNivelDias = solicitud.TiempoEnNivelDias,
            Documentos = documentos
        };
    }

    private async Task AsociarDocumentosASolicitudAsync(Guid solicitudId, List<Guid> documentosIds)
    {
        var documentosAsociados = 0;
        
        foreach (var documentoId in documentosIds)
        {
            var documento = await _documentoRepository.GetByIdAsync(documentoId);
            if (documento != null)
            {
                // Verificar que el documento no esté ya asociado a otra solicitud
                if (!documento.SolicitudAscensoId.HasValue || documento.SolicitudAscensoId == solicitudId)
                {
                    // Asociar el documento a la solicitud
                    documento.SolicitudAscensoId = solicitudId;
                    await _documentoRepository.UpdateAsync(documento);
                    documentosAsociados++;
                }
            }
        }
        
        // Verificar que los documentos se asociaron correctamente
        var documentosVerificacion = await _documentoRepository.GetBySolicitudIdAsync(solicitudId);
    }

    public async Task<bool> ReenviarSolicitudAsync(Guid solicitudId, Guid docenteId)
    {
        try
        {
            var solicitud = await _solicitudRepository.GetByIdAsync(solicitudId);
            if (solicitud == null)
                return false;

            // Verificar que la solicitud pertenece al docente
            if (solicitud.DocenteId != docenteId)
                return false;

            // Solo permitir reenviar si está rechazada
            if (solicitud.Estado != EstadoSolicitud.Rechazada)
                return false;

            // Cambiar estado a pendiente y limpiar datos de revisión
            solicitud.Estado = EstadoSolicitud.Pendiente;
            solicitud.MotivoRechazo = null;
            solicitud.FechaAprobacion = null;
            solicitud.AprobadoPorId = null;
            solicitud.FechaSolicitud = DateTime.UtcNow; // Actualizar fecha de solicitud

            await _solicitudRepository.UpdateAsync(solicitud);

            // Registrar auditoría
            await _auditoriaService.RegistrarAccionAsync(
                "REENVIAR_SOLICITUD_ASCENSO",
                docenteId.ToString(),
                null,
                "SolicitudAscenso",
                "Rechazada",
                "Pendiente",
                null
            );

            // Enviar notificación a los administradores sobre el reenvío
            var docente = await _docenteRepository.GetByIdAsync(docenteId);
            if (docente != null)
            {
                await _notificacionTiempoReal.EnviarNotificacionAdministradoresAsync(
                    "Solicitud de Ascenso Reenviada",
                    $"El docente {docente.NombreCompleto} ha reenviado su solicitud de ascenso para revisión.",
                    TipoNotificacion.NuevaSolicitud,
                    $"/admin/solicitudes"
                );
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> ReenviarSolicitudConDocumentosAsync(Guid solicitudId, Guid docenteId, Dictionary<string, List<string>> documentosSeleccionados)
    {
        try
        {
            var solicitud = await _solicitudRepository.GetByIdAsync(solicitudId);
            if (solicitud == null)
                return false;

            // Verificar que la solicitud pertenece al docente
            if (solicitud.DocenteId != docenteId)
                return false;

            // Solo permitir reenviar si está rechazada
            if (solicitud.Estado != EstadoSolicitud.Rechazada)
                return false;

            // Primero eliminamos las asociaciones existentes de documentos
            var documentosExistentes = await _documentoRepository.GetBySolicitudIdAsync(solicitudId);
            foreach (var documento in documentosExistentes)
            {
                documento.SolicitudAscensoId = null;
                await _documentoRepository.UpdateAsync(documento);
            }

            // Convertir y crear nuevos documentos
            var documentosIds = new List<Guid>();
            if (documentosSeleccionados != null && documentosSeleccionados.Any())
            {
                var documentosConvertidos = await _documentoConversionService.ConvertirYCrearDocumentosAsync(documentosSeleccionados);
                documentosIds.AddRange(documentosConvertidos);
                
                // Asociar los nuevos documentos a la solicitud
                if (documentosIds.Any())
                {
                    await AsociarDocumentosASolicitudAsync(solicitudId, documentosIds);
                }
            }

            // Cambiar estado a pendiente y limpiar datos de revisión
            solicitud.Estado = EstadoSolicitud.Pendiente;
            solicitud.MotivoRechazo = null;
            solicitud.FechaAprobacion = null;
            solicitud.AprobadoPorId = null;
            solicitud.FechaSolicitud = DateTime.UtcNow; // Actualizar fecha de solicitud

            await _solicitudRepository.UpdateAsync(solicitud);

            // Registrar auditoría
            await _auditoriaService.RegistrarAccionAsync(
                "REENVIAR_SOLICITUD_ASCENSO_CON_DOCUMENTOS",
                docenteId.ToString(),
                null,
                "SolicitudAscenso",
                "Rechazada",
                $"Pendiente - Documentos actualizados: {documentosIds.Count} documentos asociados",
                null // DireccionIP - se puede obtener desde HttpContext si es necesario
            );

            // Enviar notificación a los administradores sobre el reenvío con documentos actualizados
            var docente = await _docenteRepository.GetByIdAsync(docenteId);
            if (docente != null)
            {
                await _notificacionTiempoReal.EnviarNotificacionAdministradoresAsync(
                    "Solicitud de Ascenso Reenviada con Documentos",
                    $"El docente {docente.NombreCompleto} ha reenviado su solicitud de ascenso con documentos actualizados ({documentosIds.Count} documentos).",
                    TipoNotificacion.NuevaSolicitud,
                    $"/admin/solicitudes"
                );
            }

            return true;
        }
        catch (Exception ex)
        {
            // Log the exception for debugging but don't expose internal details
            Console.WriteLine($"Error in ReenviarSolicitudConDocumentosAsync: {ex.Message}");
            return false;
        }
    }
}
