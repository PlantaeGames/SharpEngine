namespace SharpEngineCore.Graphics;

internal abstract class PipelineVariation 
{
    public InputAssembler InputAssembler { get; protected set; }
    public VertexShaderStage VertexShaderStage { get; protected set; }
    public PixelShaderStage PixelShaderStage { get; protected set; }
    public Rasterizer Rasterizer { get; protected set; }
    public OutputMerger OutputMerger { get; protected set; }

    public State State { get; set; }
    public int VertexCount { get; protected set; }
    public int IndexCount { get; protected set; }
    public bool UseIndexRendering { get; protected set; }

    protected IPipelineStage[] _stages;
    public void Bind(DeviceContext context)
    {
        foreach(var stage in _stages)
        {
            stage.Bind(context);
        }
    }
    public void Unbind(DeviceContext context)
    {
        foreach (var stage in _stages)
        {
            stage.Unbind(context);
        }
    }
}
