using TerraFX.Interop.Windows;
using static TerraFX.Interop.Windows.Windows;

using SharpEngineCore.Graphics;
using SharpEngineCore.Utilities;
using System;
using System.Numerics;

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
    private float _angleY = 0;
    private float _anglePitch = 0f;

    private float _lastX = 0;
    private float _lastY = 0;
    private float _lastZ = 0;

    private float _speed = 10;
    private float _deltaTime = 0.016f;

    public void Update()
    {
        var pitch = GetAsyncKeyState(VK.VK_F2);
        if (pitch < 0)
        {
            _anglePitch -= _deltaTime * _speed * 5;
        }

        var pitch2 = GetAsyncKeyState(VK.VK_F1);
        if (pitch2 < 0)
        {
            _anglePitch += _deltaTime * _speed * 5;
        }

        var transform = _forwardPass
                             .PerspectiveCamera
                             .Transform;

        transform.Update(new TransformConstantData()
        {
            Position = new FColor4(_lastX, _lastY, _lastZ, 0),
            Rotation = new(_anglePitch, 0, 0, 0),
            Scale = new(1, 1, 1, 1),
            W = new(_forwardPass.PerspectiveCamera.AspectRatio, 70f, 0.1f, 1000f)
        });

        _renderer.Render();
        _swapchain.Present();

        var forwardDir = new Vector3();
        forwardDir.X = MathF.Cos(0) * MathF.Sin(_anglePitch * (float)(Math.PI / 180));
        forwardDir.Y = -MathF.Sin(0);
        forwardDir.Z = MathF.Cos(0) * MathF.Cos(_anglePitch * (float)(Math.PI / 180));
        forwardDir = Vector3.Normalize(forwardDir);

        var rightDir = new Vector3();
        rightDir.X = MathF.Cos(_anglePitch * (float)(Math.PI / 180));
        rightDir.Y = 0;
        rightDir.Z = -MathF.Sin(_anglePitch * (float)(Math.PI / 180));
        rightDir = Vector3.Normalize(rightDir);

        var upDir = Vector3.Cross(forwardDir, rightDir);
        upDir = Vector3.Normalize(upDir);

        _log.LogMessage($"{forwardDir}              Angle: {_anglePitch}");

        var newPos = new FColor4(_lastX, _lastY, _lastZ, 0);

        var up = GetAsyncKeyState(VK.VK_UP);
        if (up < 0)
        {
            _angle += 0.1f;

            newPos = new FColor4(_lastX + (forwardDir.X * _speed * _deltaTime),
               _lastY + (forwardDir.Y * _speed * _deltaTime),
               _lastZ + (forwardDir.Z * _speed * _deltaTime),
               0);
            newPos.a = 0;

            _lastX = newPos.r;
            _lastY = newPos.g;
            _lastZ = newPos.b;
        }

        var down = GetAsyncKeyState(VK.VK_DOWN);
        if (down < 0)
        {
            _angle -= 0.1f;

            newPos = new FColor4(_lastX - (forwardDir.X * _speed * _deltaTime),
               _lastY - (forwardDir.Y * _speed * _deltaTime),
               _lastZ - (forwardDir.Z * _speed * _deltaTime),
               0);
            newPos.a = 0;

            _lastX = newPos.r;
            _lastY = newPos.g;
            _lastZ = newPos.b;
        }

        var left = GetAsyncKeyState(VK.VK_LEFT);
        if (left < 0)
        {
            _angleX -= 0.1f;

            newPos = new FColor4(_lastX - (rightDir.X * _speed * _deltaTime),
               _lastY - (rightDir.Y * _speed * _deltaTime),
               _lastZ - (rightDir.Z * _speed * _deltaTime),
               0);
            newPos.a = 0;

            _lastX = newPos.r;
            _lastY = newPos.g;
            _lastZ = newPos.b;
        }

        var right = GetAsyncKeyState(VK.VK_RIGHT);
        if (right < 0)
        {
            _angleX += 0.1f;

            newPos = new FColor4(_lastX - (-rightDir.X * _speed * _deltaTime),
               _lastY - (-rightDir.Y * _speed * _deltaTime),
               _lastZ - (-rightDir.Z * _speed * _deltaTime),
               0);
            newPos.a = 0;

            _lastX = newPos.r;
            _lastY = newPos.g;
            _lastZ = newPos.b;
        }

        var y = GetAsyncKeyState(0x53);
        if (y < 0)
        {
            _angleY -= 0.1f;

            newPos = new FColor4(_lastX - (upDir.X * _speed * _deltaTime),
               _lastY - (upDir.Y * _speed * _deltaTime),
               _lastZ - (upDir.Z * _speed * _deltaTime),
               0);
            newPos.a = 0;

            _lastX = newPos.r;
            _lastY = newPos.g;
            _lastZ = newPos.b;
        }

        var y2 = GetAsyncKeyState(0x57);
        if (y2 < 0)
        {
            _angleY += 0.1f;

            newPos = new FColor4(_lastX + (upDir.X * _speed * _deltaTime),
               _lastY + (upDir.Y * _speed * _deltaTime),
               _lastZ + (upDir.Z * _speed * _deltaTime),
               0);
            newPos.a = 0;

            _lastX = newPos.r;
            _lastY = newPos.g;
            _lastZ = newPos.b;
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

        transform.Update(new TransformConstantData()
        {
            Position = newPos,
            Rotation = new(_anglePitch, 0, 0, 0),
            Scale = new(1, 1, 1, 1),
            W = new(_forwardPass.PerspectiveCamera.AspectRatio, 70f, 0.1f, 1000f)
        });

        _forwardPass
            .GraphicsObjects
            .Where(x => x.Id == _cubeId)
            .ToArray()[0]
            .TransformConstantBuffer
            .Update(new TransformConstantData()
            {
                Rotation = new(_angleZ * 3, 0, 0, 0),
                Scale = new(1,1,1,1)
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