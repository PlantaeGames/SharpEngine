using SharpEngineCore.Exceptions;

namespace SharpEngineBuildSystem.Exceptions;

public sealed class FailedToGenerateCode : SharpException
{
    public FailedToGenerateCode()
    {
    }

    public FailedToGenerateCode(string message) : base(message)
    {
    }

    public FailedToGenerateCode(string message, Exception inner) : base(message, inner)
    {
    }
}
