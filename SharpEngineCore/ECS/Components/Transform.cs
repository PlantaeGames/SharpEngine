using SharpEngineCore.Misc;
using System.Numerics;

namespace SharpEngineCore.ECS.Components;

public class Transform : Component
{
    public Vector3 Position;
    public Vector3 Rotation;
    public Vector3 Scale;
}