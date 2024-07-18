namespace SharpEngineCore.Graphics;

public sealed class LightInfo
{
    public static readonly FColor4 Point = new FColor4(0, 0, 0, 0);
    public static readonly FColor4 Directional = new FColor4(1, 0, 0, 0);

    public LightConstantData data;

    public LightInfo() { }
}
