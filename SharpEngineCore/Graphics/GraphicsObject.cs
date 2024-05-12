namespace SharpEngineCore.Graphics;

internal sealed class GraphicsObject
{
    public readonly Guid Id;
    public readonly ConstantBuffer TransformConstantBuffer;

    public GraphicsObject(ConstantBuffer transformConstantBuffer,
        Guid id)
    {
        TransformConstantBuffer = transformConstantBuffer;
        Id = id;
    }
}