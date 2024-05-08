using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal class Buffer(ComPtr<ID3D11Buffer> pBuffer, BufferInfo info) :
    Resource(ComUtilities.ToResourceNativePtr(ref pBuffer))
{
    public readonly BufferInfo Info = info;
    private readonly ComPtr<ID3D11Buffer> _ptr = new(pBuffer);

    public ComPtr<ID3D11Buffer> GetNativePtr() => new(_ptr);

    public static int UnitSize => Surface.GetPeiceSize();
}
