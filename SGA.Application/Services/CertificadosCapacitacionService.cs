using Microsoft.Extensions.Logging;
using SGA.Application.DTOs;
using SGA.Application.Interfaces;
using SGA.Application.Interfaces.Repositories;
using SGA.Domain.Entities;
using System.Text;

namespace SGA.Application.Services;

/// <summary>
/// Servicio para gestión de certificados de capacitación
/// </summary>
public class CertificadosCapacitacionService : ICertificadosCapacitacionService
{
    private readonly IDocenteRepository _docenteRepository;
    private readonly IRepository<SolicitudCertificadoCapacitacion> _solicitudCertificadoRepository;
    private readonly INotificationService _notificationService;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IPDFCompressionService _pdfCompressionService;
    private readonly ILogger<CertificadosCapacitacionService> _logger;

    public CertificadosCapacitacionService(
        IDocenteRepository docenteRepository,
        IRepository<SolicitudCertificadoCapacitacion> solicitudCertificadoRepository,
        INotificationService notificationService,
        IAuditoriaService auditoriaService,
        IPDFCompressionService pdfCompressionService,
        ILogger<CertificadosCapacitacionService> logger)
    {
        _docenteRepository = docenteRepository;
        _solicitudCertificadoRepository = solicitudCertificadoRepository;
        _notificationService = notificationService;
        _auditoriaService = auditoriaService;
        _pdfCompressionService = pdfCompressionService;
        _logger = logger;
    }

    public async Task<ResponseCertificadosCapacitacionDto> GetCertificadosDocenteAsync(string cedula)
    {
        try
        {
            _logger.LogInformation("Obteniendo certificados para cédula: {Cedula}", cedula);
            
            // Obtener todas las solicitudes de certificados del docente
            var solicitudes = await _solicitudCertificadoRepository.GetAllAsync();
            var solicitudesDocente = solicitudes
                .Where(s => s.DocenteCedula == cedula)
                .OrderByDescending(s => s.FechaCreacion)
                .ToList();

            _logger.LogInformation("Encontradas {Count} solicitudes para cédula: {Cedula}", solicitudesDocente.Count, cedula);

            var certificadosDto = solicitudesDocente.Select(s => new CertificadoCapacitacionDetalleDto
            {
                Id = s.Id,
                SolicitudId = s.Id,
                NombreCurso = s.NombreCurso,
                InstitucionOfertante = s.InstitucionOfertante,
                TipoCapacitacion = s.TipoCapacitacion,
                FechaInicio = s.FechaInicio,
                FechaFin = s.FechaFin,
                HorasDuracion = s.HorasDuracion,
                Modalidad = s.Modalidad,
                NumeroRegistro = s.NumeroRegistro,
                AreaTematica = s.AreaTematica,
                Descripcion = s.Descripcion,
                ArchivoNombre = s.ArchivoNombre,
                TieneArchivo = !string.IsNullOrEmpty(s.ArchivoNombre),
                FechaCreacion = s.FechaCreacion,
                FechaActualizacion = s.FechaModificacion,
                FechaRevision = s.FechaRevision,
                Estado = s.Estado,
                ComentariosRevision = s.ComentariosRevision,
                MotivoRechazo = s.MotivoRechazo
            }).ToList();

            return new ResponseCertificadosCapacitacionDto
            {
                Exitoso = true,
                Mensaje = "Certificados obtenidos correctamente",
                Certificados = certificadosDto,
                TotalCertificados = certificadosDto.Count
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener certificados para cédula {Cedula}", cedula);
            return new ResponseCertificadosCapacitacionDto
            {
                Exitoso = false,
                Mensaje = $"Error al obtener certificados: {ex.Message}",
                Certificados = new List<CertificadoCapacitacionDetalleDto>(),
                TotalCertificados = 0
            };
        }
    }

    public async Task<ResponseCertificadosCapacitacionDto> SolicitarNuevosCertificadosAsync(string cedula, SolicitarCertificadosCapacitacionDto solicitud)
    {
        try
        {
            _logger.LogInformation($"Iniciando solicitud de certificados para cédula: {cedula}");

            var docente = await _docenteRepository.GetByCedulaAsync(cedula);
            if (docente == null)
            {
                _logger.LogWarning($"Docente no encontrado para cédula: {cedula}");
                return new ResponseCertificadosCapacitacionDto
                {
                    Exitoso = false,
                    Mensaje = "Docente no encontrado"
                };
            }

            var grupoId = Guid.NewGuid();
            var certificadosSolicitados = new List<CertificadoCapacitacionDetalleDto>();

            _logger.LogInformation($"Procesando {solicitud.Certificados?.Count ?? 0} certificados para solicitud");

            if (solicitud.Certificados != null)
            {
                for (int i = 0; i < solicitud.Certificados.Count; i++)
                {
                    var certificado = solicitud.Certificados[i];
                    _logger.LogInformation($"Procesando certificado {i + 1}/{solicitud.Certificados.Count}: {certificado.NombreCurso}");

                    try
                    {
                        var nuevaSolicitud = new SolicitudCertificadoCapacitacion();

                        // Validar y establecer propiedades básicas
                        var nombreCurso = certificado.NombreCurso?.Trim() ?? "";
                        if (nombreCurso.Length > 500)
                        {
                            _logger.LogWarning($"Nombre del curso excede 500 caracteres ({nombreCurso.Length})");
                            nombreCurso = nombreCurso.Substring(0, 500);
                        }

                        var institucion = certificado.InstitucionOfertante?.Trim() ?? "";
                        if (institucion.Length > 255)
                        {
                            _logger.LogWarning($"Institución excede 255 caracteres ({institucion.Length})");
                            institucion = institucion.Substring(0, 255);
                        }

                        var tipoCapacitacion = certificado.TipoCapacitacion?.Trim() ?? "";
                        if (tipoCapacitacion.Length > 100)
                        {
                            _logger.LogWarning($"Tipo de capacitación excede 100 caracteres ({tipoCapacitacion.Length})");
                            tipoCapacitacion = tipoCapacitacion.Substring(0, 100);
                        }

                        var modalidad = certificado.Modalidad?.Trim() ?? "";
                        if (modalidad.Length > 50)
                        {
                            _logger.LogWarning($"Modalidad excede 50 caracteres ({modalidad.Length})");
                            modalidad = modalidad.Substring(0, 50);
                        }

                        var numeroRegistro = string.IsNullOrWhiteSpace(certificado.NumeroRegistro) ? null : certificado.NumeroRegistro.Trim();
                        if (numeroRegistro != null && numeroRegistro.Length > 100)
                        {
                            _logger.LogWarning($"Número de registro excede 100 caracteres ({numeroRegistro.Length})");
                            numeroRegistro = numeroRegistro.Substring(0, 100);
                        }

                        var areaTematica = string.IsNullOrWhiteSpace(certificado.AreaTematica) ? null : certificado.AreaTematica.Trim();
                        if (areaTematica != null && areaTematica.Length > 200)
                        {
                            _logger.LogWarning($"Área temática excede 200 caracteres ({areaTematica.Length})");
                            areaTematica = areaTematica.Substring(0, 200);
                        }

                        var descripcion = string.IsNullOrWhiteSpace(certificado.Descripcion) ? null : certificado.Descripcion.Trim();
                        if (descripcion != null && descripcion.Length > 2000)
                        {
                            _logger.LogWarning($"Descripción excede 2000 caracteres ({descripcion.Length})");
                            descripcion = descripcion.Substring(0, 2000);
                        }

                        var comentariosSolicitud = string.IsNullOrWhiteSpace(solicitud.Comentarios) ? null : solicitud.Comentarios.Trim();
                        if (comentariosSolicitud != null && comentariosSolicitud.Length > 1000)
                        {
                            _logger.LogWarning($"Comentarios de solicitud exceden 1000 caracteres ({comentariosSolicitud.Length})");
                            comentariosSolicitud = comentariosSolicitud.Substring(0, 1000);
                        }

                        // Establecer propiedades
                        nuevaSolicitud.DocenteId = docente.Id;
                        nuevaSolicitud.DocenteCedula = cedula;
                        nuevaSolicitud.SolicitudGrupoId = grupoId;
                        nuevaSolicitud.NombreCurso = nombreCurso;
                        nuevaSolicitud.InstitucionOfertante = institucion;
                        nuevaSolicitud.TipoCapacitacion = tipoCapacitacion;
                        nuevaSolicitud.FechaInicio = certificado.FechaInicio;
                        nuevaSolicitud.FechaFin = certificado.FechaFin;
                        nuevaSolicitud.HorasDuracion = certificado.HorasDuracion;
                        nuevaSolicitud.Modalidad = modalidad;
                        nuevaSolicitud.NumeroRegistro = numeroRegistro;
                        nuevaSolicitud.AreaTematica = areaTematica;
                        nuevaSolicitud.Descripcion = descripcion;
                        nuevaSolicitud.Estado = "Pendiente";
                        nuevaSolicitud.ComentariosSolicitud = comentariosSolicitud;

                        // Manejar archivo PDF
                        if (!string.IsNullOrEmpty(certificado.ArchivoContenido) && !string.IsNullOrEmpty(certificado.ArchivoNombre))
                        {
                            try
                            {
                                var contenidoBytes = Convert.FromBase64String(certificado.ArchivoContenido);
                                var archivoNombre = certificado.ArchivoNombre.Trim();
                                var archivoTipo = certificado.ArchivoTipo ?? "application/pdf";

                                // Validar tamaño (máximo 10MB)
                                if (contenidoBytes.Length > 10 * 1024 * 1024)
                                {
                                    throw new Exception($"El archivo {archivoNombre} excede el tamaño máximo permitido de 10MB");
                                }

                                // Validar que sea PDF usando el servicio de compresión
                                if (!_pdfCompressionService.ValidarPDF(contenidoBytes))
                                {
                                    throw new Exception($"El archivo {archivoNombre} no es un PDF válido");
                                }

                                // Comprimir PDF antes de almacenar en BD
                                var contenidoComprimido = await _pdfCompressionService.ComprimirPDFAsync(contenidoBytes);
                                var estadisticas = _pdfCompressionService.ObtenerEstadisticasCompresion(contenidoBytes, contenidoComprimido);

                                nuevaSolicitud.ArchivoNombre = archivoNombre;
                                nuevaSolicitud.ArchivoContenido = contenidoComprimido;
                                nuevaSolicitud.ArchivoTipo = archivoTipo;
                                nuevaSolicitud.ArchivoTamano = contenidoBytes.Length;
                                nuevaSolicitud.ArchivoTamanoComprimido = estadisticas.tamahoComprimido;
                                nuevaSolicitud.ArchivoEstaComprimido = true;

                                _logger.LogDebug("Archivo PDF comprimido y almacenado en BD: {Nombre}, Compresión: {Porcentaje:F2}%", 
                                    archivoNombre, estadisticas.porcentajeCompresion);
                            }
                            catch (Exception archivoEx)
                            {
                                _logger.LogError(archivoEx, "Error al procesar archivo para certificado: {NombreCurso}", certificado.NombreCurso);
                                throw new Exception($"Error al procesar archivo: {archivoEx.Message}", archivoEx);
                            }
                        }

                        // Guardar en base de datos
                        await _solicitudCertificadoRepository.AddAsync(nuevaSolicitud);

                        certificadosSolicitados.Add(new CertificadoCapacitacionDetalleDto
                        {
                            Id = nuevaSolicitud.Id,
                            NombreCurso = nuevaSolicitud.NombreCurso,
                            InstitucionOfertante = nuevaSolicitud.InstitucionOfertante,
                            TipoCapacitacion = nuevaSolicitud.TipoCapacitacion,
                            FechaInicio = nuevaSolicitud.FechaInicio,
                            FechaFin = nuevaSolicitud.FechaFin,
                            HorasDuracion = nuevaSolicitud.HorasDuracion,
                            Modalidad = nuevaSolicitud.Modalidad,
                            NumeroRegistro = nuevaSolicitud.NumeroRegistro,
                            AreaTematica = nuevaSolicitud.AreaTematica,
                            Descripcion = nuevaSolicitud.Descripcion,
                            ArchivoNombre = nuevaSolicitud.ArchivoNombre,
                            TieneArchivo = !string.IsNullOrEmpty(certificado.ArchivoContenido),
                            FechaCreacion = DateTime.UtcNow,
                            Estado = "Pendiente"
                        });
                    }
                    catch (Exception certificadoEx)
                    {
                        _logger.LogError(certificadoEx, $"Error al procesar certificado {i + 1}: {certificado.NombreCurso}");
                        throw new Exception($"Error al procesar certificado '{certificado.NombreCurso}': {certificadoEx.Message}", certificadoEx);
                    }
                }
            }

            // Registrar auditoría
            await _auditoriaService.RegistrarAccionAsync(
                "SOLICITAR_NUEVOS_CERTIFICADOS",
                docente.Id.ToString(),
                docente.Email,
                "SolicitudCertificadoCapacitacion",
                null,
                $"Solicitados {solicitud.Certificados?.Count ?? 0} nuevos certificados de capacitación",
                grupoId.ToString()
            );

            // Enviar notificación a administradores
            _logger.LogInformation("Enviando notificación a administradores");
            await _notificationService.NotificarNuevaSolicitudCertificadosAsync(docente.NombreCompleto, solicitud.Certificados?.Count ?? 0);

            _logger.LogInformation($"Solicitud de certificados completada exitosamente para {docente.NombreCompleto}");

            return new ResponseCertificadosCapacitacionDto
            {
                Exitoso = true,
                Mensaje = $"Solicitud enviada correctamente. Se han registrado {certificadosSolicitados.Count} certificados para revisión.",
                Certificados = certificadosSolicitados,
                TotalCertificados = certificadosSolicitados.Count
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al solicitar nuevos certificados para cédula {Cedula}", cedula);
            return new ResponseCertificadosCapacitacionDto
            {
                Exitoso = false,
                Mensaje = $"Error al procesar la solicitud: {ex.Message}"
            };
        }
    }

    public async Task<ResponseCertificadosCapacitacionDto> GetSolicitudesPendientesAsync(string cedula)
    {
        try
        {
            var solicitudes = await _solicitudCertificadoRepository.GetAllAsync();
            var solicitudesDocente = solicitudes
                .Where(s => s.DocenteCedula == cedula && s.Estado != "Aprobada")
                .OrderByDescending(s => s.FechaCreacion)
                .ToList();

            var certificadosDto = solicitudesDocente.Select(s => new CertificadoCapacitacionDetalleDto
            {
                Id = s.Id,
                SolicitudId = s.Id,
                NombreCurso = s.NombreCurso,
                InstitucionOfertante = s.InstitucionOfertante,
                TipoCapacitacion = s.TipoCapacitacion,
                FechaInicio = s.FechaInicio,
                FechaFin = s.FechaFin,
                HorasDuracion = s.HorasDuracion,
                Modalidad = s.Modalidad,
                NumeroRegistro = s.NumeroRegistro,
                AreaTematica = s.AreaTematica,
                Descripcion = s.Descripcion,
                ArchivoNombre = s.ArchivoNombre,
                TieneArchivo = !string.IsNullOrEmpty(s.ArchivoNombre),
                FechaCreacion = s.FechaCreacion,
                FechaActualizacion = s.FechaModificacion,
                FechaRevision = s.FechaRevision,
                Estado = s.Estado,
                ComentariosRevision = s.ComentariosRevision,
                MotivoRechazo = s.MotivoRechazo
            }).ToList();

            return new ResponseCertificadosCapacitacionDto
            {
                Exitoso = true,
                Mensaje = "Solicitudes pendientes obtenidas correctamente",
                Certificados = certificadosDto,
                TotalCertificados = certificadosDto.Count
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener solicitudes pendientes para cédula {Cedula}", cedula);
            return new ResponseCertificadosCapacitacionDto
            {
                Exitoso = false,
                Mensaje = $"Error al obtener solicitudes: {ex.Message}"
            };
        }
    }

    public async Task<ResponseCertificadosCapacitacionDto> GetTodasSolicitudesDocenteAsync(string cedula)
    {
        try
        {
            var solicitudes = await _solicitudCertificadoRepository.GetAllAsync();
            var solicitudesDocente = solicitudes
                .Where(s => s.DocenteCedula == cedula)
                .OrderByDescending(s => s.FechaCreacion)
                .ToList();

            var certificadosDto = solicitudesDocente.Select(s => new CertificadoCapacitacionDetalleDto
            {
                Id = s.Id,
                SolicitudId = s.Id,
                NombreCurso = s.NombreCurso,
                InstitucionOfertante = s.InstitucionOfertante,
                TipoCapacitacion = s.TipoCapacitacion,
                FechaInicio = s.FechaInicio,
                FechaFin = s.FechaFin,
                HorasDuracion = s.HorasDuracion,
                Modalidad = s.Modalidad,
                NumeroRegistro = s.NumeroRegistro,
                AreaTematica = s.AreaTematica,
                Descripcion = s.Descripcion,
                ArchivoNombre = s.ArchivoNombre,
                TieneArchivo = !string.IsNullOrEmpty(s.ArchivoNombre),
                FechaCreacion = s.FechaCreacion,
                FechaActualizacion = s.FechaModificacion,
                FechaRevision = s.FechaRevision,
                Estado = s.Estado,
                ComentariosRevision = s.ComentariosRevision,
                MotivoRechazo = s.MotivoRechazo
            }).ToList();

            return new ResponseCertificadosCapacitacionDto
            {
                Exitoso = true,
                Mensaje = "Todas las solicitudes obtenidas correctamente",
                Certificados = certificadosDto,
                TotalCertificados = certificadosDto.Count
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las solicitudes para cédula {Cedula}", cedula);
            return new ResponseCertificadosCapacitacionDto
            {
                Exitoso = false,
                Mensaje = $"Error al obtener solicitudes: {ex.Message}"
            };
        }
    }

    public async Task<byte[]?> VisualizarArchivoCertificadoAsync(Guid solicitudId, string cedula)
    {
        try
        {
            var solicitud = await _solicitudCertificadoRepository.GetByIdAsync(solicitudId);
            if (solicitud == null || solicitud.DocenteCedula != cedula)
            {
                return null;
            }

            if (solicitud.ArchivoContenido == null || solicitud.ArchivoContenido.Length == 0)
            {
                return null;
            }

            // Descomprimir el PDF desde la base de datos
            var contenidoDescomprimido = await _pdfCompressionService.DescomprimirPDFAsync(solicitud.ArchivoContenido);
            return contenidoDescomprimido;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al visualizar archivo de certificado {SolicitudId}", solicitudId);
            return null;
        }
    }

    public async Task<byte[]?> DescargarArchivoCertificadoAsync(Guid solicitudId)
    {
        try
        {
            var solicitud = await _solicitudCertificadoRepository.GetByIdAsync(solicitudId);
            if (solicitud == null || solicitud.Estado != "Aprobada")
            {
                return null;
            }

            if (solicitud.ArchivoContenido == null || solicitud.ArchivoContenido.Length == 0)
            {
                return null;
            }

            // Descomprimir el PDF desde la base de datos
            var contenidoDescomprimido = await _pdfCompressionService.DescomprimirPDFAsync(solicitud.ArchivoContenido);
            return contenidoDescomprimido;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al descargar archivo de certificado {SolicitudId}", solicitudId);
            return null;
        }
    }

    // Métodos de gestión de documentos del usuario
    public async Task<ResponseGenericoCertificadoDto> EliminarSolicitudCertificadoAsync(Guid solicitudId, string cedula)
    {
        try
        {
            var solicitud = await _solicitudCertificadoRepository.GetByIdAsync(solicitudId);
            if (solicitud == null)
            {
                return new ResponseGenericoCertificadoDto
                {
                    Exitoso = false,
                    Mensaje = "Solicitud no encontrada"
                };
            }

            if (solicitud.DocenteCedula != cedula)
            {
                return new ResponseGenericoCertificadoDto
                {
                    Exitoso = false,
                    Mensaje = "No tiene permisos para eliminar esta solicitud"
                };
            }

            if (solicitud.Estado != "Pendiente" && solicitud.Estado != "Rechazada")
            {
                return new ResponseGenericoCertificadoDto
                {
                    Exitoso = false,
                    Mensaje = "Solo se pueden eliminar solicitudes pendientes o rechazadas"
                };
            }

            // Eliminar de base de datos (el archivo PDF almacenado en BD se elimina automáticamente)
            await _solicitudCertificadoRepository.DeleteAsync(solicitud.Id);

            await _auditoriaService.RegistrarAccionAsync(
                "ELIMINAR_SOLICITUD_CERTIFICADO",
                cedula,
                "SolicitudCertificadoCapacitacion",
                solicitud.Id.ToString(),
                solicitud.Estado,
                "Eliminada",
                "Usuario"
            );

            return new ResponseGenericoCertificadoDto
            {
                Exitoso = true,
                Mensaje = "Solicitud eliminada correctamente"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar solicitud de certificado {SolicitudId}", solicitudId);
            return new ResponseGenericoCertificadoDto
            {
                Exitoso = false,
                Mensaje = $"Error al eliminar solicitud: {ex.Message}"
            };
        }
    }

    public async Task<ResponseGenericoCertificadoDto> EditarMetadatosCertificadoAsync(Guid solicitudId, string cedula, EditarMetadatosCertificadoDto metadatos)
    {
        try
        {
            var solicitud = await _solicitudCertificadoRepository.GetByIdAsync(solicitudId);
            if (solicitud == null || solicitud.DocenteCedula != cedula)
            {
                return new ResponseGenericoCertificadoDto
                {
                    Exitoso = false,
                    Mensaje = "Solicitud no encontrada o sin permisos"
                };
            }

            if (solicitud.Estado == "Aprobada")
            {
                return new ResponseGenericoCertificadoDto
                {
                    Exitoso = false,
                    Mensaje = "No se pueden editar certificados aprobados"
                };
            }

            var estadoAnterior = $"{solicitud.NombreCurso} - {solicitud.Estado}";

            // Actualizar metadatos
            if (!string.IsNullOrEmpty(metadatos.NombreCurso))
                solicitud.NombreCurso = metadatos.NombreCurso.Trim().Substring(0, Math.Min(500, metadatos.NombreCurso.Length));

            if (!string.IsNullOrEmpty(metadatos.InstitucionOfertante))
                solicitud.InstitucionOfertante = metadatos.InstitucionOfertante.Trim().Substring(0, Math.Min(255, metadatos.InstitucionOfertante.Length));

            if (!string.IsNullOrEmpty(metadatos.TipoCapacitacion))
                solicitud.TipoCapacitacion = metadatos.TipoCapacitacion.Trim().Substring(0, Math.Min(100, metadatos.TipoCapacitacion.Length));

            if (metadatos.FechaInicio.HasValue)
                solicitud.FechaInicio = metadatos.FechaInicio.Value;

            if (metadatos.FechaFin.HasValue)
                solicitud.FechaFin = metadatos.FechaFin.Value;

            if (metadatos.HorasDuracion.HasValue)
                solicitud.HorasDuracion = metadatos.HorasDuracion.Value;

            if (!string.IsNullOrEmpty(metadatos.Modalidad))
                solicitud.Modalidad = metadatos.Modalidad.Trim().Substring(0, Math.Min(50, metadatos.Modalidad.Length));

            if (metadatos.NumeroRegistro != null)
                solicitud.NumeroRegistro = string.IsNullOrWhiteSpace(metadatos.NumeroRegistro) ? null : metadatos.NumeroRegistro.Trim().Substring(0, Math.Min(100, metadatos.NumeroRegistro.Length));

            if (metadatos.AreaTematica != null)
                solicitud.AreaTematica = string.IsNullOrWhiteSpace(metadatos.AreaTematica) ? null : metadatos.AreaTematica.Trim().Substring(0, Math.Min(200, metadatos.AreaTematica.Length));

            if (metadatos.Descripcion != null)
                solicitud.Descripcion = string.IsNullOrWhiteSpace(metadatos.Descripcion) ? null : metadatos.Descripcion.Trim().Substring(0, Math.Min(2000, metadatos.Descripcion.Length));

            solicitud.FechaModificacion = DateTime.UtcNow;

            await _solicitudCertificadoRepository.UpdateAsync(solicitud);

            var estadoNuevo = $"{solicitud.NombreCurso} - {solicitud.Estado}";

            await _auditoriaService.RegistrarAccionAsync(
                "EDITAR_METADATOS_CERTIFICADO",
                cedula,
                "SolicitudCertificadoCapacitacion",
                solicitud.Id.ToString(),
                estadoAnterior,
                estadoNuevo,
                "Usuario"
            );

            return new ResponseGenericoCertificadoDto
            {
                Exitoso = true,
                Mensaje = "Metadatos actualizados correctamente"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al editar metadatos de certificado {SolicitudId}", solicitudId);
            return new ResponseGenericoCertificadoDto
            {
                Exitoso = false,
                Mensaje = $"Error al editar metadatos: {ex.Message}"
            };
        }
    }

    public async Task<ResponseGenericoCertificadoDto> ReemplazarArchivoCertificadoAsync(Guid solicitudId, string cedula, ReemplazarArchivoCertificadoDto archivo)
    {
        try
        {
            var solicitud = await _solicitudCertificadoRepository.GetByIdAsync(solicitudId);
            if (solicitud == null || solicitud.DocenteCedula != cedula)
            {
                return new ResponseGenericoCertificadoDto
                {
                    Exitoso = false,
                    Mensaje = "Solicitud no encontrada o sin permisos"
                };
            }

            if (solicitud.Estado != "Pendiente" && solicitud.Estado != "Rechazada")
            {
                return new ResponseGenericoCertificadoDto
                {
                    Exitoso = false,
                    Mensaje = "Solo se pueden reemplazar archivos en solicitudes pendientes o rechazadas"
                };
            }

            try
            {
                var contenidoBytes = Convert.FromBase64String(archivo.ArchivoContenido);

                // Validar tamaño
                if (contenidoBytes.Length > 10 * 1024 * 1024)
                {
                    return new ResponseGenericoCertificadoDto
                    {
                        Exitoso = false,
                        Mensaje = "El archivo excede el tamaño máximo permitido de 10MB"
                    };
                }

                // Validar que sea PDF usando el servicio de compresión
                if (!_pdfCompressionService.ValidarPDF(contenidoBytes))
                {
                    return new ResponseGenericoCertificadoDto
                    {
                        Exitoso = false,
                        Mensaje = "El archivo debe ser un PDF válido"
                    };
                }

                // Comprimir el nuevo PDF
                var contenidoComprimido = await _pdfCompressionService.ComprimirPDFAsync(contenidoBytes);
                var estadisticas = _pdfCompressionService.ObtenerEstadisticasCompresion(contenidoBytes, contenidoComprimido);

                // Actualizar solicitud (el archivo anterior en BD se reemplaza automáticamente)
                solicitud.ArchivoNombre = archivo.ArchivoNombre;
                solicitud.ArchivoContenido = contenidoComprimido;
                solicitud.ArchivoTipo = archivo.ArchivoTipo;
                solicitud.ArchivoTamano = contenidoBytes.Length;
                solicitud.ArchivoTamanoComprimido = estadisticas.tamahoComprimido;
                solicitud.ArchivoEstaComprimido = true;
                solicitud.FechaModificacion = DateTime.UtcNow;

                _logger.LogDebug("Archivo PDF reemplazado y comprimido en BD: {Nombre}, Compresión: {Porcentaje:F2}%", 
                    archivo.ArchivoNombre, estadisticas.porcentajeCompresion);

                // Si estaba rechazada, cambiar a pendiente
                if (solicitud.Estado == "Rechazada")
                {
                    solicitud.Estado = "Pendiente";
                    solicitud.MotivoRechazo = null;
                    solicitud.FechaRevision = null;
                    solicitud.RevisadoPorId = null;
                }

                await _solicitudCertificadoRepository.UpdateAsync(solicitud);

                await _auditoriaService.RegistrarAccionAsync(
                    "REEMPLAZAR_ARCHIVO_CERTIFICADO",
                    cedula,
                    "SolicitudCertificadoCapacitacion",
                    solicitud.Id.ToString(),
                    "Archivo reemplazado",
                    solicitud.Estado,
                    "Usuario"
                );

                return new ResponseGenericoCertificadoDto
                {
                    Exitoso = true,
                    Mensaje = "Archivo reemplazado correctamente"
                };
            }
            catch (FormatException)
            {
                return new ResponseGenericoCertificadoDto
                {
                    Exitoso = false,
                    Mensaje = "El archivo no está en formato Base64 válido"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al reemplazar archivo de certificado {SolicitudId}", solicitudId);
            return new ResponseGenericoCertificadoDto
            {
                Exitoso = false,
                Mensaje = $"Error al reemplazar archivo: {ex.Message}"
            };
        }
    }

    public async Task<ResponseGenericoCertificadoDto> AgregarComentarioCertificadoAsync(Guid solicitudId, string cedula, string comentario)
    {
        try
        {
            var solicitud = await _solicitudCertificadoRepository.GetByIdAsync(solicitudId);
            if (solicitud == null || solicitud.DocenteCedula != cedula)
            {
                return new ResponseGenericoCertificadoDto
                {
                    Exitoso = false,
                    Mensaje = "Solicitud no encontrada o sin permisos"
                };
            }

            if (solicitud.Estado != "En Proceso")
            {
                return new ResponseGenericoCertificadoDto
                {
                    Exitoso = false,
                    Mensaje = "Solo se pueden agregar comentarios a solicitudes en proceso"
                };
            }

            var comentarioExistente = solicitud.ComentariosSolicitud ?? "";
            var nuevoComentario = string.IsNullOrEmpty(comentarioExistente) 
                ? comentario 
                : $"{comentarioExistente}\n\n[{DateTime.Now:dd/MM/yyyy HH:mm}] {comentario}";

            if (nuevoComentario.Length > 1000)
            {
                nuevoComentario = nuevoComentario.Substring(0, 1000);
            }

            solicitud.ComentariosSolicitud = nuevoComentario;
            solicitud.FechaModificacion = DateTime.UtcNow;

            await _solicitudCertificadoRepository.UpdateAsync(solicitud);

            await _auditoriaService.RegistrarAccionAsync(
                "AGREGAR_COMENTARIO_CERTIFICADO",
                cedula,
                "SolicitudCertificadoCapacitacion",
                solicitud.Id.ToString(),
                "Comentario agregado",
                comentario,
                "Usuario"
            );

            return new ResponseGenericoCertificadoDto
            {
                Exitoso = true,
                Mensaje = "Comentario agregado correctamente"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al agregar comentario a certificado {SolicitudId}", solicitudId);
            return new ResponseGenericoCertificadoDto
            {
                Exitoso = false,
                Mensaje = $"Error al agregar comentario: {ex.Message}"
            };
        }
    }

    public async Task<ResponseGenericoCertificadoDto> ReenviarSolicitudCertificadoAsync(Guid solicitudId, string cedula)
    {
        try
        {
            var solicitud = await _solicitudCertificadoRepository.GetByIdAsync(solicitudId);
            if (solicitud == null || solicitud.DocenteCedula != cedula)
            {
                return new ResponseGenericoCertificadoDto
                {
                    Exitoso = false,
                    Mensaje = "Solicitud no encontrada o sin permisos"
                };
            }

            if (solicitud.Estado != "Rechazada")
            {
                return new ResponseGenericoCertificadoDto
                {
                    Exitoso = false,
                    Mensaje = "Solo se pueden reenviar solicitudes rechazadas"
                };
            }

            solicitud.Estado = "Pendiente";
            solicitud.MotivoRechazo = null;
            solicitud.FechaRevision = null;
            solicitud.RevisadoPorId = null;
            solicitud.FechaModificacion = DateTime.UtcNow;

            await _solicitudCertificadoRepository.UpdateAsync(solicitud);

            await _auditoriaService.RegistrarAccionAsync(
                "REENVIAR_SOLICITUD_CERTIFICADO",
                cedula,
                "SolicitudCertificadoCapacitacion",
                solicitud.Id.ToString(),
                "Rechazada",
                "Pendiente",
                "Usuario"
            );

            return new ResponseGenericoCertificadoDto
            {
                Exitoso = true,
                Mensaje = "Solicitud reenviada correctamente para nueva revisión"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al reenviar solicitud de certificado {SolicitudId}", solicitudId);
            return new ResponseGenericoCertificadoDto
            {
                Exitoso = false,
                Mensaje = $"Error al reenviar solicitud: {ex.Message}"
            };
        }
    }

    // Métodos para administradores
    public async Task<ResponseSolicitudesCertificadosAdminDto> GetTodasSolicitudesCertificadosAsync()
    {
        try
        {
            var solicitudes = await _solicitudCertificadoRepository.GetAllAsync();
            var solicitudesDto = new List<SolicitudCertificadoCapacitacionAdminDto>();

            foreach (var s in solicitudes)
            {
                var docente = await _docenteRepository.GetByCedulaAsync(s.DocenteCedula);
                var nombreDocente = docente?.NombreCompleto ?? "Docente no encontrado";

                solicitudesDto.Add(new SolicitudCertificadoCapacitacionAdminDto
                {
                    Id = s.Id,
                    SolicitudGrupoId = s.SolicitudGrupoId,
                    DocenteCedula = s.DocenteCedula,
                    DocenteNombre = nombreDocente,
                    NombreCurso = s.NombreCurso,
                    InstitucionOfertante = s.InstitucionOfertante,
                    TipoCapacitacion = s.TipoCapacitacion,
                    FechaInicio = s.FechaInicio,
                    FechaFin = s.FechaFin,
                    HorasDuracion = s.HorasDuracion,
                    Modalidad = s.Modalidad,
                    NumeroRegistro = s.NumeroRegistro,
                    AreaTematica = s.AreaTematica,
                    Descripcion = s.Descripcion,
                    ArchivoNombre = s.ArchivoNombre,
                    Estado = s.Estado,
                    ComentariosRevision = s.ComentariosRevision,
                    MotivoRechazo = s.MotivoRechazo,
                    ComentariosSolicitud = s.ComentariosSolicitud,
                    FechaCreacion = s.FechaCreacion,
                    FechaRevision = s.FechaRevision
                });
            }

            return new ResponseSolicitudesCertificadosAdminDto
            {
                Exitoso = true,
                Mensaje = "Todas las solicitudes obtenidas correctamente",
                Solicitudes = solicitudesDto.OrderByDescending(s => s.FechaCreacion).ToList(),
                TotalSolicitudes = solicitudesDto.Count
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las solicitudes de certificados");
            return new ResponseSolicitudesCertificadosAdminDto
            {
                Exitoso = false,
                Mensaje = $"Error al obtener solicitudes: {ex.Message}"
            };
        }
    }

    public async Task<ResponseSolicitudesCertificadosAdminDto> GetSolicitudesCertificadosPorRevisarAsync()
    {
        try
        {
            var solicitudes = await _solicitudCertificadoRepository.GetAllAsync();
            var solicitudesPendientes = solicitudes
                .Where(s => s.Estado == "Pendiente")
                .OrderBy(s => s.FechaCreacion)
                .ToList();

            var solicitudesDto = new List<SolicitudCertificadoCapacitacionAdminDto>();

            foreach (var s in solicitudesPendientes)
            {
                var docente = await _docenteRepository.GetByCedulaAsync(s.DocenteCedula);
                var nombreDocente = docente?.NombreCompleto ?? "Docente no encontrado";

                solicitudesDto.Add(new SolicitudCertificadoCapacitacionAdminDto
                {
                    Id = s.Id,
                    SolicitudGrupoId = s.SolicitudGrupoId,
                    DocenteCedula = s.DocenteCedula,
                    DocenteNombre = nombreDocente,
                    NombreCurso = s.NombreCurso,
                    InstitucionOfertante = s.InstitucionOfertante,
                    TipoCapacitacion = s.TipoCapacitacion,
                    FechaInicio = s.FechaInicio,
                    FechaFin = s.FechaFin,
                    HorasDuracion = s.HorasDuracion,
                    Modalidad = s.Modalidad,
                    NumeroRegistro = s.NumeroRegistro,
                    AreaTematica = s.AreaTematica,
                    Descripcion = s.Descripcion,
                    ArchivoNombre = s.ArchivoNombre,
                    Estado = s.Estado,
                    ComentariosRevision = s.ComentariosRevision,
                    MotivoRechazo = s.MotivoRechazo,
                    ComentariosSolicitud = s.ComentariosSolicitud,
                    FechaCreacion = s.FechaCreacion,
                    FechaRevision = s.FechaRevision
                });
            }

            return new ResponseSolicitudesCertificadosAdminDto
            {
                Exitoso = true,
                Mensaje = "Solicitudes por revisar obtenidas correctamente",
                Solicitudes = solicitudesDto,
                TotalSolicitudes = solicitudesDto.Count
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener solicitudes por revisar");
            return new ResponseSolicitudesCertificadosAdminDto
            {
                Exitoso = false,
                Mensaje = $"Error al obtener solicitudes: {ex.Message}"
            };
        }
    }

    public async Task<ResponseCertificadosCapacitacionDto> AprobarSolicitudCertificadoAsync(Guid solicitudId, string comentarios)
    {
        try
        {
            var solicitud = await _solicitudCertificadoRepository.GetByIdAsync(solicitudId);
            if (solicitud == null)
            {
                return new ResponseCertificadosCapacitacionDto
                {
                    Exitoso = false,
                    Mensaje = "Solicitud no encontrada"
                };
            }

            solicitud.Estado = "Aprobada";
            solicitud.ComentariosRevision = comentarios;
            solicitud.FechaRevision = DateTime.UtcNow;
            solicitud.FechaModificacion = DateTime.UtcNow;

            await _solicitudCertificadoRepository.UpdateAsync(solicitud);

            // Actualizar contador de horas en el docente
            var docente = await _docenteRepository.GetByIdAsync(solicitud.DocenteId);
            if (docente != null)
            {
                docente.HorasCapacitacion = (docente.HorasCapacitacion ?? 0) + solicitud.HorasDuracion;
                docente.FechaUltimaImportacion = DateTime.UtcNow;
                await _docenteRepository.UpdateAsync(docente);
            }

            await _auditoriaService.RegistrarAccionAsync(
                "APROBAR_SOLICITUD_CERTIFICADO",
                "Administrador",
                "SolicitudCertificadoCapacitacion",
                solicitud.Id.ToString(),
                "Pendiente",
                "Aprobada",
                "Administrador"
            );

            // Notificar al docente
            await _notificationService.NotificarAprobacionCertificadoAsync(
                docente?.Email ?? "", 
                solicitud.NombreCurso, 
                comentarios);

            return new ResponseCertificadosCapacitacionDto
            {
                Exitoso = true,
                Mensaje = $"Certificado '{solicitud.NombreCurso}' aprobado correctamente"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al aprobar solicitud de certificado {SolicitudId}", solicitudId);
            return new ResponseCertificadosCapacitacionDto
            {
                Exitoso = false,
                Mensaje = $"Error al aprobar solicitud: {ex.Message}"
            };
        }
    }

    public async Task<ResponseCertificadosCapacitacionDto> RechazarSolicitudCertificadoAsync(Guid solicitudId, string motivo)
    {
        try
        {
            var solicitud = await _solicitudCertificadoRepository.GetByIdAsync(solicitudId);
            if (solicitud == null)
            {
                return new ResponseCertificadosCapacitacionDto
                {
                    Exitoso = false,
                    Mensaje = "Solicitud no encontrada"
                };
            }

            solicitud.Estado = "Rechazada";
            solicitud.MotivoRechazo = motivo;
            solicitud.FechaRevision = DateTime.UtcNow;
            solicitud.FechaModificacion = DateTime.UtcNow;

            await _solicitudCertificadoRepository.UpdateAsync(solicitud);

            await _auditoriaService.RegistrarAccionAsync(
                "RECHAZAR_SOLICITUD_CERTIFICADO",
                "Administrador",
                "SolicitudCertificadoCapacitacion",
                solicitud.Id.ToString(),
                "Pendiente",
                "Rechazada",
                "Administrador"
            );

            // Notificar al docente
            var docente = await _docenteRepository.GetByIdAsync(solicitud.DocenteId);
            await _notificationService.NotificarRechazoCertificadoAsync(
                docente?.Email ?? "", 
                solicitud.NombreCurso, 
                motivo);

            return new ResponseCertificadosCapacitacionDto
            {
                Exitoso = true,
                Mensaje = $"Certificado '{solicitud.NombreCurso}' rechazado"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al rechazar solicitud de certificado {SolicitudId}", solicitudId);
            return new ResponseCertificadosCapacitacionDto
            {
                Exitoso = false,
                Mensaje = $"Error al rechazar solicitud: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Método unificado para revisar solicitudes de certificados (aprobar/rechazar)
    /// </summary>
    public async Task<ResponseGenericoCertificadoDto> RevisarSolicitudCertificadoAsync(RevisionSolicitudCertificadoDto revision, string adminEmail)
    {
        try
        {
            var solicitud = await _solicitudCertificadoRepository.GetByIdAsync(revision.SolicitudId);
            if (solicitud == null)
            {
                return new ResponseGenericoCertificadoDto
                {
                    Exitoso = false,
                    Mensaje = "Solicitud no encontrada"
                };
            }

            if (solicitud.Estado != "Pendiente")
            {
                return new ResponseGenericoCertificadoDto
                {
                    Exitoso = false,
                    Mensaje = "La solicitud ya ha sido revisada"
                };
            }

            // Actualizar estado de la solicitud
            solicitud.Estado = revision.Accion.Equals("Aprobar", StringComparison.OrdinalIgnoreCase) ? "Aprobada" : "Rechazada";
            solicitud.ComentariosRevision = revision.Comentarios;
            if (!revision.Accion.Equals("Aprobar", StringComparison.OrdinalIgnoreCase))
            {
                solicitud.MotivoRechazo = revision.Comentarios;
            }
            solicitud.FechaRevision = DateTime.UtcNow;
            solicitud.FechaModificacion = DateTime.UtcNow;

            await _solicitudCertificadoRepository.UpdateAsync(solicitud);

            // Si es aprobada, actualizar horas del docente
            if (solicitud.Estado == "Aprobada")
            {
                try
                {
                    var docente = await _docenteRepository.GetByCedulaAsync(solicitud.DocenteCedula);
                    if (docente != null)
                    {
                        docente.HorasCapacitacion = (docente.HorasCapacitacion ?? 0) + solicitud.HorasDuracion;
                        await _docenteRepository.UpdateAsync(docente);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al actualizar horas del docente para solicitud {SolicitudId}", solicitud.Id);
                    // No fallar la revisión por este error, solo loggearlo
                }
            }

            // Registrar auditoría
            await _auditoriaService.RegistrarAccionAsync(
                "REVISION_CERTIFICADO",
                adminEmail,
                "SolicitudCertificadoCapacitacion",
                solicitud.Id.ToString(),
                "Pendiente",
                $"{solicitud.Estado}. Comentarios: {revision.Comentarios}",
                "Sistema"
            );

            // Enviar notificación al docente
            var docenteNotif = await _docenteRepository.GetByCedulaAsync(solicitud.DocenteCedula);
            if (docenteNotif != null)
            {
                if (solicitud.Estado == "Aprobada")
                {
                    await _notificationService.NotificarAprobacionCertificadoAsync(
                        docenteNotif.Email,
                        solicitud.NombreCurso,
                        revision.Comentarios ?? "Certificado aprobado"
                    );
                }
                else
                {
                    await _notificationService.NotificarRechazoCertificadoAsync(
                        docenteNotif.Email,
                        solicitud.NombreCurso,
                        revision.Comentarios ?? "No especificado"
                    );
                }
            }

            var accion = solicitud.Estado == "Aprobada" ? "aprobada" : "rechazada";
            return new ResponseGenericoCertificadoDto
            {
                Exitoso = true,
                Mensaje = $"Solicitud {accion} correctamente"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al revisar solicitud {SolicitudId}", revision.SolicitudId);
            return new ResponseGenericoCertificadoDto
            {
                Exitoso = false,
                Mensaje = $"Error al revisar solicitud: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Obtener archivo de certificado para descarga por administradores
    /// </summary>
    public async Task<byte[]?> GetArchivoCertificadoSolicitudAsync(Guid solicitudId)
    {
        try
        {
            var solicitud = await _solicitudCertificadoRepository.GetByIdAsync(solicitudId);
            if (solicitud == null || solicitud.ArchivoContenido == null || solicitud.ArchivoContenido.Length == 0)
            {
                return null;
            }

            // Descomprimir el PDF desde la base de datos
            var contenidoDescomprimido = await _pdfCompressionService.DescomprimirPDFAsync(solicitud.ArchivoContenido);
            return contenidoDescomprimido;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener archivo para solicitud {SolicitudId}", solicitudId);
            return null;
        }
    }

}
