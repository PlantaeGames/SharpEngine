using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal readonly struct BufferInfo(int size, ResourceUsageInfo usageInfo)
{
    public readonly int Size { get; init; } = size;
    public readonly ResourceUsageInfo UsageInfo { get; init; } = usageInfo;
}

/// <summary>
/// Vertex buffer able to bind with vertex shader.
/// </summary>
internal sealed class VertexBuffer : Buffer
{

    /// <summary>
    /// Creates vertex buffer from already created buffer.
    /// </summary>
    /// <param name="buffer">Buffer to represent as vertex buffer</param>
    /// <exception cref="GraphicsException"></exception>
    public VertexBuffer(Buffer buffer) :
        base(buffer.GetNativePtr(), buffer.Info)
    { 
        if(buffer.Info.UsageInfo.BindFlags != D3D11_BIND_FLAG.D3D11_BIND_VERTEX_BUFFER)
        {
            // error here.
            throw new GraphicsException(
                $"Failed creating vertex buffer," +
                $"provided buffer is not bindable with vertex shader");
        }
    }
}

internal class Buffer(ComPtr<ID3D11Buffer> pBuffer, BufferInfo info) :
    Resource(ComUtilities.ToResourceNativePtr(pBuffer))
{
    public readonly BufferInfo Info = info;
    protected readonly ComPtr<ID3D11Buffer> _ptr = pBuffer;

    public ComPtr<ID3D11Buffer> GetNativePtr() => _ptr;
}
