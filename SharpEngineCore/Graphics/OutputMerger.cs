namespace SharpEngineCore.Graphics;

internal sealed class OutputMerger : IPipelineStage
{
    public RenderTargetView[] RenderTargetViews { get; init; }

    public OutputMerger(RenderTargetView[] renderTargetViews)
    {
        RenderTargetViews = renderTargetViews;
    }

    public OutputMerger()
    { }

    public void Bind(DeviceContext context)
    {
        context.OMSetRenderTargets(RenderTargetViews);
    }
}
