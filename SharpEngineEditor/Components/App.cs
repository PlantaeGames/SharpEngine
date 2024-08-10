using SharpEngineCore.Components;
using SharpEngineCore.Tests;
using SharpEngineCore.Utilities;
using SharpEngineEditor.Components;
using TerraFX.Interop.Windows;

namespace SharpEngineEditor.Core;

internal sealed class App
{
    private const string NAME = "Sharp Engine Editor";

    sealed class StartupParams
    {
        public string GameAssembly { get; private set; } = string.Empty;

        public static StartupParams Parse(string[] arguments)
        {
            var @params = new StartupParams();

            for (var i = 0; i < arguments.Length; i++)
            {
                if (string.Compare(arguments[i], "/g") == 0 ||
                    string.Compare(arguments[i], "--g") == 0)
                {
                    @params.GameAssembly = arguments[i + 1];
                }
            }

            if (@params.GameAssembly == string.Empty)
            {
                @params.GameAssembly = "SharpEngineGame.dll";
            }

            return @params;
        }

        private StartupParams()
        { }
    }

    private readonly StartupParams _params;
    private GameAssembly _gameAssembly;

    public App(string[] arguments)
    {
        _params = StartupParams.Parse(arguments);
        _gameAssembly = new GameAssembly(_params.GameAssembly);
    }

    public int Run()
    {
        var returnCode = 0;

        var window = new EditorWindow(
            NAME, new Point(0, 0), new Size(1024, 768), new HWND());
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