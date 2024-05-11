using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal sealed class DepthStencilState
{
    public readonly DepthStencilStateInfo Info;

    private readonly ComPtr<ID3D11DepthStencilState> _ptr;
    private readonly Device _device;
    public ComPtr<ID3D11DepthStencilState> GetNativePtr() => new(_ptr);

    public DepthStencilState(ComPtr<ID3D11DepthStencilState> pState, 
        DepthStencilStateInfo info,
        Device device)
    {
        _ptr = new(_ptr);
        _device = device;
        Info = info;
    }
}

