using SharpEngineCore.Utilities;
using System.Diagnostics;

using TerraFX.Interop.DirectX;

namespace SharpEngineCore.Graphics;

internal sealed class DefaultRenderPipeline : RenderPipeline
{
    private const int MAX_LIGHTS_COUNT = 8;
    private const int MAX_CAMERAS_COUNT = 8;

    private readonly Texture2D _outputTexture;

    private readonly List<(ConstantBuffer dataBuffer, CameraObject camera)> _cameras = new();
    private readonly List<LightObject> _lights = new();

    private Buffer _lightsSBuffer;
    private ShaderResourceView _lightsSRV;

    private ForwardRenderPass _forwardRenderPass;

    public DefaultRenderPipeline(Texture2D outputTexture) :
        base()
    {
        _outputTexture = outputTexture;
    }

    public override CameraObject AddCamera(CameraConstantData data, Viewport viewport)
    {
        Debug.Assert(_cameras.Count <= MAX_CAMERAS_COUNT,
            "Max cameras limit reached." +
            "Can't add more cameras.");

        var cameraObject = new CameraObject(data, viewport);
        _cameras.Add((null, cameraObject));

        return cameraObject;
    }

    public override LightObject AddLight(LightData data)
    {
        Debug.Assert(_cameras.Count <= MAX_LIGHTS_COUNT,
            "Max Lights Limit Reached." +
            "Can't add more lights.");

        var lightObject = new LightObject(data, _lights.Count);
        _lights.Add(lightObject);

        return lightObject;
    }

    public override Guid CreateGraphicsObject(Material material, Mesh mesh)
    {
        return _forwardRenderPass.CreateNewGraphicsObject(material, mesh);
    }

    public override List<GraphicsObject> GetGraphicsObjects()
    {
        return _forwardRenderPass.GetGraphicsObjects();
    }

    public override void OnGo(Device device, DeviceContext context)
    {
    }

    public override void OnInitialize(Device device, DeviceContext context)
    {
        var length = new LightData().GetFragmentsCount() * (MAX_LIGHTS_COUNT + 1);
        _lightsSBuffer = device.CreateBuffer(
            new FSurface(new(length, 1)), typeof(LightData),
            new ResourceUsageInfo()
            {
                Usage = D3D11_USAGE.D3D11_USAGE_DYNAMIC,
                BindFlags = D3D11_BIND_FLAG.D3D11_BIND_SHADER_RESOURCE,
                CPUAccessFlags = D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE
            });
        _lightsSRV = device.CreateShaderResourceView(_lightsSBuffer,
            new ViewCreationInfo()
            {
                Format = DXGI_FORMAT.DXGI_FORMAT_UNKNOWN,
                Size = _lightsSBuffer.Info.Size
            });

        _forwardRenderPass = new ForwardRenderPass(
            _outputTexture, _lightsSRV, _cameras);

        _renderPasses = [_forwardRenderPass];
    }

    public override void OnReady(Device device, DeviceContext context)
    {
        UpdateLightsBuffer();
        UpdateCameraBuffers(device);
    }

    private void UpdateCameraBuffers(Device device)
    {
        // removing expired
        var toRemove = new List<int>();
        for(var i = 0; i < _cameras.Count; i++)
        {
            if (_cameras[i].camera.State == State.Expired)
                toRemove.Add(i);
        }
        for(var i = 0; i < toRemove.Count; i++)
        {
            _cameras.Remove(_cameras[i]);
        }

        // normalizing cameras and data buffers
        for(var i = 0; i < _cameras.Count; i++)
        {
            if (_cameras[i].dataBuffer == null)
            {
                var buffer = Buffer.CreateConstantBuffer(
                    device.CreateBuffer(_cameras[i].camera._lastUpdatedData.ToSurface(),
                    typeof(CameraConstantData),
                    new ResourceUsageInfo()
                    {
                        Usage = D3D11_USAGE.D3D11_USAGE_DYNAMIC,
                        CPUAccessFlags = D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE,
                        BindFlags = D3D11_BIND_FLAG.D3D11_BIND_CONSTANT_BUFFER
                    }));
            }
        }

        // updating buffers
        for(var i = 0; i < _cameras.Count; i++)
        {
            if (_cameras[i].camera.State == State.Paused)
                continue;

            _cameras[i].dataBuffer.Update(
                _cameras[i].camera._lastUpdatedData.ToSurface());
        }
    }

    private void UpdateLightsBuffer()
    {
        // removing expired
        var toRemove = new List<int>();
        for(var i = 0; i < _lights.Count; i++)
        {
            if (_lights[i].State == State.Expired)
                toRemove.Add(i);
        }
        for(var i = 0; i < toRemove.Count; i++)
        {
            _lights.Remove(_lights[i]);
        }

        // getting currently active indices
        var actives = new List<int>();
        for(var i = 0; i < _lights.Count; i++)
        {
            if (_lights[i].State == State.Paused)
                continue;

            actives.Add(i);
        }

        // updating buffer from actives
        var lightsData = new List<LightData>();
        var firstIndexData = new LightData()
        {
            Position = new((float)actives.Count, 0, 0, 0)
        };
        lightsData.Add(firstIndexData);
        
        for(var i = 0; i < actives.Count; i++)
        {
            lightsData.Add(_lights[actives[i]]._lastUpdatedData);
        }

        var length = _lightsSBuffer.Info.Size.ToArea();
        var surface = new FSurface(new(length, 1));

        var fragments = new List<Fragment>();
        for(var i = 0; i < lightsData.Count; i++)
        {
            fragments.AddRange(lightsData[i].ToFragments());
        }

        surface.SetLinearFragments(fragments.ToArray());

        _lightsSBuffer.Update(surface);
    }
}