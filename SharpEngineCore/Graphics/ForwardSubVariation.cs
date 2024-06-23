namespace SharpEngineCore.Graphics;

internal sealed class ForwardSubVariation : PipelineVariation
{

    public ForwardSubVariation(InputLayout layout,
                               VertexBuffer vertexBuffer,
                               IndexBuffer indexBuffer,
                               VertexShader vertexShader,
                               Sampler[] vertexSamplers,
                               ConstantBuffer[] vertexConstantBuffers,
                               ShaderResourceView[] vertexResourceViews,
                               PixelShader pixelShader,
                               Sampler[] pixelSamplers,
                               ConstantBuffer[] pixelConstantBuffers,
                               ShaderResourceView[] pixelResourceViews,
                               bool useIndexedRendering) :
        base()
    {
        InputAssembler = new InputAssembler()
        {
            Layout = layout,
            VertexBuffer = vertexBuffer,
            IndexBuffer = indexBuffer,

            Flags = InputAssembler.BindFlags.IndexBuffer |
                    InputAssembler.BindFlags.VertexBuffer |
                    InputAssembler.BindFlags.Layout
        };

        VertexShaderStage = new VertexShaderStage()
        {
            VertexShader = vertexShader,
            Samplers = vertexSamplers,
            ConstantBuffers = vertexConstantBuffers,
            ShaderResourceViews = vertexResourceViews,

            Flags = VertexShaderStage.BindFlags.VertexShader |
                    VertexShaderStage.BindFlags.ConstantBuffers |
                    VertexShaderStage.BindFlags.ShaderResourceViews |
                    VertexShaderStage.BindFlags.Samplers
        };

        PixelShaderStage = new PixelShaderStage()
        {
            PixelShader = pixelShader,
            Samplers = pixelSamplers,
            ConstantBuffers = pixelConstantBuffers,
            ShaderResourceViews = pixelResourceViews,

            Flags = PixelShaderStage.BindFlags.PixelShader |
                    PixelShaderStage.BindFlags.Samplers |
                    PixelShaderStage.BindFlags.ConstantBuffers |
                    PixelShaderStage.BindFlags.ShaderResourceViews
        };

        VertexCount = vertexBuffer.VertexCount;
        IndexCount = indexBuffer.IndexCount;
        UseIndexRendering = useIndexedRendering;


        _stages = [InputAssembler,
                   VertexShaderStage,
                   PixelShaderStage];
    }
}