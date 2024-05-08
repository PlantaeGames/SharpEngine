using System.Diagnostics;

namespace SharpEngineCore.Graphics;

internal abstract class RenderPipeline : PipelineEvents
{
    protected RenderPass[] _renderPasses;

    protected RenderPipeline()
    { }
}

internal abstract class RenderPass : PipelineEvents
{
    protected Pass[] _passes;

    protected RenderPass()
    { }
}

internal abstract class Pass : PipelineEvents
{
    protected PipelineVariation _varitation;
    protected PipelineVariation[] _subVariations;

    protected Pass()
    { }
}

internal interface IPipelineStage
{
    void Bind(DeviceContext context);
}

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

#nullable enable
    public T? Get<T>()
        where T : class, IPipelineStage
    {
        foreach(var stage in _stages)
        {
            if(stage as T is not null)
            {
                return (T)stage;
            }
        }

        Debug.Assert(false,
            "Pipeline stage is not present on this varitation.");

        return null;
    }
#nullable disable

    protected PipelineVariation()
    { }
}

internal abstract class PipelineEvents
{
    public abstract void Initialize(Device device, DeviceContext context);
    public abstract void Ready(Device device, DeviceContext context);
    public abstract void Go(Device device, DeviceContext context);

    protected PipelineEvents() { }
}