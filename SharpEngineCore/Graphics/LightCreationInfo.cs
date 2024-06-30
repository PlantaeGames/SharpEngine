namespace SharpEngineCore.Graphics;

internal sealed class LightCreationInfo
{
    public readonly FColor4 Position;
    public readonly FColor4 Rotation;
    public readonly FColor4 Color;
    public readonly FColor4 AmbientColor;

    public LightCreationInfo(FColor4 position, FColor4 rotation,
        FColor4 color, FColor4 ambientColor)
    {
        Color = color;
        Position = position;
        Rotation = rotation;
        AmbientColor = ambientColor;
    }
}
