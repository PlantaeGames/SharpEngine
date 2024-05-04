using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal abstract class Buffer : Resource
{
    protected Buffer(ComPtr<ID3D11Buffer> pBuffer) :
        base(ComUtilities.ToResourceNativePtr(pBuffer))
    { }
}
