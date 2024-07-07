namespace SharpEngineCore.ECS;

public sealed class Scene
{
    private const string DEFAULT_NAME = "Scene";

    public readonly Guid Id = Guid.NewGuid();
    public string name = DEFAULT_NAME;

    public ECS ECS { get; private set; }

    internal void Tick(TickType tick)
    {
        ECS.Tick(tick);
    }

    public Scene()
    {
        ECS = new();
    }
}
