using SharpEngineCore.Components;
using SharpEngineCore.Exceptions;

namespace SharpEngineCore;
internal sealed class Program
{
    public static int Main()
    {
        var returnCode = 0;

        try
        {
            returnCode = new App().Run();
        }
        catch (SharpException e)
        {
            e.Show();
        }
        catch (Exception e)
        {
            var exception = new SharpException($"Something unexpected happened\n\n[Stack Trace]\n{e.StackTrace}", e);
            exception.Show();
        }

        return returnCode;
    }
}