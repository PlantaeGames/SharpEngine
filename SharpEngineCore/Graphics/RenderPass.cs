using System.Diagnostics;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal abstract class RenderPass : PipelineEvents
{
    protected Pass[] _passes;

    public sealed override void AddGraphics(GraphicsInfo info, Device device,
                                                      ref GraphicsObject graphics)
    {
        OnGraphicsAdd(graphics, device);

        foreach (var pass in _passes)
            pass.AddGraphics(info, device, ref graphics);
    }

    public sealed override void PauseGraphics(GraphicsObject graphics, Device device)
    {
        OnGraphicsPause(graphics, device);

        foreach (var pass in _passes)
            pass.PauseGraphics(graphics, device);
    }

    public sealed override void ResumeGraphics(GraphicsObject graphics, Device device)
    {
        OnGraphicsResume(graphics, device);

        foreach (var pass in _passes)
            pass.ResumeGraphics(graphics, device);
    }

    public sealed override void RemoveGraphics(GraphicsObject graphics, Device device)
    {
        OnGraphicsRemove(graphics, device);

        foreach (var pass in _passes)
            pass.RemoveGraphics(graphics, device);
    }

    public sealed override void AddLight(LightInfo info, Device device,
                                            ref LightObject light)
    {
        OnLightAdd(light, device);

        foreach (var pass in _passes)
            pass.AddLight(info, device, ref light);
    }
    public sealed override void RemoveLight(LightObject light, Device device)
    {
        OnLightRemove(light, device);

        foreach (var pass in _passes)
            pass.RemoveLight(light, device);
    }
    public sealed override void PauseLight(LightObject light, Device device)
    {
        OnLightPause(light, device);

        foreach (var pass in _passes)
            pass.PauseLight(light, device);
    }
    public sealed override void ResumeLight(LightObject light, Device device)
    {
        OnLightResume(light, device);

        foreach (var pass in _passes)
            pass.ResumeLight(light, device);
    }
    public sealed override void AddCamera(CameraInfo info, Device device,
                                                ref CameraObject camera)
    {
        OnCameraAdd(camera, device);

        foreach (var pass in _passes)
            pass.AddCamera(info, device, ref camera);
    }
    public sealed override void RemoveCamera(CameraObject camera, Device device)
    {
        OnCameraRemove(camera, device);

        foreach (var pass in _passes)
            pass.RemoveCamera(camera, device);
    }
    public sealed override void PauseCamera(CameraObject camera, Device device)
    {
        OnCameraPause(camera, device);

        foreach (var pass in _passes)
            pass.PauseCamera(camera, device);
    }
    public sealed override void ResumeCamera(CameraObject camera, Device device)
    {
        OnCameraResume(camera, device);

        foreach (var pass in _passes)
            pass.ResumeCamera(camera, device);
    }

    public sealed override void SetSkybox(CubemapInfo info, Device device)
    {
        OnSkyboxSet(info, device);

        foreach (var pass in _passes)
            pass.SetSkybox(info, device);
    }

#nullable enable
    protected T? Get<T>()
        where T : Pass
    {
        foreach(var pass in _passes)
        {
            if(pass is T)
            {
                return (T)pass;
            }
        }

        Debug.Assert(false,
            $"Could not found {nameof(T)}.");

        return null;
    }
#nullable disable

    public sealed override void Initialize(Device device, DeviceContext context)
    {
        OnInitialize(device, context);

        foreach (var pass in _passes)
        {
            pass.Initialize(device, context);
        }
    }

    public sealed override void Ready(Device device, DeviceContext context)
    {
        context.ClearState();

        OnReady(device, context);
    }

    public sealed override void Go(Device device, DeviceContext context)
    {
        OnGo(device, context);

        foreach (var pass in _passes)
        {
            pass.Ready(device, context);
            pass.Go(device, context);
        }
    }

    protected RenderPass()
    {
        _passes = [];
    }
}
