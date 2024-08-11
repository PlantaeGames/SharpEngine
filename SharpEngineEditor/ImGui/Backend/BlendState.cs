using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineEditor.ImGui.Backend;

internal sealed class BlendState
{
    public readonly BlendStateInfo Info;

    private readonly ComPtr<ID3D11BlendState> _ptr;
    private readonly Device _device;

    public ComPtr<ID3D11BlendState> GetNativePtr() => new(_ptr);

    public BlendState(ComPtr<ID3D11BlendState> ptr, Device device,
                      BlendStateInfo info)
    {
        Info = info;

        _ptr = new(ptr);
        _device = device;
    }
}
