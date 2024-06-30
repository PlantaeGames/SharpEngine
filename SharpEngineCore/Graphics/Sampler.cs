using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

public sealed class Sampler
{
    public readonly SamplerInfo Info;

    private readonly ComPtr<ID3D11SamplerState> _ptr;
    private readonly Device _device;

    public bool IsValid()
    {
        unsafe
        {
            return _ptr.Get() != (ID3D11SamplerState*)IntPtr.Zero;
        }
    }

    public ComPtr<ID3D11SamplerState> GetNativePtr() => new(_ptr);

    internal Sampler(ComPtr<ID3D11SamplerState> pSampler, SamplerInfo info, 
        Device device)
    {
        Info = info;

        _ptr = new(pSampler);
        _device = device;
    }
}
