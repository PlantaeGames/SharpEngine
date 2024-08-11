using TerraFX.Interop.Windows;

namespace SharpEngineEditor.ImGui.Backend;

internal sealed class Scissors
{
    public readonly Rectangle Info;

    public Scissors(Rectangle rectangle)
    {
        Info = rectangle;
    }
}
