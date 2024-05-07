using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal sealed class Blob(ComPtr<ID3DBlob> pBlob)
{
    private readonly ComPtr<ID3DBlob> _ptr = new(pBlob);

    public ComPtr<ID3DBlob> GetNativePtr() => new(_ptr);
}
