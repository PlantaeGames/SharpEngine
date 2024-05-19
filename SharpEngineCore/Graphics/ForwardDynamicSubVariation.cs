using System.Diagnostics;

namespace SharpEngineCore.Graphics;

internal sealed class ForwardDynamicSubVariation : PipelineVariation
{
    public ForwardDynamicSubVariation(PixelShaderStage subvariationPixelStage,
        ShaderResourceView[] depthTexturesToInject,
        Sampler[] depthSamplersToInject)
       : base()
    {
        Debug.Assert(depthSamplersToInject.Length == depthTexturesToInject.Length,
            "Depth textures / shader resource views count and their samplers count" +
            " does not match.");

        var views = new List<ShaderResourceView>();
        views.AddRange(subvariationPixelStage.ShaderResourceViews);

        var samplers = new List<Sampler>();
        samplers.AddRange(subvariationPixelStage.Samplers);

        for(var i = 0; i < depthTexturesToInject.Length; i++)
        {
            views[i + 1] = depthTexturesToInject[i];
            samplers[i] = depthSamplersToInject[i];
        }

        PixelShaderStage = new PixelShaderStage()
        {
            ConstantBuffers = subvariationPixelStage.ConstantBuffers,
            PixelShader = subvariationPixelStage.PixelShader,
            Samplers = samplers.ToArray(),
            ShaderResourceViews = views.ToArray(),

            Flags = subvariationPixelStage.Flags
        };

        _stages = [PixelShaderStage];
    }
}
