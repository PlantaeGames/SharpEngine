namespace SharpEngineCore.Graphics;

public readonly struct ResourceInfo
{
    public readonly Size Size { get; init; }
    public readonly ResourceUsageInfo UsageInfo { get; init; }

    public ResourceInfo(Size size, ResourceUsageInfo usageInfo)
    {
        Size = size;
        UsageInfo = usageInfo;
    }
}
