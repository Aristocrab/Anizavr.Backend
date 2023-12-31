﻿using System.Text.RegularExpressions;
using Anizavr.Backend.Application.Dtos;
using FluentValidation;

namespace Anizavr.Backend.Application.Validators;

public partial class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.Email)
            .Must(BeAValidEmail).WithMessage("Некорректный имейл")
            .NotEmpty().WithMessage("Пустой имейл");
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
    
    [GeneratedRegex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$")]
    private static partial Regex EmailRegex();
}