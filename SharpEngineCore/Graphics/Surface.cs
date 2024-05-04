using System.Diagnostics;
using System.Runtime.InteropServices;

using SharpEngineCore.Exceptions;

namespace SharpEngineCore.Graphics;

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
                _pColors = Marshal.AllocHGlobal(count * sizeof(Fragment));
            }
            catch(Exception e)
            {
                throw new SharpException("Failed to create surface", e);
            }

            // zero out memory
            Clear(new Fragment(0f, 0f, 0f, 0f));
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
                *((Fragment*)_pColors + i) = color;
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
            var offset = point.Y * Size.Width + point.X;

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

        _disposed = true;
    }

    ~Surface()
    {
        Dispose();
    }
}