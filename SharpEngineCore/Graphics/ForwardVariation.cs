namespace SharpEngineCore.Graphics;

internal sealed class ForwardVariation : PipelineVariation
{
    public ForwardVariation(RenderTargetView renderTargetView,
                            DepthStencilState depthStencilState,
                            DepthStencilView depthStencilView,
                            BlendState blendState)
        : base()
    {

        OutputMerger = new OutputMerger()
        {
            RenderTargetViews = [renderTargetView],
            DepthStencilView = depthStencilView,
            DepthStencilState = depthStencilState,
            BlendState = blendState,

            Flags = OutputMerger.BindFlags.RenderTargetViewAndDepthView |
                    OutputMerger.BindFlags.DepthStencilState          
        };

        _stages = [OutputMerger];
    }
}