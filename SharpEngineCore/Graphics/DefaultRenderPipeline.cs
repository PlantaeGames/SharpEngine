using System.Diagnostics;

using SharpEngineCore.Utilities;

using TerraFX.Interop.DirectX;
using TerraFX.Interop.WinRT;

namespace SharpEngineCore.Graphics;

internal sealed class DefaultRenderPipeline : RenderPipeline
{
    private const int MAX_LIGHTS_COUNT = 8;
    private const int MAX_CAMERAS_COUNT = 8;

    private Buffer _lightsSBuffer;
    private ShaderResourceView _lightsSRV;

    private ForwardRenderPass _forwardRenderPass;

    public DefaultRenderPipeline(Texture2D outputTexture) :
        base(outputTexture)
    { }

    public override CameraObject CreateCameraObject(
        Device device, CameraConstantData data, Viewport viewport)
    {
        Debug.Assert(_cameraObjects.Count <= MAX_LIGHTS_COUNT,
            "Cameras Limit Reached");

        var cameraObject = new CameraObject(data, viewport);
        _cameraObjects.Add(cameraObject);

        return cameraObject;
    }

    public override GraphicsObject CreateGraphicsObject(
        Device device, Material material, Mesh mesh)
    {
        var variations = 
            _forwardRenderPass.CreateVariations(device, material, mesh);

        var graphicsObject = new GraphicsObject();
        graphicsObject.AddVariations(variations);

        _graphicsObjects.Add(graphicsObject);
        return graphicsObject;
    }

    public override LightObject CreateLightObject(
        Device device, LightData data)
    {
        Debug.Assert(_lightObjects.Count <= MAX_LIGHTS_COUNT,
            "Lights Limit Reached");

        var lightObject = new LightObject(data);
        _lightObjects.Add(lightObject);

        return lightObject;
    }

    public override List<GraphicsObject> GetGraphicsObjects()
    {
        return _graphicsObjects;
    }

    public override void OnGo(Device device, DeviceContext context)
    { }

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
            _outputTexture, _lightsSRV, _lightObjects, MAX_LIGHTS_COUNT,
            _cameraObjects, _graphicsObjects);

        _renderPasses = [_forwardRenderPass];
    }

    public override void OnReady(Device device, DeviceContext context)
    {
        UpdateLightsBuffer();
    }

    private void UpdateLightsBuffer()
    {
        var lightsData = new List<LightData>();
        var firstIndexData = new LightData()
        {
            Position = new((float)_lightObjects.Count, 0, 0, 0)
        };
        lightsData.Add(firstIndexData);
        
        for(var i = 0; i < _lightObjects.Count; i++)
        {
            lightsData.Add(_lightObjects[i]._lastUpdatedData);
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