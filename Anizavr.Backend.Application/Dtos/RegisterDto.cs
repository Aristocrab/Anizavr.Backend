﻿namespace Anizavr.Backend.Application.Dtos;

public class RegisterDto
{
    public required string Username { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
}