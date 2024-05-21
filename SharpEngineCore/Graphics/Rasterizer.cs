namespace SharpEngineCore.Graphics;

internal sealed class Rasterizer : IPipelineStage
{
    [Flags]
    public enum BindFlags
    {
        None = 0,
        Viewports = 1 << 1
    }

    public Viewport[] Viewports { get; init; }
    public BindFlags Flags { get; init; }

    public Rasterizer(Viewport[] viewports, BindFlags flags)
    {
        Viewports = viewports;
        Flags = flags;
    }

    public Rasterizer()
    { }


    public void Bind(DeviceContext context)
    {
        if(Flags.HasFlag(BindFlags.Viewports))
        {
            context.RSSetViewports(Viewports);
        }
    }

    public void Unbind(DeviceContext context)
    {
        if (Flags.HasFlag(BindFlags.Viewports))
        {
            context.RSSetViewports(Viewports, true);
        }
    }
}
