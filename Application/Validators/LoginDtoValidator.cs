using Application.Dtos;
using FluentValidation;

namespace Application.Validators;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MinimumLength(6);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}