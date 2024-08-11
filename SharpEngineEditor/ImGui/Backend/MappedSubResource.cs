using TerraFX.Interop.DirectX;

namespace SharpEngineEditor.ImGui.Backend;
internal readonly struct MappedSubResource
{
    public readonly D3D11_MAPPED_SUBRESOURCE Map { get; init; }
    public readonly MapInfo Info { get; init; }

    public MappedSubResource(D3D11_MAPPED_SUBRESOURCE map,
        MapInfo info)
    {
        Map = map;
        Info = info;
    }
}