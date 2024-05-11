using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal class Buffer(ComPtr<ID3D11Buffer> pBuffer, BufferInfo info,
    Device device) :
    Resource(ComUtilities.ToResourceNativePtr(ref pBuffer), 
        new (info.Size, info.), device)
{
    public readonly BufferInfo Info = info;

    public void Update(Surface surface)
    {
        Write(surface, 0);
    }

    /// <summary>
    /// Creates a Vertex Buffer from a buffer.
    /// </summary>
    /// <param name="buffer">The buffer to create vertex buffer from.</param>
    /// <returns>Created Vertex Buffer.</returns>
    public static VertexBuffer CreateVertexBuffer(Buffer buffer)
    {
        return new(buffer, buffer._device);
    }

    /// <summary>
    /// Creates a Index Buffer from a buffer.
    /// </summary>
    /// <param name="buffer">The buffer to create index buffer from.</param>
    /// <returns>Created Index Buffer.</returns>
    public static IndexBuffer CreateIndexBuffer(Buffer buffer)
    {
        return new(buffer, buffer._device);
    }

    /// <summary>
    /// Creates a Constant Buffer from a buffer.
    /// </summary>
    /// <param name="buffer">The buffer to create constant buffer from.</param>
    /// <returns>Created Constant Buffer.</returns>
    public static ConstantBuffer CreateConstantBuffer(Buffer buffer)
    {
        return new(buffer, buffer._device);
    }

    private readonly ComPtr<ID3D11Buffer> _ptr = new(pBuffer);

    public ComPtr<ID3D11Buffer> GetNativePtr() => new(_ptr);
}
