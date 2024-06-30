using SharpEngineCore.Graphics;

namespace SharpEngineCore.Components;

internal sealed class Camera
{
    public static FColor4 Perspective = new(0, 0, 0, 0);
    public static FColor4 Orthogonal = new(1, 0, 0, 0);
}
