using TerraFX.Interop.DirectX;

namespace SharpEngineEditor.ImGui.Backend;

internal readonly struct SwapchainInfo
{ 
    public readonly DXGI_FORMAT Format { get; init; }
}
