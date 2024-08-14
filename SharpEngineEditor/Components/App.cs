using SharpEngineEditor.ImGui;
using System.Diagnostics;
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

        var window = new Components.MainWindow(
            NAME, new Point(0, 0), new Size(1024, 1024), new HWND());

        var guiRenderer = new ImGuiRenderer(window, new(1024, 1024));
        var guiPerFrameData = new ImGuiRenderer.PerFrameData();

        window.MessageListners += guiRenderer.ScanMessage;

        var counter = new Stopwatch();

        var deltaTime = 0.0f;

        try
        {
            window.Show();
            window.Start();

            // application loop
            while (true)
            {
                counter.Reset();
                counter.Start();

                guiPerFrameData.frameRate = deltaTime;

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

                guiRenderer.NewFrame();

                // other code here.
                window.Update();
                //          //

                ImGuiNET.ImGui.ShowDemoWindow();

                guiRenderer.EndFrame();
                guiRenderer.Render(guiPerFrameData);

                counter.Stop();
                deltaTime = counter.Elapsed.Microseconds / 1000000;
            }

            window.Stop();
        }
        catch
        {
            throw;
        }
        finally
        {
            window.MessageListners -= guiRenderer.ScanMessage;
            guiRenderer.Dispose();
            window.Destroy();
        }

        window.MessageListners -= guiRenderer.ScanMessage;
        guiRenderer.Dispose();
        window.Destroy();

        return returnCode;
    }
}