namespace ChattoAuth.Exceptions;

public class InvalidAuthenticationDataException : Exception
{
    public InvalidAuthenticationDataException(string message) : base(message)
    {
    }

    public InvalidAuthenticationDataException()
    {
    }
}