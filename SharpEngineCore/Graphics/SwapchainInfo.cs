using TerraFX.Interop.DirectX;

namespace SharpEngineCore.Graphics;

internal readonly struct SwapchainInfo
{ 
    public readonly DXGI_FORMAT Format { get; init; }
}
