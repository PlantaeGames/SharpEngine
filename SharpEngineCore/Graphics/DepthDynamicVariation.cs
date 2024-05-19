namespace SharpEngineCore.Graphics;

internal sealed class DepthDynamicVariation : PipelineVariation
{
    public DepthDynamicVariation(
        DepthStencilState depthState,
        DepthStencilView depthView)
    {
        OutputMerger = new OutputMerger()
        {
            DepthStencilState = depthState,
            DepthStencilView = depthView,

            Flags = OutputMerger.BindFlags.DepthStencilState
        };

        _stages = [OutputMerger];
    }
}
