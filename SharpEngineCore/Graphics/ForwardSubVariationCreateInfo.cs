namespace SharpEngineCore.Graphics;

internal sealed class ForwardSubVariationCreateInfo
{
    public readonly Guid Id;
    public readonly Mesh Mesh;

    public ForwardSubVariationCreateInfo(Mesh mesh)
    { 
        Mesh = mesh;
        Id = Guid.NewGuid();
    }
}
