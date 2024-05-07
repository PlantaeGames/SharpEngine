using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal sealed class RenderTargetView : ResourceView
{
    private readonly ComPtr<ID3D11RenderTargetView> _pView;

    public ComPtr<ID3D11RenderTargetView> GetNativePtr() => new(_pView);

    public RenderTargetView(ComPtr<ID3D11RenderTargetView> pView) :
        base()
    {
        _pView = new(pView);
    }
}