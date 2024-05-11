using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal sealed class Sampler
{
    public readonly SamplerInfo Info;

    private readonly Texture _texture;
    private readonly ComPtr<ID3D11SamplerState> _ptr;
    private readonly Device _device;

    public ComPtr<ID3D11SamplerState> GetNativePtr() => new(_ptr);

    public Sampler(ComPtr<ID3D11SamplerState> pSampler, SamplerInfo info, 
        Texture texture, Device device)
    {
        _texture = texture;
        Info = info;

        _ptr = new(pSampler);
        _device = device;
    }
}
