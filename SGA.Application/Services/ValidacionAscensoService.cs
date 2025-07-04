using SGA.Application.DTOs;
using SGA.Application.DTOs.Docentes;
using SGA.Application.Interfaces;
using SGA.Domain.Enums;
using SGA.Domain.Entities;

namespace SGA.Application.Services;

public class ValidacionAscensoService : IValidacionAscensoService
{
    private readonly IExternalDataService _externalDataService;
    private readonly IRequisitosDinamicosService _requisitosDinamicosService;

    public ValidacionAscensoService(
        IExternalDataService externalDataService,
        IRequisitosDinamicosService requisitosDinamicosService)
    {
        _externalDataService = externalDataService;
        _requisitosDinamicosService = requisitosDinamicosService;
    }

    public DTOs.Docentes.RequisitoAscensoDto ValidarRequisitos(DTOs.Docentes.DocenteDto docente, NivelTitular? nivelDestino = null)
    {
        var nivelActual = Enum.Parse<NivelTitular>(docente.NivelActual);
        var nivelObjetivo = nivelDestino ?? ObtenerSiguienteNivel(nivelActual) ?? nivelActual;

        // Intentar usar requisitos dinámicos primero
        var requisitosDinamicos = Task.Run(async () => 
            await _requisitosDinamicosService.ObtenerRequisitosAsync(nivelActual, nivelObjetivo)).Result;

        if (requisitosDinamicos != null)
        {
            return ValidarConRequisitosDinamicos(docente, requisitosDinamicos, nivelActual, nivelObjetivo);
        }

        // Fallback a validación hardcodeada si no hay requisitos dinámicos configurados
        return ValidarConRequisitosHardcodeados(docente, nivelActual, nivelObjetivo);
    }

    /// <summary>
    /// Valida usando requisitos dinámicos obtenidos de la base de datos
    /// </summary>
    private DTOs.Docentes.RequisitoAscensoDto ValidarConRequisitosDinamicos(
        DTOs.Docentes.DocenteDto docente, 
        RequisitoAscenso requisitos,
        NivelTitular nivelActual,
        NivelTitular nivelObjetivo)
    {
        var resultado = new DTOs.Docentes.RequisitoAscensoDto
        {
            NivelActual = nivelActual,
            NivelDestino = nivelObjetivo
        };

        // Calcular años en el nivel actual
        var añosEnNivel = CalcularAñosEnNivel(docente);

        // Validar antiguedad (convertir meses a años)
        resultado.AñosRequeridos = (int)Math.Ceiling(requisitos.TiempoMinimo / 12.0);
        resultado.AñosActuales = añosEnNivel;
        resultado.CumpleAntiguedad = añosEnNivel >= resultado.AñosRequeridos;

        // Validar obras académicas
        resultado.ObrasRequeridas = requisitos.ObrasMinimas;
        resultado.ObrasActuales = docente.NumeroObrasAcademicas ?? 0;
        resultado.CumpleObras = resultado.ObrasActuales >= requisitos.ObrasMinimas;

        // Validar evaluación docente
        resultado.PromedioRequerido = requisitos.PuntajeEvaluacionMinimo;
        resultado.PromedioActual = docente.PromedioEvaluaciones ?? 0m;
        resultado.CumpleEvaluacion = resultado.PromedioActual >= requisitos.PuntajeEvaluacionMinimo;

        // Validar capacitación
        resultado.HorasRequeridas = requisitos.HorasCapacitacionMinimas;
        resultado.HorasActuales = docente.HorasCapacitacion ?? 0;
        resultado.CumpleCapacitacion = resultado.HorasActuales >= requisitos.HorasCapacitacionMinimas;

        // Validar investigación (convertir meses a meses)
        resultado.MesesRequeridos = requisitos.TiempoInvestigacionMinimo;
        resultado.MesesActuales = docente.MesesInvestigacion ?? 0;
        resultado.CumpleInvestigacion = resultado.MesesActuales >= requisitos.TiempoInvestigacionMinimo;

        // Validar que cumple todos los requisitos
        resultado.PuedeAscender = resultado.CumpleAntiguedad && 
                                resultado.CumpleObras && 
                                resultado.CumpleEvaluacion && 
                                resultado.CumpleCapacitacion && 
                                resultado.CumpleInvestigacion;

        return resultado;
    }

    /// <summary>
    /// Valida usando requisitos hardcodeados (fallback)
    /// </summary>
    private DTOs.Docentes.RequisitoAscensoDto ValidarConRequisitosHardcodeados(
        DTOs.Docentes.DocenteDto docente,
        NivelTitular nivelActual,
        NivelTitular nivelObjetivo)
    {
        var resultado = new DTOs.Docentes.RequisitoAscensoDto
        {
            NivelActual = nivelActual,
            NivelDestino = nivelObjetivo
        };

        // Calcular años en el nivel actual
        var añosEnNivel = CalcularAñosEnNivel(docente);

        // Validar requisitos según el nivel objetivo usando métodos existentes
        switch (nivelObjetivo)
        {
            case NivelTitular.Titular2:
                ValidarRequisitosNivel2(resultado, docente, añosEnNivel);
                break;
            case NivelTitular.Titular3:
                ValidarRequisitosNivel3(resultado, docente, añosEnNivel);
                break;
            case NivelTitular.Titular4:
                ValidarRequisitosNivel4(resultado, docente, añosEnNivel);
                break;
            case NivelTitular.Titular5:
                ValidarRequisitosNivel5(resultado, docente, añosEnNivel);
                break;
        }

        return resultado;
    }

    /// <summary>
    /// Calcula los años que lleva el docente en su nivel actual
    /// </summary>
    private int CalcularAñosEnNivel(DTOs.Docentes.DocenteDto docente)
    {
        DateTime fechaInicio;
        if (docente.FechaInicioNivelActual.Year > 1900)
            fechaInicio = docente.FechaInicioNivelActual;
        else if (docente.FechaNombramiento.HasValue && docente.FechaNombramiento.Value.Year > 1900)
            fechaInicio = docente.FechaNombramiento.Value;
        else
            fechaInicio = DateTime.UtcNow;
            
        var tiempoEnNivel = DateTime.UtcNow - fechaInicio;
        return (int)(tiempoEnNivel.TotalDays / 365.25);
    }

    public bool PuedeAscender(DTOs.Docentes.DocenteDto docente)
    {
        var requisitos = ValidarRequisitos(docente);
        return requisitos.PuedeAscender;
    }

    public Task<DTOs.Docentes.RequisitoAscensoDto> ValidarRequisitosAscensoAsync(Docente docente, NivelTitular? nivelDestino = null)
    {
        // Convertir entidad a DTO
        var docenteDto = new DTOs.Docentes.DocenteDto
        {
            Id = docente.Id,
            Cedula = docente.Cedula,
            Nombres = docente.Nombres,
            Apellidos = docente.Apellidos,
            Email = docente.Email,
            NivelActual = docente.NivelActual.ToString(),
            FechaInicioNivelActual = docente.FechaInicioNivelActual,
            FechaUltimoAscenso = docente.FechaUltimoAscenso,
            FechaNombramiento = docente.FechaNombramiento,
            PromedioEvaluaciones = docente.PromedioEvaluaciones,
            HorasCapacitacion = docente.HorasCapacitacion,
            NumeroObrasAcademicas = docente.NumeroObrasAcademicas,
            MesesInvestigacion = docente.MesesInvestigacion
        };

        var resultado = ValidarRequisitos(docenteDto, nivelDestino);
        return Task.FromResult(resultado);
    }

    public async Task<DTOs.Docentes.RequisitoAscensoDto> ValidarRequisitosConDatosExternosAsync(string cedula, NivelTitular? nivelDestino = null)
    {
        // Obtener datos de TTHH primero (es la fuente principal)
        var datosTTHH = await _externalDataService.ImportarDatosTTHHAsync(cedula);
        if (datosTTHH == null)
        {
            throw new InvalidOperationException($"No se encontraron datos de empleado para la cédula {cedula}");
        }

        // Obtener datos de otras fuentes
        var datosDAC = await _externalDataService.ImportarDatosDACAsync(cedula);
        var datosDITIC = await _externalDataService.ImportarDatosDITICAsync(cedula);
        var datosDirInv = await _externalDataService.ImportarDatosDirInvAsync(cedula);

        // Construir DTO con todos los datos
        var docenteDto = new DTOs.Docentes.DocenteDto
        {
            Cedula = datosTTHH.Cedula,
            Nombres = datosTTHH.Nombres,
            Apellidos = datosTTHH.Apellidos,
            Email = datosTTHH.Email,
            NivelActual = datosTTHH.NivelActual,
            FechaInicioNivelActual = datosTTHH.FechaIngresoNivelActual ?? datosTTHH.FechaNombramiento ?? DateTime.Now,
            FechaNombramiento = datosTTHH.FechaNombramiento,
            
            // Datos de evaluación docente (DAC)
            PromedioEvaluaciones = datosDAC?.PromedioEvaluaciones ?? 0,
            
            // Datos de capacitación (DITIC)
            HorasCapacitacion = datosDITIC?.HorasCapacitacion ?? 0,
            
            // Datos de investigación (DIRINV)
            NumeroObrasAcademicas = datosDirInv?.NumeroObrasAcademicas ?? 0,
            MesesInvestigacion = datosDirInv?.MesesInvestigacion ?? 0
        };

        return ValidarRequisitos(docenteDto, nivelDestino);
    }

    public NivelTitular? ObtenerSiguienteNivel(NivelTitular nivelActual)
    {
        return nivelActual switch
        {
            NivelTitular.Titular1 => NivelTitular.Titular2,
            NivelTitular.Titular2 => NivelTitular.Titular3,
            NivelTitular.Titular3 => NivelTitular.Titular4,
            NivelTitular.Titular4 => NivelTitular.Titular5,
            NivelTitular.Titular5 => null, // Ya está en el nivel máximo
            _ => null
        };
    }

    private void ValidarRequisitosNivel2(DTOs.Docentes.RequisitoAscensoDto resultado, DTOs.Docentes.DocenteDto docente, int añosEnNivel)
    {
        resultado.AñosRequeridos = 4;
        resultado.AñosActuales = añosEnNivel;
        resultado.CumpleAntiguedad = añosEnNivel >= 4;

        resultado.ObrasRequeridas = 1;
        resultado.ObrasActuales = docente.NumeroObrasAcademicas ?? 0;
        resultado.CumpleObras = resultado.ObrasActuales >= 1;

        resultado.PromedioRequerido = 75m;
        resultado.PromedioActual = docente.PromedioEvaluaciones ?? 0m;
        resultado.CumpleEvaluacion = resultado.PromedioActual >= 75m;

        resultado.HorasRequeridas = 96;
        resultado.HorasActuales = docente.HorasCapacitacion ?? 0;
        resultado.CumpleCapacitacion = resultado.HorasActuales >= 96;

        resultado.MesesRequeridos = 0;
        resultado.MesesActuales = docente.MesesInvestigacion ?? 0;
        resultado.CumpleInvestigacion = true; // No se requiere para nivel 2
        
        resultado.PuedeAscender = resultado.CumpleAntiguedad && 
                                resultado.CumpleObras && 
                                resultado.CumpleEvaluacion && 
                                resultado.CumpleCapacitacion;
    }

    private void ValidarRequisitosNivel3(DTOs.Docentes.RequisitoAscensoDto resultado, DTOs.Docentes.DocenteDto docente, int añosEnNivel)
    {
        resultado.AñosRequeridos = 4;
        resultado.AñosActuales = añosEnNivel;
        resultado.CumpleAntiguedad = añosEnNivel >= 4;

        resultado.ObrasRequeridas = 2;
        resultado.ObrasActuales = docente.NumeroObrasAcademicas ?? 0;
        resultado.CumpleObras = resultado.ObrasActuales >= 2;

        resultado.PromedioRequerido = 75m;
        resultado.PromedioActual = docente.PromedioEvaluaciones ?? 0m;
        resultado.CumpleEvaluacion = resultado.PromedioActual >= 75m;

        resultado.HorasRequeridas = 96;
        resultado.HorasActuales = docente.HorasCapacitacion ?? 0;
        resultado.CumpleCapacitacion = resultado.HorasActuales >= 96;

        resultado.MesesRequeridos = 12;
        resultado.MesesActuales = docente.MesesInvestigacion ?? 0;
        resultado.CumpleInvestigacion = resultado.MesesActuales >= 12;
        
        resultado.PuedeAscender = resultado.CumpleAntiguedad && 
                                resultado.CumpleObras && 
                                resultado.CumpleEvaluacion && 
                                resultado.CumpleCapacitacion && 
                                resultado.CumpleInvestigacion;
    }

    private void ValidarRequisitosNivel4(DTOs.Docentes.RequisitoAscensoDto resultado, DTOs.Docentes.DocenteDto docente, int añosEnNivel)
    {
        resultado.AñosRequeridos = 4;
        resultado.AñosActuales = añosEnNivel;
        resultado.CumpleAntiguedad = añosEnNivel >= 4;

        resultado.ObrasRequeridas = 3;
        resultado.ObrasActuales = docente.NumeroObrasAcademicas ?? 0;
        resultado.CumpleObras = resultado.ObrasActuales >= 3;

        resultado.PromedioRequerido = 75m;
        resultado.PromedioActual = docente.PromedioEvaluaciones ?? 0m;
        resultado.CumpleEvaluacion = resultado.PromedioActual >= 75m;

        resultado.HorasRequeridas = 128;
        resultado.HorasActuales = docente.HorasCapacitacion ?? 0;
        resultado.CumpleCapacitacion = resultado.HorasActuales >= 128;

        resultado.MesesRequeridos = 24;
        resultado.MesesActuales = docente.MesesInvestigacion ?? 0;
        resultado.CumpleInvestigacion = resultado.MesesActuales >= 24;
        
        resultado.PuedeAscender = resultado.CumpleAntiguedad && 
                                resultado.CumpleObras && 
                                resultado.CumpleEvaluacion && 
                                resultado.CumpleCapacitacion && 
                                resultado.CumpleInvestigacion;
    }

    private void ValidarRequisitosNivel5(DTOs.Docentes.RequisitoAscensoDto resultado, DTOs.Docentes.DocenteDto docente, int añosEnNivel)
    {
        resultado.AñosRequeridos = 4;
        resultado.AñosActuales = añosEnNivel;
        resultado.CumpleAntiguedad = añosEnNivel >= 4;

        resultado.ObrasRequeridas = 5;
        resultado.ObrasActuales = docente.NumeroObrasAcademicas ?? 0;
        resultado.CumpleObras = resultado.ObrasActuales >= 5;

        resultado.PromedioRequerido = 75m;
        resultado.PromedioActual = docente.PromedioEvaluaciones ?? 0m;
        resultado.CumpleEvaluacion = resultado.PromedioActual >= 75m;

        resultado.HorasRequeridas = 160;
        resultado.HorasActuales = docente.HorasCapacitacion ?? 0;
        resultado.CumpleCapacitacion = resultado.HorasActuales >= 160;

        resultado.MesesRequeridos = 24;
        resultado.MesesActuales = docente.MesesInvestigacion ?? 0;
        resultado.CumpleInvestigacion = resultado.MesesActuales >= 24;
        
        resultado.PuedeAscender = resultado.CumpleAntiguedad && 
                                resultado.CumpleObras && 
                                resultado.CumpleEvaluacion && 
                                resultado.CumpleCapacitacion && 
                                resultado.CumpleInvestigacion;
    }
}
