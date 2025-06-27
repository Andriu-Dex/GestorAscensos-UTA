using MediatR;
using SGA.Application.DTOs.Docentes;

namespace SGA.Application.Features.Docentes.Queries;

public class GetDocenteByIdQuery : IRequest<DocenteDto?>
{
    public Guid Id { get; set; }
    
    public GetDocenteByIdQuery(Guid id)
    {
        Id = id;
    }
}

public class GetDocenteByUsuarioIdQuery : IRequest<DocenteDto?>
{
    public Guid UsuarioId { get; set; }
    
    public GetDocenteByUsuarioIdQuery(Guid usuarioId)
    {
        UsuarioId = usuarioId;
    }
}

public class GetAllDocentesQuery : IRequest<List<DocenteDto>>
{
    public bool IncluirInactivos { get; set; }
    
    public GetAllDocentesQuery(bool incluirInactivos = false)
    {
        IncluirInactivos = incluirInactivos;
    }
}

public class GetDocentesConRequisitosQuery : IRequest<List<DocenteDto>>
{
    public bool SoloQuePodenAscender { get; set; }
    
    public GetDocentesConRequisitosQuery(bool soloQuePodenAscender = false)
    {
        SoloQuePodenAscender = soloQuePodenAscender;
    }
}

public class ValidarRequisitosAscensoQuery : IRequest<RequisitoAscensoDto>
{
    public Guid DocenteId { get; set; }
    public Domain.Enums.NivelTitular? NivelDestino { get; set; }
    
    public ValidarRequisitosAscensoQuery(Guid docenteId, Domain.Enums.NivelTitular? nivelDestino = null)
    {
        DocenteId = docenteId;
        NivelDestino = nivelDestino;
    }
}
