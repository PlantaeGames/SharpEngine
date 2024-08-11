using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineEditor.ImGui.Backend;

internal sealed class PixelShader : Shader
{
    private readonly ComPtr<ID3D11PixelShader> _ptr;

    public ComPtr<ID3D11PixelShader> GetNativePtr() => new(_ptr);


    public PixelShader(ComPtr<ID3D11PixelShader> pShader, Blob blob, Device device) :
        base(blob, device)
    {
        _ptr = new(pShader);
    }
}
