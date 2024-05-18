using TerraFX.Interop.DirectX;

namespace SharpEngineCore.Graphics;

internal readonly struct UnorderedAccessViewInfo
{
    public enum Type
    {
        None,
        Buffer,
        Texture2D
    }

    public readonly Type ViewType { get; init; }
    public readonly Size Size { get; init; }
    public readonly DXGI_FORMAT Format { get; init; }
    public readonly D3D11_BUFFER_UAV_FLAG BufferFlags { get; init; }
    public readonly ResourceViewInfo ResourceViewInfo { get; init; }
}
