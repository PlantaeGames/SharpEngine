using TerraFX.Interop.DirectX;

namespace SharpEngineCore.Graphics;

/// <summary>
/// Vertex buffer able to bind with Input Assembler.
/// </summary>
internal sealed class VertexBuffer : Buffer
{
    public int VertexCount { get; init; }

    /// <summary>
    /// Creates vertex buffer from already created buffer.
    /// </summary>
    /// <param name="buffer">Buffer to represent as vertex buffer</param>
    /// <exception cref="GraphicsException"></exception>
    public VertexBuffer(Buffer buffer, IFragmentable layout) :
        base(buffer.GetNativePtr(), buffer.Info)
    { 
        if(buffer.Info.UsageInfo.BindFlags != D3D11_BIND_FLAG.D3D11_BIND_VERTEX_BUFFER)
        {
            // error here.
            throw new GraphicsException(
                $"Failed to create {nameof(VertexBuffer)}," +
                $"provided buffer is not bindable as Vertex Buffer.");
        }

        var size = buffer.Info.Size;
        VertexCount = size / layout.GetFragmentsCount();
    }
}
