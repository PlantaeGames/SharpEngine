using TerraFX.Interop.DirectX;
using static TerraFX.Interop.DirectX.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal sealed class Device
{
    private ComPtr<ID3D11Device> _pDevice;
    private DeviceContext _context;

    private D3D_FEATURE_LEVEL _featureLevel;

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