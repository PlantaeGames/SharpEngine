using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineEditor.ImGui.Backend;

internal sealed class ShaderResourceView : ResourceView
{
    public readonly ShaderResourceViewInfo Info;

    private readonly ComPtr<ID3D11ShaderResourceView> _ptr;
    public ComPtr<ID3D11ShaderResourceView> GetNativePtr() => new(_ptr);

    public ShaderResourceView(ComPtr<ID3D11ShaderResourceView> pView, 
        Resource resource,
        ShaderResourceViewInfo info,
        Device device) :
        base(resource, info.ResourceViewInfo, device)
    {
        Info = info;

        _ptr = new(pView);
    }
}
