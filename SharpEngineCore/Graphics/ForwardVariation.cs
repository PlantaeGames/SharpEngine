namespace SharpEngineCore.Graphics;

internal sealed class ForwardVariation : PipelineVariation
{
    public ForwardVariation(RenderTargetView renderTargetView,
                            DepthStencilState depthStencilState,
                            DepthStencilView depthStencilView)
        : base()
    {

        var outputMerger = new OutputMerger()
        {
            RenderTargetViews = [renderTargetView],
            DepthStencilView = depthStencilView,
            DepthStencilState = depthStencilState,

            Flags = OutputMerger.BindFlags.RenderTargetView |
                    OutputMerger.BindFlags.DepthStencilState
        };

        _stages = [outputMerger];
    }
}