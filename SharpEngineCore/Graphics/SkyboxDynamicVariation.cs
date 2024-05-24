namespace SharpEngineCore.Graphics;

internal sealed class SkyboxDynamicVariation : PipelineVariation
{
    public SkyboxDynamicVariation(Viewport viewport)
    {
        Rasterizer = new Rasterizer()
        {
            Viewports = [viewport],

            Flags = Rasterizer.BindFlags.Viewports
        };

        _stages = [Rasterizer];
    }
}
