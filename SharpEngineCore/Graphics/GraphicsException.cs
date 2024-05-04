using System.Runtime.CompilerServices;

using SharpEngineCore.Exceptions;
using SharpEngineCore.Utilities;

namespace SharpEngineCore.Graphics;

public class GraphicsException : SharpException
{

#nullable enable
    public static bool CheckIfAny()
    {
        return DXGIInfoQueue.GetInstance().IsMessageAvailable();
    }
#nullable disable

    public static void SetInfoQueue()
    {
        DXGIInfoQueue.GetInstance().Set();
    }

    public static GraphicsException GetLastGraphicsException(GraphicsException e)
    {
        return new GraphicsException(
            DXGIInfoQueue.GetInstance().GetMessages().ToSingleString(), e);
    }

    public GraphicsException([CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "", [CallerFilePath] string fileName = "") : base(lineNumber, memberName, fileName)
    {
    }

    public GraphicsException(string message, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "", [CallerFilePath] string fileName = "") : base(message, lineNumber, memberName, fileName)
    {
    }

    public GraphicsException(string message, Exception inner, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "", [CallerFilePath] string fileName = "") : base(message, inner, lineNumber, memberName, fileName)
    {
    }
}

