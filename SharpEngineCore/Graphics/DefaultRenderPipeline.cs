namespace SharpEngineCore.Graphics;

internal sealed class DefaultRenderPipeline : RenderPipeline
{
    private Texture2D _finalOutput;
    private RenderTargetView _targetView;

    public DefaultRenderPipeline(Texture2D output) :
        base()
    {
        _finalOutput = output;

        var forwardRenderPass = new ForwardRenderPass(_finalOutput);
        _renderPasses = [forwardRenderPass];
    }

    public override void OnGo(Device device, DeviceContext context)
    {
    }

    public override void OnInitialize(Device device, DeviceContext context)
    {
        _targetView = device.CreateRenderTargetView(_finalOutput);
    }

    public override void OnReady(Device device, DeviceContext context)
    {
       context.ClearRenderTargetView(_targetView,
                                     new FColor4(0,0,0,0));
    }
}