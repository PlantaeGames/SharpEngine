namespace SharpEngineCore.Graphics;

internal sealed class DepthSubVariation : PipelineVariation
{
    public DepthSubVariation(
        InputLayout inputLayout,
        ConstantBuffer[] vertexConstantBuffers,
        VertexBuffer vertexBuffer,
        IndexBuffer indexBuffer,
        bool useIndexedRendering)
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

        VertexShaderStage = new VertexShaderStage()
        {
            ConstantBuffers = vertexConstantBuffers,

            Flags = VertexShaderStage.BindFlags.ConstantBuffers
        };


        VertexCount = vertexBuffer.VertexCount;
        IndexCount = indexBuffer.IndexCount;
        UseIndexRendering = useIndexedRendering;

        _stages = [InputAssembler, VertexShaderStage];
    }
}
