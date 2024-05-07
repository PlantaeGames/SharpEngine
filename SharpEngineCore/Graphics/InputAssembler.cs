using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal sealed class InputAssembler
{
    public readonly D3D_PRIMITIVE_TOPOLOGY Topology =
        D3D_PRIMITIVE_TOPOLOGY.D3D10_PRIMITIVE_TOPOLOGY_TRIANGLELIST;

    public InputAssembler()
    {

    }
}

public sealed class PixelShader : Shader
{
    private readonly ComPtr<ID3D11PixelShader> _ptr;

    public ComPtr<ID3D11PixelShader> GetNativePtr() => new(_ptr);

    public PixelShader(ComPtr<ID3D11PixelShader> pShader) :
        base()
    {
        _ptr = new(pShader);
    }
}

public sealed class VertexShader : Shader
{
    private readonly ComPtr<ID3D11VertexShader> _ptr;

    public ComPtr<ID3D11VertexShader> GetNativePtr() => new(_ptr);

    public VertexShader(ComPtr<ID3D11VertexShader> pShader) :
        base()
    { 
        _ptr = new(pShader);
    }
}

public abstract class Shader
{ 
    protected Shader()
    { }
}
