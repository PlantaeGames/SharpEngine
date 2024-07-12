using System;
using System.Numerics;

using TerraFX.Interop.Windows;
using static TerraFX.Interop.Windows.Windows;

using SharpEngineCore.Graphics;
using SharpEngineCore.Utilities;
using System.Diagnostics;
using System.Xml.Linq;
using SharpEngineCore.Input;
using SharpEngineCore.ECS;

namespace SharpEngineCore.Components;

public sealed class MainWindow : Window
{
    private readonly InputManager _inputManager;
    private readonly Game _game;

    private bool _initialized;

    public void Start()
    {
        _game.StartExecution();

        SceneManager.Tick(TickType.Start);
    }

    public void Stop()
    {
        // TODO: STOP TICK HERE OF SCENE MANAGER
        

        _game.StopExecution();
    }

    public void Update()
    {
        SceneManager.Tick(TickType.Update);
        Graphics.Graphics.Render();

        ClearInputSinks();

        void ClearInputSinks()
        {
            foreach (var device in _inputManager)
            {
                device.Clear();
            }
        }
    }

    public MainWindow(
        Game game,
        string name, Point location, Size size, HWND parent) :
        base(name, location, size, parent)
    {
        _game = game;

        _inputManager = new InputManager();

        var mouse = new Mouse(this);
        var keybaord = new Keyboard(this);

        _inputManager.AddDevice(mouse);
        _inputManager.AddDevice(keybaord);

        Graphics.Graphics.Initialize(this);
        Input.Input.Initialize(_inputManager);
        SceneManager.Initialize();

        _initialized = true;
    }

    public override (bool availability, MSG msg) PeekAndDispatchMessage()
    {
        var result = base.PeekAndDispatchMessage();

        if (_initialized)
        {
            _inputManager.Feed(result.msg);
        }

        return result;
    }

    protected override LRESULT WndProc(HWND hWND, uint msg, WPARAM wParam, LPARAM lParam)
    {
        return base.WndProc(hWND, msg, wParam, lParam);
    }
}