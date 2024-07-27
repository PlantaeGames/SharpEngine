namespace SharpEngineCore.Graphics;
public sealed class CameraObject : Object
{
    [Flags]
    public enum Flags
    {
        Primiary,
        Secondary
    }

    public CameraConstantData Data { get; private set; }

    public readonly Viewport Viewport;

    public Texture2D RenderTexture { get; private set; }
    public float AspectRatio => Viewport.Info.Height / Viewport.Info.Width;

    public readonly Flags Type;

    public void UpdateCamera(CameraConstantData data)
    {
        Data = data;
    }

    protected override void OnPause() { }
    protected override void OnResume() { }
    protected override void OnRemove() { }

    internal CameraObject(CameraConstantData data, Viewport viewport, Texture2D renderTexture, Flags type = Flags.Primiary) :
        base()
    {
        Viewport = viewport;
        Data = data;
        RenderTexture = renderTexture;
        Type = type;
    }
}
