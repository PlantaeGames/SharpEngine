namespace SharpEngineCore.Graphics;

internal sealed class DefaultRenderPipeline : RenderPipeline
{
    public DefaultRenderPipeline(Texture2D outputTexture) :
        base()
    {
        var forwardRenderPass = new ForwardRenderPass(outputTexture);
        _renderPasses = [forwardRenderPass];
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