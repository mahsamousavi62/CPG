using System;

namespace CPG.Application.UseCases.Exceptions;

public abstract class AppException : Exception
{
    public abstract string Code { get; }

    protected AppException(string message) : base(message)
    {
    }
}
