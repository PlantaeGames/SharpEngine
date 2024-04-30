using System.Runtime.InteropServices;

namespace SharpEngineCore.Graphics;

/// <summary>
/// Containing Raw Colors in R,G,B,A Format, 4 byte each channel.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 16)]
public struct Fragment
{
    public int r = 0;
    public int g = 0;
    public int b = 0;
    public int a = 0;

    public Fragment(int r, int g, int b, int a)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }

    public Fragment()
    { }
}
