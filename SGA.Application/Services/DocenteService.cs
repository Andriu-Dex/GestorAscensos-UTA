using SGA.Application.DTOs.Docentes;
using SGA.Application.DTOs.Responses;
using SGA.Application.DTOs.Admin;
using SGA.Application.Constants;
using SGA.Application.Interfaces;
using SGA.Application.Interfaces.Repositories;
using SGA.Domain.Enums;
using SGA.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace SGA.Application.Services;

public class DocenteService : IDocenteService
{
    private readonly IDocenteRepository _docenteRepository;
    private readonly IObraAcademicaRepository _obraAcademicaRepository;
    private readonly IExternalDataService _externalDataService;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IImageService _imageService;
    private readonly IConfiguracionRequisitoService _configuracionRequisitoService;
    private readonly ILogger<DocenteService> _logger;

    public DocenteService(
        IDocenteRepository docenteRepository,
        IObraAcademicaRepository obraAcademicaRepository,
        IExternalDataService externalDataService,
        IAuditoriaService auditoriaService,
        IImageService imageService,
        IConfiguracionRequisitoService configuracionRequisitoService,
        ILogger<DocenteService> logger)
    {
        _docenteRepository = docenteRepository;
        _obraAcademicaRepository = obraAcademicaRepository;
        _externalDataService = externalDataService;
        _auditoriaService = auditoriaService;
        _imageService = imageService;
        _configuracionRequisitoService = configuracionRequisitoService;
        _logger = logger;
    }

    public async Task<DocenteDto?> GetDocenteByIdAsync(Guid id)
    {
        var docente = await _docenteRepository.GetByIdAsync(id);
        if (docente == null) return null;

        return new DocenteDto
        {
            Id = docente.Id,
            Cedula = docente.Cedula,
            Nombres = docente.Nombres,
            Apellidos = docente.Apellidos,
            Email = docente.Email,
            Celular = docente.Celular,
            NivelActual = docente.NivelActual.ToString(),
            FechaInicioNivelActual = docente.FechaInicioNivelActual,
            FechaUltimoAscenso = docente.FechaUltimoAscenso,
            NombreCompleto = docente.NombreCompleto,
            FechaNombramiento = docente.FechaNombramiento,
            PromedioEvaluaciones = docente.PromedioEvaluaciones,
            HorasCapacitacion = docente.HorasCapacitacion,
            NumeroObrasAcademicas = docente.NumeroObrasAcademicas,
            MesesInvestigacion = docente.MesesInvestigacion,
            FechaUltimaImportacion = docente.FechaUltimaImportacion,
            FotoPerfilBase64 = docente.FotoPerfilBase64
        };
    }

    public async Task<DocenteDto?> GetDocenteByCedulaAsync(string cedula)
    {
        var docente = await _docenteRepository.GetByCedulaAsync(cedula);
        if (docente == null) return null;

        return await GetDocenteByIdAsync(docente.Id);
    }

    public async Task<DocenteDto?> GetDocenteByEmailAsync(string email)
    {
        var docente = await _docenteRepository.GetByEmailAsync(email);
        if (docente == null) return null;

        return await GetDocenteByIdAsync(docente.Id);
    }

    public async Task<ImportarDatosResponse> ImportarDatosTTHHAsync(string cedula)
    {
        try
        {
            var datosTTHH = await _externalDataService.ImportarDatosTTHHAsync(cedula);
            
            if (datosTTHH != null)
            {
                var docente = await _docenteRepository.GetByCedulaAsync(cedula);
                if (docente != null)
                {
                    docente.FechaNombramiento = datosTTHH.FechaNombramiento;
                    // Guardar datos organizacionales importados de TTHH
                    docente.FacultadTTHH = datosTTHH.Facultad;
                    docente.DepartamentoTTHH = datosTTHH.Departamento;
                    docente.FechaUltimaImportacion = DateTime.UtcNow;
                    await _docenteRepository.UpdateAsync(docente);
                    
                    await _auditoriaService.RegistrarAccionAsync("IMPORTAR_DATOS_TTHH", 
                        docente.Id.ToString(), docente.Email, "Docente", null, 
                        $"Fecha nombramiento: {datosTTHH.FechaNombramiento}, Facultad: {datosTTHH.Facultad}, Departamento: {datosTTHH.Departamento}", null);
                }
                
                return new ImportarDatosResponse
                {
                    Exitoso = true,
                    Mensaje = "Datos importados exitosamente desde TTHH",
                    DatosImportados = new Dictionary<string, object?>
                    {
                        ["FechaNombramiento"] = datosTTHH.FechaNombramiento,
                        ["CargoActual"] = datosTTHH.CargoActual,
                        ["Facultad"] = datosTTHH.Facultad,
                        ["Departamento"] = datosTTHH.Departamento
                    }
                };
            }
            
            return new ImportarDatosResponse
            {
                Exitoso = false,
                Mensaje = "No se encontraron datos en TTHH para la cédula proporcionada"
            };
        }
        catch (Exception ex)
        {
            return new ImportarDatosResponse
            {
                Exitoso = false,
                Mensaje = $"Error al importar datos de TTHH: {ex.Message}"
            };
        }
    }

    public async Task<ImportarDatosResponse> ImportarDatosDACAsync(string cedula)
    {
        try
        {
            var datosDAC = await _externalDataService.ImportarDatosDACAsync(cedula);
            var docente = await _docenteRepository.GetByCedulaAsync(cedula);
            
            if (datosDAC != null)
            {
                if (docente != null)
                {
                    docente.PromedioEvaluaciones = Math.Round(datosDAC.PromedioEvaluaciones, 2);
                    docente.FechaUltimaImportacion = DateTime.UtcNow;
                    await _docenteRepository.UpdateAsync(docente);
                    
                    await _auditoriaService.RegistrarAccionAsync("IMPORTAR_DATOS_DAC", 
                        docente.Id.ToString(), docente.Email, "Docente", null, 
                        $"Promedio evaluaciones: {datosDAC.PromedioEvaluaciones}% - {datosDAC.PeriodosEvaluados} períodos", null);
                }
                
                return new ImportarDatosResponse
                {
                    Exitoso = true,
                    Mensaje = datosDAC.Mensaje,
                    DatosImportados = new Dictionary<string, object?>
                    {
                        ["exitoso"] = true,
                        ["evaluacionesEncontradas"] = datosDAC.PeriodosEvaluados,
                        ["promedioEvaluacion"] = datosDAC.PromedioEvaluaciones,
                        ["cumpleRequisito"] = datosDAC.CumpleRequisito,
                        ["requisitoMinimo"] = datosDAC.RequisitoMinimo,
                        ["periodoEvaluado"] = datosDAC.PeriodoEvaluado,
                        ["mensaje"] = datosDAC.Mensaje,
                        ["detalleEvaluaciones"] = datosDAC.Evaluaciones.Select(e => new {
                            periodo = e.Periodo,
                            porcentaje = e.Porcentaje,
                            fecha = e.Fecha.ToString("yyyy-MM-dd"),
                            estudiantesEvaluaron = e.EstudiantesEvaluaron
                        }).ToList()
                    }
                };
            }
            else
            {
                // Cuando no hay evaluaciones válidas, actualizar el campo a 0
                if (docente != null)
                {
                    docente.PromedioEvaluaciones = 0;
                    docente.FechaUltimaImportacion = DateTime.UtcNow;
                    await _docenteRepository.UpdateAsync(docente);
                    
                    await _auditoriaService.RegistrarAccionAsync("IMPORTAR_DATOS_DAC", 
                        docente.Id.ToString(), docente.Email, "Docente", null, 
                        "Sin evaluaciones posteriores al inicio del cargo actual", null);
                }
            }
            
            return new ImportarDatosResponse
            {
                Exitoso = false,
                Mensaje = "No se encontraron evaluaciones docentes posteriores a la fecha de inicio del cargo actual para la cédula proporcionada en DAC. Solo se consideran evaluaciones realizadas después de asumir el cargo o rol actual."
            };
        }
        catch (Exception ex)
        {
            return new ImportarDatosResponse
            {
                Exitoso = false,
                Mensaje = $"Error al importar datos de DAC: {ex.Message}"
            };
        }
    }

    public async Task<ImportarDatosResponse> ImportarDatosDITICAsync(string cedula)
    {
        try
        {
            var datosDITIC = await _externalDataService.ImportarDatosDITICAsync(cedula);
            
            if (datosDITIC != null)
            {
                var docente = await _docenteRepository.GetByCedulaAsync(cedula);
                if (docente != null)
                {
                    docente.HorasCapacitacion = datosDITIC.HorasCapacitacion;
                    docente.FechaUltimaImportacion = DateTime.UtcNow;
                    await _docenteRepository.UpdateAsync(docente);
                    
                    await _auditoriaService.RegistrarAccionAsync("IMPORTAR_DATOS_DITIC", 
                        docente.Id.ToString(), docente.Email, "Docente", null, 
                        $"Horas capacitación: {datosDITIC.HorasCapacitacion}", null);
                }
                
                return new ImportarDatosResponse
                {
                    Exitoso = true,
                    Mensaje = "Datos importados exitosamente desde DITIC",
                    DatosImportados = new Dictionary<string, object?>
                    {
                        ["HorasCapacitacion"] = datosDITIC.HorasCapacitacion,
                        ["CursosCompletados"] = datosDITIC.CursosCompletados,
                        ["FechaUltimoCurso"] = datosDITIC.FechaUltimoCurso
                    }
                };
            }
            
            return new ImportarDatosResponse
            {
                Exitoso = false,
                Mensaje = "No se encontraron datos en DITIC para la cédula proporcionada"
            };
        }
        catch (Exception ex)
        {
            return new ImportarDatosResponse
            {
                Exitoso = false,
                Mensaje = $"Error al importar datos de DITIC: {ex.Message}"
            };
        }
    }

    public async Task<ImportarDatosResponse> ImportarDatosDIRINVAsync(string cedula)
    {
        try
        {
            var datosDirInv = await _externalDataService.ImportarDatosDirInvAsync(cedula);
            
            if (datosDirInv != null)
            {
                var docente = await _docenteRepository.GetByCedulaAsync(cedula);
                if (docente != null)
                {
                    docente.NumeroObrasAcademicas = datosDirInv.NumeroObrasAcademicas;
                    docente.MesesInvestigacion = datosDirInv.MesesInvestigacion;
                    docente.FechaUltimaImportacion = DateTime.UtcNow;
                    await _docenteRepository.UpdateAsync(docente);
                    
                    await _auditoriaService.RegistrarAccionAsync("IMPORTAR_DATOS_DIRINV", 
                        docente.Id.ToString(), docente.Email, "Docente", null, 
                        $"Obras académicas: {datosDirInv.NumeroObrasAcademicas}, Meses investigación: {datosDirInv.MesesInvestigacion}", null);
                }
                
                return new ImportarDatosResponse
                {
                    Exitoso = true,
                    Mensaje = "Datos importados exitosamente desde DIRINV",
                    DatosImportados = new Dictionary<string, object?>
                    {
                        ["NumeroObrasAcademicas"] = datosDirInv.NumeroObrasAcademicas,
                        ["MesesInvestigacion"] = datosDirInv.MesesInvestigacion,
                        ["ProyectosActivos"] = datosDirInv.ProyectosActivos,
                        ["FechaUltimaPublicacion"] = datosDirInv.FechaUltimaPublicacion
                    }
                };
            }
            
            return new ImportarDatosResponse
            {
                Exitoso = false,
                Mensaje = "No se encontraron datos en DIRINV para la cédula proporcionada"
            };
        }
        catch (Exception ex)
        {
            return new ImportarDatosResponse
            {
                Exitoso = false,
                Mensaje = $"Error al importar datos de DIRINV: {ex.Message}"
            };
        }
    }

    public async Task<ValidacionRequisitosDto> ValidarRequisitosAscensoAsync(string cedula, string nivelObjetivo)
    {
        var docente = await _docenteRepository.GetByCedulaAsync(cedula);
        if (docente == null)
        {
            throw new ArgumentException("Docente no encontrado");
        }

        if (!Enum.TryParse<NivelTitular>(nivelObjetivo, out var nivel))
        {
            throw new ArgumentException("Nivel objetivo inválido");
        }

        var validacion = new ValidacionRequisitosDto
        {
            NivelActual = docente.NivelActual.ToString(),
            NivelSiguiente = nivel.ToString(),
            Requisitos = new List<RequisitoDto>()
        };

        var tiempoEnNivel = DateTime.UtcNow - docente.FechaInicioNivelActual;
        var diasEnNivel = (int)tiempoEnNivel.TotalDays;

        // Requisito de tiempo
        validacion.Requisitos.Add(new RequisitoDto
        {
            Nombre = "Tiempo en nivel actual",
            ValorRequerido = "4 años (1460 días)",
            ValorActual = $"{diasEnNivel} días",
            Cumple = diasEnNivel >= 1460,
            Descripcion = "Tiempo mínimo requerido en el nivel actual antes de ascender"
        });

        // Requisitos específicos por nivel
        switch (nivel)
        {
            case NivelTitular.Titular2:
                validacion.Requisitos.AddRange(new[]
                {
                    new RequisitoDto
                    {
                        Nombre = "Obras académicas",
                        ValorRequerido = "1",
                        ValorActual = (docente.NumeroObrasAcademicas ?? 0).ToString(),
                        Cumple = (docente.NumeroObrasAcademicas ?? 0) >= 1,
                        Descripcion = "Número mínimo de obras académicas publicadas"
                    },
                    new RequisitoDto
                    {
                        Nombre = "Promedio evaluaciones",
                        ValorRequerido = "75%",
                        ValorActual = $"{docente.PromedioEvaluaciones ?? 0}%",
                        Cumple = (docente.PromedioEvaluaciones ?? 0) >= 75,
                        Descripcion = "Promedio mínimo en evaluaciones docentes"
                    },
                    new RequisitoDto
                    {
                        Nombre = "Horas de capacitación",
                        ValorRequerido = "96 horas",
                        ValorActual = $"{docente.HorasCapacitacion ?? 0} horas",
                        Cumple = (docente.HorasCapacitacion ?? 0) >= 96,
                        Descripcion = "Horas mínimas de capacitación completadas"
                    }
                });
                break;

            case NivelTitular.Titular3:
                validacion.Requisitos.AddRange(new[]
                {
                    new RequisitoDto
                    {
                        Nombre = "Obras académicas",
                        ValorRequerido = "2",
                        ValorActual = (docente.NumeroObrasAcademicas ?? 0).ToString(),
                        Cumple = (docente.NumeroObrasAcademicas ?? 0) >= 2,
                        Descripcion = "Número mínimo de obras académicas publicadas"
                    },
                    new RequisitoDto
                    {
                        Nombre = "Promedio evaluaciones",
                        ValorRequerido = "75%",
                        ValorActual = $"{docente.PromedioEvaluaciones ?? 0}%",
                        Cumple = (docente.PromedioEvaluaciones ?? 0) >= 75,
                        Descripcion = "Promedio mínimo en evaluaciones docentes"
                    },
                    new RequisitoDto
                    {
                        Nombre = "Horas de capacitación",
                        ValorRequerido = "96 horas",
                        ValorActual = $"{docente.HorasCapacitacion ?? 0} horas",
                        Cumple = (docente.HorasCapacitacion ?? 0) >= 96,
                        Descripcion = "Horas mínimas de capacitación completadas"
                    },
                    new RequisitoDto
                    {
                        Nombre = "Meses en investigación",
                        ValorRequerido = "12 meses",
                        ValorActual = $"{docente.MesesInvestigacion ?? 0} meses",
                        Cumple = (docente.MesesInvestigacion ?? 0) >= 12,
                        Descripcion = "Tiempo mínimo dedicado a actividades de investigación"
                    }
                });
                break;

            case NivelTitular.Titular4:
                validacion.Requisitos.AddRange(new[]
                {
                    new RequisitoDto
                    {
                        Nombre = "Obras académicas",
                        ValorRequerido = "3",
                        ValorActual = (docente.NumeroObrasAcademicas ?? 0).ToString(),
                        Cumple = (docente.NumeroObrasAcademicas ?? 0) >= 3,
                        Descripcion = "Número mínimo de obras académicas publicadas"
                    },
                    new RequisitoDto
                    {
                        Nombre = "Promedio evaluaciones",
                        ValorRequerido = "75%",
                        ValorActual = $"{docente.PromedioEvaluaciones ?? 0}%",
                        Cumple = (docente.PromedioEvaluaciones ?? 0) >= 75,
                        Descripcion = "Promedio mínimo en evaluaciones docentes"
                    },
                    new RequisitoDto
                    {
                        Nombre = "Horas de capacitación",
                        ValorRequerido = "128 horas",
                        ValorActual = $"{docente.HorasCapacitacion ?? 0} horas",
                        Cumple = (docente.HorasCapacitacion ?? 0) >= 128,
                        Descripcion = "Horas mínimas de capacitación completadas"
                    },
                    new RequisitoDto
                    {
                        Nombre = "Meses en investigación",
                        ValorRequerido = "24 meses",
                        ValorActual = $"{docente.MesesInvestigacion ?? 0} meses",
                        Cumple = (docente.MesesInvestigacion ?? 0) >= 24,
                        Descripcion = "Tiempo mínimo dedicado a actividades de investigación"
                    }
                });
                break;

            case NivelTitular.Titular5:
                validacion.Requisitos.AddRange(new[]
                {
                    new RequisitoDto
                    {
                        Nombre = "Obras académicas",
                        ValorRequerido = "5",
                        ValorActual = (docente.NumeroObrasAcademicas ?? 0).ToString(),
                        Cumple = (docente.NumeroObrasAcademicas ?? 0) >= 5,
                        Descripcion = "Número mínimo de obras académicas publicadas"
                    },
                    new RequisitoDto
                    {
                        Nombre = "Promedio evaluaciones",
                        ValorRequerido = "75%",
                        ValorActual = $"{docente.PromedioEvaluaciones ?? 0}%",
                        Cumple = (docente.PromedioEvaluaciones ?? 0) >= 75,
                        Descripcion = "Promedio mínimo en evaluaciones docentes"
                    },
                    new RequisitoDto
                    {
                        Nombre = "Horas de capacitación",
                        ValorRequerido = "160 horas",
                        ValorActual = $"{docente.HorasCapacitacion ?? 0} horas",
                        Cumple = (docente.HorasCapacitacion ?? 0) >= 160,
                        Descripcion = "Horas mínimas de capacitación completadas"
                    },
                    new RequisitoDto
                    {
                        Nombre = "Meses en investigación",
                        ValorRequerido = "24 meses",
                        ValorActual = $"{docente.MesesInvestigacion ?? 0} meses",
                        Cumple = (docente.MesesInvestigacion ?? 0) >= 24,
                        Descripcion = "Tiempo mínimo dedicado a actividades de investigación"
                    }
                });
                break;
        }

        validacion.CumpleTodos = validacion.Requisitos.All(r => r.Cumple);

        return validacion;
    }

    public async Task<bool> ActualizarNivelDocenteAsync(Guid docenteId, string nuevoNivel)
    {
        var docente = await _docenteRepository.GetByIdAsync(docenteId);
        if (docente == null) return false;

        if (!Enum.TryParse<NivelTitular>(nuevoNivel, out var nivel))
            return false;

        var nivelAnterior = docente.NivelActual.ToString();
        docente.NivelActual = nivel;
        docente.FechaUltimoAscenso = DateTime.UtcNow;
        docente.FechaInicioNivelActual = DateTime.UtcNow;
        
        // Reiniciar contadores para el siguiente ascenso
        docente.PromedioEvaluaciones = null;
        docente.HorasCapacitacion = null;
        docente.NumeroObrasAcademicas = null;
        docente.MesesInvestigacion = null;

        await _docenteRepository.UpdateAsync(docente);

        await _auditoriaService.RegistrarAccionAsync("ASCENSO_DOCENTE", 
            docente.Id.ToString(), docente.Email, "Docente", 
            $"Nivel: {nivelAnterior}", $"Nivel: {nuevoNivel}", null);

        return true;
    }

    public async Task<IndicadoresDocenteDto> GetIndicadoresAsync(string cedula)
    {
        // Usar consulta optimizada sin includes innecesarios
        var docente = await _docenteRepository.GetByCedulaSimpleAsync(cedula);
        if (docente == null)
            throw new ArgumentException("Docente no encontrado");

        // Calcular tiempo en rol actual
        var fechaInicioRol = docente.FechaInicioNivelActual;
        var tiempoTranscurrido = DateTime.UtcNow - fechaInicioRol;
        var tiempoRolMeses = (int)Math.Floor(tiempoTranscurrido.TotalDays / 30.44);

        // Calcular requisitos para el siguiente nivel (si existe)
        var nivelActualNum = int.Parse(docente.NivelActual.ToString().Replace("Titular", ""));
        var tiempoRequiridoMeses = 48; // Valor por defecto
        var cumpleTiempoMinimo = true;
        var tiempoRestante = TimeSpan.Zero;
        
        if (nivelActualNum < 5) // Si no está en el nivel máximo
        {
            var siguienteNivel = $"Titular{nivelActualNum + 1}";
            try
            {
                var nivelActualEnum = (NivelTitular)nivelActualNum;
                var requisitos = await GetRequisitosDinamicosAsync(nivelActualEnum, siguienteNivel);
                tiempoRequiridoMeses = requisitos.TiempoRol;
                cumpleTiempoMinimo = tiempoRolMeses >= tiempoRequiridoMeses;
                var mesesRestantes = Math.Max(0, tiempoRequiridoMeses - tiempoRolMeses);
                tiempoRestante = TimeSpan.FromDays(mesesRestantes * 30.44);
            }
            catch (Exception)
            {
                // Si hay error al obtener configuración, usar valores por defecto
                cumpleTiempoMinimo = true;
            }
        }

        // Formatear textos de tiempo
        var tiempoTranscurridoTexto = FormatearTiempo(tiempoTranscurrido);
        var tiempoRestanteTexto = cumpleTiempoMinimo ? "Cumplido" : FormatearTiempo(tiempoRestante);

        // Obtener información adicional de evaluaciones desde DAC de forma segura
        DTOs.ExternalData.DatosDACDto? datosDAC = null;
        try
        {
            datosDAC = await _externalDataService.ImportarDatosDACAsync(cedula);
        }
        catch (Exception)
        {
            // Si falla la conexión a DAC, usar datos existentes del docente
            datosDAC = null;
        }
        
        return new IndicadoresDocenteDto
        {
            TiempoRol = tiempoRolMeses,
            NumeroObras = docente.NumeroObrasAcademicas ?? 0,
            PuntajeEvaluacion = datosDAC?.PromedioEvaluaciones != null ? Math.Round(datosDAC.PromedioEvaluaciones, 2) : 0,
            HorasCapacitacion = docente.HorasCapacitacion ?? 0,
            TiempoInvestigacion = docente.MesesInvestigacion ?? 0,
            
            // Información adicional de evaluaciones
            PeriodosEvaluados = datosDAC?.PeriodosEvaluados ?? 0,
            FechaUltimaEvaluacion = datosDAC?.FechaUltimaEvaluacion,
            PeriodoEvaluado = datosDAC?.PeriodoEvaluado ?? string.Empty,
            CumpleRequisitoEvaluacion = datosDAC?.CumpleRequisito ?? false,
            
            // Propiedades de tiempo detallado para el frontend
            FechaInicioRol = fechaInicioRol,
            TiempoTranscurrido = tiempoTranscurrido,
            TiempoRestante = tiempoRestante,
            TiempoTranscurridoTexto = tiempoTranscurridoTexto,
            TiempoRestanteTexto = tiempoRestanteTexto,
            CumpleTiempoMinimo = cumpleTiempoMinimo
        };
    }

    public async Task<RequisitosAscensoDto> GetRequisitosAscensoAsync(string cedula, string nivelObjetivo)
    {
        var docente = await _docenteRepository.GetByCedulaAsync(cedula);
        if (docente == null)
            throw new ArgumentException("Docente no encontrado");

        var indicadores = await GetIndicadoresAsync(cedula);
        
        // Obtener requisitos dinámicos desde la configuración en base de datos
        var requisitos = await GetRequisitosDinamicosAsync(docente.NivelActual, nivelObjetivo);

        return new RequisitosAscensoDto
        {
            CumpleTiempoRol = indicadores.TiempoRol >= requisitos.TiempoRol,
            CumpleObras = indicadores.NumeroObras >= requisitos.NumeroObras,
            CumpleEvaluacion = indicadores.PuntajeEvaluacion >= requisitos.PuntajeEvaluacion,
            CumpleCapacitacion = indicadores.HorasCapacitacion >= requisitos.HorasCapacitacion,
            CumpleInvestigacion = indicadores.TiempoInvestigacion >= requisitos.TiempoInvestigacion,
            
            TiempoRolRequerido = requisitos.TiempoRol,
            TiempoRolActual = indicadores.TiempoRol,
            
            ObrasRequeridas = requisitos.NumeroObras,
            ObrasActuales = indicadores.NumeroObras,
            
            EvaluacionRequerida = requisitos.PuntajeEvaluacion,
            EvaluacionActual = indicadores.PuntajeEvaluacion,
            
            CapacitacionRequerida = requisitos.HorasCapacitacion,
            CapacitacionActual = indicadores.HorasCapacitacion,
            
            InvestigacionRequerida = requisitos.TiempoInvestigacion,
            InvestigacionActual = indicadores.TiempoInvestigacion
        };
    }

    /// <summary>
    /// Obtiene los requisitos dinámicos desde la configuración en base de datos
    /// con fallback a valores por defecto si no existe configuración
    /// </summary>
    private async Task<(int TiempoRol, int NumeroObras, decimal PuntajeEvaluacion, int HorasCapacitacion, int TiempoInvestigacion)> GetRequisitosDinamicosAsync(NivelTitular nivelActual, string nivelObjetivoString)
    {
        try
        {
            // Convertir el string del nivel objetivo a enum
            if (!Enum.TryParse<NivelTitular>(nivelObjetivoString, out var nivelObjetivo))
            {
                // Si no se puede parsear, extraer número del string
                var numeroNivel = int.Parse(nivelObjetivoString.Replace("Titular", ""));
                nivelObjetivo = (NivelTitular)numeroNivel;
            }

            // Buscar configuración específica en la base de datos
            var configuracion = await _configuracionRequisitoService.GetByNivelesAsync(nivelActual, nivelObjetivo);

            if (configuracion != null)
            {
                // Usar configuración dinámica de la BD
                return (
                    TiempoRol: configuracion.TiempoMinimoMeses,
                    NumeroObras: configuracion.ObrasMinimas,
                    PuntajeEvaluacion: configuracion.PuntajeEvaluacionMinimo,
                    HorasCapacitacion: configuracion.HorasCapacitacionMinimas,
                    TiempoInvestigacion: configuracion.TiempoInvestigacionMinimo
                );
            }

            // Fallback: Si no existe configuración, usar valores por defecto basados en el nivel
            return GetRequisitosPorDefecto(nivelObjetivoString);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error al obtener requisitos dinámicos para {NivelActual} -> {NivelObjetivo}. Usando valores por defecto.", 
                nivelActual, nivelObjetivoString);
            
            // En caso de error, usar valores por defecto
            return GetRequisitosPorDefecto(nivelObjetivoString);
        }
    }

    /// <summary>
    /// Valores por defecto para compatibilidad cuando no existe configuración en BD
    /// </summary>
    private (int TiempoRol, int NumeroObras, decimal PuntajeEvaluacion, int HorasCapacitacion, int TiempoInvestigacion) GetRequisitosPorDefecto(string nivel)
    {
        return nivel switch
        {
            "Titular2" => (48, 1, 75, 96, 0),    // Titular 1 → 2: 4 años, 1 obra, 75%, 96h, 0 meses investigación
            "Titular3" => (48, 2, 75, 96, 12),   // Titular 2 → 3: 4 años, 2 obras, 75%, 96h, 12 meses investigación
            "Titular4" => (48, 3, 75, 128, 24),  // Titular 3 → 4: 4 años, 3 obras, 75%, 128h, 24 meses investigación
            "Titular5" => (48, 5, 75, 160, 24),  // Titular 4 → 5: 4 años, 5 obras, 75%, 160h, 24 meses investigación
            _ => (48, 3, 75, 120, 24) // Valores por defecto genéricos
        };
    }

    public async Task<ImportarDatosResponse> ImportarTiempoRolTTHHAsync(string cedula)
    {
        try
        {
            var datosTTHH = await _externalDataService.ImportarDatosTTHHAsync(cedula);
            
            if (datosTTHH != null)
            {
                var docente = await _docenteRepository.GetByCedulaAsync(cedula);
                if (docente != null)
                {
                    // Actualizar la fecha de nombramiento y la fecha de inicio del nivel actual
                    if (datosTTHH.FechaNombramiento.HasValue)
                    {
                        docente.FechaNombramiento = datosTTHH.FechaNombramiento.Value;
                    }
                    
                    // Usar la fecha de ingreso al nivel actual desde TTHH
                    if (datosTTHH.FechaIngresoNivelActual.HasValue)
                    {
                        docente.FechaInicioNivelActual = datosTTHH.FechaIngresoNivelActual.Value;
                    }
                    else if (datosTTHH.FechaInicioCargoActual.HasValue)
                    {
                        // Si no hay fecha específica del nivel, usar la fecha de inicio del cargo actual
                        docente.FechaInicioNivelActual = datosTTHH.FechaInicioCargoActual.Value;
                    }
                    else if (datosTTHH.FechaNombramiento.HasValue)
                    {
                        // Como último recurso, usar la fecha de nombramiento
                        docente.FechaInicioNivelActual = datosTTHH.FechaNombramiento.Value;
                    }
                    
                    // Guardar datos organizacionales importados de TTHH
                    docente.FacultadTTHH = datosTTHH.Facultad;
                    docente.DepartamentoTTHH = datosTTHH.Departamento;
                    docente.FechaUltimaImportacion = DateTime.UtcNow;
                    await _docenteRepository.UpdateAsync(docente);
                    
                    // Calcular el tiempo transcurrido en el rol actual
                    var tiempoTranscurrido = DateTime.Now - docente.FechaInicioNivelActual;
                    var mesesTranscurridos = (int)(tiempoTranscurrido.TotalDays / 30.44);
                    
                    await _auditoriaService.RegistrarAccionAsync("IMPORTAR_TIEMPO_ROL_TTHH", 
                        docente.Id.ToString(), docente.Email, "Docente", null, 
                        $"Fecha inicio rol actual: {docente.FechaInicioNivelActual} - Tiempo transcurrido: {mesesTranscurridos} meses", null);
                }
                
                return new ImportarDatosResponse
                {
                    Exitoso = true,
                    Mensaje = "Tiempo en rol actual importado exitosamente desde TTHH",
                    DatosImportados = new Dictionary<string, object?>
                    {
                        ["FechaInicioRolActual"] = docente?.FechaInicioNivelActual,
                        ["FechaNombramiento"] = datosTTHH.FechaNombramiento,
                        ["TiempoTranscurridoMeses"] = docente != null ? (int)((DateTime.Now - docente.FechaInicioNivelActual).TotalDays / 30.44) : 0,
                        ["CargoActual"] = datosTTHH.CargoActual,
                        ["Facultad"] = datosTTHH.Facultad,
                        ["Departamento"] = datosTTHH.Departamento
                    }
                };
            }
            
            return new ImportarDatosResponse
            {
                Exitoso = false,
                Mensaje = "No se encontraron datos de tiempo en rol en TTHH para la cédula proporcionada"
            };
        }
        catch (Exception ex)
        {
            return new ImportarDatosResponse
            {
                Exitoso = false,
                Mensaje = $"Error al importar tiempo en rol desde TTHH: {ex.Message}"
            };
        }
    }

    public async Task<ImportarDatosResponse> ImportarObrasAcademicasAsync(string cedula)
    {
        try
        {
            // Usar consulta optimizada sin includes innecesarios
            var docente = await _docenteRepository.GetByCedulaSimpleAsync(cedula);
            if (docente == null)
            {
                return new ImportarDatosResponse
                {
                    Exitoso = false,
                    Mensaje = "Docente no encontrado"
                };
            }

            // Obtener obras académicas con PDF desde DIRINV
            var obrasExterna = await _externalDataService.ObtenerObrasAcademicasConPdfAsync(cedula);
            
            if (!obrasExterna.Any())
            {
                return new ImportarDatosResponse
                {
                    Exitoso = false,
                    Mensaje = "No se encontraron obras académicas con PDF en DIRINV para la cédula proporcionada"
                };
            }

            var obrasImportadas = 0;
            var obrasOmitidas = 0;

            foreach (var obraExterna in obrasExterna)
            {
                // Verificar si ya existe una obra con el mismo título y fecha
                var obraExiste = await _obraAcademicaRepository.ExisteObraAsync(
                    obraExterna.Titulo, docente.Id, obraExterna.FechaPublicacion);

                if (obraExiste)
                {
                    obrasOmitidas++;
                    continue;
                }

                // Crear nueva obra académica
                var nuevaObra = new ObraAcademica
                {
                    DocenteId = docente.Id,
                    Titulo = obraExterna.Titulo,
                    TipoObra = obraExterna.Tipo,
                    FechaPublicacion = obraExterna.FechaPublicacion,
                    Revista = obraExterna.Revista,
                    Autores = obraExterna.Autores,
                    NombreArchivo = obraExterna.NombreArchivo ?? $"{obraExterna.Titulo}.pdf",
                    OrigenDatos = "DIRINV",
                    EsVerificada = true
                };

                // Comprimir y almacenar el PDF si existe
                if (obraExterna.PdfComprimido != null && obraExterna.PdfComprimido.Length > 0)
                {
                    await nuevaObra.AsignarArchivoPDFAsync(obraExterna.PdfComprimido, nuevaObra.NombreArchivo);
                }

                await _obraAcademicaRepository.CreateAsync(nuevaObra);
                obrasImportadas++;
            }

            // Actualizar SOLO el contador de obras en el docente, sin tocar otros campos
            var totalObras = await _obraAcademicaRepository.CountByDocenteIdAsync(docente.Id);
            docente.NumeroObrasAcademicas = totalObras;
            // NO actualizar FechaUltimaImportacion para no afectar otros indicadores
            
            await _docenteRepository.UpdateAsync(docente);

            // Registrar auditoría
            await _auditoriaService.RegistrarAccionAsync("IMPORTAR_OBRAS_ACADEMICAS", 
                docente.Id.ToString(), docente.Email, "Docente", null, 
                $"Obras importadas: {obrasImportadas}, Obras omitidas: {obrasOmitidas}, Total obras: {totalObras}", null);

            return new ImportarDatosResponse
            {
                Exitoso = true,
                Mensaje = $"Importación completada. {obrasImportadas} obras importadas" + 
                         (obrasOmitidas > 0 ? $", {obrasOmitidas} obras ya existían" : ""),
                DatosImportados = new Dictionary<string, object?>
                {
                    ["obrasImportadas"] = obrasImportadas,
                    ["obrasOmitidas"] = obrasOmitidas,
                    ["totalObras"] = totalObras,
                    ["fechaImportacion"] = DateTime.UtcNow
                }
            };
        }
        catch (Exception ex)
        {
            return new ImportarDatosResponse
            {
                Exitoso = false,
                Mensaje = $"Error al importar obras académicas: {ex.Message}"
            };
        }
    }
    
    public async Task<bool> ActualizarPerfilAsync(Guid docenteId, ActualizarPerfilDto dto)
    {
        try
        {
            var docente = await _docenteRepository.GetByIdAsync(docenteId);
            if (docente == null)
            {
                return false;
            }

            // Verificar si el email ya existe para otro docente
            if (docente.Email != dto.Email)
            {
                var existingDocente = await _docenteRepository.GetByEmailAsync(dto.Email);
                if (existingDocente != null && existingDocente.Id != docenteId)
                {
                    throw new InvalidOperationException("Ya existe un docente con ese correo electrónico");
                }
            }

            // Actualizar campos
            docente.Nombres = dto.Nombres;
            docente.Apellidos = dto.Apellidos;
            docente.Email = dto.Email;
            docente.Celular = dto.Celular;

            await _docenteRepository.UpdateAsync(docente);

            // Registrar auditoría
            await _auditoriaService.RegistrarAccionAsync(
                "Actualización de perfil",
                docenteId.ToString(),
                dto.Email,
                "Docente",
                null,
                $"Perfil actualizado: {docente.NombreCompleto}",
                null
            );

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar perfil del docente {DocenteId}", docenteId);
            throw;
        }
    }
    
    // Métodos para foto de perfil
    public async Task<FileUploadResponse> UploadProfilePhotoAsync(Guid docenteId, IFormFile file)
    {
        try
        {
            // Validar archivo
            var validation = _imageService.ValidateImage(file);
            if (!validation.IsValid)
            {
                return FileUploadResponse.ErrorResponse(validation.Message, FileUploadErrorType.InvalidFile);
            }

            // Obtener docente
            var docente = await _docenteRepository.GetByIdAsync(docenteId);
            if (docente == null)
            {
                return FileUploadResponse.ErrorResponse("Docente no encontrado", FileUploadErrorType.DatabaseError);
            }

            // Procesar imagen
            var processedImage = await _imageService.ProcessImageAsync(file);
            var optimizedMimeType = _imageService.GetOptimizedMimeType(file.ContentType);

            // Actualizar datos del docente
            docente.FotoPerfil = processedImage;

            // Guardar en base de datos
            await _docenteRepository.UpdateAsync(docente);

            // Registrar auditoría
            await _auditoriaService.RegistrarAccionAsync("SUBIR_FOTO_PERFIL", 
                docente.Id.ToString(), docente.Email, "Docente", null, 
                $"Foto de perfil actualizada - Tamaño: {processedImage.Length} bytes", null);

            // Generar base64 de la imagen
            var imageBase64 = Convert.ToBase64String(processedImage);

            return FileUploadResponse.SuccessResponse(
                "Foto de perfil actualizada correctamente",
                imageBase64);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al subir foto de perfil para docente {DocenteId}", docenteId);
            return FileUploadResponse.ErrorResponse(
                "Error interno del servidor al procesar la imagen",
                FileUploadErrorType.ProcessingError);
        }
    }

    public UploadConfigResponse GetUploadConfig()
    {
        return new UploadConfigResponse
        {
            MaxSizeBytes = FileLimits.ProfileImages.MaxSizeBytes,
            MaxSizeMB = FileLimits.ProfileImages.MaxSizeMB,
            AllowedExtensions = FileLimits.ProfileImages.AllowedExtensions,
            AllowedMimeTypes = FileLimits.ProfileImages.AllowedMimeTypes,
            AcceptAttribute = string.Join(",", FileLimits.ProfileImages.AllowedMimeTypes)
        };
    }

    private string FormatearTiempo(TimeSpan tiempo)
    {
        var totalDias = (int)tiempo.TotalDays;
        var años = totalDias / 365;
        var meses = (totalDias % 365) / 30;
        var dias = (totalDias % 365) % 30;

        var partes = new List<string>();

        if (años > 0)
            partes.Add($"{años} año{(años != 1 ? "s" : "")}");

        if (meses > 0)
            partes.Add($"{meses} mes{(meses != 1 ? "es" : "")}");

        if (dias > 0 && años == 0) // Solo mostrar días si no hay años
            partes.Add($"{dias} día{(dias != 1 ? "s" : "")}");

        if (partes.Count == 0)
            return "0 días";

        if (partes.Count == 1)
            return partes[0];

        if (partes.Count == 2)
            return $"{partes[0]} y {partes[1]}";

        return $"{string.Join(", ", partes.Take(partes.Count - 1))} y {partes.Last()}";
    }
}