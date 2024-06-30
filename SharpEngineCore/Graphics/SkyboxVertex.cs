using System.Runtime.InteropServices;

namespace SharpEngineCore.Graphics;

[StructLayout(LayoutKind.Sequential, Pack = 0, Size = 16)]
public struct SkyboxVertex : IFragmentable
{
    public FColor4 Position;

    public int GetFragmentsCount()
    {
        return ToFragments().Length;
    }

    public int GetSize()
    {
        unsafe
        {
            return sizeof(SkyboxVertex);
        }
    }

    public Fragment[] ToFragments()
    {
        return
         [
            Position.r,
            Position.g,
            Position.b,
            Position.a
         ];
    }
}