using System.Numerics;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal sealed class Scissors
{
    public readonly Vector4 Info;

    public Scissors(Vector4 rectangle)
    {
        Info = rectangle;
    }
}
