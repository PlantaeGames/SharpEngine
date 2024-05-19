using System.Diagnostics;

using SharpEngineCore.Utilities;

using TerraFX.Interop.DirectX;
using TerraFX.Interop.WinRT;

namespace SharpEngineCore.Graphics;

internal sealed class DefaultRenderPipeline : RenderPipeline
{
    private const int MAX_PER_VARIATION_LIGHTS_COUNT = 8;

    private ForwardRenderPass _forwardRenderPass;

    public DefaultRenderPipeline(Texture2D outputTexture) :
        base(outputTexture)
    { }

    public override CameraObject CreateCameraObject(
        Device device, CameraConstantData data, Viewport viewport)
    {
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
        _forwardRenderPass = new ForwardRenderPass(
            _outputTexture, _lightObjects, MAX_PER_VARIATION_LIGHTS_COUNT,
            _cameraObjects);

        _renderPasses = [_forwardRenderPass];
    }

    public override void OnReady(Device device, DeviceContext context)
    {}
}