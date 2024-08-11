using TerraFX.Interop.DirectX;

namespace SharpEngineCore.Graphics;

public readonly struct ResourceUsageInfo(D3D11_USAGE usage,
                          D3D11_BIND_FLAG bindFlags,
                          D3D11_CPU_ACCESS_FLAG cpuAccessFlags,
                          D3D11_RESOURCE_MISC_FLAG miscFlags)
{
    public readonly D3D11_USAGE Usage { get; init; } = usage;
    public readonly D3D11_RESOURCE_MISC_FLAG MiscFlags { get; init; }  = miscFlags;
    public readonly D3D11_BIND_FLAG BindFlags { get; init; } = bindFlags;
    public readonly D3D11_CPU_ACCESS_FLAG CPUAccessFlags { get; init; } = cpuAccessFlags;
}
