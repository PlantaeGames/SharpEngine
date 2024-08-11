namespace SharpEngineEditor.ImGui.Backend;

internal static class SizeExtensions
{
    public static int ToArea(this Size size)
    {
        return size.Width * size.Height;
    }
}
