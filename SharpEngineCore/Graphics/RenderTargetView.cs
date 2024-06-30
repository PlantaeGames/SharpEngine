using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal sealed class RenderTargetView : ResourceView
{
    public readonly RenderTargetViewInfo Info;

    private readonly ComPtr<ID3D11RenderTargetView> _pView;
    public ComPtr<ID3D11RenderTargetView> GetNativePtr() => new(_pView);

    public RenderTargetView(ComPtr<ID3D11RenderTargetView> pView,
        Resource resource,
        RenderTargetViewInfo info,
        Device device) :
        base(resource, info.ResourceViewInfo, device)
    {
        Info = info;

        _pView = new(pView);
    }
}