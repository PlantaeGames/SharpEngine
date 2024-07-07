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

    public void Start()
    {
        _game.StartExecution();
    }

    public void Stop()
    {
        _game.StopExecution();
    }

    public void Update()
    {
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
    }

    public override (bool availability, MSG msg) PeekAndDispatchMessage()
    {
        var result = base.PeekAndDispatchMessage();

        _inputManager.Feed(result.msg);

        return result;
    }

    protected override LRESULT WndProc(HWND hWND, uint msg, WPARAM wParam, LPARAM lParam)
    {
        return base.WndProc(hWND, msg, wParam, lParam);
    }
}