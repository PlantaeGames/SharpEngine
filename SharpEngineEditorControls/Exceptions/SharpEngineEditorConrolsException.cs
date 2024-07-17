using System;

namespace SharpEngineEditorControls.Exceptions;

public class SharpEngineEditorConrolsException : Exception
{
    public SharpEngineEditorConrolsException()
    {
    }

    public SharpEngineEditorConrolsException(string message) : base(message)
    {
    }

    public SharpEngineEditorConrolsException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
