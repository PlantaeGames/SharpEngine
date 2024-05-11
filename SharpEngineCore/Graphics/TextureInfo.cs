using TerraFX.Interop.DirectX;

namespace SharpEngineCore.Graphics;

public readonly struct TextureInfo(Size size, 
    Channels channels,
    DXGI_FORMAT format,
    ResourceUsageInfo usageInfo)
{
    public readonly Size Size { get; init; } = size;
    public readonly Channels Channels { get; init; } = channels;
    public readonly DXGI_FORMAT Format { get; init; } = format;
    public readonly ResourceUsageInfo UsageInfo { get; init; } = usageInfo;
}
