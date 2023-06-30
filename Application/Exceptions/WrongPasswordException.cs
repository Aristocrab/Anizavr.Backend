namespace Application.Exceptions;

public class WrongPasswordException : Exception
{
    public WrongPasswordException(string username)
        : base($"Password for user {username} is wrong")
    {
        
    }
}