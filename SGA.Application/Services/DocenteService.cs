using SGA.Application.DTOs.Docentes;
using SGA.Application.Interfaces;
using SGA.Application.Interfaces.Repositories;
using SGA.Domain.Enums;

namespace SGA.Application.Services;

public class DocenteService : IDocenteService
{
    private readonly IDocenteRepository _docenteRepository;
    private readonly IExternalDataService _externalDataService;
    private readonly IAuditoriaService _auditoriaService;

    public DocenteService(
        IDocenteRepository docenteRepository,
        IExternalDataService externalDataService,
        IAuditoriaService auditoriaService)
    {
        _docenteRepository = docenteRepository;
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
            var response = await _externalDataService.ImportarDesdeTTHHAsync(cedula);
            
            if (response.Exitoso)
            {
                var docente = await _docenteRepository.GetByCedulaAsync(cedula);
                if (docente != null)
                {
                    if (response.DatosImportados.TryGetValue("FechaNombramiento", out var fechaNombr))
                        docente.FechaNombramiento = (DateTime?)fechaNombr;
                    
                    docente.FechaUltimaImportacion = DateTime.UtcNow;
                    await _docenteRepository.UpdateAsync(docente);
                    
                    await _auditoriaService.RegistrarAccionAsync("IMPORTAR_DATOS_TTHH", 
                        docente.Id.ToString(), docente.Email, "Docente", null, 
                        $"Fecha nombramiento: {fechaNombr}", null);
                }
            }
            
            return response;
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
            var response = await _externalDataService.ImportarDesdeDADACAsync(cedula);
            
            if (response.Exitoso)
            {
                var docente = await _docenteRepository.GetByCedulaAsync(cedula);
                if (docente != null)
                {
                    if (response.DatosImportados.TryGetValue("PromedioEvaluaciones", out var promedio))
                        docente.PromedioEvaluaciones = Convert.ToDecimal(promedio);
                    
                    docente.FechaUltimaImportacion = DateTime.UtcNow;
                    await _docenteRepository.UpdateAsync(docente);
                    
                    await _auditoriaService.RegistrarAccionAsync("IMPORTAR_DATOS_DAC", 
                        docente.Id.ToString(), docente.Email, "Docente", null, 
                        $"Promedio evaluaciones: {promedio}", null);
                }
            }
            
            return response;
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
            var response = await _externalDataService.ImportarDesdeDITICAsync(cedula);
            
            if (response.Exitoso)
            {
                var docente = await _docenteRepository.GetByCedulaAsync(cedula);
                if (docente != null)
                {
                    if (response.DatosImportados.TryGetValue("HorasCapacitacion", out var horas))
                        docente.HorasCapacitacion = Convert.ToInt32(horas);
                    
                    docente.FechaUltimaImportacion = DateTime.UtcNow;
                    await _docenteRepository.UpdateAsync(docente);
                    
                    await _auditoriaService.RegistrarAccionAsync("IMPORTAR_DATOS_DITIC", 
                        docente.Id.ToString(), docente.Email, "Docente", null, 
                        $"Horas capacitación: {horas}", null);
                }
            }
            
            return response;
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
            var response = await _externalDataService.ImportarDesdeDIRINVAsync(cedula);
            
            if (response.Exitoso)
            {
                var docente = await _docenteRepository.GetByCedulaAsync(cedula);
                if (docente != null)
                {
                    if (response.DatosImportados.TryGetValue("NumeroObrasAcademicas", out var obras))
                        docente.NumeroObrasAcademicas = Convert.ToInt32(obras);
                    
                    if (response.DatosImportados.TryGetValue("MesesInvestigacion", out var meses))
                        docente.MesesInvestigacion = Convert.ToInt32(meses);
                    
                    docente.FechaUltimaImportacion = DateTime.UtcNow;
                    await _docenteRepository.UpdateAsync(docente);
                    
                    await _auditoriaService.RegistrarAccionAsync("IMPORTAR_DATOS_DIRINV", 
                        docente.Id.ToString(), docente.Email, "Docente", null, 
                        $"Obras académicas: {obras}, Meses investigación: {meses}", null);
                }
            }
            
            return response;
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
}