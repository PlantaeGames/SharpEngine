using SharpEngineCore.Input;
using System.Diagnostics;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

public sealed class SecondaryWindow : Window
{
    private CameraObject _camera;
    private Swapchain _swapchain;

    private bool _initialized;

    public SecondaryWindow(string name, Point location, Size size, HWND parent)
        : base(name, location, size, parent)
    {
    }

    internal void Initialize(Swapchain swapchain, CameraObject camera)
    {
        Debug.Assert(swapchain != null);
        Debug.Assert(camera != null);
        Debug.Assert(_initialized == false);

        _swapchain = swapchain;
        _camera = camera;
        _initialized = true;
    }

    public void Update()
    {
        Debug.Assert(_initialized);

        _swapchain.Present();
    }

    public override (bool availability, MSG msg) PeekAndDispatchMessage()
    {
        var result = base.PeekAndDispatchMessage();

        if (_initialized)
        {
            Input.Input.Feed(result.msg);
        }

        return result;
    }

    protected override LRESULT WndProc(HWND hWND, uint msg, WPARAM wParam, LPARAM lParam)
    {
        return base.WndProc(hWND, msg, wParam, lParam);
    }
}