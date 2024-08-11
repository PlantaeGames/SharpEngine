using System.Runtime.InteropServices;

namespace SharpEngineEditor.ImGui.Backend;

/// <summary>
/// A unsigned integer 4-bytes data type
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 0, Size = 4)]
public struct Unit
{
    public uint value = 0;

    public static implicit operator uint(Unit a)
    {
        return a.value;
    }

    public static implicit operator Unit(uint a)
    {
        return new Unit(a);
    }

    public Unit(uint a)
    {
        this.value = a;
    }

    public Unit()
    {
    }

    /// <summary>
    /// Size of the Structure in bytes.
    /// </summary>
    /// <returns>Size</returns>
    public static int GetSize()
    {
        unsafe
        {
            return sizeof(Unit);
        }
    }

}
