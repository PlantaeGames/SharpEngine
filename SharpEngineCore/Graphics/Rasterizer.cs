﻿namespace SharpEngineCore.Graphics;

internal sealed class Rasterizer : IPipelineStage
{
    [Flags]
    public enum BindFlags
    {
        None = 0,
        Viewports = 1 << 1,
        RasterizerState = 1 << 2
    }

    public RasterizerState RasterizerState { get; init; }
    public Viewport[] Viewports { get; init; }
    public BindFlags Flags { get; init; }

    public Rasterizer(
        RasterizerState rasterizerState,
        Viewport[] viewports, BindFlags flags)
    {
        RasterizerState = rasterizerState;
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
    }
}
