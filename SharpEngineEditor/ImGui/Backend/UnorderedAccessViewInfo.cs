using TerraFX.Interop.DirectX;

namespace SharpEngineEditor.ImGui.Backend;

internal readonly struct UnorderedAccessViewInfo
{
    public readonly ViewCreationInfo ViewInfo { get; init; }
    public readonly ResourceViewInfo ResourceViewInfo { get; init; }
}
