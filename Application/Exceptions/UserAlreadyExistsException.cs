namespace Application.Exceptions;

public class UserAlreadyExistsException : Exception
{
    public UserAlreadyExistsException(string parameterName, string parameterValue)
        : base($"User with {parameterName}={parameterValue} already exists")
    {
        
    }
}