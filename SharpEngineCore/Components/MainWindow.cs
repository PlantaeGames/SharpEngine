using System;
using System.Numerics;

using TerraFX.Interop.Windows;
using static TerraFX.Interop.Windows.Windows;

using SharpEngineCore.Graphics;
using SharpEngineCore.Utilities;

namespace SharpEngineCore.Components;

internal sealed class MainWindow : Window
{
    private Logger _log;
    //private Renderer _renderer;
    //private Swapchain _swapchain;

    //private ForwardPass _forwardPass;
    //private Guid _cubeId;

    //private float _angle = 0f;
    //private float _angleX = 0f;
    //private float _angleZ = 0;
    //private float _angleY = 0;
    //private float _anglePitch = 0f;

    //private float _lastX = 0;
    //private float _lastY = 0;
    //private float _lastZ = 0;

    //private float _speed = 10;
    //private float _deltaTime = 0.016f;

    public void Update()
    {
        Graphics.Graphics.Render();

        //var pitch = GetAsyncKeyState(VK.VK_F2);
        //if (pitch < 0)
        //{
        //    _anglePitch -= _deltaTime * _speed * 5;
        //}

        //var pitch2 = GetAsyncKeyState(VK.VK_F1);
        //if (pitch2 < 0)
        //{
        //    _anglePitch += _deltaTime * _speed * 5;
        //}

        //var transform = _forwardPass
        //                     .PerspectiveCamera
        //                     .Transform;

        //transform.Update(new TransformConstantData()
        //{
        //    Position = new FColor4(_lastX, _lastY, _lastZ, 0),
        //    Rotation = new(_anglePitch, 0, 0, 0),
        //    Scale = new(1, 1, 1, 1),
        //    W = new(_forwardPass.PerspectiveCamera.AspectRatio, 70f, 0.1f, 1000f)
        //});

        //_renderer.Render();
        //_swapchain.Present();

        //var forwardDir = new Vector3();
        //forwardDir.X = MathF.Cos(0) * MathF.Sin(_anglePitch * (float)(Math.PI / 180));
        //forwardDir.Y = -MathF.Sin(0);
        //forwardDir.Z = MathF.Cos(0) * MathF.Cos(_anglePitch * (float)(Math.PI / 180));
        //forwardDir = Vector3.Normalize(forwardDir);

        //var rightDir = new Vector3();
        //rightDir.X = MathF.Cos(_anglePitch * (float)(Math.PI / 180));
        //rightDir.Y = 0;
        //rightDir.Z = -MathF.Sin(_anglePitch * (float)(Math.PI / 180));
        //rightDir = Vector3.Normalize(rightDir);

        //var upDir = Vector3.Cross(forwardDir, rightDir);
        //upDir = Vector3.Normalize(upDir);

        //_log.LogMessage($"{forwardDir}              Angle: {_anglePitch}");

        //var newPos = new FColor4(_lastX, _lastY, _lastZ, 0);

        //var up = GetAsyncKeyState(VK.VK_UP);
        //if (up < 0)
        //{
        //    _angle += 0.1f;

        //    newPos = new FColor4(_lastX + (forwardDir.X * _speed * _deltaTime),
        //       _lastY + (forwardDir.Y * _speed * _deltaTime),
        //       _lastZ + (forwardDir.Z * _speed * _deltaTime),
        //       0);
        //    newPos.a = 0;

        //    _lastX = newPos.r;
        //    _lastY = newPos.g;
        //    _lastZ = newPos.b;
        //}

        //var down = GetAsyncKeyState(VK.VK_DOWN);
        //if (down < 0)
        //{
        //    _angle -= 0.1f;

        //    newPos = new FColor4(_lastX - (forwardDir.X * _speed * _deltaTime),
        //       _lastY - (forwardDir.Y * _speed * _deltaTime),
        //       _lastZ - (forwardDir.Z * _speed * _deltaTime),
        //       0);
        //    newPos.a = 0;

        //    _lastX = newPos.r;
        //    _lastY = newPos.g;
        //    _lastZ = newPos.b;
        //}

        //var left = GetAsyncKeyState(VK.VK_LEFT);
        //if (left < 0)
        //{
        //    _angleX -= 0.1f;

        //    newPos = new FColor4(_lastX - (rightDir.X * _speed * _deltaTime),
        //       _lastY - (rightDir.Y * _speed * _deltaTime),
        //       _lastZ - (rightDir.Z * _speed * _deltaTime),
        //       0);
        //    newPos.a = 0;

        //    _lastX = newPos.r;
        //    _lastY = newPos.g;
        //    _lastZ = newPos.b;
        //}

        //var right = GetAsyncKeyState(VK.VK_RIGHT);
        //if (right < 0)
        //{
        //    _angleX += 0.1f;

        //    newPos = new FColor4(_lastX - (-rightDir.X * _speed * _deltaTime),
        //       _lastY - (-rightDir.Y * _speed * _deltaTime),
        //       _lastZ - (-rightDir.Z * _speed * _deltaTime),
        //       0);
        //    newPos.a = 0;

        //    _lastX = newPos.r;
        //    _lastY = newPos.g;
        //    _lastZ = newPos.b;
        //}

        //var y = GetAsyncKeyState(0x53);
        //if (y < 0)
        //{
        //    _angleY -= 0.1f;

        //    newPos = new FColor4(_lastX - (upDir.X * _speed * _deltaTime),
        //       _lastY - (upDir.Y * _speed * _deltaTime),
        //       _lastZ - (upDir.Z * _speed * _deltaTime),
        //       0);
        //    newPos.a = 0;

        //    _lastX = newPos.r;
        //    _lastY = newPos.g;
        //    _lastZ = newPos.b;
        //}

        //var y2 = GetAsyncKeyState(0x57);
        //if (y2 < 0)
        //{
        //    _angleY += 0.1f;

        //    newPos = new FColor4(_lastX + (upDir.X * _speed * _deltaTime),
        //       _lastY + (upDir.Y * _speed * _deltaTime),
        //       _lastZ + (upDir.Z * _speed * _deltaTime),
        //       0);
        //    newPos.a = 0;

        //    _lastX = newPos.r;
        //    _lastY = newPos.g;
        //    _lastZ = newPos.b;
        //}

        //var forward = GetAsyncKeyState(VK.VK_LBUTTON);
        //if (forward < 0)
        //{
        //    _angleZ -= 0.1f;
        //}

        //var backward = GetAsyncKeyState(VK.VK_RBUTTON);
        //if (backward < 0)
        //{
        //    _angleZ += 0.1f;
        //}

        //transform.Update(new TransformConstantData()
        //{
        //    Position = newPos,
        //    Rotation = new(_anglePitch, 0, 0, 0),
        //    Scale = new(1, 1, 1, 1),
        //    W = new(_forwardPass.PerspectiveCamera.AspectRatio, 70f, 0.1f, 1000f)
        //});

        //_forwardPass
        //    .GraphicsObjects
        //    .Where(x => x.Id == _cubeId)
        //    .ToArray()[0]
        //    .TransformConstantBuffer
        //    .Update(new TransformConstantData()
        //    {
        //        Rotation = new(_angleZ * 3, 0, 0, 0),
        //        Scale = new(1,1,1,1)
        //    });
    }

    public MainWindow(string name, Point location, Size size) : base(name, location, size)
    {
        _log = new();
        Graphics.Graphics.Initialize(this);
        _log.LogMessage("Initialized.");

        var material = new Material();

        var vertex = new ShaderModule("Shaders\\VertexShader.hlsl");
        var pixel = new ShaderModule("Shaders\\PixelShader.hlsl");

        material.Topology = Topology.TriangleList;
        material.VertexShader = vertex;
        material.PixelShader = pixel;

        var cube = Mesh.Cube();
        var plane = Mesh.Plane();
        var triangle = Mesh.Triangle();

        var viewport = new Viewport()
        {
            Info = new TerraFX.Interop.DirectX.D3D11_VIEWPORT()
            {
                Height = GetSize().Height,
                Width = GetSize().Width,
                MaxDepth = 1f,
            }
        };

        var camera = new CameraConstantData()
        {
            Position = new(-3, 0, 0, 0),
            Rotation = new(20, 0, 0, 0),
            Scale = new(1, 1, 1, 1),
            Attributes = new(viewport.AspectRatio, 70, 0.03f, 1000f)
        };

        var light = new LightData()
        {
            Position = new(0, 0, 0, 0),
            Rotation = new(0, 0, 0, 0),
            Scale = new(1, 1, 1, 1),
            AmbientColor = new(0.25f, 0.25f, 0.25f, 0.25f),
            Color = new(1, 1, 1, 1),
            Intensity = new(1, 32, 1, 0.25f),
            Attributes =  camera.Attributes
        };


        var cubeObj = Graphics.Graphics.CreateGraphicsObject(material, cube);
        cubeObj.UpdateTransform(new TransformConstantData()
        {
            Position = new(0, 0, 5, 0),
            Scale = new(1, 1, 1, 1)
        });

        var planeObj = Graphics.Graphics.CreateGraphicsObject(material, plane);

        planeObj.UpdateTransform(
            new TransformConstantData()
            {
                Position = new(0, 0, 10, 0),
                Rotation = new(0, 0, 180, 0),
                Scale = new(10, 10, 10, 1)
            });

        var triangleObj = Graphics.Graphics.CreateGraphicsObject(material, triangle);

        triangleObj.UpdateTransform(
            new TransformConstantData()
            {
                Position = new(2, 0, 5, 0),
                Rotation = new(0, 0, 0, 0),
                Scale = new(0.3f, 0.3f, 0.3f, 1)
            });

        var lightObj = Graphics.Graphics.CreateLightObject(light);
        var cameraObj = Graphics.Graphics.CreateCameraObject(camera, viewport);
    }

    protected override LRESULT WndProc(HWND hWND, uint msg, WPARAM wParam, LPARAM lParam)
    {
        return base.WndProc(hWND, msg, wParam, lParam);
    }
}