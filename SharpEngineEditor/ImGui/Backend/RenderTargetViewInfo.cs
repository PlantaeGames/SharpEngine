namespace SharpEngineEditor.ImGui.Backend;

internal readonly struct RenderTargetViewInfo
{
    public readonly ViewCreationInfo ViewInfo { get; init; }
    public readonly ResourceViewInfo ResourceViewInfo { get; init; }
}
