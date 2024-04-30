using TerraFX.Interop.DirectX;

namespace SharpEngineCore.Graphics;

internal struct ResourceUsuageInfo
{
    public DXGI_FORMAT Format = DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;
    public D3D11_USAGE Usage = D3D11_USAGE.D3D11_USAGE_IMMUTABLE;
    public uint BindFlags = (uint) D3D11_BIND_FLAG.D3D11_BIND_SHADER_RESOURCE;
    public uint CPUAccessFlags = 0u;

    public ResourceUsuageInfo()
    { }
}
