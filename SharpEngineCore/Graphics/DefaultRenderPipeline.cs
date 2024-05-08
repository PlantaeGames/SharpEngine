namespace SharpEngineCore.Graphics;

internal sealed class DefaultRenderPipeline : RenderPipeline
{
    public DefaultRenderPipeline() :
        base()
    { 

    }

    public override void Go(Device device, DeviceContext context)
    {
        throw new NotImplementedException();
    }

    public override void Initialize(Device device, DeviceContext context)
    {
        throw new NotImplementedException();
    }

    public override void Ready(Device device, DeviceContext context)
    {
        throw new NotImplementedException();
    }
}

internal sealed class ForwardRenderPass : RenderPass
{
    public ForwardRenderPass() :
        base()
    { }

    public override void Go(Device device, DeviceContext context)
    {
        throw new NotImplementedException();
    }

    public override void Initialize(Device device, DeviceContext context)
    {
        throw new NotImplementedException();
    }

    public override void Ready(Device device, DeviceContext context)
    {
        throw new NotImplementedException();
    }
}

internal sealed class ForwardPass : Pass
{
    private Texture2D _outputTexture;

    public ForwardPass(Texture2D outptut)
        : base()
    { 
        _outputTexture = outptut;
    }

    public override void Go(Device device, DeviceContext context)
    {
        foreach(var varitation in _subVariations)
        {
            varitation.Bind(context);

            context.Draw(varitation.VertexCount, 0);
        }
    }

    public override void Initialize(Device device, DeviceContext context)
    {
        var vertexShader = device.CreateVertexShader(new ShaderModule(
            "Shaders\\VertexShader.hlsl"
            ));

        var pixelShader = device.CreatePixelShader(new ShaderModule(
            "Shaders\\PixelShader.hlsl"
            ));

        var viewport = new Viewport()
        {
            Info = new TerraFX.Interop.DirectX.D3D11_VIEWPORT()
            {
                TopLeftX = 0u,
                TopLeftY = 0u,
                Width = _outputTexture.Info.Size.Width,
                Height = _outputTexture.Info.Size.Height,
                MinDepth = 0u,
                MaxDepth = 1u
            }
        };

        var targetView = device.CreateRenderTargetView(_outputTexture);

        _varitation = new ForwardVariation(vertexShader, pixelShader,
            viewport, targetView);       
    }

    public override void Ready(Device device, DeviceContext context)
    {
        _varitation.Bind(context);
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

        VertexCount = vertexBuffer.VertexCount;
        IndexCount = indexBuffer.IndexCount;

        _stages = [inputAssembler];
    }
}