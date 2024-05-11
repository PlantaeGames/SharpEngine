using System.Diagnostics;

using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal abstract class Texture(TextureInfo info, ComPtr<ID3D11Resource> pResource,
    Device device) : 
    Resource(pResource, new (info.Size, info.UsageInfo) ,device)
{
    public readonly TextureInfo Info = info;

    public void Update(Surface surface)
    {
        Debug.Assert(surface.Channels == Info.Channels,
            "Surface and Textures Channels Does not match.");

        Write(surface, 0);
    }
}