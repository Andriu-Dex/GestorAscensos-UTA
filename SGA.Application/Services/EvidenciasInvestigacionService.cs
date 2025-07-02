using Microsoft.Extensions.Logging;
using SGA.Application.DTOs.EvidenciasInvestigacion;
using SGA.Application.Interfaces;
using SGA.Application.Interfaces.Repositories;
using SGA.Domain.Entities;

namespace SGA.Application.Services;

/// <summary>
/// Servicio para gestión de evidencias de investigación
/// Basado en CertificadosCapacitacionService
/// </summary>
public class EvidenciasInvestigacionService : IEvidenciasInvestigacionService
{
    private readonly ISolicitudEvidenciaInvestigacionRepository _solicitudEvidenciaRepository;
    private readonly IDocenteRepository _docenteRepository;
    private readonly IAuditoriaService _auditoriaService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<EvidenciasInvestigacionService> _logger;

    public EvidenciasInvestigacionService(
        ISolicitudEvidenciaInvestigacionRepository solicitudEvidenciaRepository,
        IDocenteRepository docenteRepository,
        IAuditoriaService auditoriaService,
        INotificationService notificationService,
        ILogger<EvidenciasInvestigacionService> logger)
    {
        _solicitudEvidenciaRepository = solicitudEvidenciaRepository;
        _docenteRepository = docenteRepository;
        _auditoriaService = auditoriaService;
        _notificationService = notificationService;
        _logger = logger;
    }

    /// <summary>
    /// Crear nuevas solicitudes de evidencias de investigación
    /// </summary>
    public async Task<ResponseEvidenciasInvestigacionDto> SolicitarNuevasEvidenciasAsync(string cedula, SolicitarEvidenciasInvestigacionDto solicitud)
    {
        try
        {
            _logger.LogInformation("Iniciando solicitud de evidencias para docente {Cedula} con {Count} evidencias", cedula, solicitud.Evidencias?.Count ?? 0);
            
            // Debug: Log del objeto recibido
            if (solicitud.Evidencias != null)
            {
                foreach (var evidencia in solicitud.Evidencias)
                {
                    _logger.LogInformation("Evidencia recibida: Tipo={TipoEvidencia}, Titulo={TituloProyecto}, Archivo={ArchivoNombre}", 
                        evidencia.TipoEvidencia, evidencia.TituloProyecto, evidencia.ArchivoNombre);
                }
            }
            else
            {
                _logger.LogWarning("Solicitud.Evidencias es NULL");
            }
            
            // Validar que hay evidencias para procesar
            if (solicitud.Evidencias == null || !solicitud.Evidencias.Any())
            {
                _logger.LogWarning("No se proporcionaron evidencias para procesar");
                return new ResponseEvidenciasInvestigacionDto
                {
                    Exitoso = false,
                    Mensaje = "No se proporcionaron evidencias para procesar"
                };
            }
            
            // Validar que el docente existe
            var docente = await _docenteRepository.GetByCedulaAsync(cedula);
            if (docente == null)
            {
                _logger.LogWarning("Docente con cédula {Cedula} no encontrado", cedula);
                return new ResponseEvidenciasInvestigacionDto
                {
                    Exitoso = false,
                    Mensaje = "Docente no encontrado"
                };
            }

            _logger.LogInformation("Docente encontrado: {DocenteId} - {Nombres} {Apellidos}", docente.Id, docente.Nombres, docente.Apellidos);

            var evidenciasCreadas = new List<EvidenciaInvestigacionDetalleDto>();

            foreach (var evidencia in solicitud.Evidencias)
            {
                _logger.LogInformation("Procesando evidencia: {Tipo} - {Titulo}", evidencia.TipoEvidencia, evidencia.TituloProyecto);
                
                // Validaciones básicas
                if (string.IsNullOrWhiteSpace(evidencia.TipoEvidencia) || 
                    string.IsNullOrWhiteSpace(evidencia.TituloProyecto) ||
                    string.IsNullOrWhiteSpace(evidencia.RolInvestigador) ||
                    evidencia.MesesDuracion <= 0)
                {
                    _logger.LogWarning("Evidencia con datos incompletos saltada: {Tipo} - {Titulo}", evidencia.TipoEvidencia, evidencia.TituloProyecto);
                    continue; // Saltar evidencias con datos incompletos
                }

                var nuevaSolicitud = new SolicitudEvidenciaInvestigacion
                {
                    Id = Guid.NewGuid(),
                    DocenteId = docente.Id,
                    DocenteCedula = cedula,
                    TipoEvidencia = evidencia.TipoEvidencia,
                    TituloProyecto = evidencia.TituloProyecto,
                    InstitucionFinanciadora = evidencia.InstitucionFinanciadora,
                    RolInvestigador = evidencia.RolInvestigador,
                    FechaInicio = evidencia.FechaInicio,
                    FechaFin = evidencia.FechaFin,
                    MesesDuracion = evidencia.MesesDuracion,
                    CodigoProyecto = evidencia.CodigoProyecto,
                    AreaTematica = evidencia.AreaTematica,
                    Descripcion = evidencia.Descripcion,
                    Estado = "Pendiente",
                    ComentariosSolicitud = evidencia.ComentariosSolicitud
                };

                // Manejar archivo PDF
                if (!string.IsNullOrEmpty(evidencia.ArchivoContenido) && !string.IsNullOrEmpty(evidencia.ArchivoNombre))
                {
                    try
                    {
                        var contenidoBytes = Convert.FromBase64String(evidencia.ArchivoContenido);
                        var archivoNombre = evidencia.ArchivoNombre.Trim();

                        // Validar tamaño (máximo 10MB)
                        if (contenidoBytes.Length > 10 * 1024 * 1024)
                        {
                            throw new Exception($"El archivo {archivoNombre} excede el tamaño máximo permitido de 10MB");
                        }

                        // Validar que sea PDF
                        if (!EsPDF(contenidoBytes))
                        {
                            throw new Exception($"El archivo {archivoNombre} no es un PDF válido");
                        }

                        // Generar ruta única para el archivo
                        var carpetaDestino = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "evidencias-investigacion", docente.Id.ToString());
                        Directory.CreateDirectory(carpetaDestino);

                        var nombreArchivo = $"evidencia-{nuevaSolicitud.Id}-{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";
                        var rutaCompleta = Path.Combine(carpetaDestino, nombreArchivo);

                        // Guardar archivo
                        await File.WriteAllBytesAsync(rutaCompleta, contenidoBytes);

                        // Actualizar información en la entidad
                        nuevaSolicitud.ArchivoNombre = archivoNombre;
                        nuevaSolicitud.ArchivoRuta = Path.Combine("uploads", "evidencias-investigacion", docente.Id.ToString(), nombreArchivo);
                        nuevaSolicitud.ArchivoTamano = contenidoBytes.Length;
                        nuevaSolicitud.ArchivoTipo = evidencia.ArchivoTipo ?? "application/pdf";
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error al procesar archivo para evidencia {Titulo}", evidencia.TituloProyecto);
                        throw new Exception($"Error al procesar archivo para {evidencia.TituloProyecto}: {ex.Message}");
                    }
                }

                // Guardar en base de datos
                await _solicitudEvidenciaRepository.AddAsync(nuevaSolicitud);
                _logger.LogInformation("Evidencia guardada en BD: {EvidenciaId} - {Titulo}", nuevaSolicitud.Id, nuevaSolicitud.TituloProyecto);

                // Registrar auditoría
                await _auditoriaService.RegistrarAccionAsync(
                    "CREAR_EVIDENCIA_INVESTIGACION",
                    cedula,
                    "SolicitudEvidenciaInvestigacion",
                    nuevaSolicitud.Id.ToString(),
                    null,
                    $"Tipo: {evidencia.TipoEvidencia}, Proyecto: {evidencia.TituloProyecto}",
                    "Usuario"
                );

                // Convertir a DTO para respuesta
                evidenciasCreadas.Add(new EvidenciaInvestigacionDetalleDto
                {
                    Id = nuevaSolicitud.Id,
                    TipoEvidencia = nuevaSolicitud.TipoEvidencia,
                    TituloProyecto = nuevaSolicitud.TituloProyecto,
                    InstitucionFinanciadora = nuevaSolicitud.InstitucionFinanciadora,
                    RolInvestigador = nuevaSolicitud.RolInvestigador,
                    FechaInicio = nuevaSolicitud.FechaInicio,
                    FechaFin = nuevaSolicitud.FechaFin,
                    MesesDuracion = nuevaSolicitud.MesesDuracion,
                    CodigoProyecto = nuevaSolicitud.CodigoProyecto,
                    AreaTematica = nuevaSolicitud.AreaTematica,
                    Descripcion = nuevaSolicitud.Descripcion,
                    ArchivoNombre = nuevaSolicitud.ArchivoNombre,
                    ArchivoTamano = nuevaSolicitud.ArchivoTamano,
                    Estado = nuevaSolicitud.Estado,
                    ComentariosSolicitud = nuevaSolicitud.ComentariosSolicitud,
                    FechaCreacion = nuevaSolicitud.FechaCreacion,
                    FechaModificacion = nuevaSolicitud.FechaModificacion ?? nuevaSolicitud.FechaCreacion
                });
            }

            _logger.LogInformation("Total de evidencias creadas: {Count}", evidenciasCreadas.Count);

            return new ResponseEvidenciasInvestigacionDto
            {
                Exitoso = true,
                Mensaje = $"Se crearon {evidenciasCreadas.Count} evidencias de investigación exitosamente",
                Evidencias = evidenciasCreadas
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear evidencias de investigación para docente {Cedula}", cedula);
            return new ResponseEvidenciasInvestigacionDto
            {
                Exitoso = false,
                Mensaje = $"Error al crear evidencias: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Obtener evidencias del docente
    /// </summary>
    public async Task<ResponseEvidenciasInvestigacionDto> GetMisEvidenciasAsync(string cedula)
    {
        try
        {
            _logger.LogInformation("Obteniendo evidencias para docente con cédula: {Cedula}", cedula);
            var evidencias = await _solicitudEvidenciaRepository.GetByDocenteCedulaAsync(cedula);
            _logger.LogInformation("Encontradas {Count} evidencias para cédula {Cedula}", evidencias.Count, cedula);

            var evidenciasDto = evidencias.Select(e => new EvidenciaInvestigacionDetalleDto
            {
                Id = e.Id,
                TipoEvidencia = e.TipoEvidencia,
                TituloProyecto = e.TituloProyecto,
                InstitucionFinanciadora = e.InstitucionFinanciadora,
                RolInvestigador = e.RolInvestigador,
                FechaInicio = e.FechaInicio,
                FechaFin = e.FechaFin,
                MesesDuracion = e.MesesDuracion,
                CodigoProyecto = e.CodigoProyecto,
                AreaTematica = e.AreaTematica,
                Descripcion = e.Descripcion,
                ArchivoNombre = e.ArchivoNombre,
                ArchivoTamano = e.ArchivoTamano,
                Estado = e.Estado,
                ComentariosRevision = e.ComentariosRevision,
                MotivoRechazo = e.MotivoRechazo,
                FechaRevision = e.FechaRevision,
                ComentariosSolicitud = e.ComentariosSolicitud,
                FechaCreacion = e.FechaCreacion,
                FechaModificacion = e.FechaModificacion ?? e.FechaCreacion
            }).ToList();

            return new ResponseEvidenciasInvestigacionDto
            {
                Exitoso = true,
                Mensaje = "Evidencias obtenidas correctamente",
                Evidencias = evidenciasDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener evidencias del docente {Cedula}", cedula);
            return new ResponseEvidenciasInvestigacionDto
            {
                Exitoso = false,
                Mensaje = $"Error al obtener evidencias: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Editar metadatos de evidencia de investigación
    /// </summary>
    public async Task<ResponseGenericoEvidenciaDto> EditarMetadatosEvidenciaAsync(Guid evidenciaId, string cedula, EditarMetadatosEvidenciaDto datos)
    {
        try
        {
            var evidencia = await _solicitudEvidenciaRepository.GetByIdAsync(evidenciaId);
            if (evidencia == null)
            {
                return new ResponseGenericoEvidenciaDto
                {
                    Exitoso = false,
                    Mensaje = "Evidencia no encontrada"
                };
            }

            if (evidencia.DocenteCedula != cedula)
            {
                return new ResponseGenericoEvidenciaDto
                {
                    Exitoso = false,
                    Mensaje = "No tiene permisos para editar esta evidencia"
                };
            }

            if (evidencia.Estado == "Aprobada")
            {
                return new ResponseGenericoEvidenciaDto
                {
                    Exitoso = false,
                    Mensaje = "No se pueden editar evidencias ya aprobadas"
                };
            }

            // Actualizar metadatos
            evidencia.TipoEvidencia = datos.TipoEvidencia;
            evidencia.TituloProyecto = datos.TituloProyecto;
            evidencia.InstitucionFinanciadora = datos.InstitucionFinanciadora;
            evidencia.RolInvestigador = datos.RolInvestigador;
            evidencia.FechaInicio = datos.FechaInicio;
            evidencia.FechaFin = datos.FechaFin;
            evidencia.MesesDuracion = datos.MesesDuracion;
            evidencia.CodigoProyecto = datos.CodigoProyecto;
            evidencia.AreaTematica = datos.AreaTematica;
            evidencia.Descripcion = datos.Descripcion;
            evidencia.FechaModificacion = DateTime.UtcNow;

            // Si estaba rechazada, volver a pendiente
            if (evidencia.Estado == "Rechazada")
            {
                evidencia.Estado = "Pendiente";
                evidencia.MotivoRechazo = null;
                evidencia.ComentariosRevision = null;
                evidencia.FechaRevision = null;
            }

            await _solicitudEvidenciaRepository.UpdateAsync(evidencia);

            await _auditoriaService.RegistrarAccionAsync(
                "EDITAR_EVIDENCIA_INVESTIGACION",
                cedula,
                "SolicitudEvidenciaInvestigacion",
                evidencia.Id.ToString(),
                "Metadatos editados",
                $"Proyecto: {evidencia.TituloProyecto}",
                "Usuario"
            );

            return new ResponseGenericoEvidenciaDto
            {
                Exitoso = true,
                Mensaje = "Evidencia actualizada correctamente"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al editar evidencia {EvidenciaId}", evidenciaId);
            return new ResponseGenericoEvidenciaDto
            {
                Exitoso = false,
                Mensaje = $"Error al editar evidencia: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Reemplazar archivo de evidencia
    /// </summary>
    public async Task<ResponseGenericoEvidenciaDto> ReemplazarArchivoEvidenciaAsync(Guid evidenciaId, string cedula, ReemplazarArchivoEvidenciaDto archivo)
    {
        try
        {
            var evidencia = await _solicitudEvidenciaRepository.GetByIdAsync(evidenciaId);
            if (evidencia == null)
            {
                return new ResponseGenericoEvidenciaDto
                {
                    Exitoso = false,
                    Mensaje = "Evidencia no encontrada"
                };
            }

            if (evidencia.DocenteCedula != cedula)
            {
                return new ResponseGenericoEvidenciaDto
                {
                    Exitoso = false,
                    Mensaje = "No tiene permisos para modificar esta evidencia"
                };
            }

            if (evidencia.Estado == "Aprobada")
            {
                return new ResponseGenericoEvidenciaDto
                {
                    Exitoso = false,
                    Mensaje = "No se pueden modificar evidencias ya aprobadas"
                };
            }

            // Validar nuevo archivo
            var contenidoBytes = Convert.FromBase64String(archivo.ArchivoContenido);
            
            if (contenidoBytes.Length > 10 * 1024 * 1024)
            {
                return new ResponseGenericoEvidenciaDto
                {
                    Exitoso = false,
                    Mensaje = "El archivo excede el tamaño máximo permitido de 10MB"
                };
            }

            if (!EsPDF(contenidoBytes))
            {
                return new ResponseGenericoEvidenciaDto
                {
                    Exitoso = false,
                    Mensaje = "El archivo debe ser un PDF válido"
                };
            }

            // Eliminar archivo anterior si existe
            if (!string.IsNullOrEmpty(evidencia.ArchivoRuta))
            {
                var rutaAnterior = Path.Combine(Directory.GetCurrentDirectory(), evidencia.ArchivoRuta);
                if (File.Exists(rutaAnterior))
                {
                    File.Delete(rutaAnterior);
                }
            }

            // Generar nueva ruta
            var docente = await _docenteRepository.GetByCedulaAsync(cedula);
            if (docente == null)
            {
                return new ResponseGenericoEvidenciaDto
                {
                    Exitoso = false,
                    Mensaje = "Docente no encontrado"
                };
            }
            
            var carpetaDestino = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "evidencias-investigacion", docente.Id.ToString());
            Directory.CreateDirectory(carpetaDestino);

            var nombreArchivo = $"evidencia-{evidencia.Id}-{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";
            var rutaCompleta = Path.Combine(carpetaDestino, nombreArchivo);

            // Guardar nuevo archivo
            await File.WriteAllBytesAsync(rutaCompleta, contenidoBytes);

            // Actualizar información
            evidencia.ArchivoNombre = archivo.ArchivoNombre;
            evidencia.ArchivoRuta = Path.Combine("uploads", "evidencias-investigacion", docente.Id.ToString(), nombreArchivo);
            evidencia.ArchivoTamano = contenidoBytes.Length;
            evidencia.ArchivoTipo = archivo.ArchivoTipo;
            evidencia.FechaModificacion = DateTime.UtcNow;

            // Si estaba rechazada, volver a pendiente
            if (evidencia.Estado == "Rechazada")
            {
                evidencia.Estado = "Pendiente";
                evidencia.MotivoRechazo = null;
                evidencia.ComentariosRevision = null;
                evidencia.FechaRevision = null;
            }

            await _solicitudEvidenciaRepository.UpdateAsync(evidencia);

            await _auditoriaService.RegistrarAccionAsync(
                "REEMPLAZAR_ARCHIVO_EVIDENCIA",
                cedula,
                "SolicitudEvidenciaInvestigacion",
                evidencia.Id.ToString(),
                "Archivo reemplazado",
                $"Nuevo archivo: {archivo.ArchivoNombre}",
                "Usuario"
            );

            return new ResponseGenericoEvidenciaDto
            {
                Exitoso = true,
                Mensaje = "Archivo reemplazado correctamente"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al reemplazar archivo de evidencia {EvidenciaId}", evidenciaId);
            return new ResponseGenericoEvidenciaDto
            {
                Exitoso = false,
                Mensaje = $"Error al reemplazar archivo: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Eliminar solicitud de evidencia
    /// </summary>
    public async Task<ResponseGenericoEvidenciaDto> EliminarSolicitudEvidenciaAsync(Guid evidenciaId, string cedula)
    {
        try
        {
            var evidencia = await _solicitudEvidenciaRepository.GetByIdAsync(evidenciaId);
            if (evidencia == null)
            {
                return new ResponseGenericoEvidenciaDto
                {
                    Exitoso = false,
                    Mensaje = "Evidencia no encontrada"
                };
            }

            if (evidencia.DocenteCedula != cedula)
            {
                return new ResponseGenericoEvidenciaDto
                {
                    Exitoso = false,
                    Mensaje = "No tiene permisos para eliminar esta evidencia"
                };
            }

            if (evidencia.Estado != "Pendiente" && evidencia.Estado != "Rechazada")
            {
                return new ResponseGenericoEvidenciaDto
                {
                    Exitoso = false,
                    Mensaje = "Solo se pueden eliminar evidencias pendientes o rechazadas"
                };
            }

            // Eliminar archivo físico si existe
            if (!string.IsNullOrEmpty(evidencia.ArchivoRuta))
            {
                var rutaCompleta = Path.Combine(Directory.GetCurrentDirectory(), evidencia.ArchivoRuta);
                if (File.Exists(rutaCompleta))
                {
                    File.Delete(rutaCompleta);
                }
            }

            // Eliminar de base de datos
            await _solicitudEvidenciaRepository.DeleteAsync(evidencia.Id);

            await _auditoriaService.RegistrarAccionAsync(
                "ELIMINAR_EVIDENCIA_INVESTIGACION",
                cedula,
                "SolicitudEvidenciaInvestigacion",
                evidencia.Id.ToString(),
                evidencia.Estado,
                "Eliminada",
                "Usuario"
            );

            return new ResponseGenericoEvidenciaDto
            {
                Exitoso = true,
                Mensaje = "Evidencia eliminada correctamente"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar evidencia {EvidenciaId}", evidenciaId);
            return new ResponseGenericoEvidenciaDto
            {
                Exitoso = false,
                Mensaje = $"Error al eliminar evidencia: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Obtener archivo de evidencia para docente
    /// </summary>
    public async Task<byte[]?> GetArchivoEvidenciaAsync(Guid evidenciaId, string cedula)
    {
        try
        {
            var evidencia = await _solicitudEvidenciaRepository.GetByIdAsync(evidenciaId);
            if (evidencia == null || evidencia.DocenteCedula != cedula)
            {
                return null;
            }

            if (string.IsNullOrEmpty(evidencia.ArchivoRuta))
            {
                return null;
            }

            var rutaCompleta = Path.Combine(Directory.GetCurrentDirectory(), evidencia.ArchivoRuta);
            if (!File.Exists(rutaCompleta))
            {
                return null;
            }

            return await File.ReadAllBytesAsync(rutaCompleta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener archivo de evidencia {EvidenciaId}", evidenciaId);
            return null;
        }
    }

    /// <summary>
    /// Obtener todas las solicitudes para administradores
    /// </summary>
    public async Task<ResponseSolicitudesEvidenciasAdminDto> GetTodasSolicitudesEvidenciasAsync()
    {
        try
        {
            var solicitudes = await _solicitudEvidenciaRepository.GetTodasAsync();

            var solicitudesDto = solicitudes.Select(s => new SolicitudEvidenciaInvestigacionAdminDto
            {
                Id = s.Id,
                DocenteNombre = s.Docente != null ? $"{s.Docente.Nombres} {s.Docente.Apellidos}" : "N/A",
                DocenteCedula = s.DocenteCedula,
                TipoEvidencia = s.TipoEvidencia,
                TituloProyecto = s.TituloProyecto,
                InstitucionFinanciadora = s.InstitucionFinanciadora,
                RolInvestigador = s.RolInvestigador,
                FechaInicio = s.FechaInicio,
                FechaFin = s.FechaFin,
                MesesDuracion = s.MesesDuracion,
                CodigoProyecto = s.CodigoProyecto,
                AreaTematica = s.AreaTematica,
                Descripcion = s.Descripcion,
                ArchivoNombre = s.ArchivoNombre,
                Estado = s.Estado,
                ComentariosRevision = s.ComentariosRevision,
                MotivoRechazo = s.MotivoRechazo,
                ComentariosSolicitud = s.ComentariosSolicitud,
                FechaCreacion = s.FechaCreacion,
                FechaRevision = s.FechaRevision
            }).ToList();

            return new ResponseSolicitudesEvidenciasAdminDto
            {
                Exitoso = true,
                Mensaje = "Todas las solicitudes obtenidas correctamente",
                Solicitudes = solicitudesDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las solicitudes de evidencias para admin");
            return new ResponseSolicitudesEvidenciasAdminDto
            {
                Exitoso = false,
                Mensaje = $"Error al obtener solicitudes: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Obtener solicitudes para administradores
    /// </summary>
    public async Task<ResponseSolicitudesEvidenciasAdminDto> GetSolicitudesEvidenciasPorRevisarAsync()
    {
        try
        {
            var solicitudes = await _solicitudEvidenciaRepository.GetTodasAsync();

            var solicitudesDto = solicitudes.Select(s => new SolicitudEvidenciaInvestigacionAdminDto
            {
                Id = s.Id,
                DocenteNombre = s.Docente != null ? $"{s.Docente.Nombres} {s.Docente.Apellidos}" : "N/A",
                DocenteCedula = s.DocenteCedula,
                TipoEvidencia = s.TipoEvidencia,
                TituloProyecto = s.TituloProyecto,
                InstitucionFinanciadora = s.InstitucionFinanciadora,
                RolInvestigador = s.RolInvestigador,
                FechaInicio = s.FechaInicio,
                FechaFin = s.FechaFin,
                MesesDuracion = s.MesesDuracion,
                CodigoProyecto = s.CodigoProyecto,
                AreaTematica = s.AreaTematica,
                Descripcion = s.Descripcion,
                ArchivoNombre = s.ArchivoNombre,
                Estado = s.Estado,
                ComentariosRevision = s.ComentariosRevision,
                MotivoRechazo = s.MotivoRechazo,
                ComentariosSolicitud = s.ComentariosSolicitud,
                FechaCreacion = s.FechaCreacion,
                FechaRevision = s.FechaRevision
            }).ToList();

            return new ResponseSolicitudesEvidenciasAdminDto
            {
                Exitoso = true,
                Mensaje = "Solicitudes obtenidas correctamente",
                Solicitudes = solicitudesDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener solicitudes de evidencias para admin");
            return new ResponseSolicitudesEvidenciasAdminDto
            {
                Exitoso = false,
                Mensaje = $"Error al obtener solicitudes: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Revisar solicitud de evidencia (aprobar/rechazar)
    /// </summary>
    public async Task<ResponseGenericoEvidenciaDto> RevisarSolicitudEvidenciaAsync(RevisionSolicitudEvidenciaDto revision, string adminEmail)
    {
        try
        {
            var solicitud = await _solicitudEvidenciaRepository.GetByIdAsync(revision.SolicitudId);
            if (solicitud == null)
            {
                return new ResponseGenericoEvidenciaDto
                {
                    Exitoso = false,
                    Mensaje = "Solicitud no encontrada"
                };
            }

            if (solicitud.Estado != "Pendiente")
            {
                return new ResponseGenericoEvidenciaDto
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

            await _solicitudEvidenciaRepository.UpdateAsync(solicitud);

            // Si es aprobada, actualizar meses de investigación del docente
            if (solicitud.Estado == "Aprobada")
            {
                try
                {
                    var docente = await _docenteRepository.GetByCedulaAsync(solicitud.DocenteCedula);
                    if (docente != null)
                    {
                        docente.MesesInvestigacion = (docente.MesesInvestigacion ?? 0) + solicitud.MesesDuracion;
                        await _docenteRepository.UpdateAsync(docente);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al actualizar meses de investigación del docente para solicitud {SolicitudId}", solicitud.Id);
                    // No fallar la revisión por este error, solo loggearlo
                }
            }

            // Registrar auditoría
            await _auditoriaService.RegistrarAccionAsync(
                "REVISION_EVIDENCIA_INVESTIGACION",
                adminEmail,
                "SolicitudEvidenciaInvestigacion",
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
                    await _notificationService.NotificarCambioEstadoSolicitudAsync(
                        docenteNotif.Email,
                        "Evidencia de Investigación",
                        "Aprobada"
                    );
                }
                else
                {
                    await _notificationService.NotificarCambioEstadoSolicitudAsync(
                        docenteNotif.Email,
                        "Evidencia de Investigación",
                        "Rechazada"
                    );
                }
            }

            var accion = solicitud.Estado == "Aprobada" ? "aprobada" : "rechazada";
            return new ResponseGenericoEvidenciaDto
            {
                Exitoso = true,
                Mensaje = $"Solicitud {accion} correctamente"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al revisar solicitud {SolicitudId}", revision.SolicitudId);
            return new ResponseGenericoEvidenciaDto
            {
                Exitoso = false,
                Mensaje = $"Error al revisar solicitud: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Obtener archivo de evidencia para administradores
    /// </summary>
    public async Task<byte[]?> GetArchivoEvidenciaSolicitudAsync(Guid solicitudId)
    {
        try
        {
            var solicitud = await _solicitudEvidenciaRepository.GetByIdAsync(solicitudId);
            if (solicitud == null || string.IsNullOrEmpty(solicitud.ArchivoRuta))
            {
                return null;
            }

            var rutaCompleta = Path.Combine(Directory.GetCurrentDirectory(), solicitud.ArchivoRuta);
            if (!File.Exists(rutaCompleta))
            {
                return null;
            }

            return await File.ReadAllBytesAsync(rutaCompleta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener archivo de solicitud {SolicitudId}", solicitudId);
            return null;
        }
    }

    /// <summary>
    /// Validar que un archivo es un PDF válido
    /// </summary>
    private static bool EsPDF(byte[] archivo)
    {
        if (archivo == null || archivo.Length < 4)
            return false;

        return archivo[0] == 0x25 && // %
               archivo[1] == 0x50 && // P
               archivo[2] == 0x44 && // D
               archivo[3] == 0x46;   // F
    }
}
