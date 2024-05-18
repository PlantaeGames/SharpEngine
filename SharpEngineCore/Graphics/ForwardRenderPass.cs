using TerraFX.Interop.DirectX;

namespace SharpEngineCore.Graphics;

internal sealed class ForwardRenderPass : RenderPass
{
    private Texture2D _outputTexture;

    private List<(ConstantBuffer dataBuffer, CameraObject camera)> _cameraObjects;
    private ShaderResourceView _lightsSRV;

    private DepthPass _depthPass;
    private ForwardPass _forwardPass;
    private OutputPass _outputPass;

    public ForwardRenderPass(
        Texture2D outputTexture,
        ShaderResourceView lightsResourceView,
        List<(ConstantBuffer dataBuffer, CameraObject camera)> cameraObjects) :
        base()
    {
        _outputTexture = outputTexture;
        _cameraObjects = cameraObjects;
        _lightsSRV = lightsResourceView;

        _depthPass = new DepthPass();

        _forwardPass = new ForwardPass(_outputTexture.Info.Size,
            _lightsSRV, _cameraObjects);

        _outputPass = new OutputPass(_forwardPass.OutputTexture, _outputTexture);

        _passes = [_depthPass, _forwardPass, _outputPass];
    }

    public Guid CreateNewGraphicsObject(Material material, Mesh mesh)
    {
        return _forwardPass.CreateNewGraphicsObject(material, mesh);
    }

    public List<GraphicsObject> GetGraphicsObjects()
    {
        return _forwardPass.GraphicsObjects;
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