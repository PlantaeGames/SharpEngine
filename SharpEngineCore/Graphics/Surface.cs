using SharpEngineCore.Exceptions;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SharpEngineCore.Graphics;

/// <summary>
/// Containing Raw Colors in R,G,B,A Format, 4 byte each channel.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 16)]
public struct Fragment
{
    public int r = 0;
    public int g = 0;
    public int b = 0;
    public int a = 0;

    public Fragment(int r, int g, int b, int a)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }

    public Fragment()
    { }
}

public sealed class Surface : IDisposable
{ 
    public Size Size { get; private set; }

    private IntPtr _pColors = IntPtr.Zero;

    private bool _disposed = false;

    public uint GetSliceSize()
    {
        unsafe
        {
            return (uint)(sizeof(Fragment) * Size.Width);
        }
    }

    public IntPtr GetNativePointer() => _pColors;

    public Surface(Size size)
    {
        Debug.Assert(size.IsEmpty == false, 
            "Surface can't have width or height 0.");

        Size = size;

        Create();
    }

    private void Create()
    {
        NativeCreate();

        unsafe void NativeCreate()
        {
            int count = Size.Width * Size.Height;
            try
            {
                _pColors = Marshal.AllocHGlobal(count);
            }
            catch(Exception e)
            {
                throw new SharpException("Failed to create surface", e);
            }

            // zero out memory
            Clear();
        }
    }

    public void Clear()
    {
        Clear(new Fragment());
    }

    public void Clear(Fragment color)
    {
        NativeClear();

        unsafe void NativeClear()
        {
            for (var i = 0; i < (Size.Width * Size.Height); i++)
            {
                *((Fragment*)_pColors + (i * sizeof(Fragment))) = color;
            }
        }
    }

    public void SetColor(Point point, Fragment color)
    {
        Debug.Assert(point.X <  Size.Width && point.Y < Size.Height,
            "Coordinates can't exceed surface dimensions.");

        NativeSetColor();

        unsafe void NativeSetColor()
        {
            var offset = point.Y * Size.Width * sizeof(Fragment) +
                         (point.X * sizeof(Fragment));

            *((Fragment*)_pColors + offset) = color;
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        // release here
        if (_pColors != IntPtr.Zero)
            Marshal.FreeHGlobal(_pColors);
    }

    ~Surface()
    {
        Dispose();
    }
}