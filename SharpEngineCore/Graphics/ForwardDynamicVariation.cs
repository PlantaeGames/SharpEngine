namespace SharpEngineCore.Graphics;

internal sealed class ForwardDynamicVariation : PipelineVariation
{
    public ForwardDynamicVariation(Viewport viewport)
       : base()
    {
        var rasterizer = new Rasterizer()
        {
            Viewports = [viewport],

            Flags = Rasterizer.BindFlags.Viewports
        };

        _stages = [rasterizer];
    }
}
