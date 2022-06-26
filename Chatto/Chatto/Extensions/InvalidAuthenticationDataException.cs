namespace Chatto.Extensions;

public class InvalidAuthenticationDataException : Exception
{
    public InvalidAuthenticationDataException()
    {
    }

    public InvalidAuthenticationDataException(string message) : base(message)
    {
    }
}