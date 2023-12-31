﻿namespace Anizavr.Backend.Domain.Exceptions;

public class UnauthorizedException : Exception
{
    public UnauthorizedException(Guid userId, string entityName, string entityValue) 
        : base($"Пользователь {userId} не имеет доступа к {entityName}={entityValue}") { }
}