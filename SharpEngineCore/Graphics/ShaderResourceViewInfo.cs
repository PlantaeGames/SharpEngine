namespace SharpEngineCore.Graphics;

internal readonly struct ShaderResourceViewInfo
{
    public readonly ViewCreationInfo ViewInfo { get; init; }
    public readonly ResourceViewInfo ResourceViewInfo { get; init; }
}
