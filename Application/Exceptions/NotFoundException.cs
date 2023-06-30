namespace Application.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string parameterName, string parameterValue)
        : base($"Entity with {parameterName}={parameterValue} was not found")
    {
        
    }
}