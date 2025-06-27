using Microsoft.Extensions.Logging;
using SGA.Application.DTOs;
using SGA.Application.DTOs.Docentes;
using SGA.Application.Interfaces;
using SGA.Application.Interfaces.Repositories;
using SGA.Domain.Entities;
using SGA.Domain.Entities.External;

namespace SGA.Application.Services;

public class ObrasAcademicasService : IObrasAcademicasService
{
    private readonly IDocenteRepository _docenteRepository;
    private readonly IRepository<SolicitudObraAcademica> _solicitudRepository;
    private readonly IDIRINVDataService _dirInvDataService;
    private readonly IExternalDataService _externalDataService;
    private readonly INotificationService _notificationService;
    private readonly IAuditoriaService _auditoriaService;
    private readonly ILogger<ObrasAcademicasService> _logger;

    public ObrasAcademicasService(
        IDocenteRepository docenteRepository,
        IRepository<SolicitudObraAcademica> solicitudRepository,
        IDIRINVDataService dirInvDataService,
        IExternalDataService externalDataService,
        INotificationService notificationService,
        IAuditoriaService auditoriaService,
        ILogger<ObrasAcademicasService> logger)
    {
        _docenteRepository = docenteRepository;
        _solicitudRepository = solicitudRepository;
        _dirInvDataService = dirInvDataService;
        _externalDataService = externalDataService;
        _notificationService = notificationService;
        _auditoriaService = auditoriaService;
        _logger = logger;
    }

    public async Task<ResponseObrasAcademicasDto> GetObrasDocenteAsync(string cedula)
    {
        try
        {
            var obras = await _dirInvDataService.GetObrasDocenteAsync(cedula);
            var obrasDto = obras.Select((o, index) => new ObraAcademicaDetalleDto
            {
                Id = index + 1, // Usar índice temporal
                Titulo = o.Titulo,
                TipoObra = o.Tipo,
                FechaPublicacion = o.FechaPublicacion,
                Editorial = null,
                Revista = o.Revista,
                ISBN_ISSN = null,
                DOI = null,
                EsIndexada = false,
                IndiceIndexacion = null,
                Autores = o.Autores,
                Descripcion = null,
                ArchivoNombre = null,
                TieneArchivo = false,
                FechaCreacion = o.FechaPublicacion,
                FechaActualizacion = null
            }).ToList();

            return new ResponseObrasAcademicasDto
            {
                Exitoso = true,
                Mensaje = "Obras académicas obtenidas correctamente",
                Obras = obrasDto,
                TotalObras = obrasDto.Count
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener obras académicas para cédula {Cedula}", cedula);
            return new ResponseObrasAcademicasDto
            {
                Exitoso = false,
                Mensaje = $"Error al obtener obras académicas: {ex.Message}"
            };
        }
    }

    public async Task<ResponseObrasAcademicasDto> SolicitarNuevasObrasAsync(string cedula, SolicitudObrasAcademicasDto solicitud)
    {
        try
        {
            _logger.LogInformation($"Iniciando solicitud de nuevas obras para cédula: {cedula}");
            
            var docente = await _docenteRepository.GetByCedulaAsync(cedula);
            if (docente == null)
            {
                _logger.LogWarning($"Docente no encontrado para cédula: {cedula}");
                return new ResponseObrasAcademicasDto
                {
                    Exitoso = false,
                    Mensaje = "Docente no encontrado"
                };
            }

            _logger.LogInformation($"Docente encontrado: {docente.NombreCompleto} (ID: {docente.Id})");

            var grupoId = Guid.NewGuid();
            var obrasSolicitadas = new List<ObraAcademicaDetalleDto>();

            _logger.LogInformation($"Procesando {solicitud.Obras.Count} obras para solicitud");

            // Procesar cada obra individualmente con manejo de errores robusto
            for (int i = 0; i < solicitud.Obras.Count; i++)
            {
                var obra = solicitud.Obras[i];
                _logger.LogInformation($"Procesando obra {i + 1}/{solicitud.Obras.Count}: {obra.Titulo}");
                
                try
                {
                    var nuevaSolicitud = new SolicitudObraAcademica();
                    
                    // Establecer propiedades una por una para identificar posibles problemas
                    nuevaSolicitud.DocenteId = docente.Id;
                    nuevaSolicitud.DocenteCedula = cedula;
                    nuevaSolicitud.SolicitudGrupoId = grupoId;
                    nuevaSolicitud.Titulo = obra.Titulo?.Trim() ?? "";
                    nuevaSolicitud.TipoObra = obra.TipoObra?.Trim() ?? "";
                    nuevaSolicitud.FechaPublicacion = obra.FechaPublicacion;
                    nuevaSolicitud.Editorial = string.IsNullOrWhiteSpace(obra.Editorial) ? null : obra.Editorial.Trim();
                    nuevaSolicitud.Revista = string.IsNullOrWhiteSpace(obra.Revista) ? null : obra.Revista.Trim();
                    nuevaSolicitud.ISBN_ISSN = string.IsNullOrWhiteSpace(obra.ISBN_ISSN) ? null : obra.ISBN_ISSN.Trim();
                    nuevaSolicitud.DOI = string.IsNullOrWhiteSpace(obra.DOI) ? null : obra.DOI.Trim();
                    nuevaSolicitud.EsIndexada = obra.EsIndexada;
                    nuevaSolicitud.IndiceIndexacion = string.IsNullOrWhiteSpace(obra.IndiceIndexacion) ? null : obra.IndiceIndexacion.Trim();
                    nuevaSolicitud.Autores = string.IsNullOrWhiteSpace(obra.Autores) ? null : obra.Autores.Trim();
                    nuevaSolicitud.Descripcion = string.IsNullOrWhiteSpace(obra.Descripcion) ? null : obra.Descripcion.Trim();
                    nuevaSolicitud.ArchivoNombre = string.IsNullOrWhiteSpace(obra.ArchivoNombre) ? null : obra.ArchivoNombre.Trim();
                    nuevaSolicitud.ArchivoTipo = string.IsNullOrWhiteSpace(obra.ArchivoTipo) ? null : obra.ArchivoTipo.Trim();
                    nuevaSolicitud.Estado = "Pendiente";
                    nuevaSolicitud.ComentariosSolicitud = string.IsNullOrWhiteSpace(solicitud.Comentarios) ? null : solicitud.Comentarios.Trim();

                    // Manejar archivo si se proporciona
                    if (!string.IsNullOrEmpty(obra.ArchivoContenido))
                    {
                        var archivoRuta = await GuardarArchivoAsync(obra.ArchivoContenido, obra.ArchivoNombre, nuevaSolicitud.Id);
                        nuevaSolicitud.ArchivoRuta = archivoRuta;
                        nuevaSolicitud.ArchivoTamano = Convert.FromBase64String(obra.ArchivoContenido).Length;
                    }

                    // Guardar la solicitud individualmente
                    await _solicitudRepository.AddAsync(nuevaSolicitud);
                    
                    _logger.LogInformation($"Obra {i + 1} guardada exitosamente con ID: {nuevaSolicitud.Id}");

                    obrasSolicitadas.Add(new ObraAcademicaDetalleDto
                    {
                        Id = 0, // Es una solicitud, no tiene ID en DIRINV aún
                        Titulo = obra.Titulo ?? "",
                        TipoObra = obra.TipoObra ?? "",
                        FechaPublicacion = obra.FechaPublicacion,
                        Editorial = obra.Editorial,
                        Revista = obra.Revista,
                        ISBN_ISSN = obra.ISBN_ISSN,
                        DOI = obra.DOI,
                        EsIndexada = obra.EsIndexada,
                        IndiceIndexacion = obra.IndiceIndexacion,
                        Autores = obra.Autores,
                        Descripcion = obra.Descripcion,
                        ArchivoNombre = obra.ArchivoNombre,
                        TieneArchivo = !string.IsNullOrEmpty(obra.ArchivoContenido),
                        FechaCreacion = DateTime.UtcNow
                    });
                }
                catch (Exception obraEx)
                {
                    _logger.LogError(obraEx, $"Error al procesar obra {i + 1}: {obra.Titulo}");
                    throw new Exception($"Error al procesar obra '{obra.Titulo}': {obraEx.Message}", obraEx);
                }
            }

            // Registrar auditoría
            _logger.LogInformation($"Registrando auditoría para solicitud de {solicitud.Obras.Count} obras");
            await _auditoriaService.RegistrarAccionAsync(
                "SOLICITAR_NUEVAS_OBRAS",
                docente.Id.ToString(),
                docente.Email,
                "SolicitudObraAcademica",
                null,
                $"Solicitadas {solicitud.Obras.Count} nuevas obras académicas",
                grupoId.ToString()
            );

            // Enviar notificación a administradores
            _logger.LogInformation("Enviando notificación a administradores");
            await _notificationService.NotificarNuevaSolicitudObrasAsync(docente.NombreCompleto, solicitud.Obras.Count);

            _logger.LogInformation($"Solicitud completada exitosamente para {docente.NombreCompleto}");
            
            return new ResponseObrasAcademicasDto
            {
                Exitoso = true,
                Mensaje = $"Solicitud enviada correctamente. {solicitud.Obras.Count} obras enviadas para revisión.",
                Obras = obrasSolicitadas,
                TotalObras = obrasSolicitadas.Count
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al solicitar nuevas obras para cédula {Cedula}", cedula);
            return new ResponseObrasAcademicasDto
            {
                Exitoso = false,
                Mensaje = $"Error al procesar solicitud: {ex.Message}"
            };
        }
    }

    public async Task<ResponseObrasAcademicasDto> GetSolicitudesPendientesAsync(string cedula)
    {
        try
        {
            var docente = await _docenteRepository.GetByCedulaAsync(cedula);
            if (docente == null)
            {
                return new ResponseObrasAcademicasDto
                {
                    Exitoso = false,
                    Mensaje = "Docente no encontrado"
                };
            }

            var solicitudes = await _solicitudRepository.GetAllAsync();
            var solicitudesPendientes = solicitudes
                .Where(s => s.DocenteId == docente.Id && s.Estado == "Pendiente")
                .OrderByDescending(s => s.FechaCreacion)
                .Select(s => new ObraAcademicaDetalleDto
                {
                    Id = 0, // Es una solicitud pendiente
                    Titulo = s.Titulo,
                    TipoObra = s.TipoObra,
                    FechaPublicacion = s.FechaPublicacion,
                    Editorial = s.Editorial,
                    Revista = s.Revista,
                    ISBN_ISSN = s.ISBN_ISSN,
                    DOI = s.DOI,
                    EsIndexada = s.EsIndexada,
                    IndiceIndexacion = s.IndiceIndexacion,
                    Autores = s.Autores,
                    Descripcion = s.Descripcion,
                    ArchivoNombre = s.ArchivoNombre,
                    TieneArchivo = !string.IsNullOrEmpty(s.ArchivoRuta),
                    FechaCreacion = s.FechaCreacion,
                    FechaActualizacion = s.FechaModificacion
                })
                .ToList();

            return new ResponseObrasAcademicasDto
            {
                Exitoso = true,
                Mensaje = "Solicitudes pendientes obtenidas correctamente",
                Obras = solicitudesPendientes,
                TotalObras = solicitudesPendientes.Count
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener solicitudes pendientes para cédula {Cedula}", cedula);
            return new ResponseObrasAcademicasDto
            {
                Exitoso = false,
                Mensaje = $"Error al obtener solicitudes pendientes: {ex.Message}"
            };
        }
    }

    public async Task<ResponseObrasAcademicasDto> AprobarSolicitudAsync(Guid solicitudId, string comentarios)
    {
        try
        {
            var solicitudesAll = await _solicitudRepository.GetAllAsync();
            var solicitud = solicitudesAll.FirstOrDefault(s => s.Id == solicitudId);
            
            if (solicitud == null)
            {
                return new ResponseObrasAcademicasDto
                {
                    Exitoso = false,
                    Mensaje = "Solicitud no encontrada"
                };
            }

            // Aprobar la solicitud
            solicitud.Estado = "Aprobada";
            solicitud.ComentariosRevision = comentarios;
            solicitud.FechaRevision = DateTime.UtcNow;
            // solicitud.RevisadoPorId = // TODO: Obtener del contexto actual
            await _solicitudRepository.UpdateAsync(solicitud);

            // Agregar la obra a DIRINV
            var nuevaObra = new ObraAcademicaDIRINV
            {
                Cedula = solicitud.DocenteCedula,
                Titulo = solicitud.Titulo,
                TipoObra = solicitud.TipoObra,
                FechaPublicacion = solicitud.FechaPublicacion,
                Editorial = solicitud.Editorial,
                Revista = solicitud.Revista,
                ISBN_ISSN = solicitud.ISBN_ISSN,
                EsIndexada = solicitud.EsIndexada,
                IndiceIndexacion = solicitud.IndiceIndexacion
            };

            // Crear el DTO para el servicio de DIRINV
            var obraDto = new DTOs.ExternalData.ObraAcademicaDto
            {
                Titulo = nuevaObra.Titulo,
                Tipo = nuevaObra.TipoObra,
                FechaPublicacion = nuevaObra.FechaPublicacion,
                Revista = nuevaObra.Revista ?? string.Empty,
                Autores = nuevaObra.Autores ?? string.Empty
            };

            await _dirInvDataService.AddObraAcademicaAsync(solicitud.DocenteCedula, obraDto);

            // Actualizar contador en el docente
            var docente = await _docenteRepository.GetByIdAsync(solicitud.DocenteId);
            if (docente != null)
            {
                docente.NumeroObrasAcademicas++;
                docente.FechaUltimaImportacion = DateTime.UtcNow;
                await _docenteRepository.UpdateAsync(docente);
            }

            // Notificar al docente
            await _notificationService.NotificarAprobacionObraAsync(
                docente?.Email ?? "", 
                solicitud.Titulo, 
                comentarios);

            return new ResponseObrasAcademicasDto
            {
                Exitoso = true,
                Mensaje = "Obra académica aprobada y agregada correctamente"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al aprobar solicitud {SolicitudId}", solicitudId);
            return new ResponseObrasAcademicasDto
            {
                Exitoso = false,
                Mensaje = $"Error al aprobar solicitud: {ex.Message}"
            };
        }
    }

    public async Task<ResponseObrasAcademicasDto> RechazarSolicitudAsync(Guid solicitudId, string motivo)
    {
        try
        {
            var solicitudesAll = await _solicitudRepository.GetAllAsync();
            var solicitud = solicitudesAll.FirstOrDefault(s => s.Id == solicitudId);
            
            if (solicitud == null)
            {
                return new ResponseObrasAcademicasDto
                {
                    Exitoso = false,
                    Mensaje = "Solicitud no encontrada"
                };
            }

            solicitud.Estado = "Rechazada";
            solicitud.MotivoRechazo = motivo;
            solicitud.FechaRevision = DateTime.UtcNow;
            await _solicitudRepository.UpdateAsync(solicitud);

            // Notificar al docente
            var docente = await _docenteRepository.GetByIdAsync(solicitud.DocenteId);
            await _notificationService.NotificarRechazoObraAsync(
                docente?.Email ?? "", 
                solicitud.Titulo, 
                motivo);

            return new ResponseObrasAcademicasDto
            {
                Exitoso = true,
                Mensaje = "Solicitud rechazada correctamente"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al rechazar solicitud {SolicitudId}", solicitudId);
            return new ResponseObrasAcademicasDto
            {
                Exitoso = false,
                Mensaje = $"Error al rechazar solicitud: {ex.Message}"
            };
        }
    }

    public async Task<ResponseObrasAcademicasDto> GetSolicitudesPorRevisarAsync()
    {
        try
        {
            var solicitudesAll = await _solicitudRepository.GetAllAsync();
            var solicitudesPendientes = solicitudesAll
                .Where(s => s.Estado == "Pendiente")
                .OrderBy(s => s.FechaCreacion)
                .Select(s => new ObraAcademicaDetalleDto
                {
                    Id = 0,
                    Titulo = s.Titulo,
                    TipoObra = s.TipoObra,
                    FechaPublicacion = s.FechaPublicacion,
                    Editorial = s.Editorial,
                    Revista = s.Revista,
                    ISBN_ISSN = s.ISBN_ISSN,
                    DOI = s.DOI,
                    EsIndexada = s.EsIndexada,
                    IndiceIndexacion = s.IndiceIndexacion,
                    Autores = s.Autores,
                    Descripcion = s.Descripcion,
                    ArchivoNombre = s.ArchivoNombre,
                    TieneArchivo = !string.IsNullOrEmpty(s.ArchivoRuta),
                    FechaCreacion = s.FechaCreacion
                })
                .ToList();

            return new ResponseObrasAcademicasDto
            {
                Exitoso = true,
                Mensaje = "Solicitudes por revisar obtenidas correctamente",
                Obras = solicitudesPendientes,
                TotalObras = solicitudesPendientes.Count
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener solicitudes por revisar");
            return new ResponseObrasAcademicasDto
            {
                Exitoso = false,
                Mensaje = $"Error al obtener solicitudes: {ex.Message}"
            };
        }
    }

    public async Task<byte[]?> DescargarArchivoObraAsync(Guid solicitudId)
    {
        try
        {
            var solicitudesAll = await _solicitudRepository.GetAllAsync();
            var solicitud = solicitudesAll.FirstOrDefault(s => s.Id == solicitudId);
            
            if (solicitud == null || string.IsNullOrEmpty(solicitud.ArchivoRuta))
                return null;

            if (File.Exists(solicitud.ArchivoRuta))
            {
                return await File.ReadAllBytesAsync(solicitud.ArchivoRuta);
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al descargar archivo para solicitud {SolicitudId}", solicitudId);
            return null;
        }
    }

    public async Task<ImportarDatosResponse> ImportarObrasDesdeExternoAsync(string cedula)
    {
        try
        {
            var datosDirInv = await _externalDataService.ImportarDatosDirInvAsync(cedula);
            if (datosDirInv == null)
            {
                return new ImportarDatosResponse
                {
                    Exitoso = false,
                    Mensaje = "No se pudieron obtener datos de DIRINV"
                };
            }

            return new ImportarDatosResponse
            {
                Exitoso = true,
                Mensaje = $"Importación exitosa. {datosDirInv.NumeroObrasAcademicas} obras académicas encontradas.",
                DatosImportados = new Dictionary<string, object?>
                {
                    ["NumeroObrasAcademicas"] = datosDirInv.NumeroObrasAcademicas,
                    ["NumeroProyectos"] = datosDirInv.ProyectosActivos,
                    ["FechaUltimaPublicacion"] = datosDirInv.FechaUltimaPublicacion
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al importar obras desde sistema externo para cédula {Cedula}", cedula);
            return new ImportarDatosResponse
            {
                Exitoso = false,
                Mensaje = $"Error al importar obras: {ex.Message}"
            };
        }
    }

    private async Task<string> GuardarArchivoAsync(string archivoBase64, string? nombreArchivo, Guid solicitudId)
    {
        try
        {
            var bytes = Convert.FromBase64String(archivoBase64);
            var carpetaObras = Path.Combine("uploads", "obras-academicas");
            
            if (!Directory.Exists(carpetaObras))
                Directory.CreateDirectory(carpetaObras);

            var extension = Path.GetExtension(nombreArchivo ?? "documento.pdf");
            var nombreUnico = $"{solicitudId}{extension}";
            var rutaCompleta = Path.Combine(carpetaObras, nombreUnico);

            await File.WriteAllBytesAsync(rutaCompleta, bytes);
            return rutaCompleta;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar archivo para solicitud {SolicitudId}", solicitudId);
            throw;
        }
    }
    
    // Métodos para administradores
    public async Task<ResponseSolicitudesAdminDto> GetTodasSolicitudesAsync()
    {
        try
        {
            var solicitudes = await _solicitudRepository.GetAllAsync();
            var solicitudesDto = solicitudes.Select(s => new SolicitudObraAcademicaAdminDto
            {
                Id = s.Id,
                DocenteCedula = s.DocenteCedula,
                Titulo = s.Titulo,
                TipoObra = s.TipoObra,
                FechaPublicacion = s.FechaPublicacion,
                Editorial = s.Editorial,
                Revista = s.Revista,
                ISBN_ISSN = s.ISBN_ISSN,
                DOI = s.DOI,
                EsIndexada = s.EsIndexada,
                IndiceIndexacion = s.IndiceIndexacion,
                Autores = s.Autores,
                Descripcion = s.Descripcion,
                ArchivoNombre = s.ArchivoNombre,
                Estado = s.Estado,
                ComentariosRevision = s.ComentariosRevision,
                MotivoRechazo = s.MotivoRechazo,
                ComentariosSolicitud = s.ComentariosSolicitud,
                FechaCreacion = s.FechaCreacion,
                FechaRevision = s.FechaRevision
            }).OrderByDescending(s => s.FechaCreacion).ToList();

            return new ResponseSolicitudesAdminDto
            {
                Exitoso = true,
                Mensaje = "Solicitudes obtenidas correctamente",
                Solicitudes = solicitudesDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las solicitudes");
            return new ResponseSolicitudesAdminDto
            {
                Exitoso = false,
                Mensaje = $"Error al obtener solicitudes: {ex.Message}"
            };
        }
    }

    public async Task<ResponseGenericoDto> RevisarSolicitudAsync(RevisionSolicitudDto revision, string adminEmail)
    {
        try
        {
            var solicitud = await _solicitudRepository.GetByIdAsync(revision.SolicitudId);
            if (solicitud == null)
            {
                return new ResponseGenericoDto
                {
                    Exitoso = false,
                    Mensaje = "Solicitud no encontrada"
                };
            }

            if (solicitud.Estado != "Pendiente")
            {
                return new ResponseGenericoDto
                {
                    Exitoso = false,
                    Mensaje = "La solicitud ya ha sido revisada"
                };
            }

            // Actualizar estado de la solicitud
            solicitud.Estado = revision.Accion == "Aprobar" ? "Aprobada" : "Rechazada";
            solicitud.ComentariosRevision = revision.ComentariosRevision;
            solicitud.MotivoRechazo = revision.MotivoRechazo;
            solicitud.FechaRevision = DateTime.UtcNow;
            solicitud.FechaModificacion = DateTime.UtcNow;

            await _solicitudRepository.UpdateAsync(solicitud);

            // Si es aprobada, agregar la obra a DIRINV
            if (solicitud.Estado == "Aprobada")
            {
                try
                {
                    var obraDto = new DTOs.ExternalData.ObraAcademicaDto
                    {
                        Titulo = solicitud.Titulo,
                        Tipo = solicitud.TipoObra,
                        FechaPublicacion = solicitud.FechaPublicacion,
                        Revista = solicitud.Revista ?? string.Empty,
                        Autores = solicitud.Autores ?? string.Empty
                    };

                    await _dirInvDataService.AddObraAcademicaAsync(solicitud.DocenteCedula, obraDto);
                    
                    // Actualizar contador del docente
                    var docente = await _docenteRepository.GetByCedulaAsync(solicitud.DocenteCedula);
                    if (docente != null)
                    {
                        docente.NumeroObrasAcademicas = (docente.NumeroObrasAcademicas ?? 0) + 1;
                        await _docenteRepository.UpdateAsync(docente);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al agregar obra a DIRINV para solicitud {SolicitudId}", solicitud.Id);
                    // No fallar la revisión por este error, solo loggearlo
                }
            }

            // Registrar auditoría
            await _auditoriaService.RegistrarAccionAsync(
                "RevisionObraAcademica",
                adminEmail,
                "SolicitudObraAcademica",
                solicitud.Id.ToString(),
                $"Estado anterior: Pendiente",
                $"Estado nuevo: {solicitud.Estado}. Motivo: {revision.MotivoRechazo}",
                "Sistema" // IP no disponible en este contexto
            );

            // Enviar notificación al docente
            var docenteNotif = await _docenteRepository.GetByCedulaAsync(solicitud.DocenteCedula);
            if (docenteNotif != null)
            {
                if (solicitud.Estado == "Aprobada")
                {
                    await _notificationService.NotificarAprobacionObraAsync(
                        docenteNotif.Email,
                        solicitud.Titulo,
                        revision.ComentariosRevision ?? "Obra aprobada"
                    );
                }
                else
                {
                    await _notificationService.NotificarRechazoObraAsync(
                        docenteNotif.Email,
                        solicitud.Titulo,
                        revision.MotivoRechazo ?? "No especificado"
                    );
                }
            }

            var accion = solicitud.Estado == "Aprobada" ? "aprobada" : "rechazada";
            return new ResponseGenericoDto
            {
                Exitoso = true,
                Mensaje = $"Solicitud {accion} correctamente"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al revisar solicitud {SolicitudId}", revision.SolicitudId);
            return new ResponseGenericoDto
            {
                Exitoso = false,
                Mensaje = $"Error al revisar solicitud: {ex.Message}"
            };
        }
    }

    public async Task<byte[]?> GetArchivoSolicitudAsync(Guid solicitudId)
    {
        try
        {
            var solicitud = await _solicitudRepository.GetByIdAsync(solicitudId);
            if (solicitud == null || string.IsNullOrEmpty(solicitud.ArchivoRuta))
                return null;

            if (File.Exists(solicitud.ArchivoRuta))
            {
                return await File.ReadAllBytesAsync(solicitud.ArchivoRuta);
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener archivo de solicitud {SolicitudId}", solicitudId);
            return null;
        }
    }
}
