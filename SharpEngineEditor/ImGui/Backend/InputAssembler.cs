using TerraFX.Interop.WinRT;

namespace SharpEngineEditor.ImGui.Backend;

internal sealed class InputAssembler : IPipelineStage
{
    [Flags]
    public enum BindFlags
    {
        None = 0,
        Layout = 1 << 1,
        VertexBuffer = 1 << 2,
        IndexBuffer = 1 << 3
    }

    public InputLayout Layout { get; init; }
    public VertexBuffer VertexBuffer { get; init; }
    public IndexBuffer IndexBuffer { get; init; }

    public BindFlags Flags { get; init; }

    public void Bind(DeviceContext context)
    {
        if(Flags.HasFlag(BindFlags.Layout))
        {
            context.IASetInputLayout(Layout);
            context.IASetTopology(Layout.Info.Topology);
        }
        if(Flags.HasFlag(BindFlags.VertexBuffer))
        {
            context.IASetVertexBuffer([VertexBuffer], 0);
        }
        if(Flags.HasFlag(BindFlags.IndexBuffer))
        {
            context.IASetIndexBuffer(IndexBuffer);
        }
    }

    public void Unbind(DeviceContext context)
    {
        if (Flags.HasFlag(BindFlags.Layout))
        {
            context.IASetInputLayout(Layout, true);
            context.IASetTopology(Layout.Info.Topology, true);
        }
        if (Flags.HasFlag(BindFlags.VertexBuffer))
        {
            context.IASetVertexBuffer([VertexBuffer], 0, true);
        }
        if (Flags.HasFlag(BindFlags.IndexBuffer))
        {
            context.IASetIndexBuffer(IndexBuffer, true);
        }
    }

    public InputAssembler(InputLayout layout,
                          VertexBuffer vertexBuffer,
                          IndexBuffer indexBuffer,
                          BindFlags flags)
    {
        Layout = layout;
        VertexBuffer = vertexBuffer;
        IndexBuffer = indexBuffer;

        Flags = flags;
    }

    public InputAssembler()
    {
    }
}