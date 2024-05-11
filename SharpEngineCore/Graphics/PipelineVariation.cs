namespace SharpEngineCore.Graphics;

internal abstract class PipelineVariation 
{
    public int VertexCount { get; protected set; }
    public int IndexCount { get; protected set; }

    protected IPipelineStage[] _stages;
    public void Bind(DeviceContext context)
    {
        foreach(var stage in _stages)
        {
            stage.Bind(context);
        }
    }
}
