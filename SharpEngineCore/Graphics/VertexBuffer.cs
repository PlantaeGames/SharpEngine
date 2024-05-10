using SharpEngineCore.Utilities;
using System.Diagnostics;
using TerraFX.Interop.DirectX;

namespace SharpEngineCore.Graphics;

/// <summary>
/// Vertex buffer able to bind with Input Assembler.
/// </summary>
internal sealed class VertexBuffer : Buffer
{
    public int VertexCount { get; init; }

    public VertexBuffer(Buffer buffer, Device device) :
        base(buffer.GetNativePtr(), buffer.Info, device)
    {
        Debug.Assert(device != null,
            "Device can't be null here. Use Buffer.CreateVertexBuffer().");

        Debug.Assert(
            Info.UsageInfo.BindFlags.HasFlag(D3D11_BIND_FLAG.D3D11_BIND_VERTEX_BUFFER) ||
            Info.UsageInfo.BindFlags.HasFlag(D3D11_BIND_FLAG.D3D11_BIND_STREAM_OUTPUT),
            "The given buffer is not created to be used as vertex buffer.");
        Debug.Assert(
            Info.Layout.GetInterface(nameof(IFragmentable)) != null,
            "Vertex Buffers can only have fragmentable layout.");

        var obj = Activator.CreateInstance(Info.Layout);

        var count = (int)Info.Layout.InvokeMember(nameof(IFragmentable.GetFragmentsCount),
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.InvokeMethod |
            System.Reflection.BindingFlags.Instance, Type.DefaultBinder,
            obj, null);

        VertexCount = buffer.Info.Size.ToArea() / count;
    }
}
