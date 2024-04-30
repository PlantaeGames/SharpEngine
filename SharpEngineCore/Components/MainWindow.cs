using System.Text;

using TerraFX.Interop.Windows;

using SharpEngineCore.Graphics;
using SharpEngineCore.Utilities;

namespace SharpEngineCore.Components;

internal sealed class MainWindow : Window
{
    private Logger _logger;

    public void Update()
    {
    }

    public MainWindow(string name, Point location, Size size) : base(name, location, size)
    {
        _logger = new ();

        Initialize();
    }

    private void Initialize()
    {
        // quering all adapters
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
        _logger.LogMessage("Device Created on Adapter: 0");
        _logger.LogMessage($"Obtained Feature Level: {device.GetFeatureLevel()}");

        _logger.BreakLine();

        // creating swapchain
        _logger.LogHeader("Swapchain Creation:-");
        var swapchain = DXGIFactory.GetInstance().CreateSwapchain(this, device);
        swapchain.Present();
        _logger.LogMessage("Swapchain Created on Device: 0");

        _logger.BreakLine();

        // creating render target
        _logger.LogHeader("Render Target View Creation:-");
        var renderTarget = device.CreateRenderTargetView(swapchain.GetBackTexture());
        _logger.LogMessage("Render Target View Created on Swapchain BackBuffer.");
    }

    protected override LRESULT WndProc(HWND hWND, uint msg, WPARAM wParam, LPARAM lParam)
    {
        return base.WndProc(hWND, msg, wParam, lParam);
    }
}