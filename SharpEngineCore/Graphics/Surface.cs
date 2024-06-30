using System.Diagnostics;
using System.Runtime.InteropServices;

using SharpEngineCore.Utilities;

namespace SharpEngineCore.Graphics;

public abstract class Surface : IDisposable
{
    public readonly Channels Channels;
    public Size Size { get; private set; }

    protected IntPtr _pColors = IntPtr.Zero;
    private bool _disposed = false;

    public abstract int SubDivisionCount { get; }

    public void Set(byte value, int offset)
    {
        Debug.Assert(Size.ToArea() * GetPeiceSize() <= offset,
            "Offset is beyound buffer size.");

        unsafe
        {
            *((byte*)_pColors + offset) = value;
        }
    }

    public byte Get(int offset)
    {
        Debug.Assert(Size.ToArea() * GetPeiceSize() >= offset,
            "Offset is beyound buffer size.");

        unsafe
        {
            return *((byte*)_pColors + offset);
        }
    }

    /// <summary>
    /// Gets the size of single unit of that surface in bytes.
    /// </summary>
    /// <returns>Size in bytes</returns>
    public abstract int GetPeiceSize();

    /// <summary>
    /// Gets the size of the single row in bytes.
    /// </summary>
    /// <returns>Size in bytes</returns>
    public abstract int GetSliceSize();


    public IntPtr GetNativePointer() => _pColors;

    public void Dispose()
    {
        Dispose(true);

        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        // release here
        Marshal.FreeHGlobal(_pColors);

        _disposed = true;
    }

    ~Surface()
    {
        Dispose(false);
    }

    protected Surface(Size size, Channels channels)
    {
        Size = size;
        Channels = channels;
    }
}
