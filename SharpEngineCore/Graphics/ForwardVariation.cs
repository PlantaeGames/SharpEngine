namespace SharpEngineCore.Graphics;

internal sealed class ForwardVariation : PipelineVariation
{

    public ForwardVariation(RenderTargetView renderTargetView,
                            DepthStencilState depthStencilStateOn,
                            DepthStencilState depthStencilStateOff,
                            DepthStencilView depthStencilView,
                            BlendState blendStateOn,
                            BlendState blendStateOff)
        : base()
    {

        OutputMerger = new OutputMerger()
        {
            RenderTargetViews = [renderTargetView],
            DepthStencilView = depthStencilView,
            DepthStencilState = depthStencilStateOn,
            DefaultDepthStencilState = depthStencilStateOff,
            DefaultBlendState = blendStateOff,
            BlendState = blendStateOn,

            Flags = OutputMerger.BindFlags.RenderTargetViewAndDepthView |
                    OutputMerger.BindFlags.DepthStencilState            |
                    OutputMerger.BindFlags.BlendState                   |
                    OutputMerger.BindFlags.ToggleableDepthStencilState  |
                    OutputMerger.BindFlags.ToggleableBlendState
        };

        _stages = [OutputMerger];
    }
}