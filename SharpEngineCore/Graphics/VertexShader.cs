using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

public sealed class VertexShader : Shader
{
    private readonly ComPtr<ID3D11VertexShader> _ptr;

    public ComPtr<ID3D11VertexShader> GetNativePtr() => new(_ptr);

    public VertexShader(ComPtr<ID3D11VertexShader> pShader, Blob blob) :
        base(blob)
    { 
        _ptr = new(pShader);
    }
}
