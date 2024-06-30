using System.Runtime.InteropServices;

namespace SharpEngineCore.Graphics;

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
