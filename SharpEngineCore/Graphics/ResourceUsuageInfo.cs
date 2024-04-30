using TerraFX.Interop.DirectX;

namespace SharpEngineCore.Graphics;

internal struct ResourceUsuageInfo
{
    public DXGI_FORMAT Format = DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;
    public D3D11_USAGE Usage = D3D11_USAGE.D3D11_USAGE_DEFAULT;
    public uint BindFlags = 0u;
    public uint CPUAccessFlags = 0u;

    public ResourceUsuageInfo()
    { }
}
