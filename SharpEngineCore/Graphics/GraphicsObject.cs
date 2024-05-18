namespace SharpEngineCore.Graphics;

public sealed class GraphicsObject : Object
{
    private readonly ConstantBuffer _transformConstantData;

    internal readonly PipelineVariation _variation;

    public void Update(TransformConstantData transform)
    {
        _transformConstantData.Update(transform);
    }

    protected override void OnPause()
    { }
    protected override void OnResume()
    { }
    protected override void OnRemove()
    { }

    internal GraphicsObject(Guid id, ConstantBuffer transformBuffer, PipelineVariation variation) :
        base(id)
    {
        _transformConstantData = transformBuffer;
        _variation = variation;
    }
}