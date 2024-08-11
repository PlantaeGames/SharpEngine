using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal sealed class Scissors
{
    public readonly Rectangle Info;

    public Scissors(Rectangle rectangle)
    {
        Info = rectangle;
    }
}
