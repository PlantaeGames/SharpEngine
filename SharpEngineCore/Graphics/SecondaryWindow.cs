using SharpEngineCore.Input;
using System.Diagnostics;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

public sealed class SecondaryWindow : Window
{
    private CameraObject _camera;
    private Swapchain _swapchain;

    private InputManager _inputManager;

    private bool _initialized;

    public SecondaryWindow(string name, Point location, Size size, HWND parent)
        : base(name, location, size, parent)
    {
        _inputManager = new InputManager();

        var mouse = new Mouse(this);
        var keybaord = new Keyboard(this);

        _inputManager.AddDevice(mouse);
        _inputManager.AddDevice(keybaord);

        //Input.Input.Add(_inputManager);
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

        foreach(var device in _inputManager)
        {
            device.Clear();
        }
    }

    protected override LRESULT WndProc(HWND hWND, uint msg, WPARAM wParam, LPARAM lParam)
    {
        if (_initialized)
        {
            _inputManager.Feed(new MSG()
            {
                hwnd = hWND,
                message = msg,
                wParam = wParam,
                lParam = lParam
            });
        }

        return base.WndProc(hWND, msg, wParam, lParam);
    }
}