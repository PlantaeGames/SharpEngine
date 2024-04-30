using System.Diagnostics;

using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal sealed class Texture2D : Texture
{
    private ComPtr<ID3D11Texture2D> _pTexture;

    public ComPtr<ID3D11Texture2D> GetNativePtr() => _pTexture;

    public override ComPtr<ID3D11Resource> GetNativeResourcePtr()
    {
        return NativeGetNativeResourcePtr();

        unsafe ComPtr<ID3D11Resource> NativeGetNativeResourcePtr()
        {
            var ptr = new ComPtr<ID3D11Resource>();
            var result = GetNativePtr().As(&ptr);

            Debug.Assert(result.FAILED == false,
                "Failed to query ID3D11Resource Interface from ID3D11Texture2D.");

            return ptr;
        }
    }

    public Texture2D(ComPtr<ID3D11Texture2D> pTexture2D, TextureInfo info)
        : base(info)
    { 
        _pTexture = pTexture2D;      
    }
}
