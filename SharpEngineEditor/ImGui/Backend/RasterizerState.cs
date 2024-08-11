using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineEditor.ImGui.Backend;

internal sealed class RasterizerState
{
    private readonly ComPtr<ID3D11RasterizerState> _ptr;
    private readonly Device _device;
    public ComPtr<ID3D11RasterizerState> GetNativePtr() => new(_ptr);
    public readonly RasterizerStateInfo Info;

    public RasterizerState(ComPtr<ID3D11RasterizerState> pState, 
        RasterizerStateInfo info,
        Device device)
    {
        _ptr = new(pState);
        _device = device;
        Info = info;
    }
}
