using TerraFX.Interop.DirectX;

namespace SharpEngineCore.Graphics;

internal readonly struct MapInfo
{
    public readonly D3D11_MAP MapType { get; init; }
    public readonly int SubResourceIndex { get; init; }


    public MapInfo(D3D11_MAP mapType, int subIndex) : this()
    {
        MapType = mapType;
        SubResourceIndex = subIndex;
    }

    public MapInfo()
    { }

}