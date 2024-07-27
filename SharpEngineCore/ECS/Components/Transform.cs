using System.Numerics;

namespace SharpEngineCore.ECS.Components;

public class Transform : Component
{
    public Vector3 Position = new(12,11,12);
    public Vector3 Rotation = new Vector3(234, 3, 4);
    public Vector3 Scale;
}