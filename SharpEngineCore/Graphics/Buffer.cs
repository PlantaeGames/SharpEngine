using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal class Buffer(ComPtr<ID3D11Buffer> pBuffer, BufferInfo info) :
    Resource(ComUtilities.ToResourceNativePtr(pBuffer))
{
    public readonly BufferInfo Info = info;
    protected readonly ComPtr<ID3D11Buffer> _ptr = pBuffer;

    public ComPtr<ID3D11Buffer> GetNativePtr() => _ptr;
}
