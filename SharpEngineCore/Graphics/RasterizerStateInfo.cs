using TerraFX.Interop.DirectX;

namespace SharpEngineCore.Graphics;

internal readonly struct RasterizerStateInfo
{
    public readonly D3D11_CULL_MODE CullMode { get; init; }
    public readonly D3D11_FILL_MODE FillMode { get; init; }
    public readonly bool DepthClippingEnabled { get; init; }
    public readonly bool FrontFaceCounterClockwise { get; init; }
}
