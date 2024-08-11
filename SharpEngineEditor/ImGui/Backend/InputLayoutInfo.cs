using TerraFX.Interop.DirectX;

namespace SharpEngineEditor.ImGui.Backend;

internal sealed class InputLayoutInfo
{
    public Topology Topology { get; init; }
    public IFragmentable Layout { get; init; }
    public VertexShader VertexShader { get; init; }

    public InputLayoutInfo()
    { }

    public InputLayoutInfo(
        IFragmentable layout,
        Topology topology,
        VertexShader vertexShader)
    {
        Topology = topology;
        Layout = layout;
        VertexShader = vertexShader;
    }
}
