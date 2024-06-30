namespace SharpEngineCore.Graphics;

public sealed class GraphicsObject : Object
{
    public readonly GraphicsInfo Info;

    private readonly List<PipelineVariation> _variations = new();
    private ConstantBuffer _transformBuffer;

    public TransformConstantData Transform { get; private set; }

    public void UpdateTransform(TransformConstantData data)
    {
        _transformBuffer.Update(data);
        Transform = data;
    }

    internal void SetTransformBuffer(ConstantBuffer transformBuffer)
    {
        _transformBuffer = transformBuffer;
    }

    internal ConstantBuffer GetTransformBuffer() => _transformBuffer;

    internal void AddVariation(PipelineVariation variation)
    {
        _variations.Add(variation);
    }

    protected override void OnPause()
    {
        foreach (var variation in _variations)
        {
            variation.State = State.Paused;
        }
    }
    protected override void OnResume()
    {
        foreach (var variation in _variations)
        {
            variation.State = State.Active;
        }
    }
    protected override void OnRemove()
    {
        foreach (var variation in _variations)
        {
            variation.State = State.Expired;
        }
    }

    internal GraphicsObject(GraphicsInfo info) :
        base()
    {
        Info = info;
    }
}