using System.Diagnostics;

namespace SharpEngineEditor.ImGui.Backend;

internal static class USurfaceExtensions
{
    public static int ToAreaAs(this USurface surface, IUnitable layout)
    {
        Debug.Assert(surface.ToArea() % layout.GetUnitCount() == 0,
            "Dimensions not matched.");

        return surface.ToArea() / layout.GetUnitCount();
    }
}
