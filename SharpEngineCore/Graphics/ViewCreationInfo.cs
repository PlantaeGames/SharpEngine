using TerraFX.Interop.DirectX;

namespace SharpEngineCore.Graphics;

internal readonly struct ViewCreationInfo
{
    public readonly DXGI_FORMAT Format { get; init; }
    public readonly Size Size { get; init; }
}
