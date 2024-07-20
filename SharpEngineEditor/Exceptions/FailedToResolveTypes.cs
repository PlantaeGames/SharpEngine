
namespace SharpEngineEditor.Exceptions;

public sealed class FailedToResolveTypes : SharpEngineEditorException
{
    public FailedToResolveTypes()
    {
    }

    public FailedToResolveTypes(string message) : base(message)
    {
    }

    public FailedToResolveTypes(string message, Exception innerException) : base(message, innerException)
    {
    }
}
