namespace SharpEngineCore.Graphics;

internal sealed class DepthDynamicVariation : PipelineVariation
{
    public DepthDynamicVariation(
        RenderTargetView renderTarget,
        DepthStencilState depthState,
        DepthStencilView depthView)
    {
        OutputMerger = new OutputMerger()
        {
            RenderTargetViews = [renderTarget],
            DepthStencilState = depthState,
            DepthStencilView = depthView,

            Flags = OutputMerger.BindFlags.DepthStencilState
        };

        _stages = [OutputMerger];
    }
}
