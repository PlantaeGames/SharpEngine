namespace SharpEngineCore.Graphics;

public readonly struct ResourceInfo
{
    public readonly Size SurfaceSize { get; init; }
    public readonly ResourceUsageInfo UsageInfo { get; init; }

    public ResourceInfo(Size size, ResourceUsageInfo usageInfo)
    {
        SurfaceSize = size;
        UsageInfo = usageInfo;
    }
}
