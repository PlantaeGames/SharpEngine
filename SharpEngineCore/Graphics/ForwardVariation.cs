namespace SharpEngineCore.Graphics;

internal sealed class ForwardVariation : PipelineVariation
{
    public ForwardVariation(InputLayout layout, 
                            VertexShader vertexShader,
                            PixelShader pixelShader,
                            Viewport viewport,
                            RenderTargetView renderTargetView,
                            DepthStencilState depthStencilState,
                            DepthStencilView depthStencilView)
        : base()
    {
        var inputAssembler = new InputAssembler()
        {
            Layout = layout,
            Flags = InputAssembler.BindFlags.Layout
        };

        var vertexShaderStage = new VertexShaderStage()
        {
            VertexShader = vertexShader,

            Flags = VertexShaderStage.BindFlags.VertexShader
        };

        var pixelShaderStage = new PixelShaderStage()
        {
            PixelShader = pixelShader,

            Flags = PixelShaderStage.BindFlags.PixelShader
        };

        var rasterizer = new Rasterizer()
        {
            Viewports = [viewport],

            Flags = Rasterizer.BindFlags.Viewports
        };

        var outputMerger = new OutputMerger()
        {
            RenderTargetViews = [renderTargetView],
            DepthStencilView = depthStencilView,

            Flags = OutputMerger.BindFlags.RenderTargetView |
                    OutputMerger.BindFlags.DepthStencilState
        };

        _stages = [inputAssembler,
                   vertexShaderStage,
                   pixelShaderStage,
                   rasterizer,
                   outputMerger];
    }
}