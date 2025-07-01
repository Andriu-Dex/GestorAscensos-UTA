using SGA.Application.DTOs.Docentes;
using SGA.Application.Interfaces;
using SGA.Application.Interfaces.Repositories;
using SGA.Domain.Enums;
using SGA.Domain.Entities;
using System.Linq;

namespace SGA.Application.Services;

public class DocenteService : IDocenteService
{
    private readonly IDocenteRepository _docenteRepository;
    private readonly IObraAcademicaRepository _obraAcademicaRepository;
    private readonly IExternalDataService _externalDataService;
    private readonly IAuditoriaService _auditoriaService;

    public DocenteService(
        IDocenteRepository docenteRepository,
        IObraAcademicaRepository obraAcademicaRepository,
        IExternalDataService externalDataService,
        IAuditoriaService auditoriaService)
    {
        _docenteRepository = docenteRepository;
        _obraAcademicaRepository = obraAcademicaRepository;
        _externalDataService = externalDataService;
        _auditoriaService = auditoriaService;
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
            NivelActual = docente.NivelActual.ToString(),
            FechaInicioNivelActual = docente.FechaInicioNivelActual,
            FechaUltimoAscenso = docente.FechaUltimoAscenso,
            NombreCompleto = docente.NombreCompleto,
            FechaNombramiento = docente.FechaNombramiento,
            PromedioEvaluaciones = docente.PromedioEvaluaciones,
            HorasCapacitacion = docente.HorasCapacitacion,
            NumeroObrasAcademicas = docente.NumeroObrasAcademicas,
            MesesInvestigacion = docente.MesesInvestigacion,
            FechaUltimaImportacion = docente.FechaUltimaImportacion
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
        var docentes = await _docenteRepository.GetAllAsync();
        var docente = docentes.FirstOrDefault(d => d.Email == email);
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
                    docente.FechaUltimaImportacion = DateTime.UtcNow;
                    await _docenteRepository.UpdateAsync(docente);
                    
                    await _auditoriaService.RegistrarAccionAsync("IMPORTAR_DATOS_TTHH", 
                        docente.Id.ToString(), docente.Email, "Docente", null, 
                        $"Fecha nombramiento: {datosTTHH.FechaNombramiento}", null);
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
            
            if (datosDAC != null)
            {
                var docente = await _docenteRepository.GetByCedulaAsync(cedula);
                if (docente != null)
                {
                    docente.PromedioEvaluaciones = datosDAC.PromedioEvaluaciones;
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
            
            return new ImportarDatosResponse
            {
                Exitoso = false,
                Mensaje = "No se encontraron evaluaciones docentes para la cédula proporcionada en DAC"
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

        // Calcular tiempo en rol actual en meses
        var tiempoRol = (int)Math.Floor((DateTime.UtcNow - docente.FechaInicioNivelActual).TotalDays / 30.44);

        return new IndicadoresDocenteDto
        {
            TiempoRol = tiempoRol,
            NumeroObras = docente.NumeroObrasAcademicas ?? 0,
            PuntajeEvaluacion = docente.PromedioEvaluaciones ?? 0,
            HorasCapacitacion = docente.HorasCapacitacion ?? 0,
            TiempoInvestigacion = docente.MesesInvestigacion ?? 0
        };
    }

    public async Task<RequisitosAscensoDto> GetRequisitosAscensoAsync(string cedula, string nivelObjetivo)
    {
        var docente = await _docenteRepository.GetByCedulaAsync(cedula);
        if (docente == null)
            throw new ArgumentException("Docente no encontrado");

        var indicadores = await GetIndicadoresAsync(cedula);
        var requisitos = GetRequisitosPorNivel(nivelObjetivo);

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

    private (int TiempoRol, int NumeroObras, decimal PuntajeEvaluacion, int HorasCapacitacion, int TiempoInvestigacion) GetRequisitosPorNivel(string nivel)
    {
        return nivel switch
        {
            "Titular2" => (48, 1, 75, 96, 0),    // Titular 1 → 2: 4 años, 1 obra, 75%, 96h, 0 meses investigación
            "Titular3" => (48, 2, 75, 96, 12),   // Titular 2 → 3: 4 años, 2 obras, 75%, 96h, 12 meses investigación
            "Titular4" => (48, 3, 75, 128, 24),  // Titular 3 → 4: 4 años, 3 obras, 75%, 128h, 24 meses investigación
            "Titular5" => (48, 5, 75, 160, 24),  // Titular 4 → 5: 4 años, 5 obras, 75%, 160h, 24 meses investigación
            _ => throw new ArgumentException($"Nivel no válido: {nivel}")
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
}