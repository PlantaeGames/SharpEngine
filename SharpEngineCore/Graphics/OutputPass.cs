using System.Diagnostics;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.WinRT;

namespace SharpEngineCore.Graphics;

internal sealed class OutputPass : Pass
{
    private readonly Texture2D _outputTexture;

    private readonly List<CameraObject> _cameras;

    public OutputPass(Texture2D outputTexture, List<CameraObject> cameras)
    {
        _outputTexture = outputTexture;
        _cameras = cameras;
    }

    public override void OnGo(Device device, DeviceContext context)
    {
        foreach (var camera in _cameras)
        {
            if (camera.Type == CameraObject.Flags.Secondary)
                continue;

            int offetX = (int)camera.Viewport.Info.TopLeftX;
            int offetY = (int)camera.Viewport.Info.TopLeftY;

            var box = new D3D11_BOX();
            box.left = (uint)offetX;
            box.top = (uint)offetY;
            box.right = (uint)(camera.Viewport.Info.Width - offetX);
            box.bottom = (uint)(camera.Viewport.Info.Height - offetY);

            context.CopySubresourceRegion(_outputTexture, 0, offetX, offetY, 0,
                camera.RenderTexture, 0, box);
        }
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
