using FluentValidation;
using SGA.Application.DTOs.Solicitudes;

namespace SGA.Application.Validators;

public class CrearSolicitudRequestValidator : AbstractValidator<CrearSolicitudRequest>
{
    public CrearSolicitudRequestValidator()
    {
        RuleFor(x => x.NivelSolicitado)
            .GreaterThan(0)
            .LessThanOrEqualTo(5)
            .WithMessage("El nivel solicitado debe estar entre 1 y 5");

        RuleFor(x => x.NivelActual)
            .GreaterThan(0)
            .LessThanOrEqualTo(5)
            .WithMessage("El nivel actual debe estar entre 1 y 5");

        RuleFor(x => x.DocumentosIds)
            .NotEmpty()
            .WithMessage("Debe seleccionar al menos un documento de respaldo");

        RuleFor(x => x.DocumentosIds)
            .Must(docs => docs.Count <= 10)
            .WithMessage("No puede seleccionar más de 10 documentos");
    }
}
