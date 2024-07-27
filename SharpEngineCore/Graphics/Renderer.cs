using System.Diagnostics;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal class Renderer
{
    private const int DEFAULT_ADAPTER = 0;

    protected readonly Device _device;
    protected readonly DeviceContext _context;

    protected RenderPipeline _pipeline;
    protected Swapchain _swapchain;

    private Window _primaryWindow;

    public CameraObject InitializeSecondaryWindow(SecondaryWindow window, CameraInfo cameraInfo)
    {
        var swapchain = Factory.GetInstance().CreateSwapchain(window, _device);
        var camera = new CameraObject(cameraInfo.cameraTransform, cameraInfo.viewport, swapchain.GetBackTexture(), CameraObject.Flags.Secondary);

        window.Initialize(swapchain, camera);

        _pipeline.AddCamera(cameraInfo, _device, ref camera);
        return camera;
    }

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
        var renderTexture = _device.CreateTexture2D(
            [new USurface(new((int)info.viewport.Info.Width, (int)info.viewport.Info.Height), Channels.Single)],
            new TextureCreationInfo()
            {
                Format = TerraFX.Interop.DirectX.DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM,
                UsageInfo = new ResourceUsageInfo()
                {
                    Usage = TerraFX.Interop.DirectX.D3D11_USAGE.D3D11_USAGE_DEFAULT,
                    BindFlags = TerraFX.Interop.DirectX.D3D11_BIND_FLAG.D3D11_BIND_RENDER_TARGET
                }
            });

        var camera = new CameraObject(info.cameraTransform, info.viewport, renderTexture);
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

        _primaryWindow = window;

        _device = new Device(adapters[DEFAULT_ADAPTER]);
        _context = _device.GetContext();

        _swapchain = Factory.GetInstance().CreateSwapchain(_primaryWindow, _device);

        _pipeline = new DefaultRenderPipeline(_swapchain.GetBackTexture());
        _pipeline.Initialize(_device, _context);
    }
}