namespace SharpEngineCore.Exceptions;

internal sealed class FailedToParseParamsException : SharpException
{
    public FailedToParseParamsException(string message, Exception inner) :
        base(message, inner)
    { }

    public FailedToParseParamsException(string message) : base(message)
    { }

    public FailedToParseParamsException()
        : base()
    { }
}
