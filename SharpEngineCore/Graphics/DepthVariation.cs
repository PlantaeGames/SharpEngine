namespace SharpEngineCore.Graphics;

internal sealed class DepthVariation : PipelineVariation
{
    public DepthVariation(VertexShader vertexShader,
        PixelShader pixelShader,
        Viewport viewport,
        DepthStencilState depthState)
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

            Flags = Rasterizer.BindFlags.Viewports
        };

        OutputMerger = new OutputMerger()
        {
            DepthStencilState = depthState,

            Flags = OutputMerger.BindFlags.DepthStencilState
        };

        _stages = [VertexShaderStage, PixelShaderStage, Rasterizer, OutputMerger];
    }
}
