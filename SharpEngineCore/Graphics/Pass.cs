namespace SharpEngineCore.Graphics;

internal abstract class Pass : PipelineEvents
{
    private List<PipelineVariation> _paused = new();
    protected List<PipelineVariation> _subVariations = new();

    public sealed override void AddGraphics(GraphicsInfo info, Device device,
                                                  ref GraphicsObject graphics)
    {
        OnGraphicsAdd(graphics, device);
    }

    public sealed override void PauseGraphics(GraphicsObject graphics, Device device)
    {
        OnGraphicsPause(graphics, device);
    }

    public sealed override void ResumeGraphics(GraphicsObject graphics, Device device)
    {
        OnGraphicsResume(graphics, device);
    }

    public sealed override void RemoveGraphics(GraphicsObject graphics, Device device)
    {
        OnGraphicsRemove(graphics, device);
    }

    public sealed override void AddLight(LightInfo info, Device device,
                                            ref LightObject light)
    {
        OnLightAdd(light, device);
    }
    public sealed override void RemoveLight(LightObject light, Device device)
    {
        OnLightRemove(light, device);
    }
    public sealed override void PauseLight(LightObject light, Device device)
    {
        OnLightPause(light, device);
    }
    public sealed override void ResumeLight(LightObject light, Device device)
    {
        OnLightResume(light, device);
    }
    public sealed override void AddCamera(CameraInfo info, Device device,
                                                ref CameraObject camera)
    {
        OnCameraAdd(camera, device);
    }
    public sealed override void RemoveCamera(CameraObject camera, Device device)
    {
        OnCameraRemove(camera, device);
    }
    public sealed override void PauseCamera(CameraObject camera, Device device)
    {
        OnCameraPause(camera, device);
    }
    public sealed override void ResumeCamera(CameraObject camera, Device device)
    {
        OnCameraResume(camera, device);
    }

    // main events
    public sealed override void Initialize(Device device, DeviceContext context)
    {
        OnInitialize(device, context);
    }

    public sealed override void Ready(Device device, DeviceContext context)
    {
        ClearPendingVariations();
        OnReady(device, context);
    }

    public sealed override void Go(Device device, DeviceContext context)
    {
        OnGo(device, context);
    }

    public sealed override void SetSkybox(CubemapInfo info, Device device)
    {
        OnSkyboxSet(info, device);
    }

    private void ClearPendingVariations()
    {
        // removing expired
        var toRemove = new List<int>();
        for (var i = 0; i < _subVariations.Count; i++)
        {
            if (_subVariations[i].State == State.Expired)
                toRemove.Add(i);
        }
        for (var i = 0; i < toRemove.Count; i++)
        {
            _subVariations.RemoveAt(toRemove[i]);
        }

        // getting paused
        var toPause = new List<int>();
        for (var i = 0; i < _subVariations.Count; i++)
        {
            if (_subVariations[i].State == State.Active)
                continue;

            toPause.Add(i);
        }
        for (var i = 0; i < toPause.Count; i++)
        {
            _paused.Add(_subVariations[toPause[i]]);
            _subVariations.RemoveAt(toPause[i]);
        }

        // resuming active paused
        var toResume = new List<int>();
        for (var i = 0; i < _paused.Count; i++)
        {
            if (_paused[i].State == State.Active)
            {
                toResume.Add(i);
            }
        }
        for (var i = 0; i < toRemove.Count; i++)
        {
            _subVariations.Add(_paused[toResume[i]]);
            _paused.RemoveAt(toResume[i]);
        }

        // removing paused expired
        toRemove.Clear();
        for (var i = 0; i < _paused.Count; i++)
        {
            if (_paused[i].State == State.Expired)
            {
                toRemove.Add(i);
            }
        }
        for (var i = 0; i < toRemove.Count; i++)
        {
            _paused.RemoveAt(toRemove[i]);
        }
    }

    protected Pass()
    {}
}