using TerraFX.Interop.DirectX;

namespace SharpEngineCore.Graphics;

public readonly struct ResourceUsageInfo(D3D11_USAGE usage,
                          DXGI_FORMAT format,
                          D3D11_BIND_FLAG bindFlags,
                          D3D11_CPU_ACCESS_FLAG cpuAccessFlags)
{
    public readonly D3D11_USAGE Usage { get; init; } = usage;
    public readonly DXGI_FORMAT Format { get; init; }  = format;
    public readonly D3D11_BIND_FLAG BindFlags { get; init; } = bindFlags;
    public readonly D3D11_CPU_ACCESS_FLAG CPUAccessFlags { get; init; } = cpuAccessFlags;
}
