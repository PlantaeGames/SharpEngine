using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal sealed class Sampler
{
    private Texture _texture;
    private ComPtr<ID3D11SamplerState> _ptr;
    private Device _device;

    public ComPtr<ID3D11SamplerState> GetNativePtr() => new(_ptr);

    public Sampler(ComPtr<ID3D11SamplerState> pSampler, Texture texture, Device device)
    {
        _texture = texture;

        _ptr = new(pSampler);
        _device = device;
    }
}
