namespace ChattoGuid.Exceptions;

public class InvalidJwtException : Exception
{
    public InvalidJwtException()
    {
    }

    public InvalidJwtException(string? message) : base(message)
    {
    }
}