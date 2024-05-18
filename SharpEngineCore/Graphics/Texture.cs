using System.Diagnostics;

using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

public abstract class Texture :
    Resource
{
    public readonly TextureInfo Info;

    internal Texture(TextureInfo info, ComPtr<ID3D11Resource> pResource,
        Device device) : base(pResource, new (info.Size, info.UsageInfo) ,device)
    {
        Info = info;
    }

    public void Update(Surface surface)
    {
        Debug.Assert(surface.Channels == Info.Channels,
            "Surface and Textures Channels Does not match.");

        Write(surface, 0);
    }
}