using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal abstract class Texture : Resource, IViewable
{
    public TextureInfo TextureInfo { get; private set; }

    protected Texture(TextureInfo info) 
    {
        TextureInfo = info;
    }

    public abstract ComPtr<ID3D11Resource> GetNativeResourcePtr();
}
