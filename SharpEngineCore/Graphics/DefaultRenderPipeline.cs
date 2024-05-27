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

    public override void OnCameraAdd(CameraObject camera, Device device)
    {}

    public override void OnCameraPause(CameraObject camera, Device device)
    {}

    public override void OnCameraRemove(CameraObject camera, Device device)
    {}

    public override void OnCameraResume(CameraObject camera, Device device)
    {}

    public override void OnGo(Device device, DeviceContext context)
    { }

    public override void OnGraphicsAdd(GraphicsObject graphics, Device device)
    {
        var transformBuffer = Buffer.CreateConstantBuffer(device.CreateBuffer(
                                 new TransformConstantData().ToSurface(),
                                 typeof(TransformConstantData),
                                 new ResourceUsageInfo()
                                 {       
                                     Usage = D3D11_USAGE.D3D11_USAGE_DYNAMIC,
                                     CPUAccessFlags = D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE,
                                     BindFlags = D3D11_BIND_FLAG.D3D11_BIND_CONSTANT_BUFFER

                                 }));

        graphics.SetTransformBuffer(transformBuffer);
    }

    public override void OnGraphicsPause(GraphicsObject graphics, Device device)
    {}

    public override void OnGraphicsRemove(GraphicsObject graphics, Device device)
    {}

    public override void OnGraphicsResume(GraphicsObject graphics, Device device)
    {}

    public override void OnInitialize(Device device, DeviceContext context)
    {
        _forwardRenderPass = new ForwardRenderPass(
            _outputTexture, _lightObjects, MAX_PER_VARIATION_LIGHTS_COUNT,
            _cameraObjects);

        _renderPasses = [_forwardRenderPass];
    }

    public override void OnLightAdd(LightObject light, Device device)
    {}

    public override void OnLightPause(LightObject light, Device device)
    {}

    public override void OnLightRemove(LightObject light, Device device)
    {}

    public override void OnLightResume(LightObject light, Device device)
    {}

    public override void OnReady(Device device, DeviceContext context)
    {}

    public override void OnSkyboxSet(CubemapInfo info, Device device)
    {}
}