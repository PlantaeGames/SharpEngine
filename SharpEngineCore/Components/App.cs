using System.Text;

using TerraFX.Interop.Windows;

using SharpEngineCore.Graphics;

using TerraFX.Interop.DirectX;
using static TerraFX.Interop.DirectX.DirectX;
using System.Drawing;
using System.IO;
using SharpEngineCore.Utilities;

namespace SharpEngineCore.Components;

internal sealed class App
{
    public App()
    { }

    public int Run()
    {
        var returnCode = 0;

        var window = new MainWindow("Sharp Engine", new Point(0,0), new Size(1024, 768));
        try
        {
            window.Show();

            // quering all adapters
            var _logger = new Logger();
            _logger.LogHeader("Queried Adapters:-");
            var adapters = DXGIFactory.GetInstance().GetAdpters();
            foreach (var adapter in adapters)
            {
                var description = adapter.GetDescription().Description;
                var sb = new StringBuilder();
                foreach (var c in description)
                {
                    sb.Append(c);
                }
                _logger.LogMessage(sb.ToString());
            }

            _logger.BreakLine();

            // creating device on default adapter
            _logger.LogHeader("Device Creation:-");
            var device = new Device(adapters[0]);
            var context = device.GetContext();
            var swapchain = DXGIFactory.GetInstance().CreateSwapchain(window, device);
            swapchain.Present();
            _logger.LogMessage("Device Created on Adapter: 0");
            _logger.LogMessage("Immediate Context Obtained.");
            _logger.LogMessage("Swapchain created.");
            _logger.LogMessage($"Obtained Feature Level: {device.GetFeatureLevel()}");

            _logger.BreakLine();

            //unsafe
            //{
            //    var desc = new DXGI_SWAP_CHAIN_DESC();

            //    desc.BufferDesc = new DXGI_MODE_DESC();
            //    desc.BufferDesc.Width = 0u;
            //    desc.BufferDesc.Height = 0u;
            //    desc.BufferDesc.RefreshRate = new DXGI_RATIONAL
            //    {
            //        Denominator = 0u,
            //        Numerator = 0u
            //    };
            //    desc.BufferDesc.Format = DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;
            //    desc.BufferDesc.ScanlineOrdering = DXGI_MODE_SCANLINE_ORDER.DXGI_MODE_SCANLINE_ORDER_UNSPECIFIED;
            //    desc.BufferDesc.Scaling = DXGI_MODE_SCALING.DXGI_MODE_SCALING_UNSPECIFIED;

            //    desc.BufferCount = 1u;
            //    desc.BufferUsage = DXGI.DXGI_USAGE_RENDER_TARGET_OUTPUT;

            //    desc.OutputWindow = window.HWnd;
            //    desc.Windowed = true;

            //    desc.Flags = 0u;

            //    desc.SwapEffect = DXGI_SWAP_EFFECT.DXGI_SWAP_EFFECT_DISCARD;

            //    desc.SampleDesc = new DXGI_SAMPLE_DESC
            //    {
            //        Quality = 0u,
            //        Count = 1u
            //    };


            //    IDXGISwapChain* pSwapchain = (IDXGISwapChain*)IntPtr.Zero;
            //    ID3D11Device* pDevice = (ID3D11Device*)IntPtr.Zero;
            //    ID3D11DeviceContext* pContext = (ID3D11DeviceContext*)IntPtr.Zero;

            //    var result = D3D11CreateDeviceAndSwapChain((IDXGIAdapter*)IntPtr.Zero,
            //        D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_HARDWARE,
            //        (HMODULE)IntPtr.Zero,
            //        (uint)D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_DEBUG,
            //        (D3D_FEATURE_LEVEL*)IntPtr.Zero, 0u,
            //        D3D11.D3D11_SDK_VERSION,
            //        &desc, &pSwapchain, &pDevice, (D3D_FEATURE_LEVEL*)IntPtr.Zero, &pContext);


            //    result = D3D11CreateDeviceAndSwapChain((IDXGIAdapter*)IntPtr.Zero,
            //        D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_HARDWARE,
            //        (HMODULE)IntPtr.Zero,
            //        (uint)D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_DEBUG,
            //        (D3D_FEATURE_LEVEL*)IntPtr.Zero, 0u,
            //        D3D11.D3D11_SDK_VERSION,
            //        &desc, &pSwapchain, &pDevice, (D3D_FEATURE_LEVEL*)IntPtr.Zero, &pContext);

            //    pSwapchain->Present(0u, 0u);
            //}

            //while (true) ;

            // application loop
            while (true)
            {
                bool stop = false;
                // message loop
                while (true)
                {
                    var result = window.PeekAndDispatchMessage();

                    if (result.availability == false)
                        break;
                    
                    if (result.msg.message == WM.WM_QUIT)
                    {
                        stop = true;
                        break;
                    }
                }

                if (stop)
                    break;

                // other code here.
                window.Update();
            }
        }
        catch
        {
            throw;
        }
        finally
        {
            window.Destroy();
        }

        return returnCode;
    }
}