namespace SharpEngineCore.Graphics;

public sealed class GraphicsObject : Object
{
    private readonly List<PipelineVariation> _variations = new();

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

    internal GraphicsObject() :
        base()
    {}
}