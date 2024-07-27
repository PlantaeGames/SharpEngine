using System.Diagnostics;

using TerraFX.Interop.DirectX;

namespace SharpEngineCore.Graphics;

public static class Graphics
{
    private static Device Device => Renderer.GetDevice();
    private static Renderer Renderer;
    private static Swapchain Swapchain;

    private static bool _Initialized = false;

    public static GraphicsObject CreateGraphicsObject(GraphicsInfo info)
    {
        return Renderer.CreateGraphicsObject(info);
    }
    public static LightObject CreateLightObject(LightInfo info)
    {
        return Renderer.CreateLightObject(info);
    }
    public static CameraObject CreateCameraObject(CameraInfo info)
    {
        return Renderer.CreateCameraObject(info);
    }

    public static Buffer CreateBuffer(Surface surface, Type layout, 
            bool constant = false,
            bool structured = true,
            bool mutable = false)
    {
        return Device.CreateBuffer(
                surface,
                layout,
                new ResourceUsageInfo()
                {
                    CPUAccessFlags = mutable ? D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE :
                                              0,

                    BindFlags = constant ? D3D11_BIND_FLAG.D3D11_BIND_CONSTANT_BUFFER :
                                           D3D11_BIND_FLAG.D3D11_BIND_SHADER_RESOURCE,
                    Usage = mutable ? D3D11_USAGE.D3D11_USAGE_DYNAMIC :
                                     D3D11_USAGE.D3D11_USAGE_IMMUTABLE,

                    MiscFlags = structured && !constant?
                                    D3D11_RESOURCE_MISC_FLAG.D3D11_RESOURCE_MISC_BUFFER_STRUCTURED :
                                    0
                });
    }

    public static ConstantBuffer CreateConstantBuffer(ISurfaceable layout, bool mutable = false)
    {
        return Buffer.CreateConstantBuffer(
            Graphics.CreateBuffer(layout.ToSurface(), layout.GetType(), mutable));
    }

    public static Texture2D CreateTexture2D(FSurface surface, bool mutable = false)
    {
        return Device.CreateTexture2D([surface],
            new TextureCreationInfo()
            {
                UsageInfo = new ResourceUsageInfo()
                {
                    CPUAccessFlags = mutable ?
                        D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_READ |
                        D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE :
                        0,

                    BindFlags = D3D11_BIND_FLAG.D3D11_BIND_SHADER_RESOURCE,

                    Usage = mutable ? D3D11_USAGE.D3D11_USAGE_DYNAMIC :
                                 D3D11_USAGE.D3D11_USAGE_IMMUTABLE
                }
            });
    }

    public static Sampler CreateSampler(bool pointFiltering = false)
    {
        return Device.CreateSampler(
            new SamplerInfo()
            {
                AddressMode = D3D11_TEXTURE_ADDRESS_MODE.D3D11_TEXTURE_ADDRESS_CLAMP,
                Filter = pointFiltering? D3D11_FILTER.D3D11_FILTER_MIN_MAG_MIP_POINT :
                                         D3D11_FILTER.D3D11_FILTER_ANISOTROPIC
            });
    }

    internal static CameraObject InitializeSecondaryWindow(SecondaryWindow window, CameraInfo info)
    {
        var camera = Renderer.InitializeSecondaryWindow(window, info);
        return camera;
    }

    internal static void Render()
    {
        Debug.Assert(_Initialized,
            "Graphics is not initialized yet.");

        Renderer.Render();
        Swapchain.Present();
    }

    internal static void Initialize(Window window)
    {
        Debug.Assert(_Initialized == false,
            "Graphics are already initialized.");

        Renderer = new Renderer(window);
        Swapchain = Renderer.GetSwapchain();

        _Initialized = true;
    }

    static Graphics()
    {}
}
