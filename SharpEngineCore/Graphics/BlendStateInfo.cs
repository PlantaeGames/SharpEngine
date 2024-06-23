using TerraFX.Interop.DirectX;

namespace SharpEngineCore.Graphics;

internal readonly struct BlendStateInfo
{
    public readonly bool AplhaToCoverageEnable { get; init; }
    public readonly bool IndependentBlendEnabled { get; init; }

    /// <remarks>
    /// The array will be initialized by constructor to fixed length.
    /// </remarks>
    public readonly D3D11_RENDER_TARGET_BLEND_DESC[] RenderTargetBlendDescs { get; init; }

    public BlendStateInfo()
    {
        RenderTargetBlendDescs = new D3D11_RENDER_TARGET_BLEND_DESC[8];
    }
}
