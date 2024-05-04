using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal interface IViewable
{
    public ComPtr<ID3D11Resource> GetNativePtrAsResource();
}