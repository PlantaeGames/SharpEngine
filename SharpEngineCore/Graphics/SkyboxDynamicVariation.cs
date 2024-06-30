namespace SharpEngineCore.Graphics;

internal sealed class SkyboxDynamicVariation : PipelineVariation
{
    public SkyboxDynamicVariation(
        ShaderResourceView skyboxView,
        Sampler skyboxSampler,
        Viewport viewport)
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

        _stages = [PixelShaderStage, Rasterizer];
    }
}
