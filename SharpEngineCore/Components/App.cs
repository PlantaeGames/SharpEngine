﻿using TerraFX.Interop.Windows;

using SharpEngineCore.Graphics;

namespace SharpEngineCore.Components;

internal sealed class App
{
    private Logger _logger;

    public App()
    { 
        _logger = new Logger();
    }

    public int Run()
    {
        var returnCode = 0;

        var window = new MainWindow("Sharp Engine", new Point(0,0), new Size(1024, 768));
        try
        {
            window.Show();

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

                _logger.LogMessage("Tick.");
            }
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