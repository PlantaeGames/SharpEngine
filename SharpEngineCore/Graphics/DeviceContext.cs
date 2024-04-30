using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal abstract class DeviceContext
{
    protected readonly ComPtr<ID3D11DeviceContext> _pContext;

    public DeviceContext(ComPtr<ID3D11DeviceContext> pDeviceContext)
    {
        _pContext = pDeviceContext;
    }
}

internal sealed class ImmediateDeviceContext : DeviceContext
{
    public ImmediateDeviceContext(ComPtr<ID3D11DeviceContext> pDeviceContext)
        : base(pDeviceContext) { }
}

