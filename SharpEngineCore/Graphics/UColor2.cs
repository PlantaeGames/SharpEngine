using System.Runtime.InteropServices;

namespace SharpEngineCore.Graphics;

[StructLayout(LayoutKind.Sequential, Pack = 0, Size = 8)]
public struct UColor2 : IUnitable
{
    public Unit r = 0;
    public Unit g = 0;

    public UColor2(Unit r, Unit g)
    {
        this.r = r;
        this.g = g;
    }

    public UColor2()
    { }

    /// <summary>
    /// Size of this structure in bytes.
    /// </summary>
    /// <returns>Size.</returns>
    public int GetSize()
    {
        unsafe
        {
            return sizeof(UColor2);
        }
    }

    public int GetUnitCount()
    {
        return ToUnits().Length;
    }

    public Unit[] ToUnits()
    {
        return [r, g];
    }
}
