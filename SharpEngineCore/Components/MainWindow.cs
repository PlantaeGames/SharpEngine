﻿using System;
using System.Numerics;

using TerraFX.Interop.Windows;
using static TerraFX.Interop.Windows.Windows;

using SharpEngineCore.Graphics;
using SharpEngineCore.Utilities;
using System.Diagnostics;
using System.Xml.Linq;

namespace SharpEngineCore.Components;

public sealed class MainWindow : Window
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
    private float _anglePitch = 0f;

    //private float _lastX = 0;
    //private float _lastY = 0;
    //private float _lastZ = 0;

    private float _speed = 10;
    private float _deltaTime = 0.16f;

    private GraphicsObject _cube;
    private CameraObject _camera;
    private LightObject _light;
    private LightObject _light2;

    public void Update()
    {
        Graphics.Graphics.Render();

        var pitch = GetAsyncKeyState(VK.VK_F2);
        if (pitch < 0)
        {
            _anglePitch -= _deltaTime * _speed;
        }

        var pitch2 = GetAsyncKeyState(VK.VK_F1);
        if (pitch2 < 0)
        {
            _anglePitch += _deltaTime * _speed;
        }
        _light.Update(new()
        {
            Position = new(-5, 0, -10, 0),
            Rotation = new(0, 0, 0, 0),
            Scale = new(20, 20f, 1000, 1),
            AmbientColor = new(0.45f, 0.45f, 0.4f, 0.45f),
            Color = new(0, 0f, 1f, 1f),
            Intensity = new(1f, 128, 1, 0.25f),
            Attributes = _camera.Data.Attributes,
            LightType = Light.Point
        });
        _light2.Update(new()
        {
            Position = new(_anglePitch * _deltaTime * _speed, 0, -10, 0),
            Rotation = new(0, 0, 0, 0),
            Scale = new(20, 20f, 1000, 1),
            AmbientColor = new(0.45f, 0.45f, 0.4f, 0.45f),
            Color = new(1, 0f, 0f, 1f),
            Intensity = new(1f, 128, 1, 0.25f),
            Attributes = _camera.Data.Attributes,
            LightType = Light.Point
        });

        _camera.UpdateCamera(new CameraConstantData()
        {
            Position = new(-8, 0, -20, 0),
            Rotation = new(20, 0, 0, 0),
            Scale = new(20, 20, 1000, 1),
            Projection = Camera.Perspective,
            Attributes = new(_camera.Data.Attributes.r, 90, 0.03f, 1000f)
        });

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

    public MainWindow(string name, Point location, Size size, HWND parent) :
        base(name, location, size, parent)
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

        material.CullMode = TerraFX.Interop.DirectX.D3D11_CULL_MODE.D3D11_CULL_BACK;
        material.FillMode = TerraFX.Interop.DirectX.D3D11_FILL_MODE.D3D11_FILL_SOLID;

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
            Scale = new(20, 20, 1000, 1),
            Projection = Camera.Perspective,
            Attributes = new(viewport.AspectRatio, 90, 0.03f, 1000f)
        };

        var light = new LightConstantData()
        {
            Position = new(0, 0, 0, 0),
            Rotation = new(0, 0, 0, 0),
            Scale = new(20, 20, 1000, 1),
            AmbientColor = new(0.45f, 0.45f, 0.4f, 0.45f),
            Color = new(1f, 1f, 1f, 1f),
            Intensity = new(1f, 128, 1, 0.25f),
            Attributes =  camera.Attributes,
            LightType = Light.Directional
        };


        _cube = Graphics.Graphics.CreateGraphicsObject(new() 
        {
            material = material,
            mesh = cube 
        });
        _cube.UpdateTransform(new TransformConstantData()
        {
            Position = new(0, 0, -5, 0),
            Scale = new(1f, 1f, 1f, 1)
        });

        var cube2 = Graphics.Graphics.CreateGraphicsObject(new()
        {
            material = material,
            mesh = cube
        });

        cube2.UpdateTransform(new TransformConstantData()
        {
            Position = new(0, -2.5f, -7.5f, 0),
            Scale = new(1f, 1f, 1f, 1)
        });


        material.CullMode = TerraFX.Interop.DirectX.D3D11_CULL_MODE.D3D11_CULL_NONE;

        var planeObj = Graphics.Graphics.CreateGraphicsObject(new()
        {
            material = material, 
            mesh = plane
        });

        var planeObj2 = Graphics.Graphics.CreateGraphicsObject(new()
        {
            material = material,
            mesh = plane
        });

        planeObj.UpdateTransform(
            new TransformConstantData()
            {
                Position = new(0, 0, 0, 0),
                Rotation = new(0, 0, 0, 0),
                Scale = new(30, 10, 10, 1)
            });

        planeObj2.UpdateTransform(
            new TransformConstantData()
            {
                Position = new(0, -5, -5, 0),
                Rotation = new(0, 0, 90, 0),
                Scale = new(30, 10, 10, 1)
            });

        _light = Graphics.Graphics.CreateLightObject(new()
        { 
            data = light
        });
        _light2 = Graphics.Graphics.CreateLightObject(new()
        {
            data = light
        });
        _camera = Graphics.Graphics.CreateCameraObject(new()
        {
            cameraTransform = camera,
            viewport = viewport
        });
    }

    protected override LRESULT WndProc(HWND hWND, uint msg, WPARAM wParam, LPARAM lParam)
    {
        return base.WndProc(hWND, msg, wParam, lParam);
    }
}