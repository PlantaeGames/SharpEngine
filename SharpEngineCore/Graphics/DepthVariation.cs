namespace SharpEngineCore.Graphics;

internal sealed class DepthVariation : PipelineVariation
{
    public DepthVariation(VertexShader vertexShader,
        ConstantBuffer lightPerspectiveCBuffer,
        PixelShader pixelShader,
        Viewport viewport)
    {
        VertexShaderStage = new VertexShaderStage()
        {
            VertexShader = vertexShader,
            ConstantBuffers = [lightPerspectiveCBuffer],

            Flags = VertexShaderStage.BindFlags.VertexShader |
                    VertexShaderStage.BindFlags.ConstantBuffers
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

        _stages = [VertexShaderStage, PixelShaderStage, Rasterizer];
    }
}
