namespace SharpEngineCore.Graphics;

internal sealed class ForwardSubVariation : PipelineVariation
{
    public ForwardSubVariation(VertexBuffer vertexBuffer, IndexBuffer indexBuffer,
        ConstantBuffer transformConstantBuffer,
        ConstantBuffer camTransformConstantBuffer) :
        base()
    {
        var inputAssembler = new InputAssembler()
        {
            VertexBuffer = vertexBuffer,
            IndexBuffer = indexBuffer,

            Flags = InputAssembler.BindFlags.IndexBuffer |
                    InputAssembler.BindFlags.VertexBuffer
        };

        var vertexShaderStage = new VertexShaderStage()
        {
            ConstantBuffers = [transformConstantBuffer, camTransformConstantBuffer],

            Flags = VertexShaderStage.BindFlags.ConstantBuffers
        };

        VertexCount = vertexBuffer.VertexCount;
        IndexCount = indexBuffer.IndexCount;

        _stages = [inputAssembler, vertexShaderStage];
    }
}