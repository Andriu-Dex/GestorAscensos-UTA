using FluentValidation;
using SGA.Application.DTOs.Auth;

namespace SGA.Application.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("El email es requerido")
            .EmailAddress()
            .WithMessage("El formato del email no es válido")
            .MaximumLength(255)
            .WithMessage("El email no puede exceder 255 caracteres");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("La contraseña es requerida")
            .MinimumLength(8)
            .WithMessage("La contraseña debe tener al menos 8 caracteres")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
            .WithMessage("La contraseña debe contener al menos: 1 minúscula, 1 mayúscula, 1 número y 1 símbolo");

        RuleFor(x => x.Nombres)
            .NotEmpty()
            .WithMessage("Los nombres son requeridos")
            .MaximumLength(255)
            .WithMessage("Los nombres no pueden exceder 255 caracteres");

        RuleFor(x => x.Apellidos)
            .NotEmpty()
            .WithMessage("Los apellidos son requeridos")
            .MaximumLength(255)
            .WithMessage("Los apellidos no pueden exceder 255 caracteres");

        RuleFor(x => x.Cedula)
            .NotEmpty()
            .WithMessage("La cédula es requerida")
            .Length(10)
            .WithMessage("La cédula debe tener exactamente 10 dígitos")
            .Matches(@"^\d+$")
            .WithMessage("La cédula solo puede contener números");
    }
}

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("El email es requerido")
            .EmailAddress()
            .WithMessage("El formato del email no es válido");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("La contraseña es requerida");
    }
}
