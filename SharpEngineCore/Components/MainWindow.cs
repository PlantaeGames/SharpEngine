using System.Text;

using TerraFX.Interop.Windows;
using TerraFX.Interop.DirectX;

using SharpEngineCore.Graphics;
using SharpEngineCore.Utilities;

namespace SharpEngineCore.Components;

internal sealed class MainWindow : Window
{
    private Logger _logger;

    private Device _device;
    private DeviceContext _context;
    private Swapchain _swapchain;
    private RenderTargetView _view;

    private float _count = 0f;

    public void Update()
    {
        _context.ClearRenderTargetView(_view, new Fragment
        {
            r = Math.Abs(MathF.Sin(_count)),
            g = Math.Abs(MathF.Sin(_count * 1.1f)),
            b = Math.Abs(MathF.Sin(_count * 0.7f)),
            a = 1f
        });

        _swapchain.Present();

        _count += 0.016f;
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
        _device = new Device(adapters[0]);
        _context = _device.GetContext();
        _logger.LogMessage("Device Created on Adapter: 0");
        _logger.LogMessage($"Obtained Feature Level: {_device.GetFeatureLevel()}");

        _logger.BreakLine();

        // creating swapchain
        _logger.LogHeader("Swapchain Creation:-");
         _swapchain = DXGIFactory.GetInstance().CreateSwapchain(this, _device);
        _logger.LogMessage("Swapchain Created on Device: 0");

        _logger.BreakLine();

        // creating render target
        _logger.LogHeader("Render Target View Creation:-");
        _view = _device.CreateRenderTargetView(_swapchain.GetBackTexture());
        _logger.LogMessage("Render Target View Created on Swapchain BackBuffer.");

        _logger.BreakLine();

        // clearing render target view
        _logger.LogHeader("Clearing Render Target View:-");
        _context.ClearRenderTargetView(_view, new Fragment
        {
            r = 0f,
            g = 0f,
            b = 1f,
            a = 1f
        });
        _logger.LogMessage("Render Target View Cleared.");
        
        _logger.BreakLine();

        // creating test texture 2d
        using var surface = new Surface(new Size(256, 256));
        _logger.LogHeader($"Creating test texture of {surface.Size}");
        var texture = _device.CreateTexture2D(surface,
            new ResourceUsageInfo()
            {
                Format = DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM_SRGB,
                Usage = D3D11_USAGE.D3D11_USAGE_IMMUTABLE,
                BindFlags = D3D11_BIND_FLAG.D3D11_BIND_SHADER_RESOURCE,
                CPUAccessFlags = 0u
            });

        _logger.LogMessage("Test Texture Created.");

        _logger.BreakLine();

        // creating test buffer
        var fragments = new Fragment[1024];
        _logger.LogHeader($"Creating test buffer of {fragments.Length * 16} bytes");
        
        var vertexBuffer = new VertexBuffer(
            _device.CreateBuffer(fragments, new ResourceUsageInfo()
            {
                Usage = D3D11_USAGE.D3D11_USAGE_IMMUTABLE,
                BindFlags = D3D11_BIND_FLAG.D3D11_BIND_VERTEX_BUFFER
                
            }));
        _logger.LogMessage("Test Buffer Created.");

        _logger.BreakLine();

        // creating shaders
        const string vertexShaderPath = "Shaders\\VertexShader.hlsl";
        const string pixelShaderPath = "Shaders\\PixelShader.hlsl";

        _logger.LogHeader("Creating Shaders.");
        
        var vertexModule = new ShaderModule(vertexShaderPath);
        var vertexShader = _device.CreateVertexShader(vertexModule);
        _logger.LogMessage("Vertex Shader Created.");

        var pixelModule = new ShaderModule(pixelShaderPath);
        var pixelShader = _device.CreatePixelShader(pixelModule);
        _logger.LogMessage("Pixel Shader Created.");

        _logger.BreakLine();

        // creating inputlayout
        _logger.LogHeader("Creating Input Layout.");
        var inputLayout = _device.CreateInputLayout(new InputLayoutInfo()
        {
            Topology = D3D_PRIMITIVE_TOPOLOGY.D3D10_PRIMITIVE_TOPOLOGY_TRIANGLELIST,
            Layout = new Vertex(),
            VertexShader = vertexShader
        });
        _logger.LogMessage("Input Layout Created.");

        _logger.BreakLine();
        // presenting
        _swapchain.Present();
        _logger.LogMessage("Presenting...");

        _logger.BreakLine();
    }

    protected override LRESULT WndProc(HWND hWND, uint msg, WPARAM wParam, LPARAM lParam)
    {
        return base.WndProc(hWND, msg, wParam, lParam);
    }
}