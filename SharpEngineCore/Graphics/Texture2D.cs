using System.Diagnostics;

using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal sealed class Texture2D : Texture
{
    private readonly ComPtr<ID3D11Texture2D> _pTexture;

    public ComPtr<ID3D11Texture2D> GetNativePtr() => _pTexture;

    public Texture2D(ComPtr<ID3D11Texture2D> pTexture2D, TextureInfo info)
        : base(info, ComUtilities.ToResourceNativePtr(pTexture2D))
    { 
        _pTexture = pTexture2D;
    }
}
