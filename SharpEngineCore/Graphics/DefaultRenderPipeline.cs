namespace SharpEngineCore.Graphics;

internal sealed class DefaultRenderPipeline : RenderPipeline
{
    public DefaultRenderPipeline() :
        base()
    { 

    }

    public override void Go(Device device)
    {
        throw new NotImplementedException();
    }

    public override void Initialize(Device device)
    {
        throw new NotImplementedException();
    }

    public override void Ready(Device device)
    {
        throw new NotImplementedException();
    }
}

internal sealed class ForwardRenderPass : RenderPass
{
    public ForwardRenderPass() :
        base()
    { }

    public override void Go(Device device)
    {
        throw new NotImplementedException();
    }

    public override void Initialize(Device device)
    {
        throw new NotImplementedException();
    }

    public override void Ready(Device device)
    {
        throw new NotImplementedException();
    }
}

internal sealed class ForwardPass : Pass
{
    public ForwardPass()
        : base()
    { }

    public override void Go(Device device)
    {
        throw new NotImplementedException();
    }

    public override void Initialize(Device device)
    {
        throw new NotImplementedException();
    }

    public override void Ready(Device device)
    {
        throw new NotImplementedException();
    }
}

internal sealed class ForwardVariation : PipelineVariation
{
    public ForwardVariation(VertexShader vertexShader, PixelShader pixelShader,
        Viewport viewport, RenderTargetView renderTargetView)
        : base()
    {
        var vertexShaderStage = new VertexShaderStage()
        {
            VertexShader = vertexShader
        };

        var pixelShaderStage = new PixelShaderStage()
        {
            PixelShader = pixelShader
        };

        var rasterizer = new Rasterizer()
        {
            Viewports = [viewport]
        };

        var outputMerger = new OutputMerger()
        {
            RenderTargetViews = [renderTargetView]
        };


        _stages = [vertexShaderStage, pixelShaderStage, rasterizer, outputMerger];
    }
}

internal sealed class ForwardSubVariation : PipelineVariation
{
    public ForwardSubVariation(VertexBuffer vertexBuffer, IndexBuffer indexBuffer,
        InputLayout inputLayout) :
        base()
    {
        var inputAssembler = new InputAssembler()
        {
            Layout = inputLayout,
            VertexBuffer = vertexBuffer,
            IndexBuffer = indexBuffer
        };

        _stages = [inputAssembler];
    }
}