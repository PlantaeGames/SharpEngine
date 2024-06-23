namespace SharpEngineCore.Graphics;

internal sealed class OutputMerger : IPipelineStage
{
    [Flags]
    public enum BindFlags
    {
        None = 0,
        RenderTargetViewAndDepthView = 1 << 1,
        DepthStencilState = 1 << 2,
        UnorderedAccessViews = 1 << 3,
        BlendState = 1 << 4
    }

    public RenderTargetView[] RenderTargetViews { get; init; }
    public DepthStencilState DepthStencilState { get; init; }
    public DepthStencilView DepthStencilView { get; init; }
    public UnorderedAccessView[] UnorderedAccessViews { get; init; }
    public BlendState BlendState { get; init; }

    public BindFlags Flags { get; init; }

    public OutputMerger(RenderTargetView[] renderTargetViews,
        DepthStencilState depthStencilState,
        DepthStencilView depthStencilView,
        UnorderedAccessView[] unorderedAccessViews,
        BlendState blendState,
        BindFlags flags)
    {
        RenderTargetViews = renderTargetViews;
        DepthStencilState = depthStencilState;
        DepthStencilView = depthStencilView;
        UnorderedAccessViews = unorderedAccessViews;
        BlendState = blendState;

        Flags = flags;
    }

    public OutputMerger()
    { }

    public void Bind(DeviceContext context)
    {
        if(Flags.HasFlag(BindFlags.RenderTargetViewAndDepthView) &&
          !Flags.HasFlag(BindFlags.UnorderedAccessViews))
        {
            context.OMSetRenderTargets(RenderTargetViews,
                DepthStencilView);
        }

        if (Flags.HasFlag(BindFlags.DepthStencilState))
        {
            context.OMSetDepthStencilState(DepthStencilState);
        }

        if (Flags.HasFlag(BindFlags.UnorderedAccessViews))
        {
            context.OMSetRenderTargetsAndUnorderedAccessViews(
                RenderTargetViews, DepthStencilView, UnorderedAccessViews, 0);
        }

        if(Flags.HasFlag(BindFlags.BlendState))
        {
            context.OMSetBlendState(BlendState);
        }
    }

    public void Unbind(DeviceContext context)
    {
        if (Flags.HasFlag(BindFlags.RenderTargetViewAndDepthView) &&
            !Flags.HasFlag(BindFlags.UnorderedAccessViews))
        {
            context.OMSetRenderTargets(RenderTargetViews,
                DepthStencilView, true);
        }

        if (Flags.HasFlag(BindFlags.DepthStencilState))
        {
            context.OMSetDepthStencilState(DepthStencilState, true);
        }

        if (Flags.HasFlag(BindFlags.UnorderedAccessViews))
        {
            context.OMSetRenderTargetsAndUnorderedAccessViews(
                RenderTargetViews, DepthStencilView, UnorderedAccessViews, 0, true);
        }

        if (Flags.HasFlag(BindFlags.BlendState))
        {
            context.OMSetBlendState(BlendState, true);
        }
    }
}
