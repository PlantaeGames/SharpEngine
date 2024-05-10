using TerraFX.Interop.DirectX;

namespace SharpEngineCore.Graphics;

public readonly struct TextureInfo(Size size, 
    Channels channels,
    ResourceUsageInfo usageInfo)
{
    public readonly Size Size { get; init; } = size;
    public Channels Channels { get; init; } = channels;
    public readonly ResourceUsageInfo UsageInfo { get; init; } = usageInfo;
}
