namespace SharpEngineCore.Graphics;

internal sealed class ForwardVariation : PipelineVariation
{

    public ForwardVariation(DepthStencilState depthStencilStateOn,
                            DepthStencilState depthStencilStateOff,
                            BlendState blendStateOn,
                            BlendState blendStateOff)
        : base()
    {

        OutputMerger = new OutputMerger()
        {
            DepthStencilState = depthStencilStateOn,
            DefaultDepthStencilState = depthStencilStateOff,
            DefaultBlendState = blendStateOff,
            BlendState = blendStateOn,

            Flags = OutputMerger.BindFlags.DepthStencilState            |
                    OutputMerger.BindFlags.BlendState                   |
                    OutputMerger.BindFlags.ToggleableDepthStencilState  |
                    OutputMerger.BindFlags.ToggleableBlendState
        };

        _stages = [OutputMerger];
    }
}