using TerraFX.Interop.DirectX;

namespace SharpEngineCore.Graphics;

public readonly struct TextureInfo()
{
    public readonly int MipLevels { get; init; } = 1;
    public readonly Size Size { get; init; }
    public readonly Channels Channels { get; init; }
    public readonly DXGI_FORMAT Format { get; init; }
    public readonly ResourceUsageInfo UsageInfo { get; init; }
}
