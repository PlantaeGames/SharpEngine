using System.Runtime.InteropServices;

namespace SharpEngineEditor.ImGui.Backend;

[StructLayout(LayoutKind.Sequential, Pack = 0, Size = 4)]
public struct UColor1 : IUnitable
{
    public Unit r = 0;

    public UColor1(Unit r)
    {
        this.r = r;
    }

    public UColor1()
    { }

    /// <summary>
    /// Size of this structure in bytes.
    /// </summary>
    /// <returns>Size.</returns>
    public int GetSize()
    {
        unsafe
        {
            return sizeof(UColor1);
        }
    }

    public int GetUnitCount()
    {
        return ToUnits().Length;
    }

    public Unit[] ToUnits()
    {
        return [r];
    }
}
