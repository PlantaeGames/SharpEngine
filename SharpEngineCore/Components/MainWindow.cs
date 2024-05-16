using TerraFX.Interop.Windows;

using SharpEngineCore.Graphics;

namespace SharpEngineCore.Components;

internal sealed class MainWindow : Window
{
    private Logger _logger;

    public void Tick()
    {
        _logger.LogMessage("Tick");
    }

    public MainWindow(string name, Point location, Size size) : base(name, location, size)
    {
        _logger = new ();
    }

    protected override LRESULT WndProc(HWND hWND, uint msg, WPARAM wParam, LPARAM lParam)
    {
        return base.WndProc(hWND, msg, wParam, lParam);
    }
}
