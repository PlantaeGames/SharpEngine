using TerraFX.Interop.DirectX;

namespace SharpEngineCore.Graphics;

internal readonly struct SamplerInfo
{ 
    public readonly D3D11_FILTER Filter { get; init; }

    public readonly D3D11_TEXTURE_ADDRESS_MODE AddressMode { get; init; }

    public SamplerInfo(D3D11_FILTER filter, D3D11_TEXTURE_ADDRESS_MODE addressMode) : this()
    {
        AddressMode = addressMode;
        Filter = filter;
    }

    public SamplerInfo()
    { }
}
