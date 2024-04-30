using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal abstract class Resource
{
    protected Resource ()
    { }
}


internal abstract class ResourceView
{ 
    protected ResourceView()
    { }
}

internal sealed class RenderTargetView : ResourceView
{
    private ComPtr<ID3D11RenderTargetView> _pView;

    public RenderTargetView(ComPtr<ID3D11RenderTargetView> pView) :
        base()
    {
        _pView = pView;
    }
}