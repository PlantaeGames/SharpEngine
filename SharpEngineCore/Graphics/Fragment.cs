using System.Runtime.InteropServices;

namespace SharpEngineCore.Graphics;

/// <summary>
/// Containing Raw Colors in R,G,B,A Format, 4 byte each channel.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 0, Size = 4)]
public struct Fragment
{
    public byte r = 0;
    public byte g = 0;
    public byte b = 0;
    public byte a = 0;

    public Fragment(byte r, byte g, byte b, byte a)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }

    public Fragment()
    { }
}
