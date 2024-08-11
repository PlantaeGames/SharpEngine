namespace SharpEngineEditor.ImGui.Backend;

internal interface IPipelineStage
{
    void Bind(DeviceContext context);
    void Unbind(DeviceContext context);
}
