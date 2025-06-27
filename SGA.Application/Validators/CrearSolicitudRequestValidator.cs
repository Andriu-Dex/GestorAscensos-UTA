using FluentValidation;
using SGA.Application.DTOs.Solicitudes;

namespace SGA.Application.Validators;

public class CrearSolicitudRequestValidator : AbstractValidator<CrearSolicitudRequest>
{
    public CrearSolicitudRequestValidator()
    {
        RuleFor(x => x.NivelSolicitado)
            .NotEmpty()
            .WithMessage("El nivel solicitado es requerido");

        RuleFor(x => x.Documentos)
            .NotEmpty()
            .WithMessage("Debe adjuntar al menos un documento de respaldo");

        RuleFor(x => x.Documentos)
            .Must(docs => docs.Count <= 10)
            .WithMessage("No puede adjuntar más de 10 documentos");
    }
}
