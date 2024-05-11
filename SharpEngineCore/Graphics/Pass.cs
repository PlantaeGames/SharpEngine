namespace SharpEngineCore.Graphics;

internal abstract class Pass : PipelineEvents
{
    protected Queue<PipelineVariation> _subVariations;

    public sealed override void Initialize(Device device, DeviceContext context)
    {
        OnInitialize(device, context);
    }

    public sealed override void Ready(Device device, DeviceContext context)
    {
        OnReady(device, context);
    }

    public sealed override void Go(Device device, DeviceContext context)
    {
        OnGo(device, context);
    }

    protected Pass()
    { }
}
