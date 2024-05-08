using TerraFX.Interop.DirectX;

namespace SharpEngineCore.Graphics;

internal sealed class InputLayoutInfo
{
    public D3D_PRIMITIVE_TOPOLOGY Topology { get; init; }
    public IFragmentable Layout { get; init; }
    public VertexShader VertexShader { get; init; }

    public InputLayoutInfo()
    { }

    public InputLayoutInfo(
        D3D_PRIMITIVE_TOPOLOGY topology,
        IFragmentable layout,
        VertexShader vertexShader)
    {
        Topology = topology;
        Layout = layout;
        VertexShader = vertexShader;
    }
}
