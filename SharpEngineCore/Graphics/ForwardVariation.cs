namespace SharpEngineCore.Graphics;

internal sealed class ForwardVariation : PipelineVariation
{

    public ForwardVariation(RenderTargetView renderTargetView,
                            DepthStencilState depthStencilState,
                            DepthStencilView depthStencilView,
                            BlendState blendStateOn,
                            BlendState blendStateOff)
        : base()
    {

        OutputMerger = new OutputMerger()
        {
            RenderTargetViews = [renderTargetView],
            DepthStencilView = depthStencilView,
            DepthStencilState = depthStencilState,
            DefaultBlendState = blendStateOff,
            BlendState = blendStateOn,

            Flags = OutputMerger.BindFlags.RenderTargetViewAndDepthView |
                    OutputMerger.BindFlags.DepthStencilState            |
                    OutputMerger.BindFlags.BlendState
        };

        _stages = [OutputMerger];
    }
}