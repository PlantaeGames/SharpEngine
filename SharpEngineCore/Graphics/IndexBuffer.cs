using TerraFX.Interop.DirectX;

namespace SharpEngineCore.Graphics;

/// <summary>
/// Index buffer able to bind with Input Assembler.
/// </summary>
internal sealed class IndexBuffer : Buffer
{
    /// <summary>
    /// Creates index buffer from already created buffer.
    /// </summary>
    /// <param name="buffer">Buffer to represent as index buffer</param>
    /// <exception cref="GraphicsException"></exception>
    public IndexBuffer(Buffer buffer) :
        base(buffer.GetNativePtr(), buffer.Info)
    {
        if (buffer.Info.UsageInfo.BindFlags != D3D11_BIND_FLAG.D3D11_BIND_INDEX_BUFFER)
        {
            // error here.
            throw new GraphicsException(
                $"Failed to create {nameof(IndexBuffer)}," +
                $"provided buffer is not bindable as Index Buffer.");
        }
    }
}
