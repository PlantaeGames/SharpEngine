using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineEditor.ImGui.Backend;

internal class UnorderedAccessView : ResourceView
{
    public readonly UnorderedAccessViewInfo Info;

    private readonly ComPtr<ID3D11UnorderedAccessView> _ptr;
    public ComPtr<ID3D11UnorderedAccessView> GetNativePtr() => new(_ptr);

    public UnorderedAccessView(ComPtr<ID3D11UnorderedAccessView> pView,
        Resource resource,
        UnorderedAccessViewInfo info, Device device) :
        base(resource, info.ResourceViewInfo, device)
    {
        Info = info;

        _ptr = new(pView);
    }
}