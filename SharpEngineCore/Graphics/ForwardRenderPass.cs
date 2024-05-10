namespace SharpEngineCore.Graphics;

internal sealed class ForwardRenderPass : RenderPass
{
    public ForwardRenderPass(Texture2D output) :
        base()
    { 
        var forwardPass = new ForwardPass(output);
        _passes = [forwardPass];
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
