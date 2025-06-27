using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SGA.Application.DTOs.Docentes;
using SGA.Application.Features.Docentes.Queries;
using SGA.Application.Interfaces;
using SGA.Domain.Entities;
using SGA.Domain.Enums;

namespace SGA.Application.Features.Docentes.Handlers;

public class DocenteQueryHandlers :
    IRequestHandler<GetDocenteByIdQuery, DocenteDto?>,
    IRequestHandler<GetDocenteByUsuarioIdQuery, DocenteDto?>,
    IRequestHandler<GetAllDocentesQuery, List<DocenteDto>>,
    IRequestHandler<GetDocentesConRequisitosQuery, List<DocenteDto>>,
    IRequestHandler<ValidarRequisitosAscensoQuery, RequisitoAscensoDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IValidacionAscensoService _validacionService;

    public DocenteQueryHandlers(
        IApplicationDbContext context,
        IMapper mapper,
        IValidacionAscensoService validacionService)
    {
        _context = context;
        _mapper = mapper;
        _validacionService = validacionService;
    }

    public async Task<DocenteDto?> Handle(GetDocenteByIdQuery request, CancellationToken cancellationToken)
    {
        var docente = await _context.Docentes
            .Include(d => d.Usuario)
            .Include(d => d.SolicitudesAscenso)
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (docente == null) return null;

        var dto = _mapper.Map<DocenteDto>(docente);
        await EnriquecerConRequisitos(dto);
        return dto;
    }

    public async Task<DocenteDto?> Handle(GetDocenteByUsuarioIdQuery request, CancellationToken cancellationToken)
    {
        var docente = await _context.Docentes
            .Include(d => d.Usuario)
            .Include(d => d.SolicitudesAscenso)
            .FirstOrDefaultAsync(d => d.UsuarioId == request.UsuarioId, cancellationToken);

        if (docente == null) return null;

        var dto = _mapper.Map<DocenteDto>(docente);
        await EnriquecerConRequisitos(dto);
        return dto;
    }

    public async Task<List<DocenteDto>> Handle(GetAllDocentesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Docentes
            .Include(d => d.Usuario)
            .Include(d => d.SolicitudesAscenso)
            .AsQueryable();

        if (!request.IncluirInactivos)
        {
            query = query.Where(d => d.EstaActivo);
        }

        var docentes = await query
            .OrderBy(d => d.Apellidos)
            .ThenBy(d => d.Nombres)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<DocenteDto>>(docentes);
        
        foreach (var dto in dtos)
        {
            await EnriquecerConRequisitos(dto);
        }

        return dtos;
    }

    public async Task<List<DocenteDto>> Handle(GetDocentesConRequisitosQuery request, CancellationToken cancellationToken)
    {
        var docentes = await _context.Docentes
            .Include(d => d.Usuario)
            .Include(d => d.SolicitudesAscenso)
            .Where(d => d.EstaActivo)
            .OrderBy(d => d.Apellidos)
            .ThenBy(d => d.Nombres)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<DocenteDto>>(docentes);
        
        var resultado = new List<DocenteDto>();
        
        foreach (var dto in dtos)
        {
            await EnriquecerConRequisitos(dto);
            
            if (!request.SoloQuePodenAscender || dto.PuedeAscender)
            {
                resultado.Add(dto);
            }
        }

        return resultado;
    }

    public async Task<RequisitoAscensoDto> Handle(ValidarRequisitosAscensoQuery request, CancellationToken cancellationToken)
    {
        var docente = await _context.Docentes
            .FirstOrDefaultAsync(d => d.Id == request.DocenteId, cancellationToken);

        if (docente == null)
        {
            throw new ArgumentException($"No se encontró el docente con ID {request.DocenteId}");
        }

        var nivelDestino = request.NivelDestino ?? ObtenerSiguienteNivel(docente.NivelActual);
        
        return await _validacionService.ValidarRequisitosAscensoAsync(docente, nivelDestino);
    }

    private async Task EnriquecerConRequisitos(DocenteDto dto)
    {
        try
        {
            var nivelActual = Enum.Parse<NivelTitular>(dto.NivelActual);
            var requisitos = await _validacionService.ValidarRequisitosAscensoAsync(
                new Docente 
                { 
                    Id = dto.Id,
                    NivelActual = nivelActual,
                    FechaInicioNivelActual = dto.FechaInicioNivelActual,
                    FechaNombramiento = dto.FechaNombramiento,
                    PromedioEvaluaciones = dto.PromedioEvaluaciones,
                    HorasCapacitacion = dto.HorasCapacitacion,
                    NumeroObrasAcademicas = dto.NumeroObrasAcademicas,
                    MesesInvestigacion = dto.MesesInvestigacion
                }, 
                ObtenerSiguienteNivel(nivelActual));

            dto.Requisitos = new ValidacionRequisitosDto
            {
                NivelActual = dto.NivelActual,
                NivelSiguiente = ObtenerSiguienteNivel(nivelActual).ToString(),
                CumpleTodos = requisitos.PuedeAscender,
                Requisitos = ConvertirARequisitosDto(requisitos)
            };
            dto.PuedeAscender = requisitos.PuedeAscender;
            dto.SiguienteNivel = ObtenerSiguienteNivel(nivelActual).ToString();
        }
        catch
        {
            dto.PuedeAscender = false;
            dto.SiguienteNivel = "No disponible";
        }
    }

    private static NivelTitular ObtenerSiguienteNivel(NivelTitular nivelActual)
    {
        return nivelActual switch
        {
            NivelTitular.Titular1 => NivelTitular.Titular2,
            NivelTitular.Titular2 => NivelTitular.Titular3,
            NivelTitular.Titular3 => NivelTitular.Titular4,
            NivelTitular.Titular4 => NivelTitular.Titular5,
            _ => nivelActual
        };
    }

    private static List<RequisitoDto> ConvertirARequisitosDto(DTOs.Docentes.RequisitoAscensoDto requisitos)
    {
        var result = new List<RequisitoDto>();
        
        result.Add(new RequisitoDto 
        { 
            Nombre = "Años en el nivel actual", 
            ValorRequerido = $"{requisitos.AñosRequeridos} años", 
            ValorActual = $"{requisitos.AñosActuales} años",
            Cumple = requisitos.CumpleAntiguedad,
            Descripcion = "Tiempo mínimo requerido en el nivel actual"
        });
        
        result.Add(new RequisitoDto 
        { 
            Nombre = "Obras académicas", 
            ValorRequerido = $"{requisitos.ObrasRequeridas} obras", 
            ValorActual = $"{requisitos.ObrasActuales} obras",
            Cumple = requisitos.CumpleObras,
            Descripcion = "Cantidad mínima de publicaciones, libros o artículos"
        });
        
        result.Add(new RequisitoDto 
        { 
            Nombre = "Evaluación docente", 
            ValorRequerido = $"{requisitos.PromedioRequerido}%", 
            ValorActual = $"{requisitos.PromedioActual}%",
            Cumple = requisitos.CumpleEvaluacion,
            Descripcion = "Promedio mínimo en evaluaciones de los últimos períodos"
        });
        
        result.Add(new RequisitoDto 
        { 
            Nombre = "Capacitación", 
            ValorRequerido = $"{requisitos.HorasRequeridas} horas", 
            ValorActual = $"{requisitos.HorasActuales} horas",
            Cumple = requisitos.CumpleCapacitacion,
            Descripcion = "Horas mínimas de capacitación en los últimos años"
        });
        
        if (requisitos.MesesRequeridos > 0)
        {
            result.Add(new RequisitoDto 
            { 
                Nombre = "Investigación", 
                ValorRequerido = $"{requisitos.MesesRequeridos} meses", 
                ValorActual = $"{requisitos.MesesActuales} meses",
                Cumple = requisitos.CumpleInvestigacion,
                Descripcion = "Tiempo mínimo dedicado a proyectos de investigación"
            });
        }
        
        return result;
    }
}