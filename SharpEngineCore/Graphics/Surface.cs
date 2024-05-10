using System.Runtime.InteropServices;

namespace SharpEngineCore.Graphics;

public abstract class Surface : IDisposable
{
    public readonly Channels Channels;
    public Size Size { get; private set; }

    protected IntPtr _pColors = IntPtr.Zero;
    private bool _disposed = false;

    public abstract int SubDivisionCount { get; }

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
