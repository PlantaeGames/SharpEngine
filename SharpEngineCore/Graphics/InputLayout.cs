using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal sealed class InputLayout
{
    private readonly InputLayoutInfo _info;
    private readonly ComPtr<ID3D11InputLayout> _ptr;

    public ComPtr<ID3D11InputLayout> GetNativePtr() => new(_ptr);

    public InputLayout(ComPtr<ID3D11InputLayout> pInputLayout,
                       InputLayoutInfo info)
    {
        _ptr = new(pInputLayout);
        _info = info;
    }
}
