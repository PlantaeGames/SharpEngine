namespace SharpEngineCore.Graphics;

internal sealed class SkyboxVariation : PipelineVariation
{
    public SkyboxVariation(
        InputLayout inputLayout,
        VertexBuffer vertexBuffer,
        IndexBuffer indexBuffer,
        VertexShader vertexShader,
        ConstantBuffer vertexTransformBuffer,
        PixelShader pixelShader,
        ShaderResourceView cubeMapView,
        Sampler cubeMapSampler,
        RenderTargetView renderTargetView,
        DepthStencilView depthView)
    {
        InputAssembler = new InputAssembler()
        {
            Layout = inputLayout,
            VertexBuffer = vertexBuffer,
            IndexBuffer = indexBuffer,
            
            Flags = InputAssembler.BindFlags.VertexBuffer |
                    InputAssembler.BindFlags.IndexBuffer |
                    InputAssembler.BindFlags.Layout
        };

        VertexShaderStage = new VertexShaderStage()
        {
            VertexShader = vertexShader,
            ConstantBuffers = [vertexTransformBuffer],

            Flags = VertexShaderStage.BindFlags.VertexShader |
                    VertexShaderStage.BindFlags.ConstantBuffers
        };

        PixelShaderStage = new PixelShaderStage()
        {
            PixelShader = pixelShader,
            ShaderResourceViews = [cubeMapView],
            Samplers = [cubeMapSampler],
            SamplerStartIndex = 0,

            Flags = PixelShaderStage.BindFlags.PixelShader |
                    PixelShaderStage.BindFlags.ShaderResourceViews |
                    PixelShaderStage.BindFlags.Samplers
        };

        OutputMerger = new OutputMerger()
        {
            DepthStencilView = depthView,
            RenderTargetViews = [renderTargetView],

            Flags = OutputMerger.BindFlags.RenderTargetView,
        };

        VertexCount = vertexBuffer.VertexCount;
        IndexCount = indexBuffer.IndexCount;
        UseIndexRendering = false;

        _stages = [InputAssembler, VertexShaderStage, PixelShaderStage, OutputMerger];
    }
}