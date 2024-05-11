using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal sealed class ShaderResourceView
{
    private ComPtr<ID3D11ShaderResourceView> _ptr;
    public ComPtr<ID3D11ShaderResourceView> GetNativePtr() => new(_ptr);
    private Device _device;

    public ShaderResourceView(ComPtr<ID3D11ShaderResourceView> pView, Device device)
    {
        _ptr = new(pView);
        _device = device;
    }
}
