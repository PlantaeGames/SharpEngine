namespace SharpEngineCore.Graphics;

internal sealed class InputAssembler : IPipelineStage
{
    public InputLayout Layout { get; init; }
    public VertexBuffer VertexBuffer { get; init; }
    public IndexBuffer IndexBuffer { get; init; }

    public void Bind(DeviceContext context)
    {
        context.IASetInputLayout(Layout);
        context.IASetVertexBuffer([VertexBuffer], 0);
        context.IASetIndexBuffer(IndexBuffer);
    }

    public InputAssembler(InputLayout layout,
                          VertexBuffer vertexBuffer,
                          IndexBuffer indexBuffer)
    {
        Layout = layout;
        VertexBuffer = vertexBuffer;
        IndexBuffer = indexBuffer;
    }

    public InputAssembler()
    {
    }
}