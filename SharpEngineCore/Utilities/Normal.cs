using System.Numerics;

namespace SharpEngineCore.Utilities;

public sealed class Normal
{
    public static Vector3 CalculateTriangleNormal(Vector3 a, Vector3 b, Vector3 c)
    {
        var u = Vector3.Subtract(b, a);
        var v = Vector3.Subtract(c, a);

        var n = Vector3.Cross(u, v);

        Vector3 result = Vector3.Normalize(n);
        return result;
    }

    public static float Normalize(float current, float max) =>
        Math.Clamp(current / max, -1f, 1f);
}
