using System.Diagnostics;

namespace SharpEngineCore.Graphics;


internal class Renderer
{
    private const int DEFAULT_ADAPTER = 0;

    protected readonly Device _device;
    protected readonly DeviceContext _context;

    protected RenderPipeline _pipeline;
    protected Swapchain _swapchain;

    public GraphicsObject CreateGraphicsObject(GraphicsInfo info)
    {
        var graphics = new GraphicsObject(info);
        _pipeline.AddGraphics(info, _device, ref graphics);

        return graphics;
    }

    public LightObject CreateLightObject(LightInfo info)
    {
        var light = new LightObject(info.data); 
        _pipeline.AddLight(info, _device, ref light);

        return light;
    }

    public CameraObject CreateCameraObject(CameraInfo info)
    {
        var camera = new CameraObject(info.cameraTransform, info.viewport);
        _pipeline.AddCamera(info, _device, ref camera);

        return camera;
    }

    public void PauseCameraObject(CameraObject camera)
    {
        _pipeline.PauseCamera(camera, _device);
    }

    public void ResumeCameraObject(CameraObject camera)
    {
        _pipeline.ResumeCamera(camera, _device);
    }

    public void RemoveCameraInfo(CameraObject camera)
    {
        _pipeline.RemoveCamera(camera, _device);
    }

    public void RemoveLightObject(LightObject light)
    {
        _pipeline.RemoveLight(light, _device);
    }

    public void PauseLightObject(LightObject light)
    {
        _pipeline.PauseLight(light, _device);
    }

    public void ResumeLightObject(LightObject light)
    {
        _pipeline.ResumeLight(light, _device);
    }

    public void SetSkybox(CubemapInfo info)
    {
        _pipeline.SetSkybox(info, _device);
    }

    internal Device GetDevice() => _device;
    internal Swapchain GetSwapchain() => _swapchain;

    public void Render()
    {
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