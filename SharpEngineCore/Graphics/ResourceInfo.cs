namespace SharpEngineCore.Graphics;

internal readonly struct ResourceInfo
{
    public readonly Size Size { get; init; }

    public ResourceInfo(Size size)
    {
        Size = size;
    }
}
