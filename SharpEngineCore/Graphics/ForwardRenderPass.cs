using TerraFX.Interop.DirectX;

namespace SharpEngineCore.Graphics;

internal sealed class ForwardRenderPass : RenderPass
{
    private Texture2D _outputTexture;

    private DepthPass _depthPass;
    private ForwardPass _forwardPass;
    private OutputPass _outputPass;

    public ForwardRenderPass(
        Texture2D outputTexture,
        ShaderResourceView lightsResourceView,
        List<LightObject> lightObjects,
        int maxLightsCount,
        List<CameraObject> cameraObjects,
        List<GraphicsObject> graphicsObjects) :
        base()
    {
        _outputTexture = outputTexture;

        _depthPass = new DepthPass(lightObjects, maxLightsCount, graphicsObjects);

        _forwardPass = new ForwardPass(_outputTexture.Info.Size,
            lightsResourceView, cameraObjects, _depthPass.DepthTextures);

        _outputPass = new OutputPass(_forwardPass.OutputTexture, _outputTexture);

        _passes = [_depthPass, _forwardPass, _outputPass];
    }

    public PipelineVariation[] CreateVariations(
        Device device, Material material, Mesh mesh)
    {
        return [
            _depthPass.CreateSubVariation(device, material, mesh),
            _forwardPass.CreateSubVariation(device, material, mesh)
            ];
    }

    public override void OnGo(Device device, DeviceContext context)
    {
    }

    public override void OnInitialize(Device device, DeviceContext context)
    {
    }

    public override void OnReady(Device device, DeviceContext context)
    {
    }
}