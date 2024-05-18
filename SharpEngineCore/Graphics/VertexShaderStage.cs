namespace SharpEngineCore.Graphics;

internal sealed class VertexShaderStage : IPipelineStage
{
    [Flags]
    public enum BindFlags
    {
        None = 0,
        VertexShader = 1 << 1,
        ConstantBuffers = 1 << 2,
        Samplers = 1 << 3,
        ShaderResourceViews = 1 << 4
    }


    public VertexShader VertexShader { get; init; }
    public ConstantBuffer[] ConstantBuffers { get; init; }
    public Sampler[] Samplers {get; init;}
    public ShaderResourceView[] ShaderResourceViews { get; init; }

    public BindFlags Flags { get; init; }

    public VertexShaderStage(
        VertexShader vertexShader,
        Sampler[] samplers,
        ConstantBuffer[] constantBuffers,
        ShaderResourceView[] shaderResourceViews,
        BindFlags flags)
    {
        VertexShader = vertexShader;
        ConstantBuffers = constantBuffers;
        Samplers = samplers;
        ShaderResourceViews = shaderResourceViews;

        Flags = flags;
    }

    public VertexShaderStage()
    { }

    public void Bind(DeviceContext context)
    {
        if(Flags.HasFlag(BindFlags.VertexShader))
        {
            context.VSSetShader(VertexShader);
        }

        if (Flags.HasFlag(BindFlags.ConstantBuffers))
        {
            context.VSSetConstantBuffers(ConstantBuffers, 0);
        }

        if (Flags.HasFlag(BindFlags.ShaderResourceViews))
        {
            context.VSSetShaderResources(ShaderResourceViews, 0);
        }

        if (Flags.HasFlag(BindFlags.Samplers))
        {
            context.VSSetSamplers(Samplers, 0);
        }
    }
}
