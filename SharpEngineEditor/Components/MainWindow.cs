using TerraFX.Interop.Windows;
using SharpEngineEditor.ImGui.Backend;

namespace SharpEngineEditor.Components;

internal sealed class MainWindow : Window
{
    public delegate void MessageHook(MSG msg);
    public event MessageHook MessageListners;

    public MainWindow(
        string name, Point location, Size size, HWND parent) : 
        base(name, location, size, parent)
    {}

    public void Start()
    {}
    public void Update()
    {}
    public void Stop()
    { }

    protected override LRESULT WndProc(HWND hWND, uint msg, WPARAM wParam, LPARAM lParam)
    {
        MessageListners?.Invoke(new MSG()
        {
            hwnd = hWND,
            lParam = new LPARAM(lParam),
            wParam = new WPARAM(wParam),
            message = msg
        });

        return base.WndProc(hWND, msg, wParam, lParam);
    }
}
