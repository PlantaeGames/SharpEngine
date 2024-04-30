using System.Runtime.CompilerServices;

using SharpEngineCore.Exceptions;
using SharpEngineCore.Utilities;

namespace SharpEngineCore.Graphics;

public class GraphicsSharpException : SharpException
{
    public static void SetInfoQueue()
    {
        DXGIInfoQueue.GetInstance().Set();
    }

    public static GraphicsSharpException GetLastGraphicsException(GraphicsSharpException e)
    {
        return new GraphicsSharpException(
            DXGIInfoQueue.GetInstance().GetMessages().ToSingleString(), e);
    }

    public GraphicsSharpException([CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "", [CallerFilePath] string fileName = "") : base(lineNumber, memberName, fileName)
    {
    }

    public GraphicsSharpException(string message, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "", [CallerFilePath] string fileName = "") : base(message, lineNumber, memberName, fileName)
    {
    }

    public GraphicsSharpException(string message, Exception inner, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "", [CallerFilePath] string fileName = "") : base(message, inner, lineNumber, memberName, fileName)
    {
    }
}

