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
    private readonly GameAssembly _assembly;

    private bool _initialized;
    private bool _isPlaying;

    public void Start()
    {
        _assembly.StartExecution();

        SceneManager.Start();
        SceneManager.Tick(TickType.Start);

        _isPlaying = true;
    }

    public void Stop()
    {
        // TODO: STOP TICK HERE OF SCENE MANAGER

        SceneManager.Stop();
        _assembly.StopExecution();

        _isPlaying = false;
    }

    public void Update()
    {
        if (_isPlaying)
        {
            SceneManager.Tick(TickType.Update);
        }

        SceneManager.Tick(TickType.OnPreRender);
        Graphics.Graphics.Render();
        SceneManager.Tick(TickType.OnPostRender);

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
        GameAssembly game,
        string name, Point location, Size size, HWND parent) :
        base(name, location, size, parent)
    {
        _assembly = game;

        _inputManager = new InputManager();

        var mouse = new Mouse(this);
        var keybaord = new Keyboard(this);

        _inputManager.AddDevice(mouse);
        _inputManager.AddDevice(keybaord);

        Graphics.Graphics.Initialize(this);
        Input.Input.Add(_inputManager);
        SceneManager.Initialize();

        _initialized = true;
    }

    public CameraObject InitializeSecondaryWindow(SecondaryWindow window, CameraInfo info)
    {
        var camera = Graphics.Graphics.InitializeSecondaryWindow(window, info);
        return camera;
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