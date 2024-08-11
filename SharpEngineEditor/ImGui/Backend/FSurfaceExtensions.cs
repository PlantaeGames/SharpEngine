using System.Diagnostics;

namespace SharpEngineEditor.ImGui.Backend;

internal static class FSurfaceExtensions
{
    public static int ToAreaAs(this FSurface surface, IFragmentable layout)
    {
        Debug.Assert(surface.ToArea() % layout.GetFragmentsCount() == 0,
            "Dimensions not matched.");

        return surface.ToArea() / layout.GetFragmentsCount();
    }
}
