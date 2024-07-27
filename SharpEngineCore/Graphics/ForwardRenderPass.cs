using TerraFX.Interop.DirectX;

namespace SharpEngineCore.Graphics;

internal sealed class ForwardRenderPass : RenderPass
{
    private readonly Texture2D _outputTexture;

    private readonly DepthPass _depthPass;
    private readonly SkyboxPass _skyboxPass;
    private readonly ForwardPass _forwardPass;
    private readonly OutputPass _outputPass;

    public ForwardRenderPass(
        Texture2D outputTexture,
        List<LightObject> lightObjects,
        int maxPerLightsCount,
        List<CameraObject> cameraObjects) :
        base()
    {
        _outputTexture = outputTexture;

        _depthPass = new DepthPass(lightObjects, maxPerLightsCount);
        _skyboxPass = new SkyboxPass(cameraObjects);

        _forwardPass = new ForwardPass(maxPerLightsCount,
            lightObjects, cameraObjects, _depthPass);

        _outputPass = new OutputPass(_outputTexture, cameraObjects);

        _passes = [_depthPass, _skyboxPass, _forwardPass, _outputPass];
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