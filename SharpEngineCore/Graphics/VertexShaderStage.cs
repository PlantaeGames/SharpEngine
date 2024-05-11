namespace SharpEngineCore.Graphics;

internal sealed class VertexShaderStage : IPipelineStage
{
    [Flags]
    public enum BindFlags
    {
        None = 0, 
        VertexShader = 1 << 1,
        ConstantBuffers = 1 << 2,
        ShaderResourceViews = 1 << 4
    }


    public VertexShader VertexShader { get; init; }
    public ConstantBuffer[] ConstantBuffers { get; init; }
    public ShaderResourceView[] ShaderResourceViews { get; init; }

    public BindFlags Flags { get; init; }

    public VertexShaderStage(
        VertexShader vertexShader,
        ConstantBuffer[] constantBuffers,
        ShaderResourceView[] shaderResourceViews,
        BindFlags flags)
    {
        VertexShader = vertexShader;
        ConstantBuffers = constantBuffers;
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
            context.PSSetConstantBuffers(ConstantBuffers, 0);
        }

        if (Flags.HasFlag(BindFlags.ShaderResourceViews))
        {
            context.PSSetShaderResources(ShaderResourceViews, 0);
        }
    }
}
