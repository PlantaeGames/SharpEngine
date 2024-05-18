using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

public class Buffer : Resource
{
    public readonly BufferInfo Info;

    public void Update(Surface surface)
    {
        Write(surface, 0);
    }

    /// <summary>
    /// Creates a Vertex Buffer from a buffer.
    /// </summary>
    /// <param name="buffer">The buffer to create vertex buffer from.</param>
    /// <returns>Created Vertex Buffer.</returns>
    internal static VertexBuffer CreateVertexBuffer(Buffer buffer)
    {
        return new(buffer, buffer._device);
    }

    /// <summary>
    /// Creates a Index Buffer from a buffer.
    /// </summary>
    /// <param name="buffer">The buffer to create index buffer from.</param>
    /// <returns>Created Index Buffer.</returns>
    internal static IndexBuffer CreateIndexBuffer(Buffer buffer)
    {
        return new(buffer, buffer._device);
    }

    /// <summary>
    /// Creates a Constant Buffer from a buffer.
    /// </summary>
    /// <param name="buffer">The buffer to create constant buffer from.</param>
    /// <returns>Created Constant Buffer.</returns>
    internal static ConstantBuffer CreateConstantBuffer(Buffer buffer)
    {
        return new(buffer, buffer._device);
    }

    private readonly ComPtr<ID3D11Buffer> _ptr;

    internal Buffer(ComPtr<ID3D11Buffer> pBuffer, BufferInfo info,
        Device device) : base(ComUtilities.ToResourceNativePtr(ref pBuffer), 
            new (info.Size, info.UsageInfo), device)
    {
        Info = info;
        _ptr = new(pBuffer);
    }

    internal ComPtr<ID3D11Buffer> GetNativePtr() => new(_ptr);
}
