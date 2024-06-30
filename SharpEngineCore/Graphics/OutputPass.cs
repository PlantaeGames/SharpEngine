using System.Diagnostics;

namespace SharpEngineCore.Graphics;

internal sealed class OutputPass : Pass
{
    private Texture2D _srcTexture;
    private readonly Texture2D _outputTexture;

    public void SetSrcTexture(Texture2D texture)
    {
        _srcTexture = texture;
    }

    public OutputPass(Texture2D outputTexture)
    {
        _outputTexture = outputTexture;
    }

    public override void OnGo(Device device, DeviceContext context)
    {
        Debug.Assert(_srcTexture.Info.Format == _outputTexture.Info.Format,
                    "Textures must have same output to be copied." +
                     "Or used in Output pass.");

        context.CopyResource(_srcTexture, _outputTexture);
    }

    public override void OnInitialize(Device device, DeviceContext context)
    {
    }

    public override void OnReady(Device device, DeviceContext context)
    {
    }

    public override void OnLightAdd(LightObject light, Device device)
    { 
    }

    public override void OnLightRemove(LightObject light, Device device)
    {
    }

    public override void OnLightPause(LightObject light, Device device)
    { 
    }

    public override void OnLightResume(LightObject light, Device device)
    {
    }

    public override void OnCameraAdd(CameraObject camera, Device device)
    {
    }

    public override void OnCameraPause(CameraObject camera, Device device)
    {
    }

    public override void OnCameraResume(CameraObject camera, Device device)
    {
    }

    public override void OnCameraRemove(CameraObject camera, Device device)
    {
    }

    public override void OnGraphicsAdd(GraphicsObject graphics, Device device)
    {
    }

    public override void OnGraphicsRemove(GraphicsObject graphics, Device device)
    {
    }

    public override void OnGraphicsPause(GraphicsObject graphics, Device device)
    {
    }

    public override void OnGraphicsResume(GraphicsObject graphics, Device device)
    {
    }

    public override void OnSkyboxSet(CubemapInfo info, Device device)
    {
    }
}
