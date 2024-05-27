using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using TerraFX.Interop.Windows;

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

    public static bool operator ==(FColor4 a, FColor4 b)
    {
        return (
            a.r == b.r &&
            a.g == b.g &&
            a.b == b.b &&
            a.a == b.a
            );
    }

    public static bool operator !=(FColor4 a, FColor4 b)
    {
        return !(a == b);
    }

    public override bool Equals([NotNullWhen(true)] object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
