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
            lightObjects, cameraObjects, _depthPass.DepthTextures);

        _passes = [_depthPass, _skyboxPass, _forwardPass];
    }

    public (PipelineVariation[] variations, 
            ConstantBuffer transformBuffer) CreateVariations(
        Device device, Material material, Mesh mesh)
    {
        var forwardVariation = _forwardPass.CreateSubVariation(device, material, mesh);

        var transformBuffer = forwardVariation.VertexShaderStage.ConstantBuffers[1];

        var depthVariation = _depthPass.CreateSubVariation(device, material, mesh,
            transformBuffer);

        return ([forwardVariation, depthVariation], transformBuffer);
    }

    public override void OnGo(Device device, DeviceContext context)
    {
    }

    public override void OnInitialize(Device device, DeviceContext context)
    {
    }

    public override void OnReady(Device device, DeviceContext context)
    {}
}