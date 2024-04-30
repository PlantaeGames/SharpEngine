using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal struct ResourceUsuageInfo
{
    public DXGI_FORMAT Format = DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;
    public D3D11_USAGE Usage = D3D11_USAGE.D3D11_USAGE_DEFAULT;
    public uint BindFlags = 0u;
    public uint CPUAccessFlags = 0u;

    public ResourceUsuageInfo()
    { }
}

internal struct TextureInfo
{
    public Size Size = new();
    public ResourceUsuageInfo UsuageInfo = new();

    public TextureInfo()
    { }
}

internal sealed class Texture2D : Texture
{
    private ComPtr<ID3D11Texture2D> _pTexture;
    public TextureInfo TextureInfo { get; private set; }

    public Texture2D(ComPtr<ID3D11Texture2D> pTexture2D, TextureInfo info)
        : base()
    { 
        _pTexture = pTexture2D;
        TextureInfo = info;
      
    }
}

internal abstract class Texture : Resource, IViewable
{
    protected Texture() { }
}
