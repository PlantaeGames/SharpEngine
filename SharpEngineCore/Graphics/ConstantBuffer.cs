using TerraFX.Interop.DirectX;

namespace SharpEngineCore.Graphics;

/// <summary>
/// Constant buffer able to bind with shaders.
/// </summary>
internal sealed class ConstantBuffer : Buffer
{
    /// <summary>
    /// Creates constant buffer from already created buffer.
    /// </summary>
    /// <param name="buffer">Buffer to represent as constant buffer</param>
    /// <exception cref="GraphicsException"></exception>
    public ConstantBuffer(Buffer buffer) :
        base(buffer.GetNativePtr(), buffer.Info)
    {
        if (buffer.Info.UsageInfo.BindFlags != D3D11_BIND_FLAG.D3D11_BIND_CONSTANT_BUFFER)
        {
            // error here.
            throw new GraphicsException(
                $"Failed to create {nameof(ConstantBuffer)}," +
                $"provided buffer is not bindable as Constant Buffer.");
        }
    }
}
