namespace SharpEngineCore.Graphics;

internal sealed class ForwardSubVariation : PipelineVariation
{
    public ForwardSubVariation(VertexBuffer vertexBuffer, IndexBuffer indexBuffer) :
        base()
    {
        var inputAssembler = new InputAssembler()
        {
            VertexBuffer = vertexBuffer,
            IndexBuffer = indexBuffer,

            Flags = InputAssembler.BindFlags.IndexBuffer |
                    InputAssembler.BindFlags.VertexBuffer
        };

        VertexCount = vertexBuffer.VertexCount;
        IndexCount = indexBuffer.IndexCount;

        _stages = [inputAssembler];
    }
}