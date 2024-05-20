using System.Diagnostics;

namespace SharpEngineCore.Graphics;

internal class Renderer
{
    private const int DEFAULT_ADAPTER = 0;

    protected readonly Device _device;
    protected readonly DeviceContext _context;

    protected RenderPipeline _pipeline;
    protected Swapchain _swapchain;

    public GraphicsObject CreateGraphicsObject(Material material, Mesh mesh)
    {
        return _pipeline.CreateGraphicsObject(_device, material, mesh);
    }

    public LightObject CreateLightObject(LightData data)
    {
        return _pipeline.CreateLightObject(_device, data);
    }

    public CameraObject CreateCameraObject(CameraConstantData data, Viewport viewport)
    {
        return _pipeline.CreateCameraObject(_device, data, viewport);
    }

    internal Device GetDevice() => _device;
    internal Swapchain GetSwapchain() => _swapchain;

    public void Render()
    {
        _context.ClearState();
        _pipeline.Ready(_device, _context);
        _pipeline.Go(_device, _context);
    }

    public Renderer(Window window)
    {
        var adapters = Factory.GetInstance().GetAdpters();

        Debug.Assert(adapters.Length > 0,
            "No Graphics Adapter found.");

        _device = new Device(adapters[DEFAULT_ADAPTER]);
        _context = _device.GetContext();

        _swapchain = Factory.GetInstance().CreateSwapchain(window, _device);

        _pipeline = new DefaultRenderPipeline(_swapchain.GetBackTexture());
        _pipeline.Initialize(_device, _context);
    }
}