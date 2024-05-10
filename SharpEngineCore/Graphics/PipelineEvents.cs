namespace SharpEngineCore.Graphics;

internal abstract class PipelineEvents
{
    public abstract void Initialize(Device device, DeviceContext context);
    public abstract void Ready(Device device, DeviceContext context);
    public abstract void Go(Device device, DeviceContext context);
    public abstract void OnInitialize(Device device, DeviceContext context);
    public abstract void OnReady(Device device, DeviceContext context);
    public abstract void OnGo(Device device, DeviceContext context);

    protected PipelineEvents() { }
}