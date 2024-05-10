using System.Runtime.InteropServices;

namespace SharpEngineCore.Graphics;

/// <summary>
/// Containing Raw Colors in R,G,B,A Format, 4 byte each channel.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 0, Size = 16)]
public struct FColor4 : IFragmentable
{
    public Fragment r = 0f;
    public Fragment g = 0f;
    public Fragment b = 0f;
    public Fragment a = 0f;

    public FColor4(Fragment r, Fragment g, Fragment b, Fragment a)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }

    public FColor4()
    { }

    /// <summary>
    /// Size of this structure in bytes.
    /// </summary>
    /// <returns>Size.</returns>
    public int GetSize()
    {
        unsafe
        {
            return sizeof(FColor4);
        }
    }

    public Fragment[] ToFragments()
    {
        return [r, g, b, a];
    }

    public int GetFragmentsCount()
    {
        return ToFragments().Length;
    }
}
