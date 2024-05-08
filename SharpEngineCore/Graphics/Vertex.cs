using System.Runtime.InteropServices;

namespace SharpEngineCore.Graphics;

[StructLayout(LayoutKind.Sequential, Pack = 0, Size = 64)]
public struct Vertex : IFragmentable
{
    public Fragment Position;
    public Fragment Normal;
    public Fragment Color;
    public Fragment TexCoord;

    public int GetFragmentsCount()
    {
        return ToFragments().Length;
    }

    public Fragment[] ToFragments()
    {
        return [Position, Normal, Color, TexCoord];
    }
}
