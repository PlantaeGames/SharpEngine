using System.Diagnostics;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal sealed class OutputMerger : IPipelineStage
{
    [Flags]
    public enum BindFlags
    {
        None                             = 0,
        RenderTargetViewAndDepthView     = 1 << 1,
        DepthStencilState                = 1 << 2,
        UnorderedAccessViews             = 1 << 3,
        BlendState                       = 1 << 4,
        ToggleableDepthStencilState      = 1 << 5,
        ToggleableBlendState             = 1 << 6,
    }

    public RenderTargetView[] RenderTargetViews { get; init; }
    public DepthStencilState DefaultDepthStencilState { get; init; }
    public DepthStencilState DepthStencilState { get; init; }
    public DepthStencilView DepthStencilView { get; init; }
    public UnorderedAccessView[] UnorderedAccessViews { get; init; }
    public BlendState DefaultBlendState { get; init; }
    public BlendState BlendState { get; init; }

    private DepthStencilState _currentDepthStencilState;
    private BlendState _currentBlendState;

    public bool DepthStencilOn { get; private set; }
    public bool BlendOn { get; private set; }

    public BindFlags Flags { get; init; }

    public OutputMerger(RenderTargetView[] renderTargetViews,
        DepthStencilState defaultDepthStencilState,
        DepthStencilState depthStencilState,
        DepthStencilView depthStencilView,
        UnorderedAccessView[] unorderedAccessViews,
        BlendState defaultBlendState,
        BlendState blendState,
        BindFlags flags)
    {
        RenderTargetViews = renderTargetViews;
        DefaultDepthStencilState = defaultDepthStencilState;
        DepthStencilState = depthStencilState;
        DepthStencilView = depthStencilView;
        UnorderedAccessViews = unorderedAccessViews;
        DefaultBlendState = defaultBlendState;
        BlendState = blendState;

        _currentDepthStencilState = DefaultDepthStencilState;
        _currentBlendState = DefaultBlendState;
        
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
            if (Flags.HasFlag(BindFlags.ToggleableDepthStencilState))
                context.OMSetDepthStencilState(_currentDepthStencilState);
            else
                context.OMSetDepthStencilState(DepthStencilState);
        }

        if (Flags.HasFlag(BindFlags.UnorderedAccessViews))
        {
            context.OMSetRenderTargetsAndUnorderedAccessViews(
                RenderTargetViews, DepthStencilView, UnorderedAccessViews, 0);
        }

        if(Flags.HasFlag(BindFlags.BlendState))
        {
            if(Flags.HasFlag(BindFlags.ToggleableBlendState))
                context.OMSetBlendState(_currentBlendState);
            else
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
            if (Flags.HasFlag(BindFlags.ToggleableDepthStencilState))
                context.OMSetDepthStencilState(_currentDepthStencilState, true);
            else
                context.OMSetDepthStencilState(DepthStencilState, true);
        }

        if (Flags.HasFlag(BindFlags.UnorderedAccessViews))
        {
            context.OMSetRenderTargetsAndUnorderedAccessViews(
                RenderTargetViews, DepthStencilView, UnorderedAccessViews, 0, true);
        }

        if (Flags.HasFlag(BindFlags.BlendState))
        {
            if (Flags.HasFlag(BindFlags.ToggleableBlendState))
                context.OMSetBlendState(_currentBlendState, true);
            else
                context.OMSetBlendState(BlendState, true);
        }
    }

    public void ToggleDepthStencilState(bool on)
    {
        Debug.Assert(Flags.HasFlag(BindFlags.ToggleableDepthStencilState));

        DepthStencilOn = on;
        _currentDepthStencilState = on ? DepthStencilState : DefaultDepthStencilState;
    }

    public void ToggleBlendState(bool on)
    {
        Debug.Assert(Flags.HasFlag(BindFlags.ToggleableBlendState));

        BlendOn = on;
        _currentBlendState = on ? BlendState : DefaultBlendState;
    }
}
