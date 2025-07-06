using AutoMapper;
using SGA.Application.DTOs;
using SGA.Application.DTOs.Auth;
using SGA.Application.DTOs.Docentes;
using SGA.Application.DTOs.Documentos;
using SGA.Application.DTOs.Solicitudes;
using SGA.Application.Features.Docentes.Commands;
using SGA.Domain.Entities;
using SGA.Domain.Enums;
using SGA.Domain.Extensions;

namespace SGA.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Mapeos de Docente
        CreateMap<Docente, DocenteDto>()
            .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src => $"{src.Nombres} {src.Apellidos}"));

        CreateMap<CreateDocenteDto, Docente>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<CreateDocenteCommand, Docente>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UsuarioId, opt => opt.Ignore());

        CreateMap<UpdateDocenteDto, Docente>()
            .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.Cedula, opt => opt.Ignore())
            .ForMember(dest => dest.UsuarioId, opt => opt.Ignore());

        CreateMap<UpdateDocenteCommand, Docente>()
            .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.Cedula, opt => opt.Ignore())
            .ForMember(dest => dest.UsuarioId, opt => opt.Ignore());

        // Mapeos de SolicitudAscenso
        CreateMap<SolicitudAscenso, DTOs.Solicitudes.SolicitudAscensoDto>()
            .ForMember(dest => dest.DocenteNombre, opt => opt.MapFrom(src => $"{src.Docente.Nombres} {src.Docente.Apellidos}"))
            .ForMember(dest => dest.NivelActual, opt => opt.MapFrom(src => src.NivelActual.GetDescription()))
            .ForMember(dest => dest.NivelSolicitado, opt => opt.MapFrom(src => src.NivelSolicitado.GetDescription()))
            .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado.GetDescription()));

        CreateMap<CreateSolicitudAscensoDto, SolicitudAscenso>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => EstadoSolicitud.Pendiente))
            .ForMember(dest => dest.FechaSolicitud, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdateSolicitudAscensoDto, SolicitudAscenso>()
            .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.FechaAprobacion, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.DocenteId, opt => opt.Ignore())
            .ForMember(dest => dest.FechaSolicitud, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore());

        // Mapeos de Documento
        CreateMap<Documento, DTOs.Solicitudes.DocumentoDto>()
            .ForMember(dest => dest.TipoDocumento, opt => opt.MapFrom(src => src.TipoDocumento.ToString()));

        CreateMap<DTOs.Solicitudes.DocumentoUploadDto, Documento>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TamanoArchivo, opt => opt.MapFrom(src => src.ContenidoArchivo.Length))
            .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src => DateTime.UtcNow));

        // Mapeos de Usuario
        CreateMap<Usuario, DTOs.Auth.UsuarioDto>()
            .ForMember(dest => dest.Docente, opt => opt.MapFrom(src => src.Docente));
        
        CreateMap<RegisterDto, Usuario>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.Rol, opt => opt.MapFrom(src => "Docente"))
            .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.EstaActivo, opt => opt.MapFrom(src => true));

        // Mapeos de ConfiguracionRequisito
        CreateMap<ConfiguracionRequisito, DTOs.Admin.ConfiguracionRequisitoDto>()
            .ForMember(dest => dest.NombreAscenso, opt => opt.MapFrom(src => $"{src.NivelActual.GetDescription()} → {src.NivelSolicitado.GetDescription()}"))
            .ForMember(dest => dest.NivelActualNombre, opt => opt.MapFrom(src => src.NivelActual.GetDescription()))
            .ForMember(dest => dest.NivelSolicitadoNombre, opt => opt.MapFrom(src => src.NivelSolicitado.GetDescription()))
            .ForMember(dest => dest.ResumenRequisitos, opt => opt.MapFrom(src => 
                $"{src.TiempoMinimoMeses} meses, {src.ObrasMinimas} obras, {src.PuntajeEvaluacionMinimo}% eval, {src.HorasCapacitacionMinimas}h cap"));

        CreateMap<DTOs.Admin.CrearActualizarConfiguracionRequisitoDto, ConfiguracionRequisito>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.ModificadoPor, opt => opt.Ignore());
    }
}
