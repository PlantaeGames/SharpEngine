namespace SharpEngineCore.Graphics;

public readonly struct BufferInfo(Size size, Type layout, ResourceUsageInfo usageInfo)
{
    public readonly Type Layout { get; init; } = layout;
    public readonly Size Size { get; init; } = size;
    public readonly ResourceUsageInfo UsageInfo { get; init; } = usageInfo;
}
