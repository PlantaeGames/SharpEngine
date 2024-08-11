using System.Runtime.InteropServices;

namespace SharpEngineCore.Graphics;

[StructLayout(LayoutKind.Sequential, Pack = 0, Size = 4)]
public struct FColor1 : IFragmentable
{
    public Fragment r = 0f;

    public FColor1(Fragment r)
    {
        this.r = r;
    }

    public FColor1()
    { }

    /// <summary>
    /// Size of this structure in bytes.
    /// </summary>
    /// <returns>Size.</returns>
    public int GetSize()
    {
        unsafe
        {
            return sizeof(FColor1);
        }
    }

    public Fragment[] ToFragments()
    {
        return [r];
    }

    public int GetFragmentsCount()
    {
        return ToFragments().Length;
    }
}