using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineEditor.ImGui.Backend;

public sealed class Blob(ComPtr<ID3DBlob> pBlob)
{
    private readonly ComPtr<ID3DBlob> _ptr = new(pBlob);

    internal ComPtr<ID3DBlob> GetNativePtr() => new(_ptr);
}
