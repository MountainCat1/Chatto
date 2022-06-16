using System;

namespace ChattoAuth.Exceptions;

public class InvalidJwtException : Exception
{
    public InvalidJwtException()
    {
    }

    public InvalidJwtException(string? message) : base(message)
    {
    }
}