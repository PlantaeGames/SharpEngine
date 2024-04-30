using TerraFX.Interop.DirectX;
using static TerraFX.Interop.DirectX.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal sealed class Device
{
    private ComPtr<ID3D11Device> _pDevice;
    private DeviceContext _context;

    private D3D_FEATURE_LEVEL _featureLevel;

    public Texture2D CreateTexture2D(Surface surface, TextureInfo info)
    {
        return new Texture2D(NativeCreateTexture2D(), info);

        unsafe ComPtr<ID3D11Texture2D> NativeCreateTexture2D()
        {
            var desc = new D3D11_TEXTURE2D_DESC();
            desc.Width = (uint)surface.Size.Width;
            desc.Height = (uint)surface.Size.Height;
            desc.MipLevels = 1u;
            desc.ArraySize = 1u;
            desc.SampleDesc = new DXGI_SAMPLE_DESC
            {
                Quality = 0u,
                Count = 1u
            };
            desc.MiscFlags = 0u;
            desc.Format = info.UsuageInfo.Format;
            desc.Usage = info.UsuageInfo.Usage;
            desc.BindFlags = info.UsuageInfo.BindFlags;
            desc.CPUAccessFlags = info.UsuageInfo.CPUAccessFlags;

            var initialData = new D3D11_SUBRESOURCE_DATA();
            initialData.pSysMem = surface.GetNativePointer().ToPointer();
            initialData.SysMemPitch = surface.GetSliceSize();

            fixed (ID3D11Device** ppDevice = _pDevice)
            {
                var pTexture2D = new ComPtr<ID3D11Texture2D>();
                fixed (ID3D11Texture2D** ppTexture2D = pTexture2D)
                {
                    GraphicsException.SetInfoQueue();
                    var result = (*ppDevice)->CreateTexture2D(&desc, 
                        &initialData, ppTexture2D);

                    if(result.FAILED)
                    {
                        // error here
                        throw GraphicsException.GetLastGraphicsException(
                            new GraphicsException($"Failed to create texture 2d\nError Code: {result}"));
                    }
                }

                return pTexture2D;
            }
        }
    }

    public ComPtr<ID3D11Device> GetNativePtr() => _pDevice;

    public ImmediateDeviceContext GetContext() => (ImmediateDeviceContext) _context;
    public D3D_FEATURE_LEVEL GetFeatureLevel() => _featureLevel;

    public Device(Adapter adapter, bool isEnumalted = false)
    {
        Create(adapter, isEnumalted);
    }

    private void Create(Adapter adaper, bool isEnumalted)
    {
        NativeCreate();

        unsafe void NativeCreate()
        {
            fixed(ID3D11Device** ppDevice = _pDevice)
            {
                var pImContext = new ComPtr<ID3D11DeviceContext>();
                fixed(ID3D11DeviceContext** ppDeviceContext = pImContext)
                {
                    fixed (IDXGIAdapter** ppAdapter = adaper.GetNativePtr())
                    {
                        D3D_FEATURE_LEVEL featureLevel;
                        D3D_DRIVER_TYPE driverType = isEnumalted ?
                                                     D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_SOFTWARE :
                                                     D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_UNKNOWN;
                        uint flags = 0u;
#if DEBUG
                        flags |= (uint)D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_DEBUG;
#endif

                        GraphicsException.SetInfoQueue();
                        var result = D3D11CreateDevice(*ppAdapter, driverType,
                            (HMODULE)IntPtr.Zero, flags, (D3D_FEATURE_LEVEL*)IntPtr.Zero, 0u, 
                            D3D11.D3D11_SDK_VERSION, ppDevice, &featureLevel, ppDeviceContext);

                        if (result.FAILED)
                        {
                            // error here.
                            throw GraphicsException.GetLastGraphicsException(
                                new GraphicsException($"Failed to create device\nError Code: {result}"));
                        }

                        _featureLevel = featureLevel;
                    }
                }

                _context = new ImmediateDeviceContext(pImContext);
            }
        }
    }
}