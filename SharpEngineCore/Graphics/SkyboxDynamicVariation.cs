namespace SharpEngineCore.Graphics;

internal sealed class SkyboxDynamicVariation : PipelineVariation
{
    public SkyboxDynamicVariation(
        ShaderResourceView skyboxView,
        Sampler skyboxSampler,
        Viewport viewport,
        DepthStencilView depthView,
        RenderTargetView outputView)
    {
        PixelShaderStage = new PixelShaderStage()
        {
            ShaderResourceViews = [skyboxView],
            Samplers = [skyboxSampler],

            Flags = PixelShaderStage.BindFlags.ShaderResourceViews |
                    PixelShaderStage.BindFlags.Samplers
        };

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

        _stages = [PixelShaderStage, Rasterizer, OutputMerger];
    }
}
