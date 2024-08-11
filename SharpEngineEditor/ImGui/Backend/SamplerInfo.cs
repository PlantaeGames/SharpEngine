using TerraFX.Interop.DirectX;

namespace SharpEngineEditor.ImGui.Backend;

public readonly struct SamplerInfo
{
    public readonly D3D11_FILTER Filter { get; init; }
    public readonly D3D11_COMPARISON_FUNC ComparisionFunc { get; init; }
    public readonly D3D11_TEXTURE_ADDRESS_MODE AddressMode { get; init; }
    public readonly FColor4 BorderColor { get; init; }
}