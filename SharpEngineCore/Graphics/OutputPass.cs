using System.Diagnostics;

namespace SharpEngineCore.Graphics;

internal sealed class OutputPass : Pass
{
    private Texture2D _srcTexture;
    private Texture2D _outputTexture;

    public OutputPass(Texture2D srcTexture, Texture2D outputTexture)
    {
        Debug.Assert(srcTexture.Info.Format == outputTexture.Info.Format,
            "Textures must have same output to be copied." +
            "Or used in Output pass.");

        _srcTexture = srcTexture;
        _outputTexture = outputTexture;
    }

    public override void OnGo(Device device, DeviceContext context)
    {
        context.CopyResource(_srcTexture, _outputTexture);
    }

    public override void OnInitialize(Device device, DeviceContext context)
    {
    }

    public override void OnReady(Device device, DeviceContext context)
    {
    }
}
