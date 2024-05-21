namespace SharpEngineCore.Graphics;
public sealed class CameraObject : Object
{
    public CameraConstantData Data { get; private set; }

    public readonly Viewport Viewport;
    public float AspectRatio => Viewport.Info.Height / Viewport.Info.Width;

    public void UpdateCamera(CameraConstantData data)
    {
        Data = data;
    }

    protected override void OnPause() { }
    protected override void OnResume() { }
    protected override void OnRemove() { }

    internal CameraObject(CameraConstantData data, Viewport viewport) :
        base()
    {
        Viewport = viewport;
        Data = data;
    }
}
