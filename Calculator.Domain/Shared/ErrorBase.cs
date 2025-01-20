using FluentResults;

namespace Calculator.Domain.Shared;

public class ErrorBase : Error
{

    public int Error { get; }

    public ErrorBase(int error, string message)
    {
        Error = error;
        Message = message;
    }

}
