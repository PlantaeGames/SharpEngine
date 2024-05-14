using TerraFX.Interop.Windows;
using static TerraFX.Interop.Windows.Windows;

using SharpEngineCore.Graphics;
using SharpEngineCore.Utilities;

namespace SharpEngineCore.Components;

internal sealed class MainWindow : Window
{
    private Logger _log;
    private Renderer _renderer;
    private Swapchain _swapchain;

    private ForwardPass _forwardPass;
    private Guid _cubeId;

    private float _angle = 0f;
    private float _angleX = 0f;
    private float _angleZ = 0;
    private float _angleYaw = 0f;

    public void Update()
    {
        var up = GetAsyncKeyState(VK.VK_UP);
        if(up < 0)
        {
            _angle += 0.1f;
        }

        var down = GetAsyncKeyState(VK.VK_DOWN);
        if (down < 0)
        {
            _angle -= 0.1f;
        }

        var left = GetAsyncKeyState(VK.VK_LEFT);
        if (left < 0)
        {
            _angleX -= 0.1f;
        }

        var right = GetAsyncKeyState(VK.VK_RIGHT);
        if (right < 0)
        {
            _angleX += 0.1f;
        }

        var forward = GetAsyncKeyState(VK.VK_LBUTTON);
        if (forward < 0)
        {
            _angleZ -= 0.1f;
        }

        var backward = GetAsyncKeyState(VK.VK_RBUTTON);
        if (backward < 0)
        {
            _angleZ += 0.1f;
        }

        var yaw2 = GetAsyncKeyState(VK.VK_F2);
        if (yaw2 < 0)
        {
            _angleYaw -= 0.1f;
        }

        var yaw = GetAsyncKeyState(VK.VK_F1);
        if (yaw < 0)
        {
            _angleYaw += 0.1f;
        }

        _renderer.Render();
        _swapchain.Present();

        _forwardPass.GraphicsObjects
            .Where(x => x.Id == _cubeId)
            .ToArray()[0]
            .TransformConstantBuffer
            .Update(new TransformConstantData()
            {
                Position = new(_angleX, _angleYaw, _angle, 0f),
                Rotation = new(_angleZ * 3, 0, 0, 0),
                Scale = new(1, 1, 1, 1)
            });
    }

    private void Initialize()
    {
        _log.LogHeader("Creating Renderer.");

        var factory = Factory.GetInstance();
        var adapter = factory.GetAdpters()[0];

        _renderer = new Renderer(adapter);

        _swapchain = _renderer.CreateSwapchain(this);
        var backTexture = _swapchain.GetBackTexture();

        var pipeline = new DefaultRenderPipeline(backTexture);

        _renderer.SetPipeline(pipeline);

        _forwardPass = pipeline
                .Get<ForwardRenderPass>()
                .Get<ForwardPass>();

        var mesh = new ObjLoader().Load("Modals\\cube.obj");
        _cubeId = _forwardPass.AddSubVariation(new(Mesh.Cube()));
        

        _log.LogMessage("Renderer Created.");
        _log.BreakLine();
        _log.LogMessage("Presenting...");
    }

    public MainWindow(string name, Point location, Size size) : base(name, location, size)
    {
        _log = new();
        Initialize();
    }

    protected override LRESULT WndProc(HWND hWND, uint msg, WPARAM wParam, LPARAM lParam)
    {
        return base.WndProc(hWND, msg, wParam, lParam);
    }
}