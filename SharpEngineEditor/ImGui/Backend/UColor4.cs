using System.Runtime.InteropServices;

namespace SharpEngineEditor.ImGui.Backend;

[StructLayout(LayoutKind.Sequential, Pack = 0, Size = 16)]
public struct UColor4 : IUnitable
{
    public Unit r = 0;
    public Unit g = 0;
    public Unit b = 0;
    public Unit a = 0;

    public UColor4(Unit r, Unit g, Unit b, Unit a)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }

    public UColor4()
    { }

    /// <summary>
    /// Size of this structure in bytes.
    /// </summary>
    /// <returns>Size.</returns>
    public int GetSize()
    {
        unsafe
        {
            return sizeof(UColor4);
        }
    }

    public int GetUnitCount()
    {
        return ToUnits().Length;
    }

    public Unit[] ToUnits()
    {
        return [r, g, b, a];
    }
}
