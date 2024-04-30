using TerraFX.Interop.DirectX;
using static TerraFX.Interop.DirectX.DirectX;
using TerraFX.Interop.Windows;

using SharpEngineCore.Exceptions;
using SharpEngineCore.Utilities;

namespace SharpEngineCore.Graphics;

internal sealed class Device
{
    private ComPtr<ID3D11Device> _pDevice;
    private ComPtr<ID3D11DeviceContext> _pImContext;

    private D3D_FEATURE_LEVEL _featureLevel;

    public D3D_FEATURE_LEVEL GetFeatureLevel() => _featureLevel;

    public Device(Adapter adapter, bool isEnumalted = false)
    {
        _pDevice = new();
        _pImContext = new();

        Create(adapter, isEnumalted);
    }

    private void Create(Adapter adaper, bool isEnumalted)
    {
        NativeCreate();

        unsafe void NativeCreate()
        {
            fixed(ID3D11Device** ppDevice = _pDevice)
            {
                fixed(ID3D11DeviceContext** ppDeviceContext = _pImContext)
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

                        GraphicsSharpException.SetInfoQueue();
                        var result = D3D11CreateDevice(*ppAdapter, driverType,
                            (HMODULE)IntPtr.Zero, flags, (D3D_FEATURE_LEVEL*)IntPtr.Zero, 0u, 
                            D3D11.D3D11_SDK_VERSION, ppDevice, &featureLevel, ppDeviceContext);

                        if (result.FAILED)
                        {
                            // error here.
                            throw GraphicsSharpException.GetLastGraphicsException(
                                new GraphicsSharpException($"Failed to create device\nError Code: {result}"));
                        }

                        _featureLevel = featureLevel;
                    }
                }
            }
        }
    }
}