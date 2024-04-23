using TerraFX.Interop.Windows;

using SharpEngineCore.Graphics;

namespace SharpEngineCore.Components;

internal sealed class MainWindow : Window
{
    public MainWindow(string name, Point location, Size size) : base(name, location, size)
    { }

    protected override LRESULT WndProc(HWND hWND, uint msg, WPARAM wParam, LPARAM lParam)
    {
        return base.WndProc(hWND, msg, wParam, lParam);
    }
}
