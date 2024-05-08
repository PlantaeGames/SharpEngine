using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

public sealed class PixelShader : Shader
{
    private readonly ComPtr<ID3D11PixelShader> _ptr;

    public ComPtr<ID3D11PixelShader> GetNativePtr() => new(_ptr);

    public PixelShader(ComPtr<ID3D11PixelShader> pShader, Blob blob) :
        base(blob)
    {
        _ptr = new(pShader);
    }
}
