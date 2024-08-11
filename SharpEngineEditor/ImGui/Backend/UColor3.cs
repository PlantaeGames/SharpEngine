using System.Runtime.InteropServices;

namespace SharpEngineEditor.ImGui.Backend;

[StructLayout(LayoutKind.Sequential, Pack = 0, Size = 12)]
public struct UColor3 : IUnitable
{
    public Unit r = 0;
    public Unit g = 0;
    public Unit b = 0;

    public UColor3(Unit r, Unit g, Unit b)
    {
        this.r = r;
        this.g = g;
        this.b = b;
    }

    public UColor3()
    { }

    /// <summary>
    /// Size of this structure in bytes.
    /// </summary>
    /// <returns>Size.</returns>
    public int GetSize()
    {
        unsafe
        {
            return sizeof(UColor3);
        }
    }

    public int GetUnitCount()
    {
        return ToUnits().Length;
    }

    public Unit[] ToUnits()
    {
        return [r, g, b];
    }
}
