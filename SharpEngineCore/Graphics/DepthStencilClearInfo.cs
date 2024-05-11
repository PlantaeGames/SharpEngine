using TerraFX.Interop.DirectX;

namespace SharpEngineCore.Graphics;

internal readonly struct DepthStencilClearInfo
{
    public readonly D3D11_CLEAR_FLAG ClearFlags { get; init; }
    public readonly float Depth { get; init; }
    public readonly byte Stencil { get; init; }
}
