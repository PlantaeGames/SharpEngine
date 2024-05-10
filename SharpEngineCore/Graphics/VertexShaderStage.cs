namespace SharpEngineCore.Graphics;

internal sealed class VertexShaderStage : IPipelineStage
{
    [Flags]
    public enum BindFlags
    {
        None = 0, 
        VertexShader = 1 << 1
    }


    public VertexShader VertexShader { get; init; }
    public BindFlags Flags { get; init; }

    public VertexShaderStage(VertexShader vertexShader, BindFlags flags)
    {
        VertexShader = vertexShader;
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
    }
}
