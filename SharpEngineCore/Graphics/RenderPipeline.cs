namespace SharpEngineCore.Graphics;

internal abstract class RenderPipeline : PipelineEvents
{
    protected RenderPass[] _renderPasses;

    protected RenderPipeline()
    {

    }
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
    protected IPipelineStage[] _stages;
    public void Bind(DeviceContext context)
    {
        foreach(var stage in _stages)
        {
            stage.Bind(context);
        }
    }

    protected PipelineVariation()
    { }
}

internal abstract class PipelineEvents
{


    public abstract void Initialize(Device device);
    public abstract void Ready(Device device);
    public abstract void Go(Device device);

    protected PipelineEvents() { }
}