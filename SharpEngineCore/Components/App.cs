using SharpEngineCore.Tests;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Components;

internal sealed class App
{
    public App(string arguments)
    { }

    public int Run()
    {
        var returnCode = 0;
        
        // TODO: PARSE ARGUMENTS FROM HERE AND CREATE GAME.
        var game = new Game();

        var window = new MainWindow("Sharp Engine", new Point(0,0), new Size(1024, 768), new HWND());
        try
        {
            window.Show();
            window.Start();

            // application loop
            while (true)
            {
                bool stop = false;
                // message loop
                while (true)
                {
                    var result = window.PeekAndDispatchMessage();

                    if (result.availability == false)
                        break;
                    
                    if (result.msg.message == WM.WM_QUIT)
                    {
                        stop = true;
                        break;
                    }
                }

                if (stop)
                    break;

                // other code here.
                window.Update();
                //          //
            }

            window.Stop();
        }
        catch
        {
            throw;
        }
        finally
        {
            window.Destroy();
        }

        return returnCode;
    }
}