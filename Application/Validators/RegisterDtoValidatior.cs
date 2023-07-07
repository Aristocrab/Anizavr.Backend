using System.Text.RegularExpressions;
using Application.Dtos;
using FluentValidation;

namespace Application.Validators;

public partial class RegisterDtoValidatior : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidatior()
    {
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.Username).NotEmpty()
            .MinimumLength(6)
            .Must(BeAValidUsername); // todo
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }

    private static bool BeAValidUsername(string username)
    {
        return UsernameRegex().IsMatch(username);
    }

    [GeneratedRegex("^[A-Za-z0-9_]+$")]
    private static partial Regex UsernameRegex();
}