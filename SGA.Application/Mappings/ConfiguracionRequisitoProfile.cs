using AutoMapper;
using SGA.Application.DTOs.Admin;
using SGA.Domain.Entities;
using SGA.Domain.Extensions;

namespace SGA.Application.Mappings;

/// <summary>
/// Perfil de AutoMapper para ConfiguracionRequisito
/// </summary>
public class ConfiguracionRequisitoProfile : Profile
{
    public ConfiguracionRequisitoProfile()
    {
        // Mapeo de entidad a DTO principal
        CreateMap<ConfiguracionRequisito, ConfiguracionRequisitoDto>()
            .ForMember(dest => dest.NombreAscenso, 
                opt => opt.MapFrom(src => GetNombreAscenso(src)))
            .ForMember(dest => dest.ResumenRequisitos, 
                opt => opt.MapFrom(src => src.ObtenerResumenRequisitos()))
            .ForMember(dest => dest.NivelActualNombre, 
                opt => opt.MapFrom(src => src.NivelActual.HasValue ? src.NivelActual.Value.GetDescription() : 
                    (src.TituloActual != null ? src.TituloActual.Nombre : "N/A")))
            .ForMember(dest => dest.NivelSolicitadoNombre, 
                opt => opt.MapFrom(src => src.NivelSolicitado.HasValue ? src.NivelSolicitado.Value.GetDescription() : 
                    (src.TituloSolicitado != null ? src.TituloSolicitado.Nombre : "N/A")));

        // Mapeo de DTO de creación/actualización a entidad
        CreateMap<CrearActualizarConfiguracionRequisitoDto, ConfiguracionRequisito>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore())
            .ForMember(dest => dest.ModificadoPor, opt => opt.Ignore());

        // Mapeo de entidad a DTO de resumen
        CreateMap<ConfiguracionRequisito, ConfiguracionRequisitoResumenDto>()
            .ForMember(dest => dest.NombreAscenso, 
                opt => opt.MapFrom(src => GetNombreAscenso(src)))
            .ForMember(dest => dest.ResumenRequisitos, 
                opt => opt.MapFrom(src => src.ObtenerResumenRequisitos()))
            .ForMember(dest => dest.FechaModificacion, 
                opt => opt.MapFrom(src => src.FechaModificacion ?? src.FechaCreacion));

        // Mapeo inverso de DTO a entidad para actualizaciones
        CreateMap<ConfiguracionRequisitoDto, ConfiguracionRequisito>()
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore());
    }

    private static string GetNombreAscenso(ConfiguracionRequisito src)
    {
        var nivelActual = src.NivelActual?.GetDescription() ?? src.TituloActual?.Nombre ?? "N/A";
        var nivelSolicitado = src.NivelSolicitado?.GetDescription() ?? src.TituloSolicitado?.Nombre ?? "N/A";
        return $"{nivelActual} → {nivelSolicitado}";
    }
}
