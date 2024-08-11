using TerraFX.Interop.DirectX;

namespace SharpEngineEditor.ImGui.Backend;

internal readonly struct TextureCreationInfo()
{
    public readonly int MipLevels { get; init; } = 1;
    public readonly DXGI_FORMAT Format { get; init; }
    public readonly ResourceUsageInfo UsageInfo { get; init; }
}
