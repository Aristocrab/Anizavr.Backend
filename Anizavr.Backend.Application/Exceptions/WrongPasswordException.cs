namespace Anizavr.Backend.Application.Exceptions;

public class WrongPasswordException : Exception
{
    public WrongPasswordException(string username)
        : base($"Неправильный пароль")
    {
        
    }
}