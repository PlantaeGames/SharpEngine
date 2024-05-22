using TerraFX.Interop.DirectX;

namespace SharpEngineCore.Graphics;

internal readonly struct TextureCreationInfo()
{
    public readonly int MipLevels { get; init; } = 1;
    public readonly DXGI_FORMAT Format { get; init; }
    public readonly ResourceUsageInfo Usage { get; init; }
}
