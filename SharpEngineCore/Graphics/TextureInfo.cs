namespace SharpEngineCore.Graphics;

public readonly struct TextureInfo(Size size, ResourceUsageInfo usageInfo)
{
    public readonly Size Size { get; init; } = size;
    public readonly ResourceUsageInfo UsageInfo { get; init; } = usageInfo;
}
