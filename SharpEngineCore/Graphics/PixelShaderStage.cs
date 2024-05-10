namespace SharpEngineCore.Graphics;

internal sealed class PixelShaderStage : IPipelineStage
{
    [Flags]
    public enum BindFlags
    {
        None = 0,
        PixelShader = 1 << 1
    }
    public PixelShader PixelShader { get; init; }
    public BindFlags Flags { get; init; }

    public PixelShaderStage(PixelShader pixelShader,
                            BindFlags flags)
    {
        PixelShader = pixelShader;

        Flags = flags;
    }

    public PixelShaderStage()
    { }

    public void Bind(DeviceContext context)
    {
        if(Flags.HasFlag(BindFlags.PixelShader))
        {
            context.PSSetShader(PixelShader);
        }
    }
}
