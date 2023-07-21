namespace Application.Exceptions;

public class UserAlreadyExistsException : Exception
{
    public UserAlreadyExistsException(string parameterName, string parameterValue)
        : base($"Пользователь с {parameterName}={parameterValue} уже существует")
    {
        
    }
}