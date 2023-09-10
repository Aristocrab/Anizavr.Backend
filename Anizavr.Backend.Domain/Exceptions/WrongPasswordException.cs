namespace Anizavr.Backend.Domain.Exceptions;

public class WrongPasswordException : Exception
{
    public WrongPasswordException() : base("Неправильный пароль")
    {
        
    }
}