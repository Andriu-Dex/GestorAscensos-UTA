using FluentValidation;
using SGA.Application.DTOs.Documentos;

namespace SGA.Application.Validators;

public class SubirDocumentoRequestValidator : AbstractValidator<SubirDocumentoRequestDto>
{
    public SubirDocumentoRequestValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty()
            .WithMessage("El nombre del documento es requerido")
            .MaximumLength(255)
            .WithMessage("El nombre del documento no puede exceder 255 caracteres");

        RuleFor(x => x.TipoContenido)
            .NotEmpty()
            .WithMessage("El tipo de contenido es requerido")
            .Must(x => x == "application/pdf")
            .WithMessage("Solo se permiten archivos PDF");

        RuleFor(x => x.Contenido)
            .NotNull()
            .WithMessage("El contenido del archivo es requerido")
            .Must(x => x.Length > 0)
            .WithMessage("El archivo no puede estar vacío")
            .Must(x => x.Length <= 10 * 1024 * 1024) // 10MB
            .WithMessage("El archivo no puede exceder 10MB");

        RuleFor(x => x.Tipo)
            .IsInEnum()
            .WithMessage("El tipo de documento no es válido");
    }
}
