using System.Diagnostics;

namespace SharpEngineCore.Graphics;

internal sealed class ForwardDynamicSubVariation : PipelineVariation
{
    public ForwardDynamicSubVariation(PixelShaderStage subvariationPixelStage,
        ShaderResourceView[] depthTexturesToInject)
       : base()
    {
        var views = new List<ShaderResourceView>();
        views.AddRange(subvariationPixelStage.ShaderResourceViews);

        for(var i = 0; i < depthTexturesToInject.Length; i++)
        {
            views[i + subvariationPixelStage.SamplerStartIndex] = depthTexturesToInject[i];
        }

        PixelShaderStage = new PixelShaderStage()
        {
            ConstantBuffers = subvariationPixelStage.ConstantBuffers,
            PixelShader = subvariationPixelStage.PixelShader,
            Samplers = subvariationPixelStage.Samplers,
            SamplerStartIndex = subvariationPixelStage.SamplerStartIndex,
            ShaderResourceViews = views.ToArray(),

            Flags = subvariationPixelStage.Flags
        };

        _stages = [PixelShaderStage];
    }
}
