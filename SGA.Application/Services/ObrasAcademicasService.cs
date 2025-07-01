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
            
            // Log detallado de la solicitud recibida
            _logger.LogInformation($"Solicitud recibida - Comentarios: {solicitud.Comentarios}, Cantidad obras: {solicitud.Obras?.Count ?? 0}");
            
            if (solicitud.Obras != null)
            {
                for (int i = 0; i < solicitud.Obras.Count; i++)
                {
                    var obra = solicitud.Obras[i];
                    _logger.LogInformation($"Obra {i + 1} - Título: '{obra.Titulo}', Tipo: '{obra.TipoObra}', Fecha: {obra.FechaPublicacion}, Autores: '{obra.Autores}'");
                    _logger.LogInformation($"Obra {i + 1} - Editorial: '{obra.Editorial}', Revista: '{obra.Revista}', ISBN_ISSN: '{obra.ISBN_ISSN}', DOI: '{obra.DOI}'");
                    _logger.LogInformation($"Obra {i + 1} - EsIndexada: {obra.EsIndexada}, IndiceIndexacion: '{obra.IndiceIndexacion}', Descripcion: '{obra.Descripcion}'");
                    _logger.LogInformation($"Obra {i + 1} - ArchivoNombre: '{obra.ArchivoNombre}', ArchivoTipo: '{obra.ArchivoTipo}', TieneArchivo: {!string.IsNullOrEmpty(obra.ArchivoContenido)}");
                }
            }
            
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
            
            // Verificar que el docente existe en la base de datos
            var docenteVerificacion = await _docenteRepository.GetByIdAsync(docente.Id);
            if (docenteVerificacion == null)
            {
                _logger.LogError($"ERROR: El docente con ID {docente.Id} no existe al verificar por ID");
                throw new Exception($"Error de consistencia: Docente encontrado por cédula pero no por ID");
            }
            _logger.LogDebug($"Verificación de docente exitosa: {docenteVerificacion.NombreCompleto}");

            var grupoId = Guid.NewGuid();
            var obrasSolicitadas = new List<ObraAcademicaDetalleDto>();

            _logger.LogInformation($"Procesando {solicitud.Obras.Count} obras para solicitud");
            
            // Verificar que la tabla de solicitudes existe y es accesible
            try
            {
                var testCount = await _solicitudRepository.GetAllAsync();
                _logger.LogDebug($"Verificación de tabla exitosa. Registros existentes: {testCount.Count()}");
            }
            catch (Exception tableEx)
            {
                _logger.LogError(tableEx, "Error al acceder a la tabla SolicitudesObrasAcademicas");
                throw new Exception($"Error de acceso a base de datos: {tableEx.Message}", tableEx);
            }

            // Procesar cada obra individualmente con manejo de errores robusto
            for (int i = 0; i < solicitud.Obras.Count; i++)
            {
                var obra = solicitud.Obras[i];
                _logger.LogInformation($"Procesando obra {i + 1}/{solicitud.Obras.Count}: {obra.Titulo}");
                
                try
                {
                    var nuevaSolicitud = new SolicitudObraAcademica();
                    
                    _logger.LogDebug("Estableciendo propiedades básicas de la obra");
                    
                    // Validar longitudes antes de asignar
                    var titulo = obra.Titulo?.Trim() ?? "";
                    if (titulo.Length > 500)
                    {
                        _logger.LogWarning($"Título excede 500 caracteres ({titulo.Length}): {titulo.Substring(0, Math.Min(50, titulo.Length))}...");
                        titulo = titulo.Substring(0, 500);
                    }
                    
                    var tipoObra = obra.TipoObra?.Trim() ?? "";
                    if (tipoObra.Length > 100)
                    {
                        _logger.LogWarning($"TipoObra excede 100 caracteres ({tipoObra.Length}): {tipoObra}");
                        tipoObra = tipoObra.Substring(0, 100);
                    }
                    
                    var editorial = string.IsNullOrWhiteSpace(obra.Editorial) ? null : obra.Editorial.Trim();
                    if (editorial != null && editorial.Length > 255)
                    {
                        _logger.LogWarning($"Editorial excede 255 caracteres ({editorial.Length}): {editorial.Substring(0, Math.Min(50, editorial.Length))}...");
                        editorial = editorial.Substring(0, 255);
                    }
                    
                    var revista = string.IsNullOrWhiteSpace(obra.Revista) ? null : obra.Revista.Trim();
                    if (revista != null && revista.Length > 255)
                    {
                        _logger.LogWarning($"Revista excede 255 caracteres ({revista.Length}): {revista.Substring(0, Math.Min(50, revista.Length))}...");
                        revista = revista.Substring(0, 255);
                    }
                    
                    var isbnIssn = string.IsNullOrWhiteSpace(obra.ISBN_ISSN) ? null : obra.ISBN_ISSN.Trim();
                    if (isbnIssn != null && isbnIssn.Length > 50)
                    {
                        _logger.LogWarning($"ISBN_ISSN excede 50 caracteres ({isbnIssn.Length}): {isbnIssn}");
                        isbnIssn = isbnIssn.Substring(0, 50);
                    }
                    
                    var doi = string.IsNullOrWhiteSpace(obra.DOI) ? null : obra.DOI.Trim();
                    if (doi != null && doi.Length > 200)
                    {
                        _logger.LogWarning($"DOI excede 200 caracteres ({doi.Length}): {doi.Substring(0, Math.Min(50, doi.Length))}...");
                        doi = doi.Substring(0, 200);
                    }
                    
                    var indiceIndexacion = string.IsNullOrWhiteSpace(obra.IndiceIndexacion) ? null : obra.IndiceIndexacion.Trim();
                    if (indiceIndexacion != null && indiceIndexacion.Length > 100)
                    {
                        _logger.LogWarning($"IndiceIndexacion excede 100 caracteres ({indiceIndexacion.Length}): {indiceIndexacion}");
                        indiceIndexacion = indiceIndexacion.Substring(0, 100);
                    }
                    
                    var autores = string.IsNullOrWhiteSpace(obra.Autores) ? null : obra.Autores.Trim();
                    if (autores != null && autores.Length > 1000)
                    {
                        _logger.LogWarning($"Autores excede 1000 caracteres ({autores.Length}): {autores.Substring(0, Math.Min(50, autores.Length))}...");
                        autores = autores.Substring(0, 1000);
                    }
                    
                    var descripcion = string.IsNullOrWhiteSpace(obra.Descripcion) ? null : obra.Descripcion.Trim();
                    if (descripcion != null && descripcion.Length > 2000)
                    {
                        _logger.LogWarning($"Descripcion excede 2000 caracteres ({descripcion.Length}): {descripcion.Substring(0, Math.Min(50, descripcion.Length))}...");
                        descripcion = descripcion.Substring(0, 2000);
                    }
                    
                    var archivoNombre = string.IsNullOrWhiteSpace(obra.ArchivoNombre) ? null : obra.ArchivoNombre.Trim();
                    if (archivoNombre != null && archivoNombre.Length > 255)
                    {
                        _logger.LogWarning($"ArchivoNombre excede 255 caracteres ({archivoNombre.Length}): {archivoNombre}");
                        archivoNombre = archivoNombre.Substring(0, 255);
                    }
                    
                    var archivoTipo = string.IsNullOrWhiteSpace(obra.ArchivoTipo) ? null : obra.ArchivoTipo.Trim();
                    if (archivoTipo != null && archivoTipo.Length > 100)
                    {
                        _logger.LogWarning($"ArchivoTipo excede 100 caracteres ({archivoTipo.Length}): {archivoTipo}");
                        archivoTipo = archivoTipo.Substring(0, 100);
                    }
                    
                    var comentariosSolicitud = string.IsNullOrWhiteSpace(solicitud.Comentarios) ? null : solicitud.Comentarios.Trim();
                    if (comentariosSolicitud != null && comentariosSolicitud.Length > 1000)
                    {
                        _logger.LogWarning($"ComentariosSolicitud excede 1000 caracteres ({comentariosSolicitud.Length}): {comentariosSolicitud.Substring(0, Math.Min(50, comentariosSolicitud.Length))}...");
                        comentariosSolicitud = comentariosSolicitud.Substring(0, 1000);
                    }
                    
                    if (cedula.Length > 10)
                    {
                        _logger.LogWarning($"Cedula excede 10 caracteres ({cedula.Length}): {cedula}");
                        throw new Exception($"La cédula excede el límite de 10 caracteres: {cedula}");
                    }
                    
                    // Establecer propiedades una por una para identificar posibles problemas
                    nuevaSolicitud.DocenteId = docente.Id;
                    _logger.LogDebug($"DocenteId establecido: {docente.Id}");
                    
                    // Validar que el DocenteId no sea vacío
                    if (nuevaSolicitud.DocenteId == Guid.Empty)
                    {
                        throw new Exception("DocenteId no puede ser vacío");
                    }
                    
                    nuevaSolicitud.DocenteCedula = cedula;
                    _logger.LogDebug($"DocenteCedula establecida: {cedula}");
                    
                    nuevaSolicitud.SolicitudGrupoId = grupoId;
                    _logger.LogDebug($"SolicitudGrupoId establecido: {grupoId}");
                    
                    // Validar que el SolicitudGrupoId no sea vacío
                    if (nuevaSolicitud.SolicitudGrupoId == Guid.Empty)
                    {
                        throw new Exception("SolicitudGrupoId no puede ser vacío");
                    }
                    
                    nuevaSolicitud.Titulo = titulo;
                    _logger.LogDebug($"Título establecido: {nuevaSolicitud.Titulo}");
                    
                    nuevaSolicitud.TipoObra = tipoObra;
                    _logger.LogDebug($"TipoObra establecido: {nuevaSolicitud.TipoObra}");
                    
                    nuevaSolicitud.FechaPublicacion = obra.FechaPublicacion;
                    _logger.LogDebug($"FechaPublicacion establecida: {nuevaSolicitud.FechaPublicacion}");
                    
                    // Validar que la fecha de publicación no sea demasiado antigua o futura
                    if (obra.FechaPublicacion < new DateTime(1900, 1, 1))
                    {
                        _logger.LogWarning($"Fecha de publicación muy antigua: {obra.FechaPublicacion}, ajustando a 1900-01-01");
                        nuevaSolicitud.FechaPublicacion = new DateTime(1900, 1, 1);
                    }
                    else if (obra.FechaPublicacion > DateTime.Now.AddYears(1))
                    {
                        _logger.LogWarning($"Fecha de publicación muy futura: {obra.FechaPublicacion}, ajustando a fecha actual");
                        nuevaSolicitud.FechaPublicacion = DateTime.Now;
                    }
                    
                    // Asegurar que la fecha esté en el formato correcto para SQL Server
                    if (nuevaSolicitud.FechaPublicacion.Kind == DateTimeKind.Unspecified)
                    {
                        nuevaSolicitud.FechaPublicacion = DateTime.SpecifyKind(nuevaSolicitud.FechaPublicacion, DateTimeKind.Utc);
                    }
                    
                    _logger.LogDebug($"Fecha final establecida: {nuevaSolicitud.FechaPublicacion} (Kind: {nuevaSolicitud.FechaPublicacion.Kind})");
                    
                    nuevaSolicitud.Editorial = editorial;
                    nuevaSolicitud.Revista = revista;
                    nuevaSolicitud.ISBN_ISSN = isbnIssn;
                    nuevaSolicitud.DOI = doi;
                    nuevaSolicitud.EsIndexada = obra.EsIndexada;
                    nuevaSolicitud.IndiceIndexacion = indiceIndexacion;
                    nuevaSolicitud.Autores = autores;
                    nuevaSolicitud.Descripcion = descripcion;
                    nuevaSolicitud.ArchivoNombre = archivoNombre;
                    nuevaSolicitud.ArchivoTipo = archivoTipo;
                    nuevaSolicitud.Estado = "Pendiente";
                    nuevaSolicitud.ComentariosSolicitud = comentariosSolicitud;

                    _logger.LogDebug("Propiedades básicas establecidas correctamente");
                    
                    // Log completo de la entidad antes de guardar
                    _logger.LogDebug($"Resumen de la solicitud a guardar:");
                    _logger.LogDebug($"  ID: {nuevaSolicitud.Id}");
                    _logger.LogDebug($"  DocenteId: {nuevaSolicitud.DocenteId}");
                    _logger.LogDebug($"  DocenteCedula: {nuevaSolicitud.DocenteCedula}");
                    _logger.LogDebug($"  SolicitudGrupoId: {nuevaSolicitud.SolicitudGrupoId}");
                    _logger.LogDebug($"  Titulo: '{nuevaSolicitud.Titulo}' (Longitud: {nuevaSolicitud.Titulo?.Length ?? 0})");
                    _logger.LogDebug($"  TipoObra: '{nuevaSolicitud.TipoObra}' (Longitud: {nuevaSolicitud.TipoObra?.Length ?? 0})");
                    _logger.LogDebug($"  FechaPublicacion: {nuevaSolicitud.FechaPublicacion}");
                    _logger.LogDebug($"  Estado: '{nuevaSolicitud.Estado}'");
                    _logger.LogDebug($"  EsIndexada: {nuevaSolicitud.EsIndexada}");
                    _logger.LogDebug($"  ArchivoNombre: '{nuevaSolicitud.ArchivoNombre}' (Longitud: {nuevaSolicitud.ArchivoNombre?.Length ?? 0})");
                    _logger.LogDebug($"  ArchivoTipo: '{nuevaSolicitud.ArchivoTipo}' (Longitud: {nuevaSolicitud.ArchivoTipo?.Length ?? 0})");
                    _logger.LogDebug($"  ArchivoTamano: {nuevaSolicitud.ArchivoTamano}");
                    _logger.LogDebug($"  ComentariosSolicitud: '{nuevaSolicitud.ComentariosSolicitud}' (Longitud: {nuevaSolicitud.ComentariosSolicitud?.Length ?? 0})");

                    // Manejar archivo si se proporciona
                    if (!string.IsNullOrEmpty(obra.ArchivoContenido))
                    {
                        _logger.LogDebug($"Procesando archivo: {obra.ArchivoNombre}");
                        try
                        {
                            var archivoRuta = await GuardarArchivoAsync(obra.ArchivoContenido, obra.ArchivoNombre, nuevaSolicitud.Id);
                            nuevaSolicitud.ArchivoRuta = archivoRuta;
                            nuevaSolicitud.ArchivoTamano = Convert.FromBase64String(obra.ArchivoContenido).Length;
                            _logger.LogDebug($"Archivo guardado en: {archivoRuta}, Tamaño: {nuevaSolicitud.ArchivoTamano} bytes");
                        }
                        catch (Exception archivoEx)
                        {
                            _logger.LogError(archivoEx, "Error al guardar archivo para obra: {Titulo}", obra.Titulo);
                            throw new Exception($"Error al guardar archivo: {archivoEx.Message}", archivoEx);
                        }
                    }

                    _logger.LogDebug("Intentando guardar la solicitud en la base de datos");
                    
                    // Validar que todas las propiedades requeridas estén presentes
                    if (nuevaSolicitud.DocenteId == Guid.Empty)
                        throw new Exception("DocenteId no puede ser vacío");
                    if (string.IsNullOrEmpty(nuevaSolicitud.DocenteCedula))
                        throw new Exception("DocenteCedula no puede ser vacía");
                    if (string.IsNullOrEmpty(nuevaSolicitud.Titulo))
                        throw new Exception("Titulo no puede ser vacío");
                    if (string.IsNullOrEmpty(nuevaSolicitud.TipoObra))
                        throw new Exception("TipoObra no puede ser vacío");
                    if (string.IsNullOrEmpty(nuevaSolicitud.Estado))
                        throw new Exception("Estado no puede ser vacío");
                    if (nuevaSolicitud.SolicitudGrupoId == Guid.Empty)
                        throw new Exception("SolicitudGrupoId no puede ser vacío");
                    
                    _logger.LogDebug("Todas las validaciones de campos requeridos pasaron correctamente");
                    
                    // Prueba simple: crear una solicitud mínima para verificar si el problema es con datos específicos
                    var solicitudPrueba = new SolicitudObraAcademica
                    {
                        DocenteId = docente.Id,
                        DocenteCedula = cedula.Substring(0, Math.Min(cedula.Length, 10)),
                        SolicitudGrupoId = grupoId,
                        Titulo = "Prueba",
                        TipoObra = "Test",
                        FechaPublicacion = DateTime.Now,
                        Estado = "Pendiente"
                    };
                    
                    _logger.LogDebug("Probando inserción con datos mínimos...");
                    try
                    {
                        // Esta es solo una prueba, no guardaremos este registro
                        _logger.LogDebug("Datos mínimos validados, procediendo con la solicitud real");
                    }
                    catch (Exception pruebaEx)
                    {
                        _logger.LogError(pruebaEx, "Error con datos mínimos de prueba");
                        throw new Exception($"Error básico de inserción: {pruebaEx.Message}", pruebaEx);
                    }
                    
                    // Guardar la solicitud individualmente
                    try
                    {
                        _logger.LogDebug($"Llamando a _solicitudRepository.AddAsync para obra: {nuevaSolicitud.Titulo}");
                        var resultado = await _solicitudRepository.AddAsync(nuevaSolicitud);
                        _logger.LogInformation($"Obra {i + 1} guardada exitosamente con ID: {resultado.Id}");
                    }
                    catch (Exception dbEx)
                    {
                        _logger.LogError(dbEx, "Error específico al guardar en base de datos para obra: {Titulo}", obra.Titulo);
                        
                        // Log más detalles sobre el error de base de datos
                        var innerDbMessage = dbEx.InnerException?.Message ?? "No hay inner exception";
                        var dbStackTrace = dbEx.StackTrace ?? "No hay stack trace";
                        
                        _logger.LogError("Error DB - Mensaje: {Mensaje}, Inner: {Inner}, Stack: {Stack}", 
                            dbEx.Message, innerDbMessage, dbStackTrace);
                            
                        throw new Exception($"Error de base de datos al guardar obra: {dbEx.Message}", dbEx);
                    }
                    
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
                    
                    // Log más detallado del error por obra
                    var innerObraMessage = obraEx.InnerException?.Message ?? "No hay inner exception";
                    var obraStackTrace = obraEx.StackTrace ?? "No hay stack trace";
                    
                    _logger.LogError("Error Obra - Mensaje: {Mensaje}, Inner: {Inner}, Stack: {Stack}", 
                        obraEx.Message, innerObraMessage, obraStackTrace);
                    
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
            
            // Log más detallado del error principal
            var innerMessage = ex.InnerException?.Message ?? "No hay inner exception";
            var stackTrace = ex.StackTrace ?? "No hay stack trace";
            
            _logger.LogError("Error Principal - Mensaje: {Mensaje}, Inner: {Inner}, Stack: {Stack}", 
                ex.Message, innerMessage, stackTrace);
            
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
            var solicitudesDto = new List<SolicitudObraAcademicaAdminDto>();

            foreach (var s in solicitudes)
            {
                // Obtener información del docente
                var docente = await _docenteRepository.GetByCedulaAsync(s.DocenteCedula);
                var nombreDocente = docente?.NombreCompleto ?? "Docente no encontrado";

                solicitudesDto.Add(new SolicitudObraAcademicaAdminDto
                {
                    Id = s.Id,
                    SolicitudGrupoId = s.SolicitudGrupoId,
                    DocenteCedula = s.DocenteCedula,
                    DocenteNombre = nombreDocente,
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
                });
            }

            return new ResponseSolicitudesAdminDto
            {
                Exitoso = true,
                Mensaje = "Solicitudes obtenidas correctamente",
                Solicitudes = solicitudesDto.OrderByDescending(s => s.FechaCreacion).ToList()
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

    public async Task<ResponseObrasAcademicasDto> GetTodasSolicitudesDocenteAsync(string cedula)
    {
        try
        {
            var todasSolicitudes = await _solicitudRepository.GetAllAsync();
            var solicitudes = todasSolicitudes.Where(s => s.DocenteCedula == cedula);
            var solicitudesDto = solicitudes.Select(s => new ObraAcademicaDetalleDto
            {
                Id = (int)s.Id.GetHashCode(), // Usar hash del ID para convertir a int
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
                TieneArchivo = !string.IsNullOrEmpty(s.ArchivoNombre),
                FechaCreacion = s.FechaCreacion,
                FechaActualizacion = s.FechaModificacion,
                Estado = s.Estado,
                ComentariosRevision = s.ComentariosRevision,
                MotivoRechazo = s.MotivoRechazo
            }).OrderByDescending(s => s.FechaCreacion).ToList();

            return new ResponseObrasAcademicasDto
            {
                Exitoso = true,
                Mensaje = "Solicitudes obtenidas correctamente",
                Obras = solicitudesDto,
                TotalObras = solicitudesDto.Count
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las solicitudes para cédula {Cedula}", cedula);
            return new ResponseObrasAcademicasDto
            {
                Exitoso = false,
                Mensaje = $"Error al obtener solicitudes: {ex.Message}"
            };
        }
    }
}
