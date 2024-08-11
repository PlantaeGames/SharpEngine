using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineEditor.ImGui.Backend;

internal sealed class DepthStencilView : ResourceView
{
    public readonly DepthStencilViewInfo Info;

    private readonly ComPtr<ID3D11DepthStencilView> _ptr;
    public ComPtr<ID3D11DepthStencilView> GetNativePtr() => new(_ptr);

    public DepthStencilView(ComPtr<ID3D11DepthStencilView> pView,
        Resource resource,
        DepthStencilViewInfo info,
        Device device) :
        base(resource, info.ResourceViewInfo, device)
    {
        Info = info;

        _ptr = new(pView);
    }
}

