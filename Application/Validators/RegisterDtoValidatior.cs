using System.Text.RegularExpressions;
using Application.Dtos;
using FluentValidation;

namespace Application.Validators;

public partial class RegisterDtoValidatior : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidatior()
    {
        RuleFor(x => x.Email)
            .Must(BeAValidEmail).WithMessage("Некорректный емейл")
            .NotEmpty().WithMessage("Пустой емейл");
        RuleFor(x => x.Username)
            .Must(BeAValidUsername).WithMessage("Никнейм может содержать только английские буквы, цифры и подчёркивание")
            .NotEmpty().WithMessage("Пустой никнейм")
            .MinimumLength(6).WithMessage("Длина никнейма должна быть минимум 6 символов");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пустой пароль")
            .MinimumLength(6).WithMessage("Длина пароля должна быть минимум 6 символов");
    }

    private static bool BeAValidUsername(string username)
    {
        return UsernameRegex().IsMatch(username);
    }
    
    private static bool BeAValidEmail(string username)
    {
        return EmailRegex().IsMatch(username);
    }

    [GeneratedRegex("^[A-Za-z0-9_]+$")]
    private static partial Regex UsernameRegex();
    
    [GeneratedRegex("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$")]
    private static partial Regex EmailRegex();
}