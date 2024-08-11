using System.Diagnostics;
using System.Runtime.InteropServices;

using SharpEngineCore.Exceptions;
using SharpEngineCore.Utilities;

namespace SharpEngineCore.Graphics;

/// <summary>
/// Holds data in heap in the form of units.
/// </summary>
public sealed class USurface : Surface
{
    private readonly IUnitable _unit;

    public override int SubDivisionCount => _unit.GetUnitCount();

    /// <summary>
    /// Gets the size of single unit of that surface in bytes.
    /// </summary>
    /// <returns>Size in bytes</returns>
    public override int GetPeiceSize()
    {
        return Unit.GetSize();
    }

    /// <summary>
    /// Gets the size of the single row in bytes.
    /// </summary>
    /// <returns>Size in bytes</returns>
    public override int GetSliceSize()
    {
        unsafe
        {
            return GetPeiceSize() * Size.Width;
        }
    }


    public USurface(Size size, Channels channels = Channels.Single) :
        base(size, channels)
    {
        Debug.Assert(size.IsEmpty == false, 
            "Surface can't have width or height 0.");

        switch (Channels)
        {
            case Channels.Single:
                _unit = new SharpEngineCore.Graphics.UColor1();
                break;
            case Channels.Double:
                _unit = new SharpEngineCore.Graphics.UColor2();
                break;
            case Channels.Triple:
                _unit = new SharpEngineCore.Graphics.UColor3();
                break;
            case Channels.Quad:
                _unit = new SharpEngineCore.Graphics.UColor4();
                break;
        }

        Create();
    }

    private void Create()
    {
        Create(new UColor1(0));
    }

    private void Create(IUnitable color)
    {
        int count = Size.Width * Size.Height;
        try
        {
            _pColors = Marshal.AllocHGlobal(count * Unit.GetSize());
        }
        catch (Exception e)
        {
            throw new SharpException("Failed to create surface", e);
        }

        // zero out memory
        Clear(color);
    }

    public void Clear()
    {
        Clear(_unit);
    }

    public void Clear(IUnitable unitable)
    {
        for (var y = 0; y < Size.Height; y++)
        {
            for (var x = 0; x < Size.Width; x++)
            {
                SetUnit(new(x, y), unitable);
            }
        }
    }

    public void SetLinearUnits(Unit[] units)
    {
        Debug.Assert(units.Length > 0,
            "Given units can't be equal or less than zero.");
        Debug.Assert(
            Size.ToArea() * SubDivisionCount >= units.Length,
            "Given units can't be greater than the surface");

        NativeSetLinearunits();

        unsafe void NativeSetLinearunits()
        {
            for (var i = 0; i < units.Length; i++)
            {
                *((Unit*)_pColors + i) = units[i];
            }
        }
    }

    public void SetUnit(Point point, IUnitable unitable)
    {
        Debug.Assert(point.X + unitable.GetUnitCount() <=  Size.Width &&
                     point.Y + unitable.GetUnitCount() <= Size.Height,
            "Coordinates can't exceed surface dimensions.");

        NativeSetColor();

        unsafe void NativeSetColor()
        {
            var mem = SubDivisionCount;
            var offset = point.Y * (Size.Width * mem) + (point.X * mem);

            var units = unitable.ToUnits();
            var step = units.Length;
            for (var f = 0; f < step; f++)
            {
                *((Unit*)_pColors + offset + f) = units[f];
            }
        }
    }
}