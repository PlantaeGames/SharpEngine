using TerraFX.Interop.DirectX;

namespace SharpEngineEditor.ImGui.Backend;

internal readonly struct DepthStencilStateInfo
{
    public readonly bool DepthEnabled { get; init; }
    public readonly D3D11_DEPTH_WRITE_MASK DepthWriteMask { get; init; }
    public readonly D3D11_COMPARISON_FUNC DepthComparisionFunc { get; init; }
    public readonly bool StencilEnable { get; init; }
    public readonly byte StencilReadMask { get; init; }
    public readonly byte StencilWriteMask { get; init; }
    public readonly D3D11_DEPTH_STENCILOP_DESC FrontFace { get; init; }
    public readonly D3D11_DEPTH_STENCILOP_DESC BackFace { get; init; }
}