using SharpEngineCore.Utilities;
using System.Diagnostics;
using TerraFX.Interop.DirectX;

namespace SharpEngineCore.Graphics;

/// <summary>
/// Index buffer able to bind with Input Assembler.
/// </summary>
internal sealed class IndexBuffer : Buffer
{
    public int IndexCount { get; private set; }

    public IndexBuffer(Buffer buffer, Device device) :
        base(buffer.GetNativePtr(), buffer.Info, device)
    {
        Debug.Assert(
            Info.UsageInfo.BindFlags == D3D11_BIND_FLAG.D3D11_BIND_INDEX_BUFFER,
        $"Failed to create Index Buffer, provided buffer is not bindable as Index Buffer.");
        Debug.Assert(
            Info.Layout.GetFields().Length == 1,
            "Index Buffers can only have one unit.");
        Debug.Assert(
            Info.Layout.GetInterface(nameof(IUnitable)) != null,
            "Index Buffer must have unitable layout.");

     
        IndexCount = buffer.Info.SurfaceSize.ToArea() / Info.Layout.GetFields().Length;
    }
}
