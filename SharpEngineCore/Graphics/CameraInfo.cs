namespace SharpEngineCore.Graphics;

public sealed class CameraInfo
{
    public static FColor4 Perspective = new(0, 0, 0, 0);
    public static FColor4 Orthogonal = new(1, 0, 0, 0);

    public Viewport viewport;
    public CameraConstantData cameraTransform;

    public CameraInfo()
    {}
}
