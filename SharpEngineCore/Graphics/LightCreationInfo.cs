namespace SharpEngineCore.Graphics;

internal sealed class LightCreationInfo
{
    public readonly FColor4 Position;
    public readonly FColor4 Rotation;
    public readonly FColor4 Color;

    public LightCreationInfo(FColor4 color, FColor4 rotation, FColor4 position)
    {
        Color = color;
        Position = position;
        Rotation = rotation;
    }
}
