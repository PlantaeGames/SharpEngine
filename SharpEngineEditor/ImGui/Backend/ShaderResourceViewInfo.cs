namespace SharpEngineEditor.ImGui.Backend;

internal readonly struct ShaderResourceViewInfo
{
    public readonly ViewCreationInfo ViewInfo { get; init; }
    public readonly ResourceViewInfo ResourceViewInfo { get; init; }
}
