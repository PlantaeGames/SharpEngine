using TerraFX.Interop.DirectX;

namespace SharpEngineEditor.ImGui.Backend;

internal readonly struct RasterizerStateInfo
{
    public readonly D3D11_CULL_MODE CullMode { get; init; }
    public readonly D3D11_FILL_MODE FillMode { get; init; }
    public readonly bool ScissorsEnabled { get; init; }
    public readonly bool DepthClippingEnabled { get; init; }
    public readonly bool FrontFaceCounterClockwise { get; init; }
}
