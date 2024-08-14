using System.Numerics;
using TerraFX.Interop.Windows;

namespace SharpEngineEditor.ImGui.Backend;

internal sealed class Scissors
{
    public readonly Vector4 Info;

    public Scissors(Vector4 rectangle)
    {
        Info = rectangle;
    }
}
