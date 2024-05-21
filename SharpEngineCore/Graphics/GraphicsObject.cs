namespace SharpEngineCore.Graphics;

public sealed class GraphicsObject : Object
{
    private readonly List<PipelineVariation> _variations = new();
    private readonly ConstantBuffer _transformBuffer;

    public TransformConstantData Transform { get; private set; }

    public void UpdateTransform(TransformConstantData data)
    {
        _transformBuffer.Update(data);
        Transform = data;
    }

    internal void AddVariations(PipelineVariation[] variations)
    {
        _variations.AddRange(variations);
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

    internal GraphicsObject(ConstantBuffer transformBuffer) :
        base()
    {
        _transformBuffer = transformBuffer;
    }
}