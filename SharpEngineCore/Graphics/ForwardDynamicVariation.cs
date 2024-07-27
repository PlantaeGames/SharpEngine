using System.Diagnostics;

namespace SharpEngineCore.Graphics;

internal sealed class ForwardDynamicVariation : PipelineVariation
{
    public ForwardDynamicVariation(
        DepthStencilView depthView,
        RenderTargetView outputView,
        Viewport viewport)
       : base()
    {
        Rasterizer = new Rasterizer()
        {
            Viewports = [viewport],

            Flags = Rasterizer.BindFlags.Viewports
        };

        OutputMerger = new OutputMerger()
        {
            DepthStencilView = depthView,
            RenderTargetViews = [outputView],

            Flags = OutputMerger.BindFlags.RenderTargetViewAndDepthView
        };
        
        _stages = [Rasterizer, OutputMerger];
    }
}