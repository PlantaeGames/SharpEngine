namespace SharpEngineCore.Graphics;

internal sealed class DepthVariation : PipelineVariation
{
    public DepthVariation(VertexShader vertexShader,
        PixelShader pixelShader,
        Viewport viewport,
        DepthStencilState depthState,
        DepthStencilView depthView,
        RenderTargetView renderTargetView,
        RasterizerState rasterizerState)
    {
        VertexShaderStage = new VertexShaderStage()
        {
            VertexShader = vertexShader,

            Flags = VertexShaderStage.BindFlags.VertexShader
        };

        PixelShaderStage = new PixelShaderStage()
        {
            PixelShader = pixelShader,

            Flags = PixelShaderStage.BindFlags.PixelShader
        };

        Rasterizer = new Rasterizer()
        {
            Viewports = [viewport],
            RasterizerState = rasterizerState,

            Flags = Rasterizer.BindFlags.Viewports |
                    Rasterizer.BindFlags.RasterizerState
        };

        OutputMerger = new OutputMerger()
        {
            DepthStencilState = depthState,
            RenderTargetViews = [renderTargetView],
            DepthStencilView = depthView,

            Flags = OutputMerger.BindFlags.DepthStencilState |
                    OutputMerger.BindFlags.RenderTargetViewAndDepthView
        };

        _stages = [VertexShaderStage, PixelShaderStage, Rasterizer, OutputMerger];
    }
}
