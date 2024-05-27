using TerraFX.Interop.DirectX;

namespace SharpEngineCore.Graphics;

internal sealed class ForwardRenderPass : RenderPass
{
    private Texture2D _outputTexture;

    private DepthPass _depthPass;
    private SkyboxPass _skyboxPass;
    private ForwardPass _forwardPass;

    public ForwardRenderPass(
        Texture2D outputTexture,
        List<LightObject> lightObjects,
        int maxPerLightsCount,
        List<CameraObject> cameraObjects) :
        base()
    {
        _outputTexture = outputTexture;

        _depthPass = new DepthPass(lightObjects, maxPerLightsCount);
        _skyboxPass = new SkyboxPass(_outputTexture, cameraObjects);

        _forwardPass = new ForwardPass(_outputTexture, maxPerLightsCount,
            lightObjects, cameraObjects, _depthPass.DepthShaderViews, _depthPass.DepthSamplers);

        _passes = [_depthPass, _skyboxPass, _forwardPass];
    }

    public override void OnCameraAdd(CameraObject camera, Device device)
    {
    }

    public override void OnCameraPause(CameraObject camera, Device device)
    {
    }

    public override void OnCameraRemove(CameraObject camera, Device device)
    {
    }

    public override void OnCameraResume(CameraObject camera, Device device)
    {
    }

    public override void OnGo(Device device, DeviceContext context)
    {
    }

    public override void OnGraphicsAdd(GraphicsObject graphics, Device device)
    {
    }

    public override void OnGraphicsPause(GraphicsObject graphics, Device device)
    {
    }

    public override void OnGraphicsRemove(GraphicsObject graphics, Device device)
    {
    }

    public override void OnGraphicsResume(GraphicsObject graphics, Device device)
    {
    }

    public override void OnInitialize(Device device, DeviceContext context)
    {
    }

    public override void OnLightAdd(LightObject light, Device device)
    {
    }

    public override void OnLightPause(LightObject light, Device device)
    {
    }

    public override void OnLightRemove(LightObject light, Device device)
    {
    }

    public override void OnLightResume(LightObject light, Device device)
    {
    }

    public override void OnReady(Device device, DeviceContext context)
    {}

    public override void OnSkyboxSet(CubemapInfo info, Device device)
    {
    }
}