namespace Anizavr.Backend.Application.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string entityName, string parameterName, string parameterValue)
        : base($"{entityName} с {parameterName}={parameterValue} не сущевствует")
    {
        
    }
}