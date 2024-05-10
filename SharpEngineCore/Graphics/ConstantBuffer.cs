using System.Diagnostics;
using TerraFX.Interop.DirectX;

namespace SharpEngineCore.Graphics;

/// <summary>
/// Constant buffer able to bind with shaders.
/// </summary>
internal sealed class ConstantBuffer : Buffer
{
    public ConstantBuffer(Buffer buffer, Device device) :
        base(buffer.GetNativePtr(), buffer.Info, device)
    {
        Debug.Assert(
            buffer.Info.UsageInfo.BindFlags != D3D11_BIND_FLAG.D3D11_BIND_CONSTANT_BUFFER,
            "Given buffer is not binadable as Constant Buffer.");
    }
}