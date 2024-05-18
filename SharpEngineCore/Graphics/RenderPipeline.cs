using System.Diagnostics;

namespace SharpEngineCore.Graphics;

internal abstract class RenderPipeline : PipelineEvents
{
    public bool Initalized { get; private set; }
    protected RenderPass[] _renderPasses = [];

    public abstract Guid CreateGraphicsObject(Material material, Mesh mesh);
    public abstract CameraObject AddCamera(CameraConstantData data, Viewport viewport);
    public abstract LightObject AddLight(LightData data);
    public abstract List<GraphicsObject> GetGraphicsObjects();

#nullable enable
    protected T? Get<T>()
        where T : RenderPass
    {
        foreach (var renderPass in _renderPasses)
        {
            if (renderPass is T)
            {
                return (T)renderPass;
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

        foreach (var renderPass in _renderPasses)
        {
            renderPass.Initialize(device, context);
        }

        Initalized = true;
    }

    public sealed override void Ready(Device device, DeviceContext context)
    {
        OnReady(device, context);

        foreach (var renderPass in _renderPasses)
        {
            renderPass.Ready(device, context);
        }
    }

    public sealed override void Go(Device device, DeviceContext context)
    {
        OnGo(device, context);

        foreach (var renderPass in _renderPasses)
        {
            renderPass.Go(device, context);
        }
    }
}
