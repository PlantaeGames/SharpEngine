using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal class MainWindow : Window
{
    public MainWindow(string name, Point position, Size size)
        : base(name, position, size)
    {
    }

    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);
    }
}
