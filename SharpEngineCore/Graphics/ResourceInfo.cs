namespace SharpEngineCore.Graphics;

public readonly struct ResourceInfo
{
    public readonly int SubresourceCount { get; init; }
    public readonly Size SurfaceSize { get; init; }
    public readonly ResourceUsageInfo UsageInfo { get; init; }

    public ResourceInfo(int subResourceCount, Size size, ResourceUsageInfo usageInfo)
    {
        SubresourceCount = subResourceCount;
        SurfaceSize = size;
        UsageInfo = usageInfo;
    }
}
