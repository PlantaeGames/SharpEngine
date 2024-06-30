namespace SharpEngineCore.Graphics;

internal abstract class PipelineEvents
{
    public abstract void Initialize(Device device, DeviceContext context);
    public abstract void Ready(Device device, DeviceContext context);
    public abstract void Go(Device device, DeviceContext context);
    public abstract void OnInitialize(Device device, DeviceContext context);
    public abstract void OnReady(Device device, DeviceContext context);
    public abstract void OnGo(Device device, DeviceContext context);

    public abstract void OnLightAdd(LightObject light, Device device);
    public abstract void OnLightRemove(LightObject light, Device device);
    public abstract void OnLightPause(LightObject light, Device device);
    public abstract void OnLightResume(LightObject light, Device device);
    public abstract void OnCameraAdd(CameraObject camera, Device device);
    public abstract void OnCameraPause(CameraObject camera, Device device);
    public abstract void OnCameraResume(CameraObject camera, Device device);
    public abstract void OnCameraRemove(CameraObject camera, Device device);

    public abstract void AddLight(LightInfo info, Device device,
                                        ref LightObject light);
    public abstract void RemoveLight(LightObject light, Device device);
    public abstract void PauseLight(LightObject light, Device device);
    public abstract void ResumeLight(LightObject light, Device device);
    public abstract void AddCamera(CameraInfo info, Device device, 
                                            ref CameraObject camera);
    public abstract void PauseCamera(CameraObject camera, Device device);
    public abstract void ResumeCamera(CameraObject camera, Device device);
    public abstract void RemoveCamera(CameraObject camera, Device device);

    public abstract void AddGraphics(GraphicsInfo info, Device device, 
                                                ref GraphicsObject graphics);
    public abstract void RemoveGraphics(GraphicsObject graphics, Device device);
    public abstract void PauseGraphics(GraphicsObject graphics, Device device);
    public abstract void ResumeGraphics(GraphicsObject graphics, Device device);
    public abstract void OnGraphicsAdd(GraphicsObject graphics, Device device);
    public abstract void OnGraphicsRemove(GraphicsObject graphics, Device device);
    public abstract void OnGraphicsPause(GraphicsObject graphics, Device device);
    public abstract void OnGraphicsResume(GraphicsObject graphics, Device device);

    public abstract void SetSkybox(CubemapInfo info, Device device);
    public abstract void OnSkyboxSet(CubemapInfo info, Device device);

    protected PipelineEvents() { }
}