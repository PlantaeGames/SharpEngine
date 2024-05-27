namespace SharpEngineCore.Graphics;

public sealed class Cubemap
{
    private readonly Texture2D _texture;
    public readonly CubemapInfo Info;

    public Texture2D GetInternalTexture() => _texture;

    public Cubemap(Texture2D texture, CubemapInfo info)
    {
        _texture = texture;
        Info = info;
    }
}
