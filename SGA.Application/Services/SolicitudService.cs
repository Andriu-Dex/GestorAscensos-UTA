using SGA.Application.DTOs.Solicitudes;
using SGA.Application.DTOs.Documentos;
using SGA.Application.Interfaces;
using SGA.Application.Interfaces.Repositories;
using SGA.Domain.Entities;
using SGA.Domain.Enums;

namespace SGA.Application.Services;

public class SolicitudService : ISolicitudService
{
    private readonly ISolicitudAscensoRepository _solicitudRepository;
    private readonly IDocenteRepository _docenteRepository;
    private readonly IDocumentoService _documentoService;
    private readonly IDocenteService _docenteService;
    private readonly IAuditoriaService _auditoriaService;

    public SolicitudService(
        ISolicitudAscensoRepository solicitudRepository,
        IDocenteRepository docenteRepository,
        IDocumentoService documentoService,
        IDocenteService docenteService,
        IAuditoriaService auditoriaService)
    {
        _solicitudRepository = solicitudRepository;
        _docenteRepository = docenteRepository;
        _documentoService = documentoService;
        _docenteService = docenteService;
        _auditoriaService = auditoriaService;
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

        // Asociar documentos existentes a la solicitud (si es necesario)
        // Por ahora omitimos esta funcionalidad ya que los documentos
        // se asocian directamente en la base de datos

        await _auditoriaService.RegistrarAccionAsync("CREAR_SOLICITUD", 
            docente.Id.ToString(), docente.Email, "SolicitudAscenso", 
            null, $"Nivel solicitado: Titular{request.NivelSolicitado}", null);

        return await GetSolicitudByIdAsync(solicitud.Id) ?? throw new Exception("Error al recuperar solicitud creada");
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
        if (solicitud == null) return null;

        return await ConvertToDto(solicitud);
    }

    public async Task<bool> ProcesarSolicitudAsync(Guid solicitudId, ProcesarSolicitudRequest request, Guid administradorId)
    {
        var solicitud = await _solicitudRepository.GetByIdAsync(solicitudId);
        if (solicitud == null) return false;

        if (solicitud.Estado != EstadoSolicitud.Pendiente && solicitud.Estado != EstadoSolicitud.EnProceso)
            return false;

        solicitud.Estado = request.Aprobar ? EstadoSolicitud.Aprobada : EstadoSolicitud.Rechazada;
        solicitud.FechaAprobacion = DateTime.UtcNow;
        solicitud.AprobadoPorId = administradorId;
        solicitud.MotivoRechazo = request.MotivoRechazo;

        await _solicitudRepository.UpdateAsync(solicitud);

        // Si se aprueba, actualizar nivel del docente
        if (request.Aprobar)
        {
            await _docenteService.ActualizarNivelDocenteAsync(solicitud.DocenteId, solicitud.NivelSolicitado.ToString());
        }

        await _auditoriaService.RegistrarAccionAsync(
            request.Aprobar ? "APROBAR_SOLICITUD" : "RECHAZAR_SOLICITUD",
            administradorId.ToString(), null, "SolicitudAscenso",
            solicitud.Estado.ToString(), request.Aprobar ? "Aprobada" : "Rechazada", null);

        return true;
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
        
        // Convertir los documentos de la solicitud
        var documentos = new List<DocumentoDto>();
        if (solicitud.Documentos != null && solicitud.Documentos.Any())
        {
            foreach (var doc in solicitud.Documentos)
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
            NivelActual = solicitud.NivelActual.ToString(),
            NivelSolicitado = solicitud.NivelSolicitado.ToString(),
            Estado = solicitud.Estado.ToString(),
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
}
