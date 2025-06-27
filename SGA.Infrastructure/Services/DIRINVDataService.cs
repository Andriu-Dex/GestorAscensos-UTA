using Microsoft.EntityFrameworkCore;
using SGA.Application.DTOs.ExternalData;
using SGA.Application.Interfaces;
using SGA.Domain.Entities.External;
using SGA.Infrastructure.Data.External;

namespace SGA.Infrastructure.Services;

public class DIRINVDataService : IDIRINVDataService
{
    private readonly DIRINVDbContext _context;

    public DIRINVDataService(DIRINVDbContext context)
    {
        _context = context;
    }

    public async Task<List<ObraAcademicaDto>> GetObrasDocenteAsync(string cedula)
    {
        var obras = await _context.ObrasAcademicas
            .Where(o => o.Cedula == cedula)
            .OrderByDescending(o => o.FechaPublicacion)
            .ToListAsync();

        return obras.Select(o => new ObraAcademicaDto
        {
            Titulo = o.Titulo,
            Tipo = o.TipoObra,
            FechaPublicacion = o.FechaPublicacion,
            Revista = o.Revista ?? "",
            Autores = ""
        }).ToList();
    }

    public async Task AddObraAcademicaAsync(string cedula, ObraAcademicaDto obra)
    {
        var nuevaObra = new ObraAcademicaDIRINV
        {
            Cedula = cedula,
            Titulo = obra.Titulo,
            TipoObra = obra.Tipo,
            FechaPublicacion = obra.FechaPublicacion,
            Revista = obra.Revista,
            EsIndexada = false
        };

        _context.ObrasAcademicas.Add(nuevaObra);
        await _context.SaveChangesAsync();
    }
}
