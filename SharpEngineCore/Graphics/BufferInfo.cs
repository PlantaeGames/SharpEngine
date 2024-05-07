namespace SharpEngineCore.Graphics;

internal readonly struct BufferInfo(int size, ResourceUsageInfo usageInfo)
{
    public readonly int Size { get; init; } = size;
    public readonly ResourceUsageInfo UsageInfo { get; init; } = usageInfo;
}
