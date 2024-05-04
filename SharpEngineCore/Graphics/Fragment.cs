using System.Runtime.InteropServices;

namespace SharpEngineCore.Graphics;

/// <summary>
/// Containing Raw Colors in R,G,B,A Format, 4 byte each channel.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 0, Size = 16)]
public struct Fragment
{
    public float r = 0f;
    public float g = 0f;
    public float b = 0f;
    public float a = 0f;

    public Fragment(float r, float g, float b, float a)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }

    public Fragment()
    { }
}
