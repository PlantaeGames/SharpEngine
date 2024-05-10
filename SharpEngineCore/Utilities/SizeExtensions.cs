namespace SharpEngineCore.Utilities;

internal static class SizeExtensions
{
    public static int ToArea(this Size size)
    {
        return size.Width * size.Height;
    }
}
