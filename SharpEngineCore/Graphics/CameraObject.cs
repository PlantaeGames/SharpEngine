namespace SharpEngineCore.Graphics;
public sealed class CameraObject : Object
{
    internal CameraConstantData _lastUpdatedData;

    public readonly Viewport Viewport;
    public float AspectRatio => Viewport.Info.Height / Viewport.Info.Width;

    public void UpdateCamera(CameraConstantData data)
    {
        _lastUpdatedData = data;
    }

    protected override void OnPause() { }
    protected override void OnResume() { }
    protected override void OnRemove() { }

    internal CameraObject(CameraConstantData data, Viewport viewport) :
        base(Guid.NewGuid())
    {
        Viewport = viewport;
        _lastUpdatedData = data;
    }
}
