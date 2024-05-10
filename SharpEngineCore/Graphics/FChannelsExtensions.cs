using TerraFX.Interop.DirectX;

namespace SharpEngineCore.Graphics;

public static class ChannelsExtensions
{
    public static DXGI_FORMAT ToUFormat(this Channels channels)
    => channels switch
    {
        Channels.Single => DXGI_FORMAT.DXGI_FORMAT_R32_SINT,
        Channels.Double => DXGI_FORMAT.DXGI_FORMAT_R32G32_SINT,
        Channels.Triple => DXGI_FORMAT.DXGI_FORMAT_R32G32B32_SINT,
        Channels.Quad => DXGI_FORMAT.DXGI_FORMAT_R32G32B32_SINT,
        _ => DXGI_FORMAT.DXGI_FORMAT_UNKNOWN,
    };

    public static DXGI_FORMAT ToFFormat(this Channels channels)
        => channels switch
        {
            Channels.Single => DXGI_FORMAT.DXGI_FORMAT_R32_FLOAT,
            Channels.Double => DXGI_FORMAT.DXGI_FORMAT_R32G32_FLOAT,
            Channels.Triple => DXGI_FORMAT.DXGI_FORMAT_R32G32B32_FLOAT,
            Channels.Quad => DXGI_FORMAT.DXGI_FORMAT_R32G32B32_FLOAT,
            _ => DXGI_FORMAT.DXGI_FORMAT_UNKNOWN,
        };
}
