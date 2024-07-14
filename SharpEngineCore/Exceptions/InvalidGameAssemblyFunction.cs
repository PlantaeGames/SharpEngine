namespace SharpEngineCore.Exceptions;

public sealed class InvalidGameAssemblyFunction :
    SharpException
{
    public InvalidGameAssemblyFunction(string message, Exception inner) :
        base(message, inner)
    { }
}