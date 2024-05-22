using TerraFX.Interop.DirectX;

namespace SharpEngineCore.Graphics;

public readonly struct SamplerInfo
{
    public readonly D3D11_FILTER Filter { get; init; }
    public readonly D3D11_TEXTURE_ADDRESS_MODE AddressMode { get; init; }
    public readonly FColor4 BorderColor { get; init; }
}