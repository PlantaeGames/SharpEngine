using System.Runtime.InteropServices;

namespace SharpEngineEditor.ImGui.Backend;

[StructLayout(LayoutKind.Sequential, Pack = 0, Size = 4)]
public struct Index : IUnitable
{
    public Unit Value;

    public Index(Unit value)
    {
        Value = value;
    }

    public Index()
    { }

    public int GetUnitCount()
    {
        return ToUnits().Length;
    }

    public Unit[] ToUnits()
    {
        return [Value];
    }

    public int GetSize()
    {
        unsafe
        {
            return sizeof(Index);
        }
    }
}
