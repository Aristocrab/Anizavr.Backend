﻿namespace Anizavr.Backend.Application.Dtos;

public class LoginDto
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}