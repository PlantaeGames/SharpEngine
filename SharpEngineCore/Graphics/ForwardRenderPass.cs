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
        List<LightObject> lightObjects,
        int maxPerLightsCount,
        List<CameraObject> cameraObjects) :
        base()
    {
        _outputTexture = outputTexture;

        _depthPass = new DepthPass(lightObjects, maxPerLightsCount);

        _forwardPass = new ForwardPass(_outputTexture.Info.Size, maxPerLightsCount,
            lightObjects, cameraObjects, _depthPass.DepthTextures);

        _outputPass = new OutputPass(_outputTexture);

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
        _outputPass.SetSrcTexture(_forwardPass.OutputTexture);
    }
}