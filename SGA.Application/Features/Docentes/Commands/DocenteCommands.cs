using MediatR;
using SGA.Application.DTOs;
using SGA.Application.DTOs.Docentes;

namespace SGA.Application.Features.Docentes.Commands;

public class CreateDocenteCommand : IRequest<DocenteDto>
{
    public string Cedula { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string NivelActual { get; set; } = string.Empty;
    public DateTime FechaInicioNivelActual { get; set; }
    public Guid? UsuarioId { get; set; }
    
    public CreateDocenteCommand(CreateDocenteDto docente, Guid? usuarioId = null)
    {
        Cedula = docente.Cedula;
        Nombres = docente.Nombres;
        Apellidos = docente.Apellidos;
        Email = docente.Email;
        NivelActual = docente.NivelActual.ToString();
        FechaInicioNivelActual = docente.FechaInicioNivelActual;
        UsuarioId = usuarioId;
    }
}

public class UpdateDocenteCommand : IRequest<DocenteDto>
{
    public Guid Id { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string NivelActual { get; set; } = string.Empty;
    public DateTime FechaInicioNivelActual { get; set; }
    public bool EstaActivo { get; set; }
    public Guid? UsuarioId { get; set; }
    
    public UpdateDocenteCommand(UpdateDocenteDto docente, Guid? usuarioId = null)
    {
        Id = docente.Id;
        Nombres = docente.Nombres;
        Apellidos = docente.Apellidos;
        Email = docente.Email;
        NivelActual = docente.NivelActual.ToString();
        FechaInicioNivelActual = docente.FechaInicioNivelActual;
        EstaActivo = docente.EstaActivo;
        UsuarioId = usuarioId;
    }
}

public class ImportarDatosTTHHCommand : IRequest<DocenteDto>
{
    public Guid DocenteId { get; set; }
    public Guid? UsuarioId { get; set; }
    
    public ImportarDatosTTHHCommand(Guid docenteId, Guid? usuarioId = null)
    {
        DocenteId = docenteId;
        UsuarioId = usuarioId;
    }
}

public class ImportarDatosDACCommand : IRequest<DocenteDto>
{
    public Guid DocenteId { get; set; }
    public Guid? UsuarioId { get; set; }
    
    public ImportarDatosDACCommand(Guid docenteId, Guid? usuarioId = null)
    {
        DocenteId = docenteId;
        UsuarioId = usuarioId;
    }
}

public class ImportarDatosDITICCommand : IRequest<DocenteDto>
{
    public Guid DocenteId { get; set; }
    public Guid? UsuarioId { get; set; }
    
    public ImportarDatosDITICCommand(Guid docenteId, Guid? usuarioId = null)
    {
        DocenteId = docenteId;
        UsuarioId = usuarioId;
    }
}

public class ImportarDatosDirInvCommand : IRequest<DocenteDto>
{
    public Guid DocenteId { get; set; }
    public Guid? UsuarioId { get; set; }
    
    public ImportarDatosDirInvCommand(Guid docenteId, Guid? usuarioId = null)
    {
        DocenteId = docenteId;
        UsuarioId = usuarioId;
    }
}

public class ImportarTodosLosDatosCommand : IRequest<DocenteDto>
{
    public Guid DocenteId { get; set; }
    public Guid? UsuarioId { get; set; }
    
    public ImportarTodosLosDatosCommand(Guid docenteId, Guid? usuarioId = null)
    {
        DocenteId = docenteId;
        UsuarioId = usuarioId;
    }
}

public class ActivarDesactivarDocenteCommand : IRequest<DocenteDto>
{
    public Guid DocenteId { get; set; }
    public bool Activar { get; set; }
    
    public ActivarDesactivarDocenteCommand(Guid docenteId, bool activar)
    {
        DocenteId = docenteId;
        Activar = activar;
    }
}
