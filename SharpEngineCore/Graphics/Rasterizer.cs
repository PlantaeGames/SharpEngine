namespace SharpEngineCore.Graphics;

internal sealed class Rasterizer : IPipelineStage
{
    public Viewport[] Viewports { get; init; }

    public Rasterizer(Viewport[] viewports)
    {
        Viewports = viewports;
    }

    public Rasterizer()
    { }


    public void Bind(DeviceContext context)
    {
        context.RSSetViewports(Viewports);
    }
}
