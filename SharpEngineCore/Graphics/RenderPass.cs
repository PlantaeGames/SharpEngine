using System.Diagnostics;

namespace SharpEngineCore.Graphics;

internal abstract class RenderPass : PipelineEvents
{
    protected Pass[] _passes;

#nullable enable
    public T? Get<T>()
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
        OnReady(device, context);

        foreach (var pass in _passes)
        {
            pass.Ready(device, context);
        }
    }

    public sealed override void Go(Device device, DeviceContext context)
    {
        OnGo(device, context);

        foreach (var pass in _passes)
        {
            pass.Go(device, context);
        }
    }

    protected RenderPass()
    {
        _passes = [];
    }
}
