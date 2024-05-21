namespace SharpEngineCore.Graphics;

internal sealed class DepthDynamicVariation : PipelineVariation
{
    public DepthDynamicVariation(
        RenderTargetView renderTarget,
        DepthStencilView depthView)
    {
        OutputMerger = new OutputMerger()
        {
            RenderTargetViews = [renderTarget],
            DepthStencilView = depthView,

            Flags = OutputMerger.BindFlags.RenderTargetView
        };

        _stages = [OutputMerger];
    }
}
