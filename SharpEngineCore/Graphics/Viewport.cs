using TerraFX.Interop.DirectX;

namespace SharpEngineCore.Graphics;

public readonly struct Viewport
{
    public readonly D3D11_VIEWPORT Info { get; init; }

    public Viewport(D3D11_VIEWPORT info)
    {
        Info = info;
    }

    public Viewport()
    { }
}
