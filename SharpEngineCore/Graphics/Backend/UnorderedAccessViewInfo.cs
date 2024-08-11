using TerraFX.Interop.DirectX;

namespace SharpEngineCore.Graphics;

internal readonly struct UnorderedAccessViewInfo
{
    public readonly ViewCreationInfo ViewInfo { get; init; }
    public readonly ResourceViewInfo ResourceViewInfo { get; init; }
}
