using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal abstract class Texture(TextureInfo info, ComPtr<ID3D11Resource> pResource)
    : Resource(pResource)
{
    public readonly TextureInfo Info = info;
}