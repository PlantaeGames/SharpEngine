using TerraFX.Interop.DirectX;

namespace SharpEngineEditor.ImGui.Backend;

public enum Topology
{
    None = D3D_PRIMITIVE_TOPOLOGY.D3D11_PRIMITIVE_TOPOLOGY_UNDEFINED,
    TriangleList = D3D_PRIMITIVE_TOPOLOGY.D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST
}