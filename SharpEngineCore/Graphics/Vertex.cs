using System.Runtime.InteropServices;

namespace SharpEngineCore.Graphics;

[StructLayout(LayoutKind.Sequential, Pack = 0, Size = 64)]
public struct Vertex : IFragmentable
{
    public FColor4 Position;
    public FColor4 Normal;
    public FColor4 Color;
    public FColor4 TexCoord;

    public int GetFragmentsCount()
    {
        return ToFragments().Length;
    }

    public int GetSize()
    {
        unsafe
        {
            return sizeof(Vertex);
        }
    }

    public Fragment[] ToFragments()
    {
        return 
         [
            Position.r,
            Position.g,
            Position.b,
            Position.a,

            Normal.r,
            Normal.g,
            Normal.b,
            Normal.a,

            Color.r,
            Color.g,
            Color.b,
            Color.a,

            TexCoord.r,
            TexCoord.g,
            TexCoord.b,
            TexCoord.a 
         ];
    }
}