using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal sealed class VertexShader : Shader
{
    private readonly ComPtr<ID3D11VertexShader> _ptr;

    public ComPtr<ID3D11VertexShader> GetNativePtr() => new(_ptr);

    public VertexShader(ComPtr<ID3D11VertexShader> pShader, Blob blob, Device device) :
        base(blob, device)
    { 
        _ptr = new(pShader);
    }
}
