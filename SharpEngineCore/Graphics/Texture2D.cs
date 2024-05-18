using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

public sealed class Texture2D : Texture
{
    private readonly ComPtr<ID3D11Texture2D> _pTexture;

    public ComPtr<ID3D11Texture2D> GetNativePtr() => new(_pTexture);

    internal Texture2D(ComPtr<ID3D11Texture2D> pTexture2D, TextureInfo info, Device device)
        : base(info, ComUtilities.ToResourceNativePtr(ref pTexture2D), device)
    { 
        _pTexture = new(pTexture2D);
    }
}