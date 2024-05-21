namespace SharpEngineCore.Graphics;

internal sealed class PixelShaderStage : IPipelineStage
{
    [Flags]
    public enum BindFlags
    {
        None = 0,
        PixelShader = 1 << 1,
        ConstantBuffers = 1 << 2,
        Samplers = 1 << 3,
        ShaderResourceViews = 1 << 4
    }
    public PixelShader PixelShader { get; init; }
    public ConstantBuffer[] ConstantBuffers { get; init; }
    public Sampler[] Samplers { get; init; } 
    public ShaderResourceView[] ShaderResourceViews { get; init; }

    public BindFlags Flags { get; init; }

    public PixelShaderStage(
        PixelShader pixelShader,
        ConstantBuffer[] constantBuffers,
        Sampler[] samplers,
        ShaderResourceView[] shaderResourceViews,
        BindFlags flags)
    {
        PixelShader = pixelShader;
        ConstantBuffers = constantBuffers;
        Samplers = samplers;
        ShaderResourceViews = shaderResourceViews;

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

        if (Flags.HasFlag(BindFlags.ConstantBuffers))
        {
            context.PSSetConstantBuffers(ConstantBuffers, 0);
        }

        if (Flags.HasFlag(BindFlags.Samplers))
        {
            context.PSSetSamplers(Samplers, 1);
        }

        if (Flags.HasFlag(BindFlags.ShaderResourceViews))
        {
            context.PSSetShaderResources(ShaderResourceViews, 0);
        }
    }

    public void Unbind(DeviceContext context)
    {
        if (Flags.HasFlag(BindFlags.PixelShader))
        {
            context.PSSetShader(PixelShader, true);
        }

        if (Flags.HasFlag(BindFlags.ConstantBuffers))
        {
            context.PSSetConstantBuffers(ConstantBuffers, 0, true);
        }

        if (Flags.HasFlag(BindFlags.Samplers))
        {
            context.PSSetSamplers(Samplers, 1, true);
        }

        if (Flags.HasFlag(BindFlags.ShaderResourceViews))
        {
            context.PSSetShaderResources(ShaderResourceViews, 0, true);
        }
    }
}
