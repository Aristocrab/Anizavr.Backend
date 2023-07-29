using Anizavr.Backend.Application.Dtos;
using FluentValidation;

namespace Anizavr.Backend.Application.Validators;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Пустой имейл");
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Пустой пароль");
    }
}