using Application.Dtos;
using FluentValidation;

namespace Application.Validators;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Пустой емейл");
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Пустой пароль");
    }
}