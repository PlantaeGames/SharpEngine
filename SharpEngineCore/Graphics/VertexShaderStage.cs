namespace SharpEngineCore.Graphics;

internal sealed class VertexShaderStage : IPipelineStage
{
    public VertexShader VertexShader { get; init; }

    public VertexShaderStage(VertexShader vertexShader)
    {
        VertexShader = vertexShader;
    }

    public VertexShaderStage()
    { }

    public void Bind(DeviceContext context)
    {
        context.VSSetShader(VertexShader);
    }
}
