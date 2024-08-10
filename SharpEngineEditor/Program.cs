using SharpEngineCore.Components;
using SharpEngineCore.Exceptions;
using SharpEngineCore.Graphics;
using SharpEngineEditor.Core;

namespace SharpEngineEditor;

internal sealed class Program
{
    public static int Main(params string[] args)
    {
        var returnCode = 0;

        try
        {
            returnCode = new App(args).Run();
        }
        catch (GraphicsException e)
        {
            e.Show();
        }
        catch (SharpException e)
        {
            e.Show();
        }
        catch (Exception e)
        {
            var exception = new SharpException($"Something unexpected happened\n\n" +
                $"[Stack Trace]\n{e.StackTrace}\n\nError Code: {e.HResult}", e);
            exception.Show();
        }

        return returnCode;
    }
}