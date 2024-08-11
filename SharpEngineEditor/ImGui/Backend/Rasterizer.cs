using TerraFX.Interop.Windows;

namespace SharpEngineEditor.ImGui.Backend;

internal sealed class Rasterizer : IPipelineStage
{
    [Flags]
    public enum BindFlags
    {
        None = 0,
        Viewports = 1 << 1,
        RasterizerState = 1 << 2,
        Scissors = 1 << 3
    }

    public RasterizerState RasterizerState { get; init; }
    public Viewport[] Viewports { get; init; }
    public Scissors[] Scissors { get; init; }
    public BindFlags Flags { get; init; }

    public Rasterizer(
        RasterizerState rasterizerState,
        Viewport[] viewports, Scissors[] scissors, BindFlags flags)
    {
        RasterizerState = rasterizerState;
        Scissors = scissors;
        Viewports = viewports;
        Flags = flags;
    }

    public Rasterizer()
    { }


    public void Bind(DeviceContext context)
    {
        if(Flags.HasFlag(BindFlags.Viewports))
        {
            context.RSSetViewports(Viewports);
        }

        if (Flags.HasFlag(BindFlags.RasterizerState))
        {
            context.RSSetState(RasterizerState);
        }

        if(Flags.HasFlag(BindFlags.Scissors))
        {
            context.RSSetScissors(Scissors);
        }
    }

    public void Unbind(DeviceContext context)
    {
        if (Flags.HasFlag(BindFlags.Viewports))
        {
            context.RSSetViewports(Viewports, true);
        }

        if (Flags.HasFlag(BindFlags.RasterizerState))
        {
            context.RSSetState(RasterizerState, true);
        }

        if (Flags.HasFlag(BindFlags.Scissors))
        {
            context.RSSetScissors(Scissors, true);
        }
    }
}
