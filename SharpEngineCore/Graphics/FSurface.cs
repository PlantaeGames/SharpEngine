using System.Diagnostics;
using System.Runtime.InteropServices;

using SharpEngineCore.Exceptions;
using SharpEngineCore.Utilities;

namespace SharpEngineCore.Graphics;

/// <summary>
/// Holds data in heap in the form of fragments.
/// </summary>
public sealed class FSurface : Surface
{
    private readonly IFragmentable _unitFragment;

    public override int SubDivisionCount => _unitFragment.GetFragmentsCount();

    /// <summary>
    /// Gets the size of single unit of that surface in bytes.
    /// </summary>
    /// <returns>Size in bytes</returns>
    public override int GetPeiceSize()
    {
        return _unitFragment.GetSize();
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

    public FSurface(Size size, Channels channels = Channels.Single) :
        base(size, channels)
    {
        Debug.Assert(size.IsEmpty == false,
            "Surface can't have width or height 0.");

        switch (Channels)
        {
            case Channels.Single:
                _unitFragment = new SharpEngineCore.Graphics.FColor1();
                break;
            case Channels.Double:
                _unitFragment = new SharpEngineCore.Graphics.FColor2();
                break;
            case Channels.Triple:
                _unitFragment = new SharpEngineCore.Graphics.FColor3();
                break;
            case Channels.Quad:
                _unitFragment = new SharpEngineCore.Graphics.FColor4();
                break;
        }

        Create();
    }

    private void Create()
    {
        Create(new FColor1(0));
    }

    private void Create(IFragmentable color)
    {
        int count = Size.Width * Size.Height;
        try
        {
            _pColors = Marshal.AllocHGlobal(count * _unitFragment.GetSize());
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
        Clear(_unitFragment);
    }

    public void Clear(IFragmentable fragmentable)
    {
        for (var y = 0; y < Size.Height; y++)
        {
            for (var x = 0; x < Size.Width; x++)
            {
                SetFragment(new(x, y), fragmentable);
            }
        }
    }

    public void SetLinearFragments(Fragment[] fragments)
    {
        Debug.Assert(fragments.Length > 0,
            "Given fragments can't be equal or less than zero.");
        Debug.Assert(
            Size.ToArea() * SubDivisionCount >= fragments.Length,
            "Given fragments can't be greater than the surface");

        NativeSetLinearFragments();

        unsafe void NativeSetLinearFragments()
        {
            for (var i = 0; i < fragments.Length; i++)
            {
                *((Fragment*)_pColors + i) = fragments[i];
            }
        }
    }

    public void SetFragment(Point point, IFragmentable fragmentable)
    {
        Debug.Assert(point.X + fragmentable.GetFragmentsCount() <= Size.Width &&
                     point.Y + fragmentable.GetFragmentsCount() <= Size.Height,
            "Coordinates can't exceed surface dimensions.");

        NativeSetColor();

        unsafe void NativeSetColor()
        {
            var mem = SubDivisionCount;
            var offset = point.Y * (Size.Width * mem) + (point.X * mem);

            var fragments = fragmentable.ToFragments();
            var step = fragments.Length;
            for (var f = 0; f < step; f++)
            {
                *((Fragment*)_pColors + offset + f) = fragments[f];
            }
        }
    }
}