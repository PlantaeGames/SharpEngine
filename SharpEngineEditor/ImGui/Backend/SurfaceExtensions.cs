using SharpEngineCore.Graphics;
using SharpEngineCore.Utilities;

namespace SharpEngineEditor.ImGui.Backend;

internal static class SurfaceExtensions
{
    public static int ToArea(this Surface surface)
    {
        return surface.Size.ToArea() * surface.SubDivisionCount;
    }
}
