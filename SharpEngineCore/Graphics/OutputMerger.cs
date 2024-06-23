using System.Diagnostics;
using TerraFX.Interop.Windows;

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
        BlendState = 1 << 4,
        ToogleableBlendState = 1 << 5
    }

    public RenderTargetView[] RenderTargetViews { get; init; }
    public DepthStencilState DepthStencilState { get; init; }
    public DepthStencilView DepthStencilView { get; init; }
    public UnorderedAccessView[] UnorderedAccessViews { get; init; }
    public BlendState DefaultBlendState { get; init; }
    public BlendState BlendState { get; init; }

    private BlendState _currentBlendState;

    public bool BlendOn { get; private set; }

    public BindFlags Flags { get; init; }

    public OutputMerger(RenderTargetView[] renderTargetViews,
        DepthStencilState depthStencilState,
        DepthStencilView depthStencilView,
        UnorderedAccessView[] unorderedAccessViews,
        BlendState defaultBlendState,
        BlendState blendState,
        BindFlags flags)
    {
        RenderTargetViews = renderTargetViews;
        DepthStencilState = depthStencilState;
        DepthStencilView = depthStencilView;
        UnorderedAccessViews = unorderedAccessViews;
        DefaultBlendState = defaultBlendState;
        BlendState = blendState;

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
            context.OMSetDepthStencilState(DepthStencilState);
        }

        if (Flags.HasFlag(BindFlags.UnorderedAccessViews))
        {
            context.OMSetRenderTargetsAndUnorderedAccessViews(
                RenderTargetViews, DepthStencilView, UnorderedAccessViews, 0);
        }

        if(Flags.HasFlag(BindFlags.BlendState))
        {
            context.OMSetBlendState(_currentBlendState);
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
            context.OMSetBlendState(_currentBlendState, true);
        }
    }

    public void ToggleBlendState(bool on)
    {
        Debug.Assert(Flags.HasFlag(BindFlags.BlendState),
                      "The output merger does not have toggleable blend state.");

        _currentBlendState = on ? BlendState : DefaultBlendState;
    }
}
