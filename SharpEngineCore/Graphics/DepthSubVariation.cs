namespace SharpEngineCore.Graphics;

internal sealed class DepthSubVariation : PipelineVariation
{
    public DepthSubVariation(
        InputLayout inputLayout,
        VertexBuffer vertexBuffer,
        IndexBuffer indexBuffer)
    {
        InputAssembler = new InputAssembler()
        {
            Layout = inputLayout,
            VertexBuffer = vertexBuffer,
            IndexBuffer = indexBuffer,

            Flags = InputAssembler.BindFlags.IndexBuffer |
                    InputAssembler.BindFlags.VertexBuffer |
                    InputAssembler.BindFlags.Layout
        };

        _stages = [InputAssembler];
    }
}
