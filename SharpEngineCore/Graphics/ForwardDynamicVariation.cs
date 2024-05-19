using System.Diagnostics;

namespace SharpEngineCore.Graphics;

internal sealed class ForwardDynamicVariation : PipelineVariation
{
    public ForwardDynamicVariation(Viewport viewport)
       : base()
    {
        Rasterizer = new Rasterizer()
        {
            Viewports = [viewport],

            Flags = Rasterizer.BindFlags.Viewports
        };

        _stages = [Rasterizer];
    }
}