using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal sealed class InputLayout
{
    public readonly InputLayoutInfo Info;
    private readonly ComPtr<ID3D11InputLayout> _ptr;
    private readonly Device _device;

    public ComPtr<ID3D11InputLayout> GetNativePtr() => new(_ptr);

    public InputLayout(ComPtr<ID3D11InputLayout> pInputLayout,
                       InputLayoutInfo info,
                        Device device)
    {
        _ptr = new(pInputLayout);
        Info = info;
        _device = device;
    }
}
