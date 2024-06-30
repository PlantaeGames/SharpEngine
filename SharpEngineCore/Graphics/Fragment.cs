using System.Runtime.InteropServices;

namespace SharpEngineCore.Graphics;

[StructLayout(LayoutKind.Sequential, Pack = 0, Size = 4)]
public struct Fragment
{
    public float value = 0f;

    public static implicit operator float(Fragment a)
    {
        return a.value;
    }

    public static implicit operator Fragment(float a)
    {
        return new Fragment(a);
    }

    public Fragment(float a)
    {
        this.value = a;
    }

    public Fragment()
    { }

    /// <summary>
    /// Size of the structure in bytes.
    /// </summary>
    /// <returns>Size</returns>
    public static int GetSize()
    {
        unsafe
        {
            return sizeof(Fragment);
        }
    }

    public override string ToString()
    {
        return $"{value}";
    }
}
