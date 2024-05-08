namespace SharpEngineCore.Graphics;

internal sealed class PixelShaderStage : IPipelineStage
{
    public PixelShader PixelShader { get; init; }

    public PixelShaderStage(PixelShader pixelShader)
    {
        PixelShader = pixelShader;
    }

    public PixelShaderStage()
    { }

    public void Bind(DeviceContext context)
    {
        context.PSSetShader(PixelShader);
    }
}
