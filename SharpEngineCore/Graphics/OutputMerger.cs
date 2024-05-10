namespace SharpEngineCore.Graphics;
internal sealed class OutputMerger : IPipelineStage
{
    [Flags]
    public enum BindFlags
    {
        None = 0,
        RenderTargetViews = 1 << 1
    }

    public RenderTargetView[] RenderTargetViews { get; init; }
    public BindFlags Flags { get; init; }

    public OutputMerger(RenderTargetView[] renderTargetViews, BindFlags flags)
    {
        RenderTargetViews = renderTargetViews;
        Flags = flags;
    }

    public OutputMerger()
    { }

    public void Bind(DeviceContext context)
    {
        if(Flags.HasFlag(BindFlags.RenderTargetViews))
        {
            context.OMSetRenderTargets(RenderTargetViews);
        }
    }
}
