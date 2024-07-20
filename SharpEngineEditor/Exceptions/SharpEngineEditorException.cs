using System.Runtime.Serialization;

namespace SharpEngineEditor.Exceptions;

public class SharpEngineEditorException : Exception
{
    public SharpEngineEditorException()
    {
    }

    public SharpEngineEditorException(string message) : base(message)
    {
    }

    public SharpEngineEditorException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
